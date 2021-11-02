using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DirectoryInteractable : XRSimpleInteractable
{
    public Vector3 Position;
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

    // When an interactable directory node is selected with the grip button, disable and enable rings and teleport player
    protected override void OnSelectExited(SelectExitEventArgs args) 
    {
        RingToDisable.DisableActions();
        Rig.MoveCameraToWorldLocation(new Vector3(Position.x, Rig.cameraInRigSpaceHeight, Position.z));
        RingToEnable.EnableActions();
    }
}
