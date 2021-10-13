using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumReader : MonoBehaviour {

    private Mesh meshFilter;
    private Vector3[] vertices;
    private GameObject[] spheres;

    // Start is called before the first frame update
    void Start() {
        meshFilter = GetComponent<MeshFilter>().mesh;
        vertices = meshFilter.vertices;
        spheres = new GameObject[vertices.Length];

        // Obtain names of all logical drives on the computer, set root to first drive.
        string[] drives = Environment.GetLogicalDrives();
        UnityDirectory root = new UnityDirectory(drives[0], 0);

        GameObject rootSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rootSphere.transform.position = new Vector3(0.75f, 0.33f, -1.5f);
        rootSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Renderer rootRenderer = rootSphere.GetComponent<Renderer>();
        rootRenderer.material.SetColor("_Color", Color.blue);

        // Iterate through root's children, 
        for (int i = 0; i < root.Children.Count; i++) {
            UnityFileSystemEntry child = root.Children[i];
            Vector3 position = (transform.position.y + 1) * vertices[UnityEngine.Random.Range(0, vertices.Length)];
            position.y += transform.position.y;

            spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spheres[i].transform.position = position;
            spheres[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Renderer renderer = spheres[i].GetComponent<Renderer>();
            renderer.material.SetColor("_Color", child.EntryType == UnityFileSystemEntry.Type.File ? Color.red : Color.blue);
        }
    }

    // Update is called once per frame
    void Update() {
        foreach (GameObject sphere in spheres) {
            if (sphere != null) {
                sphere.transform.RotateAround(transform.position, Vector3.down, 1.25f * Time.deltaTime);
                sphere.transform.RotateAround(transform.position, Vector3.left, 1.25f * Time.deltaTime);
            }
        }
    }
}
