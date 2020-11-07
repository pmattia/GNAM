using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Eatable))]
public abstract class BaseModifier : MonoBehaviour
{
    Eatable eatable;
    void Awake()
    {

        eatable = GetComponent<Eatable>();
        eatable.onEated += OnEated;
    }

    void OnEated(EaterDto eater)
    {
        eater.Mouth.modifierActions.Add(Activate);
        //StartCoroutine(ExplodeWithDelay(eater, eatable.eatTime));
    }

    public abstract void Activate(EaterDto eater);
}
