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
    public bool isRunning { get; private set; }
    public event Action onExpired;
    public bool isExpiring { get; private set; }
    [SerializeField] float expiringEdge;
    // Start is called before the first frame update
    void Start()
    {
        realtimeCooldown = cooldown;
        foreach (var timer in timers)
        {
            timer.gameObject.SetActive(false);
        }
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
        if (realtimeCooldown > 0 && isRunning)
        {
            realtimeCooldown -= Time.deltaTime;
            isExpiring = realtimeCooldown < expiringEdge;
        }
        else if (realtimeCooldown <= 0 && isRunning)
        {
            isExpiring = false;
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
        foreach(var timer in timers)
        {
            timer.gameObject.SetActive(true);
        }
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        isRunning = false;
        realtimeCooldown = 0;
    }

    public int GetResidueSeconds()
    {
        return Mathf.CeilToInt(realtimeCooldown);
    }

    public void Highligh(bool isHighlighted)
    {
        foreach (var timer in timers)
        {
            timer.fontSize = isHighlighted? 150 : 100;
        }
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
