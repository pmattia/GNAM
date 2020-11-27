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

        eater.Hands.ChangeHandsModel(index);
    }
}
