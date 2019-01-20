using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA.WebCam;


public class ImageCapture : MonoBehaviour {

    public static ImageCapture instance;
    public int tapsCount;
    private PhotoCapture photoCaptureObject = null;
    private GestureRecognizer recognizer;
    private bool currentlyCapturing = false;
    public GameObject buddyObj;

    private void Awake()
    {
        // Allows this instance to behave like a singleton
        instance = this;

        buddyObj = GameObject.Find("Buddy");
    }

    public void PlaySound(GameObject obj)
    {
        AudioSource audio = obj.GetComponent<AudioSource>();
        audio.Play();
    }

    void Start()
    {
        // subscribing to the Hololens API gesture recognizer to track user gestures
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.Tapped += TapHandler;
        recognizer.StartCapturingGestures();
    }

    /// <summary>
    /// Respond to Tap Input.
    /// </summary>
    private void TapHandler(TappedEventArgs obj)
    {
        // Only allow capturing, if not currently processing a request.
        if (currentlyCapturing == false)
        {
            currentlyCapturing = true;

            // increment taps count, used to name images when saving
            tapsCount++;

            // Create a label in world space using the ResultsLabel class
            ResultsLabel.instance.CreateLabel();

            // Begins the image capture and analysis procedure
            ExecuteImageCaptureAndAnalysis();

            //CUSTOM TRIGGER EVENTS HERE:
            PlaySound(buddyObj);
        }
    }

    void OnCapturePhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame frame)
    {
        //frame.GetUnsafePointerToBuffer()
        if (result.success)
        {
            List<byte> buffer = new List<byte>();
            buffer.Clear();
            frame.CopyRawImageDataIntoBuffer(buffer);
            StartCoroutine(VisionManager.instance.AnalyzeImage(buffer.ToArray()));
        }
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Dispose from the object in memory and request the image analysis 
        // to the VisionManager class
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    /// <summary>    
    /// Begin process of Image Capturing and send To Azure     
    /// Computer Vision service.   
    /// </summary>    
    private void ExecuteImageCaptureAndAnalysis()
    {
        // Set the camera resolution to be the highest possible
        IEnumerable<Resolution> resolutions = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height);
        int numAvailableRes = resolutions.Count();
        Resolution cameraResolution = resolutions.Skip(numAvailableRes/2).First(); //get a medium resolution
            //.Last(); //lowest resolution for fastest resoponse. use .First() for highest
        
        Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Begin capture process, set the image format    
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;
            CameraParameters camParameters = new CameraParameters();
            camParameters.hologramOpacity = 0.0f;
            camParameters.cameraResolutionWidth = targetTexture.width;
            camParameters.cameraResolutionHeight = targetTexture.height;
            camParameters.pixelFormat = CapturePixelFormat.JPEG;
    
            // Capture the image from the camera and save it in the App internal folder    
            captureObject.StartPhotoModeAsync(camParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                photoCaptureObject.TakePhotoAsync(OnCapturePhotoToMemory);

                currentlyCapturing = false;
            });
        });
    }
}
