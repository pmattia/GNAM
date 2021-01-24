using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float cooldown = 4;
    float realtimeCooldown;
    public TextMeshPro[] timers;
    bool isPlaying = false;
    public event Action onExpired;
    // Start is called before the first frame update
    void Start()
    {
        realtimeCooldown = cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (timers.Length > 0)
        {
            foreach (var timer in timers) { 
                timer.text = Mathf.Round(realtimeCooldown).ToString();
            }
        }
        if (realtimeCooldown > 0 && isPlaying)
        {
            realtimeCooldown -= Time.deltaTime;
        }
        else if (realtimeCooldown <= 0 && isPlaying)
        {
            realtimeCooldown = 0;
            StopTimer();
            if(onExpired!= null)
            {
                onExpired();
            }
        }
    }

    public void StartTimer()
    {
        isPlaying = true;
    }

    public void StopTimer()
    {
        isPlaying = false;
    }

    public void ResetTimer()
    {
        isPlaying = false;
        realtimeCooldown = 0;
    }

    public void SetTimer(float cooldown)
    {
        this.cooldown = cooldown;
        this.realtimeCooldown = cooldown;
    }

    public float AddTime(float time) {
        realtimeCooldown += time;
        return realtimeCooldown;
    }
}
