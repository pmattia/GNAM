using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallHandsModifier : BaseModifier
{
    public override void Activate(EaterDto eater)
    {
        eater.HandsSelector.LeftHandGFXHolder.parent = null;
        var rigidBodyL = eater.HandsSelector.LeftHandGFXHolder.gameObject.AddComponent<Rigidbody>();
        rigidBodyL.useGravity = true;
        rigidBodyL.mass = 1;

        eater.HandsSelector.RightHandGFXHolder.parent = null;
        var rigidBodyR = eater.HandsSelector.RightHandGFXHolder.gameObject.AddComponent<Rigidbody>();
        rigidBodyR.useGravity = true;
        rigidBodyR.mass = 1;
    }
}
