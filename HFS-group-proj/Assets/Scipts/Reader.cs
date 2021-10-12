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

        //DrawTreeTest.TreeHelpers.CalculateNodePositions(root);
        root.LogPrint(root);

        /*

        //Place(root);

        //Vector3 spawnPosition = new Vector3(-.5f * (root.Size), 0f, 10f);

        //root.Size

        //foreach (UnityFileSystemEntry fsEntry in root.Children)
        //{

        //Instantiate(EntryType[(int)fsEntry.EntryType], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        //var ent = Instantiate(EntryType[(int) fsEntry.EntryType], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
            ent.transform.position = new Vector3(ent.transform.position.x, 0, ent.transform.position.z);

            spawnPosition = spawnPosition + new Vector3(1, 0, 0);
            //Debug.Log(fsEntry.EntryType);
        //}
        */
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*void Place(UnityFileSystemEntry entry)
    {
        Vector3 startPosition = entry.Parent.spawnPosition - new Vector3(-.5f * (entry.Size), 0f, 3f);

        foreach (UnityDirectory fsEntry in entry.Children)
        {
            Place(fsEntry);
            Instantiate(EntryType[(int)fsEntry.EntryType], startPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
            startPosition = startPosition + new Vector3(1, 0, 0);
        }
    }*/
}