using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Eatable : MonoBehaviour
{
    public event Action<EaterDto> onEated;
    public List<MouthModifierObject> mouthModifiers = new List<MouthModifierObject>();
    public List<HandsModifierObject> handsModifiers = new List<HandsModifierObject>();
    public float eatTime = 3f;

    public void Eat(EaterDto eater)
    {
        if (onEated != null)
        {
            onEated(eater);
        }

        Destroy(gameObject);
    }

    public List<MouthModifierObject> GetMouthModifiers()
    {
        return mouthModifiers;
    }

    public List<HandsModifierObject> GetHandsModifiers()
    {
        return handsModifiers;
    }
}
