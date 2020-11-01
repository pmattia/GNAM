using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eatable : MonoBehaviour
{
    public event Action<Mouth> onEated;

    public float eatTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Eat(Mouth mouth)
    {
        if (!mouth.isEating)
        {
            if (onEated != null)
            {
                onEated(mouth);
            }
            Destroy(gameObject);
        }
    }
}
