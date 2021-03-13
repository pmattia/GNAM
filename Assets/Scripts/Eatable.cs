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
    [SerializeField] float eatDuration = .2f;
    public List<GameObject> grabPoints = new List<GameObject>();
    public bool IsEatable = true;
    public float EatDuration { get { return eatDuration; } }

    private void Start()
    {
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

    public void AddModifiers(List<GnamModifier> newModifiers)
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null && newModifiers.Count > 0)
        {
            //3DEE15
           // Debug.Log($"COLORA {name}");
            renderer.material.SetColor("Color_B5C1F6F5", new Color32(0x3D, 0xEE, 0x15, 0));
            
            modifiers.AddRange(newModifiers);
        }
    }

    public void SetModifiers(List<GnamModifier> newModifiers)
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null && newModifiers.Count > 0)
        {
            renderer.material.SetColor("Color_B5C1F6F5", new Color32(0x3D, 0xEE, 0x15, 0));

            modifiers.Clear();
            modifiers.AddRange(newModifiers);
        }
    }
}
