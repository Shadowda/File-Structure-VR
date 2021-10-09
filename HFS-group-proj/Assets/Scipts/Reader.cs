using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reader : MonoBehaviour
{
    public GameObject[] EntryType;

    // Start is called before the first frame update
    void Start()
    {
        // Obtain names of all logical drives on the computer, set root to first drive.
        string[] drives = Environment.GetLogicalDrives();
        UnityDirectory root = new UnityDirectory(drives[0], 0);

        Vector3 spawnPosition = new Vector3(0f, 0f, 10f);

        foreach (UnityFileSystemEntry fsEntry in root.Children)
        {
            Instantiate(EntryType[(int) fsEntry.EntryType], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
            spawnPosition = spawnPosition + new Vector3(1, 0, 1);
            //Debug.Log(fsEntry.EntryType);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Place(UnityFileSystemEntry entry)
    {



    }
}