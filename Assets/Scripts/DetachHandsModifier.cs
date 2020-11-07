using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachHandsModifier : BaseModifier
{
    public override void Activate(EaterDto eater)
    {
        eater.HandsSelector.LeftHandGFXHolder.parent = null;

        eater.HandsSelector.RightHandGFXHolder.parent = null;

    }
}
