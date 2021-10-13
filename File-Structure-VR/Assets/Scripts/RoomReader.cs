using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomReader : MonoBehaviour {

    private Mesh meshFilter;
    private Vector3[] vertices;
    private GameObject[] spheres;

    // Start is called before the first frame update
    void Start() {
        // Obtain names of all logical drives on the computer, set root to first drive.
        string[] drives = Environment.GetLogicalDrives();
        UnityDirectory root = new UnityDirectory(drives[0], 0);

        // Iterate through root's children, 
        for (int i = 0; i < root.Children.Count; i++) {

        }
    }

    // Update is called once per frame
    void Update() {

    }
}

