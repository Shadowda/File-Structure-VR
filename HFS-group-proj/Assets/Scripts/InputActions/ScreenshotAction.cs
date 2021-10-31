using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenshotAction : MonoBehaviour
{
    public InputActionReference screenshotReference = null;

    private void Awake()
    {
        screenshotReference.action.started += CaptureScreenshot;
    }

    private void OnDestroy() 
    {
        screenshotReference.action.started -= CaptureScreenshot;    
    }

    private void CaptureScreenshot(InputAction.CallbackContext context) 
    {
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshots/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png");
        UnityEditor.AssetDatabase.Refresh();
    }
}
