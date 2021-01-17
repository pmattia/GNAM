using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameObjectSpawner : MonoBehaviour
{
    public GameObject[] objects;
    GameObject lastGameobject;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnNewObject()
    {
        if (lastGameobject != null)
        {
            var lastAutodestroyer = lastGameobject.GetComponent<Autodestroy>();
            if (lastAutodestroyer != null)
            {
                Destroy(lastGameobject);
            }
        }

        var modifier = objects[UnityEngine.Random.Range(0, objects.Length)];
        lastGameobject = Instantiate(modifier.gameObject, transform.position, Quaternion.identity);

        var autodestroyer = lastGameobject.GetComponent<Autodestroy>();
        if (autodestroyer == null)
        {
            autodestroyer = lastGameobject.AddComponent<Autodestroy>();
            autodestroyer.Countdown = 5;
        }

        audioSource.Play();
    }

    public void SpawnPermanentObject(GameObject prefab, Action<EaterDto> onEated)
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
