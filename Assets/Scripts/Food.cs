using Assets.Scripts;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class Food : GrabbableEvents
{
    public List<Eatable> eatableParts;
    public event Action<Mouth> onEated;
    public event Action<Mouth> onBited;

    public virtual void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var eatable in eatableParts)
        {
            eatable.onEated += (mouth) => PartEated(mouth, eatable);
        }
    }

    private void PartEated(Mouth mouth, Eatable eated)
    {
        Destroy(eated.gameObject);
        if (onBited != null)
        {
            onBited(mouth);
        }
        eatableParts.Remove(eated);

        Debug.Log($"eatables: {eatableParts.Count()}");
        if (eatableParts.Count() == 0)
        {
            if (onEated != null)
            {
                onEated(mouth);
            }
            Destroy(gameObject);
        }
    }
    public override void OnGrab(Grabber grabber)
    {

    }


}
