﻿using BNG;
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
        public HandModelSelector handModelSelecter;

        public List<Action<EaterDto>> modifierActions = new List<Action<EaterDto>>();
        List<MouthModifierObject> mouthModifiers = new List<MouthModifierObject>();
        List<HandsModifierObject> handsModifiers = new List<HandsModifierObject>();

        //public event Action<Eatable> onBite;
        public event Action onSwallow;

        private EaterDto _eater;
        public EaterDto Eater { 
            get {
                if (_eater != null) return _eater;

                _eater = new EaterDto(this, handModelSelecter);
                return _eater;
            } 
        }

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
                    mouthModifiers.AddRange(eatable.GetMouthModifiers());
                    handsModifiers.AddRange(eatable.GetHandsModifiers());

                    audioSource.PlayOneShot(gnamAudio);
                    StartCoroutine(WaitToSwallow(eatable));

                    eatable.Eat(Eater);
                }
            }
        }
        IEnumerator WaitToSwallow(Eatable eatable)
        {
            Debug.Log("not iteractive");
            isEating = true;

            audioSource.clip = crunchingAudio;
            audioSource.Play();

            yield return new WaitForSeconds(eatable.eatTime);

            audioSource.Stop();

            Debug.Log("iteractive");
            isEating = false;

            foreach(var modifier in mouthModifiers)
            {
                modifier.Activate(this);
            }
            mouthModifiers.Clear();

            foreach (var modifier in handsModifiers)
            {
                modifier.Activate(handModelSelecter);
            }
            handsModifiers.Clear();

            foreach(var modifier in modifierActions)
            {
                modifier(Eater);
            }
            modifierActions.Clear();
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
