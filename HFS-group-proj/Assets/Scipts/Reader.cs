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

        root.LogPrint(root);

        DrawTreeTest.TreeHelpers.CalculateNodePositions(root);
        Place(root);

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

    public void Place(UnityDirectory node)
    {
        Vector3 spawnPosition = new Vector3(node.X, 0f, node.Y);

        var ob = Instantiate(EntryType[(int)node.EntryType], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);

        if(node.Parent != null) { 
            LineRenderer lineRenderer = ob.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = 2;

            Vector3 spawnPosition2 = new Vector3(node.Parent.X, 0f, node.Parent.Y);
            lineRenderer.SetPosition(0, spawnPosition);
            lineRenderer.SetPosition(1, spawnPosition2);
        }
        foreach (var child in node.Children)
        {
            var child2 = child as UnityDirectory;
            if (child2 != null)
            {
                Place(child2);
            }
        }
    }
}