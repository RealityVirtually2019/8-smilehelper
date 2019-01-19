using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using VideoFrameAnalyzer;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using UnityEngine.Networking;

public class FaceAnalysis : MonoBehaviour {
    /// <summary>
    /// Allows this class to behave like a singleton
    /// </summary>
    public static FaceAnalysis Instance;

    /// <summary>
    /// The analysis result text
    /// </summary>
    private TextMesh labelText;

    /// <summary>
    /// Bytes of the image captured with camera
    /// </summary>
    internal byte[] imageBytes;

    /// <summary>
    /// Path of the image captured with camera
    /// </summary>
    internal string imagePath;

    /// <summary>
    /// Base endpoint of Face Recognition Service
    /// </summary>
    const string baseEndpoint = "https://westus.api.cognitive.microsoft.com/face/v1.0/";

    /// <summary>
    /// Auth key of Face Recognition Service
    /// </summary>
    private const string key = "d503595ddaca483f8215296698c12f00";

    /// <summary>
    /// Id (name) of the created person group 
    /// </summary>
    private const string personGroupId = "emotions";
}





//Deserializing objects
public class Group_RootObject
{
    public string personGroupId { get; set; }
    public string name { get; set; }
    public object userData { get; set; }
}

/// <summary>
/// The Person Face object
/// </summary>
public class Face_RootObject
{
    public string faceId { get; set; }
}

/// <summary>
/// Collection of faces that needs to be identified
/// </summary>
public class FacesToIdentify_RootObject
{
    public string personGroupId { get; set; }
    public List<string> faceIds { get; set; }
    public int maxNumOfCandidatesReturned { get; set; }
    public double confidenceThreshold { get; set; }
}

/// <summary>
/// Collection of Candidates for the face
/// </summary>
public class Candidate_RootObject
{
    public string faceId { get; set; }
    public List<Candidate> candidates { get; set; }
}

public class Candidate
{
    public string personId { get; set; }
    public double confidence { get; set; }
}

/// <summary>
/// Name and Id of the identified Person
/// </summary>
public class IdentifiedPerson_RootObject
{
    public string personId { get; set; }
    public string name { get; set; }
}


/// <summary>
/// Detect faces from a submitted image
/// </summary>
internal IEnumerator DetectFacesFromImage()
{
    WWWForm webForm = new WWWForm();
    string detectFacesEndpoint = $"{baseEndpoint}detect";

    // Change the image into a bytes array
    imageBytes = GetImageAsByteArray(imagePath);

    using (UnityWebRequest www =
        UnityWebRequest.Post(detectFacesEndpoint, webForm))
    {
        www.SetRequestHeader("Ocp-Apim-Subscription-Key", key);
        www.SetRequestHeader("Content-Type", "application/octet-stream");
        www.uploadHandler.contentType = "application/octet-stream";
        www.uploadHandler = new UploadHandlerRaw(imageBytes);
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();
        string jsonResponse = www.downloadHandler.text;
        Face_RootObject[] face_RootObject =
            JsonConvert.DeserializeObject<Face_RootObject[]>(jsonResponse);

        List<string> emotionsList = new List<string>();
        // Create a list with the face Ids of faces detected in image
        foreach (Face_RootObject faceRO in face_RootObject)
        {
            emotionsList.Add(faceRO.faceId);
            Debug.Log($"Detected face - Id: {faceRO.faceId}");
        }

        StartCoroutine(IdentifyFaces(emotionsList));
    }
}

/// <summary>
/// Returns the contents of the specified file as a byte array.
/// </summary>
static byte[] GetImageAsByteArray(string imageFilePath)
{
    FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
    BinaryReader binaryReader = new BinaryReader(fileStream);
    return binaryReader.ReadBytes((int)fileStream.Length);
}





//SENDS IMAGE AS A JPEG FILE
private void ExecuteImageCaptureAndAnalysis()
{
    Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending
        ((res) => res.width * res.height).First();
    Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

    PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = targetTexture.width;
        c.cameraResolutionHeight = targetTexture.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, delegate (PhotoCapture.PhotoCaptureResult result)
        {
            string filename = string.Format(@"CapturedImage{0}.jpg", tapsCount);
            string filePath = Path.Combine(Application.persistentDataPath, filename);

            // Set the image path on the FaceAnalysis class
            FaceAnalysis.Instance.imagePath = filePath;

            photoCaptureObject.TakePhotoAsync
            (filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
        });
    });
}


/// <summary>
/// Spawns cursor for the Main Camera
/// </summary>
private void CreateLabel()
{
    // Create a sphere as new cursor
    GameObject newLabel = new GameObject();

    // Attach the label to the Main Camera
    newLabel.transform.parent = gameObject.transform;

    // Resize and position the new cursor
    newLabel.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    newLabel.transform.position = new Vector3(0f, 3f, 60f);

    // Creating the text of the Label
    labelText = newLabel.AddComponent<TextMesh>();
    labelText.anchor = TextAnchor.MiddleCenter;
    labelText.alignment = TextAlignment.Center;
    labelText.tabSize = 4;
    labelText.fontSize = 50;
    labelText.text = ".";
}


/// <summary>
/// Called right after the photo capture process has concluded
/// </summary>
void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
{
    photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
}

/// <summary>
/// Register the full execution of the Photo Capture. If successfull, it will begin the Image Analysis process.
/// </summary>
void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
{
    photoCaptureObject.Dispose();
    photoCaptureObject = null;

    // Request image caputer analysis
    StartCoroutine(FaceAnalysis.Instance.DetectFacesFromImage());
}


private void Awake()
{
    // Allows this instance to behave like a singleton
    Instance = this;

    // Add the ImageCapture Class to this Game Object
    gameObject.AddComponent<ImageCapture>();

    // Create the text label in the scene
    CreateLabel();
}