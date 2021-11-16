using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FileRing 
{
    public List<GameObject> FileObjects;
    public GameObject RingObject;
    public InputActionAsset ActionsAsset;

    private UnityDirectory Directory;
    private float Radius;

    public FileRing(ref UnityDirectory directory)
    {
        Directory = directory;
        Radius = directory.width;

        // Dynamically add rotate action script to Ring GameObject
        RingObject = new GameObject();
        RingObject.name = "Ring \"" + Directory.Name + "\"";
        ActionsAsset = new XRIDefaultInputActions().asset;
        InputAction inputAction = ActionsAsset.FindAction("HFS CustomActions/Rotate");
        RotateAction rotateScript = RingObject.AddComponent<RotateAction>();
        rotateScript.rotateReference = InputActionReference.Create(inputAction);

        FileObjects = new List<GameObject>();
    }

    public void PlaceFileRing(Vector3 position)
    {
        RingObject.transform.position = position;

        float arcLength = (360 / (Directory.Children.Count + 20));
        float centerOffset = 90 - (arcLength * (Directory.Children.Count - 1) * 0.5f);
        float distance = Radius * 0.333f;
        int textOffset = 0;

        for (int i = 0; i < Directory.Children.Count; i++) 
        {
            UnityFileSystemEntry child = Directory.Children[i];

            float degrees = (i * arcLength) + centerOffset;
            float radians = degrees * Mathf.Deg2Rad;
            Vector3 circlePos = new Vector3(Mathf.Cos(radians) * distance, 1, Mathf.Sin(radians) * distance);

            GameObject objPrefab = Resources.Load("File") as GameObject;
            GameObject fileSphere = GameObject.Instantiate(objPrefab) as GameObject;

             //GameObject fileSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fileSphere.transform.parent = RingObject.transform;
            fileSphere.transform.localPosition = circlePos;
            fileSphere.transform.localScale = new Vector3(1f, 1f, 1f);

            //Renderer renderer = fileSphere.GetComponent<Renderer>();
            //renderer.material.SetColor("_Color", child.EntryType == UnityFileSystemEntry.Type.File ? Color.red : Color.blue);

            //create new object to hold text
            GameObject txtHolder = new GameObject();

            //make fileSphere parent of txtholder
            txtHolder.transform.parent = fileSphere.transform;
            txtHolder.name = "Text Holder";

            //create text mesh
            TextMesh textMesh = txtHolder.AddComponent<TextMesh>();
            textMesh.text = child.Name;
            textMesh.characterSize = (float)0.05;

            //Set postion of the TextMesh with offset
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.transform.position = new Vector3(fileSphere.transform.position.x, fileSphere.transform.position.y + textOffset, fileSphere.transform.position.z);

            FileObjects.Add(fileSphere);

        }
    }

    public void EnableActions()
    {
        ActionsAsset.Enable();
    }

    public void DisableActions() 
    {
        ActionsAsset.Disable();
    }
}
