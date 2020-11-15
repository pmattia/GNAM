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
    public TextMeshPro timer;
    GameObject currentModifier;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //InvokeRepeating("SpawnNewBonus", cooldown, cooldown);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer != null)
        {
            timer.text = Mathf.Round(realtimeCooldown).ToString();
        }
        if (realtimeCooldown > 0)
        {
            //Debug.Log(realtimeFoodbagCooldown);
            realtimeCooldown -= Time.deltaTime;
        }
        else if (realtimeCooldown <= 0)
        {
            realtimeCooldown = 0;
        }
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
