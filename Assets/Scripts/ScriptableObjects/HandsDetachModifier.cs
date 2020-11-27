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

        eater.Hands.StartCoroutine(WaitToReattach(leftHandHolder, rightHandHolder));
    }

    IEnumerator WaitToReattach(Transform leftHandHolder, Transform rightHandHolder)
    {
        yield return new WaitForSeconds(duration);

        leftHandHolder.SetParent(prevParentL);
        leftHandHolder.localPosition = Vector3.zero;
        leftHandHolder.localRotation = Quaternion.identity;

        rightHandHolder.SetParent(prevParentR);
        rightHandHolder.localPosition = Vector3.zero;
        rightHandHolder.localRotation = Quaternion.identity;

    }
}