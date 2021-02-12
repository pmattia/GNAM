using Assets.Scripts;
using Assets.Scripts.Interfaces;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Food : MonoBehaviour
{
    public List<Eatable> eatableParts;
    public event Action<EaterDto> onEated;
    public event Action<EaterDto> onBited;
    public FoodFamily foodFamily;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var eatable in eatableParts)
        {
            eatable.onEated += (eater) => PartEated(eater, eatable);
        }
    }

    void PartEated(EaterDto eater, Eatable eated)
    {
        Destroy(eated.gameObject);
        if (onBited != null)
        {
            onBited(eater);
        }
        eatableParts.Remove(eated);

        Debug.Log($"eatables: {eatableParts.Count()}");
        if (eatableParts.Count() == 0)
        {
            if (onEated != null)
            {
                onEated(eater);
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckProjectile(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckProjectile(other.gameObject);
    }

    private void CheckProjectile(GameObject projectile)
    {
        if (projectile.GetComponent<Projectile>() != null)
        {
            if (projectile.GetComponent<GnamModifierProjectile>() == null
                && projectile.GetComponent<GnamDuplicatorProjectile>() == null)
            {
                Destroy(gameObject);
            }
        }
    }

    public enum FoodFamily
    {
        Vegetable,
        Fruit,
        Meat,
        Carbo,
        Candy,
        Fish
        //Fat
    }
    
}
