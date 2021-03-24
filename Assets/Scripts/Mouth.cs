using Assets.Scripts.Interfaces;
using Assets.Scripts.ScriptableObjects;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using System.Linq;

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
        [SerializeField] SnapZone leftPistolHolder;
        [SerializeField] SnapZone rightPistolHolder;

        List<GnamModifier> modifiers = new List<GnamModifier>();
        List<GnamModifier> currentModifiers = new List<GnamModifier>();

        //public event Action<Eatable> onBite;
        public event Action onSwallow;

        private EaterDto _eater;
        public EaterDto Eater
        {
            get
            {
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

            leftPistolHolder.gameObject.SetActive(false);
            handsController.onLeftHandGrab += (grabbable) =>
            {
                if (grabbable.tag.Equals("Weapon"))
                {
                    leftPistolHolder.gameObject.SetActive(true);
                }
            };
            handsController.onLeftHandRelease += (grabbable) =>
            {
                StartCoroutine(DelayedCallback(.2f, () =>
                {
                    // Debug.Log($"drop left {leftPistolHolder.HeldItem != null}");
                    if (handsController.LeftGrabber.HeldGrabbable != null && handsController.LeftGrabber.HeldGrabbable.tag.Equals("Weapon"))
                    {
                        leftPistolHolder.gameObject.SetActive(true);
                    }
                    else
                    {
                        leftPistolHolder.gameObject.SetActive(leftPistolHolder.HeldItem != null);
                    }
                }));
            };

            rightPistolHolder.gameObject.SetActive(false);
            handsController.onRightHandGrab += (grabbable) =>
            {
                if (grabbable.tag.Equals("Weapon"))
                {
                    rightPistolHolder.gameObject.SetActive(true);
                }
            };
            handsController.onRightHandRelease += (grabbable) =>
            {
                StartCoroutine(DelayedCallback(.2f, () =>
                {
                    // Debug.Log($"drop right {rightPistolHolder.HeldItem != null}");
                    if (handsController.RightGrabber.HeldGrabbable != null && handsController.RightGrabber.HeldGrabbable.tag.Equals("Weapon"))
                    {
                        rightPistolHolder.gameObject.SetActive(true);
                    }
                    else
                    {
                        rightPistolHolder.gameObject.SetActive(rightPistolHolder.HeldItem != null);
                    }

                }));
            };

            timeController = new VrifTimeControllerAdapter(playerTimeController);
        }

        IEnumerator DelayedCallback(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
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

            var runtimeEatTime = Eater.Time.TimeSlowing ? eatable.EatDuration * Eater.Time.SlowTimeScale : eatable.EatDuration;
            yield return new WaitForSeconds(runtimeEatTime);

            audioSource.Stop();

            isEating = false;

            var tModifiers = new List<GnamModifier>();
            tModifiers.AddRange(modifiers);
            foreach (var modifier in tModifiers.Where(t => t != null))
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
