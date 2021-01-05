using Assets.Scripts.ScriptableObjects;
using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HandsDetachModifier : GnamModifier
{
    public float duration = 3;
    Transform prevParentL;
    Transform prevParentR;
    public override void Activate(EaterDto eater)
    {
        var leftHandHolder = eater.Hands.LeftHandHolder;
        var rightHandHolder = eater.Hands.RightHandHolder;

        prevParentL = leftHandHolder.parent;
        leftHandHolder.parent = null;

        prevParentR = rightHandHolder.parent;
        rightHandHolder.parent = null;

        eater.Hands.StartCoroutine(WaitToDeactivate(eater, duration));
    }

    public override void Deactivate(EaterDto eater)
    {
        eater.Hands.LeftHandHolder.SetParent(prevParentL);
        eater.Hands.LeftHandHolder.localPosition = Vector3.zero;
        eater.Hands.LeftHandHolder.localRotation = Quaternion.identity;

        eater.Hands.RightHandHolder.SetParent(prevParentR);
        eater.Hands.RightHandHolder.localPosition = Vector3.zero;
        eater.Hands.RightHandHolder.localRotation = Quaternion.identity;
    }
}