using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reader : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        // Obtain names of all logical drives on the computer, set root to first drive.
        string[] drives = Environment.GetLogicalDrives();
        UnityDirectory root = new UnityDirectory(drives[0], 0);

        foreach(UnityFileSystemEntry fsEntry in root.Children) {
            Debug.Log(fsEntry.EntryType);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
