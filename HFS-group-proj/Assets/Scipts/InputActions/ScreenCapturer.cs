using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCapturer : MonoBehaviour
{
    public PrimaryButtonWatcher watcher;

    // Start is called before the first frame update
    void Start()
    {
        watcher.primaryButtonPress.AddListener(onPrimaryButtonEvent);
    }

    public void onPrimaryButtonEvent(bool pressed) 
    {
        if (pressed) 
        {
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshots/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png");
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
