using Assets.Scripts.Interfaces;
using Assets.Scripts.ScriptableObjects;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

namespace Assets.Scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class Mouth : MonoBehaviour, IMouthController
    {
        public bool isEnabled = true;

        bool isEating = false;
        bool isTurbo = false;

        AudioSource[] audiosources;
        AudioSource audioSource;
        AudioSource audioLoop;
        [SerializeField] AudioClip crunchingAudio;
        [SerializeField] AudioClip gnamAudio;
        [SerializeField] AudioClip chokeAudio;
        [SerializeField] HandModelSelector handModelSelecter;
        IHandsController handsController;
        [SerializeField] TimeController playerTimeController;
        ITimeController timeController;
        [SerializeField] GameObject briciolePrefab;

        List<GnamModifier> modifiers = new List<GnamModifier>();
        List<GnamModifier> currentModifiers = new List<GnamModifier>();

        //public event Action<Eatable> onBite;
        public event Action onSwallow;

        private EaterDto _eater;
        public EaterDto Eater { 
            get {
                if (_eater != null) return _eater;
                
                _eater = new EaterDto(this, handsController, timeController);
                return _eater;
            }
        }
        public GameObject GameObject { get { return gameObject; } }

        void Awake()
        {
            audiosources = GetComponents<AudioSource>();
            audioSource = audiosources[0];
            audioLoop = audiosources[1];
            handsController = new VrifHandsControllerAdapter(handModelSelecter);
            timeController = new VrifTimeControllerAdapter(playerTimeController);
        }


        private void OnTriggerEnter(Collider other)
        {
           // Debug.Log("collide " + other.name);
            if ((isTurbo || !isEating) && isEnabled)
            {
                var eatable = other.GetComponent<Eatable>();
                if (eatable != null && eatable.IsEatable)
                {
                    modifiers.AddRange(eatable.GetModifiers());

                    audioSource.PlayOneShot(gnamAudio);
                    StartCoroutine(WaitToSwallow(eatable));

                    eatable.Eat(Eater);
                }

                //if (other.GetComponent<Projectile>() != null) //sei morto
                //{
                //    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
                //}
            }
            else
            {
                if (!isTurbo && isEating && isEnabled)
                {
                    var eatable = other.GetComponent<Eatable>();
                    if (eatable != null)
                    {
                        //audioSource.PlayOneShot(chokeAudio);
                        //eventually add haptics
                    }
                }
            }
        }
        IEnumerator WaitToSwallow(Eatable eatable)
        {
            isEating = true;

            audioSource.clip = crunchingAudio;
            audioSource.Play();
           // Instantiate(briciolePrefab, transform.position, transform.rotation);

            var runtimeEatTime = Eater.Time.TimeSlowing? eatable.eatTime * Eater.Time.SlowTimeScale : eatable.eatTime;
            yield return new WaitForSeconds(runtimeEatTime);

            audioSource.Stop();

            isEating = false;

            var tModifiers = new List<GnamModifier>();
            tModifiers.AddRange(modifiers);
            foreach (var modifier in tModifiers)
            {
                currentModifiers.ForEach(m => m.Deactivate(Eater));
                currentModifiers.Clear();

                modifier.Activate(Eater);
                currentModifiers.Add(modifier);
                modifiers.Remove(modifier);
            }
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        public void PlayLoop(AudioClip clip)
        {
            audioLoop.clip = clip;
            audioLoop.Play();
        }

        public void StopSound()
        {
            audioSource.Stop();
            audioLoop.Stop();
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
