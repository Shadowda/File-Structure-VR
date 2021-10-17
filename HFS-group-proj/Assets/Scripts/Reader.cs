using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using NLT;

public class Reader : MonoBehaviour
{
    public string RootPath;
    public XRRig Rig;
    public GameObject[] EntryType;

    // Start is called before the first frame update
    void Start()
    {
        // Obtain names of all logical drives on the computer, set root to first drive.
        string[] drives = Environment.GetLogicalDrives();

        UnityDirectory root = new UnityDirectory(RootPath == "" ? drives[0] : RootPath, 0);
        NLT_Tree.Tree treeRoot = root.convert(root);

        treeRoot.layout(treeRoot);
        Place(treeRoot, null);
        //root.LogPrint(root);

        Rig.MoveCameraToWorldLocation(new Vector3(treeRoot.x, 0, treeRoot.y));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Place(NLT_Tree.Tree node, NLT_Tree.Tree Parent)
    {
        //Vector3 spawnPosition = new Vector3(node.x * 2, 0, node.y * 20);
        //GameObject ob = Instantiate(EntryType[2], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);

        Vector3 s1 = new Vector3(node.x, 0, node.y);
        Vector3 s2 = new Vector3(node.x + node.w - 1, 0, node.y);
        Vector3 s3 = new Vector3(node.x, 0, node.y + node.h);
        Vector3 s4 = new Vector3(node.x + node.w - 1, 0, node.y + node.h);

        float xc = (node.x + node.x + node.w - 1) / 2;
        float yc = (node.y + node.y + node.h) / 2;

        Vector3 cen = new Vector3(xc, 0, yc);

        GameObject ob = Instantiate(EntryType[0], cen + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        ob.name = node.Path;
        ob.transform.localScale = new Vector3(node.w - 2, 1, 1);

        GameObject ob1 = Instantiate(EntryType[1], s2 + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        GameObject ob2 = Instantiate(EntryType[1], s3 + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        GameObject ob3 = Instantiate(EntryType[1], s4 + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        ob1.transform.SetParent(ob.transform);
        ob2.transform.SetParent(ob.transform);
        ob3.transform.SetParent(ob.transform);

        //ob.transform.localScale = new Vector3(node.w, 1, node.h);

        if (Parent != null)
        {
            LineRenderer lineRenderer = ob.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = 2;

            xc = (Parent.x + Parent.x + Parent.w - 1) / 2;
            yc = (Parent.y + Parent.y + Parent.h) / 2;
            Vector3 cen2 = new Vector3(xc, 0, yc);
            //Vector3 spawnPosition2 = new Vector3(Parent.x, 0, Parent.y);

            lineRenderer.SetPosition(0, cen);
            lineRenderer.SetPosition(1, cen2);
        }

        foreach (var child in node.c)
        {
            Place(child, node);
        }
    }

    /*
    public void Place(UnityDirectory node)
    {
        Vector3 spawnPosition = new Vector3(node.X, 0 , node.Y * 15);

        //Vector3 spawnPosition = new Vector3(node.X, -node.Y * 15, node.Z);

        node.ob = Instantiate(EntryType[2], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);
        node.ob.name = node.Path;

        //Debug.Log(node.ob.name);
        //Debug.Log(node.ob.transform.position);
        //Debug.Log(node.ob.transform.localPosition);

        if (node.Parent != null) { 
            node.ob.transform.SetParent(node.Parent.ob.transform);
            //node.ob.transform.localPosition += new Vector3(0, 10, 0);
            //node.ob.transform.SetParent(null);
        }

        //ob.transform.localScale = ob.transform.localScale + new Vector3(node.Size/10, 0, node.Size/10);

        //var ob = Instantiate(EntryType[(int)node.EntryType], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);


        

        foreach (var child in node.Children)
        {
            var child2 = child as UnityDirectory;
            if (child2 != null)
            {
                Place(child2);
            }
        }
    }
    */
}