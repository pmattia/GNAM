using Assets.Scripts.Interfaces;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class BonusSpawner : GameObjectSpawner
    {
        [SerializeField] SnapZone snapZone;
        [SerializeField] GameObject highlightPrefab;
        [SerializeField] Light highlight;
        [SerializeField] AudioSource audioSource;

        public GameObject[] bonuses;
        Vector3 originalHighlightScale;
        public bool HasBonusToTake { get {
                return snapZone.HeldItem != null;
            } 
        }

        private void Start()
        {
            highlightPrefab.SetActive(false);
            originalHighlightScale = highlightPrefab.transform.localScale;
        }

        public void SpawnBonus()
        {
            if (lastGameobject != null)
            {
                var lastAutodestroyer = lastGameobject.GetComponent<Autodestroy>();
                if (lastAutodestroyer != null)
                {
                    Destroy(lastGameobject);
                }
            }

            var modifier = bonuses[UnityEngine.Random.Range(0, bonuses.Length)];
            lastGameobject = Instantiate(modifier.gameObject, transform.position, Quaternion.identity);

            var autodestroyer = lastGameobject.GetComponent<Autodestroy>();
            if (autodestroyer == null)
            {
                autodestroyer = lastGameobject.AddComponent<Autodestroy>();
                autodestroyer.Countdown = 10;
            }

            snapZone.GrabGrabbable(lastGameobject.GetComponent<Grabbable>());

            audioSource.Play();
            StartCoroutine(FlashHighlight());
        }

        void highLightIterator(float value)
        {
            highlight.intensity = value;
           // Debug.Log(value);
        }


        IEnumerator FlashHighlight()
        {
            highlightPrefab.SetActive(true);
            var flashcount = 5;
            do
            {
                var turnOn = StartCoroutine(LerpIterator(1, 0, 10, highLightIterator));
                yield return turnOn;
                var turnOff = StartCoroutine(LerpIterator(1, 10, 0, highLightIterator));
                yield return turnOff;
                flashcount--;
            }
            while (flashcount > 0 && HasBonusToTake);
            highlightPrefab.SetActive(false);
        }

        IEnumerator LerpIterator(float animationDuration, float initialValue, float finalValue, Action<float> command)
        {
            float progress = 0;

            while (progress <= animationDuration)
            {
                var value = Mathf.Lerp(initialValue, finalValue, progress);
                command(value);
             //   Debug.Log($"{progress} {initialValue} {finalValue} = {value}");

                progress += Time.deltaTime;
                yield return null;
            }
            command(finalValue);

        }
    }
}
