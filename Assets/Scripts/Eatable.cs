using Assets.Scripts;
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
    public event Action onExploded;

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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision);
    //    CheckProjectile(collision.gameObject);
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log(other);
    //    CheckProjectile(other.gameObject);
    //}

    private void CheckProjectile(GameObject projectile)
    {
        if (projectile.GetComponent<Projectile>() != null)
        {
            if (onExploded != null)
            {
                onExploded();
            }
            Destroy(gameObject);
        }
    }
}
