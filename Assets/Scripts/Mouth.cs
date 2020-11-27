﻿using Assets.Scripts.ScriptableObjects;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace Assets.Scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class Mouth : MonoBehaviour, IMouthController
    {
        public bool isEnabled = true;

        bool isEating = false;
        bool isTurbo = false;

        public AudioSource audioSource;
        public AudioClip crunchingAudio;
        public AudioClip gnamAudio;
        public HandModelSelector handModelSelecter;
        public IHandsController handsController;

        List<GnamModifier> modifiers = new List<GnamModifier>();

        //public event Action<Eatable> onBite;
        public event Action onSwallow;

        private EaterDto _eater;
        public EaterDto Eater { 
            get {
                if (_eater != null) return _eater;
                
                _eater = new EaterDto(this, handsController);
                return _eater;
            }
        }
        public GameObject GameObject { get { return gameObject; } }

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            handsController = new VrifHandsControllerAdapter(handModelSelecter);
        }


        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("trigger " + other.gameObject.name);

            if ((isTurbo || !isEating) && isEnabled)
            {
                var eatable = other.GetComponent<Eatable>();
                if (eatable != null)
                {
                    modifiers.AddRange(eatable.GetModifiers());

                    audioSource.PlayOneShot(gnamAudio);
                    StartCoroutine(WaitToSwallow(eatable));

                    eatable.Eat(Eater);
                }
            }
        }
        IEnumerator WaitToSwallow(Eatable eatable)
        {
            isEating = true;

            audioSource.clip = crunchingAudio;
            audioSource.Play();

            yield return new WaitForSeconds(eatable.eatTime);

            audioSource.Stop();

            isEating = false;

            var tModifiers = new List<GnamModifier>();
            tModifiers.AddRange(modifiers);
            foreach (var modifier in tModifiers)
            {
                modifier.Activate(Eater);
                modifiers.Remove(modifier);
            }
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        public void EnableMouth()
        {
            isEnabled = true;
        }

        public void DisableMouth()
        {
            isEnabled = false;
        }

        public void EnableTurbo()
        {
            isTurbo = true;
        }

        public void DisableTurbo()
        {
            isTurbo = false;
        }

        public void DisableMouthForSeconds(float time)
        {
            isEnabled = false;
            StartCoroutine(WaitForEnable(time));
        }

        IEnumerator WaitForEnable(float time)
        {
            yield return new WaitForSeconds(time);
            isEnabled = true;
        }
    }
}
