using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using Assets.Scripts;
using Assets.Scripts.ScriptableObjects;

[CreateAssetMenu]
public class HandsSwapperModifier : GnamModifier
{
    public int handModelIndex;

    int prevLeftHandIndex;
    int prevRightHandIndex;

    public float duration = 3;

    public override void Activate(EaterDto eater)
    {
        int index;
        if (handModelIndex != -1)
        {
            index = handModelIndex;
        }
        else
        {
            index = Random.Range(0, eater.Hands.ModelCount);
        }

        prevLeftHandIndex = eater.Hands.GetLeftHandIndex();
        prevRightHandIndex = eater.Hands.GetRightHandIndex();

        eater.Hands.ChangeHandsModel(index);

        eater.Mouth.StartCoroutine(WaitToDeactivate(eater, duration));
    }

    public override void Deactivate(EaterDto eater)
    {
        eater.Hands.DisableLeftHand();
        eater.Hands.EnableLeftHand(prevLeftHandIndex);

        eater.Hands.DisableRightHand();
        eater.Hands.EnableRightHand(prevLeftHandIndex);
    }

   
}
