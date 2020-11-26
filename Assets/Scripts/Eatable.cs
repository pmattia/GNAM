using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Eatable : MonoBehaviour
{
    public event Action<EaterDto> onEated;
    public List<VomitModifierBuilder> mouthModifiers = new List<VomitModifierBuilder>();
    public List<HandsSwapperBuilder> handsModifiers = new List<HandsSwapperBuilder>();
    public float eatTime = 3f;

    public void Eat(EaterDto eater)
    {
        if (onEated != null)
        {
            onEated(eater);
        }

        Destroy(gameObject);
    }

    public List<VomitModifierBuilder> GetMouthModifiers()
    {
        return mouthModifiers;
    }

    public List<HandsSwapperBuilder> GetHandsModifiers()
    {
        return handsModifiers;
    }
}
