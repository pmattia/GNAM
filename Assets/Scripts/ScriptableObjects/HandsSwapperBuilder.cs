using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using Assets.Scripts;

[CreateAssetMenu]
public class HandsSwapperBuilder : ScriptableObject
{
    public int handModelIndex;
    public virtual void Activate(HandModelSelector hands)
    {
        int index;
        if (handModelIndex != -1)
        {
            index = handModelIndex;
        }
        else
        {
            index = Random.Range(0, hands.LeftHandGFXHolder.childCount);
        }
       
        hands.ChangeHandsModel(index);
    }
}
