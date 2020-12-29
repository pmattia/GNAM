using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameObjectSpawner : MonoBehaviour
{
    public GameObject[] objects;
    GameObject currentModifier;
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

    public void SpawnNewBonus()
    {
        if(currentModifier != null)
        {
            Destroy(currentModifier);
        }
        var modifier = objects[Random.Range(0, objects.Length)];
        currentModifier = Instantiate(modifier.gameObject, transform.position, Quaternion.identity);
        audioSource.Play();
    }
}
