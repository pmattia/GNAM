using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HandsDetachModifierBuilder : HandsSwapperBuilder
{
    public float duration = 3;
    Transform prevParentL;
    Transform prevParentR;
    public override void Activate(HandModelSelector hands)
    {
        prevParentL = hands.LeftHandGFXHolder.parent;
        hands.LeftHandGFXHolder.parent = null;

        prevParentR = hands.RightHandGFXHolder.parent;
        hands.RightHandGFXHolder.parent = null;

        hands.StartCoroutine(WaitToReattach(hands));
    }

    IEnumerator WaitToReattach(HandModelSelector hands)
    {
        
        yield return new WaitForSeconds(duration);
      
        hands.LeftHandGFXHolder.SetParent(prevParentL);
        hands.LeftHandGFXHolder.localPosition = Vector3.zero;
        hands.LeftHandGFXHolder.localRotation = Quaternion.identity;

        hands.RightHandGFXHolder.SetParent(prevParentR);
        hands.RightHandGFXHolder.localPosition = Vector3.zero;
        hands.RightHandGFXHolder.localRotation = Quaternion.identity;

    }
}