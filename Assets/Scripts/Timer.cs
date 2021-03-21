using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] float cooldown = 4;
    float realtimeCooldown;
    [SerializeField] TextMeshPro timerLabel;
    public bool isRunning { get; private set; }
    public event Action onExpired;
    public bool isExpiring { get; private set; }
    [SerializeField] float expiringEdge;
    [SerializeField] List<GameObject> eggStatuses;
    [SerializeField] AudioClip timeExiring;
    [SerializeField] AudioClip timerRing;
    [SerializeField] AudioSource timerAudio;
    // Start is called before the first frame update
    void Start()
    {
        realtimeCooldown = cooldown;
        timerLabel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timerLabel.text = Mathf.Round(realtimeCooldown).ToString();
        
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
            timerAudio.Stop();
            timerAudio.pitch = Time.timeScale;
            timerAudio.PlayOneShot(timerRing);
            SetEggStatus(EggStatus.Exploded);

            if (onExpired!= null)
            {
                onExpired();
            }
        }
    }

    public void StartTimer()
    {
        timerLabel.gameObject.SetActive(true);
        
        isRunning = true;
    }

    public void StopTimer()
    {

        timerAudio.Stop();
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
        if (isHighlighted)
        {
            Debug.Log("timer expiring");
            if (!timerAudio.isPlaying)
            {
                timerAudio.PlayOneShot(timeExiring);
                timerAudio.pitch = Time.timeScale;
            }
            SetEggStatus(EggStatus.Bouble);
        }
        else
        {
            timerAudio.Stop();
            timerAudio.pitch = Time.timeScale;
            SetEggStatus(EggStatus.Normal);
        }

        timerLabel.fontSize = isHighlighted? 150 : 100;
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

    void SetEggStatus(EggStatus status)
    {
        if(status == EggStatus.Bouble)
        {
            timerLabel.rectTransform.localPosition = new Vector3(1.759f, 1.91f, -0.164f);
        }
        else
        {
            timerLabel.rectTransform.localPosition = new Vector3(1.759f, 1.91f, 0.263f);
        }
        eggStatuses.ForEach(e => e.SetActive(false));

        int index = (int)status;
        eggStatuses[index].SetActive(true);
    }
}

enum EggStatus
{
    Normal = 0,
    Bouble = 1,
    Exploded = 2
}