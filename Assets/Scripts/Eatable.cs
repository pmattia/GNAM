using Assets.Scripts;
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
    public bool IsEatable = true;

    private void Start()
    {
        eatTime = 0.2f;
    }

    public void Eat(EaterDto eater)
    {
        //Debug.Log($"{name} {IsEatable}");
        if (IsEatable)
        {
            if (onEated != null)
            {
                onEated(eater);
            }

            grabPoints.ForEach(Destroy);
            Destroy(gameObject);
        }
    }

    public List<GnamModifier> GetModifiers()
    {
        return modifiers;
    }

    public void AddModifiers(List<GnamModifier> modifiers)
    {
        var renderer = GetComponent<Renderer>();
        //renderer.material.SetColor("_Color", Color.white);
        //renderer.material.mainTexture = null;
        if (renderer != null && modifiers.Count > 0)
        {
            //3DEE15
            
            renderer.material.SetColor("Color_B5C1F6F5", new Color32(0x3D, 0xEE, 0x15, 0));
            //   renderer.material = null;
            modifiers.AddRange(modifiers);
        }
    }

}
