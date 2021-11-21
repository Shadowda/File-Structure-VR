using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DirectoryInteractable : XRSimpleInteractable
{
    public Vector3 TargetPosition;
    public GameObject TargetPlatform;

    public FileRing RingToDisable;
    public FileRing RingToEnable;

    private XRRig Rig;

    // On awake, find the XR Rig object for usage in teleportation
    protected override void Awake()
    {
        base.Awake();

        GameObject RigGameObject = GameObject.Find("XR Rig");
        Rig = RigGameObject.GetComponent<XRRig>();
    }

    // When an interactable directory node is hovered, highlight the target platform
    protected override void OnHoverEntered(HoverEnterEventArgs args) 
    {
        base.OnHoverEntered(args);

        Renderer renderer = TargetPlatform.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.red);
    }

    // When an interactable directory node is unhovered, unhighlight the target platform
    protected override void OnHoverExited(HoverExitEventArgs args) 
    {
        base.OnHoverExited(args);

        Renderer renderer = TargetPlatform.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.white);
    }

    // When an interactable directory node is selected with the grip button, disable and enable rings and teleport player
    protected override void OnSelectExited(SelectExitEventArgs args) 
    {
        base.OnSelectExited(args);

        RingToDisable.DisableActions();
        Rig.MoveCameraToWorldLocation(new Vector3(TargetPosition.x, Rig.cameraInRigSpaceHeight, TargetPosition.z));
        RingToEnable.EnableActions();
    }
}
