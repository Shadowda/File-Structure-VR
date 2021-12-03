using UnityEngine;
using UnityEngine.InputSystem;

public class SortAction : MonoBehaviour
{
    public InputActionReference sortNextReference = null;
    public InputActionReference sortLastReference = null;

    public FileRing fileRing;

    public void OnInputActionReferenceAssigned() 
    {
        sortNextReference.action.started += TriggerNextSort;
        sortLastReference.action.started += TriggerLastSort;
    }

    private void OnDestroy()
    {
        sortNextReference.action.started -= TriggerNextSort;
        sortLastReference.action.started -= TriggerLastSort;
    }

    private void TriggerNextSort(InputAction.CallbackContext context) 
    {
        fileRing.SelectNextSort(1);
    }

    private void TriggerLastSort(InputAction.CallbackContext context)
    {
        fileRing.SelectNextSort(-1);
    }
}
