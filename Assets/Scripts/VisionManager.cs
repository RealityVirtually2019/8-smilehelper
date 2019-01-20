using System.Collections;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;



public class VisionManager : MonoBehaviour {

    [System.Serializable]
    public class TagData
    {
        public string name;
        public float confidence;
    }

    [System.Serializable]
    public class AnalysedObject
    {
        public TagData[] tags;
        public string requestId;
        public object metadata;
    }

    public static VisionManager instance;

    // you must insert your service key here!    
    private string authorizationKey = "d503595ddaca483f8215296698c12f00";
    private const string ocpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
    private string visionAnalysisEndpoint =
        "https://eastus.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion";   // This is where you need to update your endpoint, if you set your location to something other than west-us.

    internal byte[] imageBytes;

    internal string imagePath;

    private void Awake()
    {
        // allows this instance to behave like a singleton
        instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Call the Computer Vision Service to submit the image.
    /// </summary>
    public IEnumerator AnalyseLastImageCaptured()
    {
        WWWForm webForm = new WWWForm();
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(visionAnalysisEndpoint, webForm))
        {
            // gets a byte array out of the saved image
            imageBytes = GetImageAsByteArray(imagePath);
            unityWebRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            unityWebRequest.SetRequestHeader(ocpApimSubscriptionKeyHeader, authorizationKey);

            // the download handler will help receiving the analysis from Azure
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            // the upload handler will help uploading the byte array with the request
            unityWebRequest.uploadHandler = new UploadHandlerRaw(imageBytes);
            unityWebRequest.uploadHandler.contentType = "application/octet-stream";

            yield return unityWebRequest.SendWebRequest();

            long responseCode = unityWebRequest.responseCode;

            try
            {
                string jsonResponse = null;
                jsonResponse = unityWebRequest.downloadHandler.text;
                //[{
                //"faceId":"f1f3b05a-1b63-4d77-9ecd-36f11df020c7",
                //"faceRectangle":{"top":10,"left":312,"width":140,"height":140},
                //"faceAttributes":{"emotion":{"anger":0.0,"contempt":0.0,"disgust":0.0,"fear":0.0,"happiness":0.392,"neutral":0.607,"sadness":0.0,"surprise":0.0}}}]

                // The response will be in Json format
                // therefore it needs to be deserialized into the classes AnalysedObject and TagData
                
                Debug.Log(jsonResponse.ToString());
                List<string> facesIdList = new List<string>();
                Face_RootObject[] face_RootObject =
                    JsonConvert.DeserializeObject<Face_RootObject[]>(jsonResponse);

                string outputLabel = "";
                if (face_RootObject.Length > 0)
                {
                    Dictionary<string, object> face = JsonConvert.DeserializeObject<Dictionary<string, object>>(face_RootObject[0].faceAttributes.ToString());
                    Dictionary<string, double> emotions = JsonConvert.DeserializeObject<Dictionary<string, double>>(face["emotion"].ToString());

                    string prominentEmotion = null;
                    double prominentEmotionConf = 0f;

                    foreach (string emotion in emotions.Keys)
                    {
                        double currentEmotionConf = emotions[emotion];
                        if (currentEmotionConf >= prominentEmotionConf)
                        {
                            prominentEmotion = emotion;
                            prominentEmotionConf = currentEmotionConf;
                        }

                        Debug.Log($"Detected emotion {emotion} and confidence {emotions[emotion]}");
                    }
                    outputLabel = $"{prominentEmotion}";
                }
                ResultsLabel.instance.SetTagsToLastLabel(outputLabel);
            }
            catch (Exception exception)
            {
                Debug.Log("Json exception.Message: " + exception.Message);
            }

            yield return null;
        }
    }

    /// <summary>
    /// Returns the contents of the specified file as a byte array.
    /// </summary>
    private static byte[] GetImageAsByteArray(string imageFilePath)
    {
        FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader(fileStream);
        return binaryReader.ReadBytes((int)fileStream.Length);
    }
}