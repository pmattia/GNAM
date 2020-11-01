using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Assets.Scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class Mouth : MonoBehaviour
    {
        [HideInInspector]
        public bool isEating = false;

        public AudioSource audioSource;
        public AudioClip crunchingAudio;
        public AudioClip gnamAudio;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();

        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("trigger " + other.gameObject.name);

            if (!isEating)
            {
                var eatable = other.GetComponent<Eatable>();
                if (eatable != null)
                {
                    eatable.Eat(this);
                    audioSource.PlayOneShot(gnamAudio);
                    StartCoroutine(WaitToEat(eatable.eatTime));
                }
            }
        }
        IEnumerator WaitToEat(float seconds)
        {
            Debug.Log("not iteractive");
            isEating = true;

            audioSource.clip = crunchingAudio;
            audioSource.Play();

            yield return new WaitForSeconds(seconds);

            audioSource.Stop();

            Debug.Log("iteractive");
            isEating = false;
        }
    }
}
