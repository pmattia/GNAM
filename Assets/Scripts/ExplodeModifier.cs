using Assets.Scripts;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExplodeModifier: BaseModifier
{
    public AudioClip explodeClip;
    public GameObject explodeParticle;

    public override void Activate(EaterDto eater)
    {
        eater.Mouth.PlaySound(explodeClip);
        foreach (Transform model in eater.HandsSelector.LeftHandGFXHolder.transform)
        {
            Destroy(model.gameObject);
        }
        var particleGameobjectL = Instantiate(explodeParticle, eater.HandsSelector.LeftHandGFXHolder.transform.position, eater.HandsSelector.LeftHandGFXHolder.transform.rotation);
        particleGameobjectL.transform.parent = eater.HandsSelector.LeftHandGFXHolder.transform;

        foreach (Transform model in eater.HandsSelector.RightHandGFXHolder.transform)
        {
            Destroy(model.gameObject);
        }
        var particleGameobjectR = Instantiate(explodeParticle, eater.HandsSelector.RightHandGFXHolder.transform.position, eater.HandsSelector.RightHandGFXHolder.transform.rotation);
        particleGameobjectR.transform.parent = eater.HandsSelector.RightHandGFXHolder.transform;
    }
}
