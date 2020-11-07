using Assets.Scripts;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Food : GrabbableEvents
{
    public List<Eatable> eatableParts;
    public event Action<EaterDto> onEated;
    public event Action<EaterDto> onBited;

    public virtual void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var eatable in eatableParts)
        {
            eatable.onEated += (eater) => PartEated(eater, eatable);
        }
    }

    private void PartEated(EaterDto eater, Eatable eated)
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
    public override void OnGrab(Grabber grabber)
    {

    }


}
