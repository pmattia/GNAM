using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsEatableModifier : BaseModifier
{
    public GameObject eatablePrefab;

    public override void Activate(EaterDto eater)
    {
        Debug.Log("eatable");
        foreach(Transform model in eater.HandsSelector.LeftHandGFXHolder.transform)
        {
            Destroy(model.gameObject);
        }
        var particleGameobjectL = Instantiate(eatablePrefab, eater.HandsSelector.LeftHandGFXHolder.transform.position, eater.HandsSelector.LeftHandGFXHolder.transform.rotation);
        particleGameobjectL.transform.parent = eater.HandsSelector.LeftHandGFXHolder.transform;

        foreach (Transform model in eater.HandsSelector.RightHandGFXHolder.transform)
        {
            Destroy(model.gameObject);
        }
        var particleGameobjectR = Instantiate(eatablePrefab, eater.HandsSelector.RightHandGFXHolder.transform.position, eater.HandsSelector.RightHandGFXHolder.transform.rotation);
        particleGameobjectR.transform.parent = eater.HandsSelector.RightHandGFXHolder.transform;
    }
}
   
