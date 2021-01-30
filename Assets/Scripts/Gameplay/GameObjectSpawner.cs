using Assets.Scripts;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameObjectSpawner : MonoBehaviour
{
    
    protected GameObject lastGameobject;
    [SerializeField] protected AudioSource audioSource;
   


    public void SpawnObject(GameObject prefab, Action<EaterDto> onEated)
    {
        if (lastGameobject != null)
        {
            var lastAutodestroyer = lastGameobject.GetComponent<Autodestroy>();
            if (lastAutodestroyer != null)
            {
                Destroy(lastGameobject);
            }
        }

        lastGameobject = Instantiate(prefab, transform.position, Quaternion.identity);
        lastGameobject.GetComponent<Eatable>().onEated += onEated;

        audioSource.Play();
    }
}
