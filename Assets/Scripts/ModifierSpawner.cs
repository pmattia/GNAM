using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ModifierSpawner : MonoBehaviour
{
    public GameObject[] modifiersRepository;
    public float cooldown = 4;
    float realtimeCooldown;
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
        realtimeCooldown = cooldown;
        var modifier = modifiersRepository[Random.Range(0, modifiersRepository.Length)];
        currentModifier = Instantiate(modifier.gameObject, transform.position, Quaternion.identity);
        audioSource.Play();
    }
}
