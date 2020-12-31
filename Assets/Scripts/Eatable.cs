﻿using Assets.Scripts;
using Assets.Scripts.Interfaces;
using Assets.Scripts.ScriptableObjects;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Eatable : MonoBehaviour
{
    public event Action<EaterDto> onEated;

    public List<GnamModifier> modifiers = new List<GnamModifier>();
    public float eatTime = 3f;
    public List<GameObject> grabPoints = new List<GameObject>();

    public void Eat(EaterDto eater)
    {
        if (onEated != null)
        {
            onEated(eater);
        }

        grabPoints.ForEach(Destroy);
        Destroy(gameObject);
    }

    public List<GnamModifier> GetModifiers()
    {
        return modifiers;
    }
}
