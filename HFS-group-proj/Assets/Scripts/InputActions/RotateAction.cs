using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAction : MonoBehaviour
{
    public InputActionReference rotateReference = null;

    private void Update() 
    {
        if (rotateReference != null) 
        {
            float value = rotateReference.action.ReadValue<float>();
            transform.Rotate(new Vector3(0, (5.0f * Time.deltaTime) * value, 0));
        }
    }
}
