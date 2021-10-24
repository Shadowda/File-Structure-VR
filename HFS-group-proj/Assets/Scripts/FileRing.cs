using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileRing : MonoBehaviour
{
    public List<GameObject> FileObjects;
    public GameObject RingObject;

    private UnityDirectory Directory;
    private float MaxRotation;

    public FileRing(ref UnityDirectory directory)
    {
        Directory = directory;
        RingObject = new GameObject();
        FileObjects = new List<GameObject>();
    }

    public void PlaceFileRing(Vector3 position)
    {
        RingObject.transform.position = position;

        float arcLength = (360 / (Directory.Children.Count + 20));
        float centerOffset = 90 - (arcLength * (Directory.Children.Count - 1) * 0.5f);
        float distance = Directory.Children.Count * 0.2f;
        for (int i = 0; i < Directory.Children.Count; i++) 
        {
            UnityFileSystemEntry child = Directory.Children[i];

            float degrees = (i * arcLength) + centerOffset;
            float radians = degrees * Mathf.Deg2Rad;
            Vector3 circlePos = new Vector3(Mathf.Cos(radians) * distance, 1, Mathf.Sin(radians) * distance);

            GameObject fileSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fileSphere.transform.parent = RingObject.transform;
            fileSphere.transform.localPosition = circlePos;
            fileSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            Renderer renderer = fileSphere.GetComponent<Renderer>();
            renderer.material.SetColor("_Color", child.EntryType == UnityFileSystemEntry.Type.File ? Color.red : Color.blue);

            FileObjects.Add(fileSphere);
        }

        MaxRotation = centerOffset;
    }
}
