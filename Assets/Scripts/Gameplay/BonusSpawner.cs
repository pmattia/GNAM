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

        List<GameObject> bonus = new List<GameObject>();
        [SerializeField] List<GameObject> lowLevelBonus;
        [SerializeField] List<GameObject> midLevelBonus;
        [SerializeField] List<GameObject> highLevelBonus;

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

        public GameObject GetBonus(Difficulty difficulty)
        {
            bonus.Clear();
            if (difficulty >= Difficulty.Low)
            {
                bonus.AddRange(lowLevelBonus);
            }
            if (difficulty >= Difficulty.Mid)
            {
                bonus.AddRange(midLevelBonus);
                bonus.AddRange(midLevelBonus);
            }
            if (difficulty >= Difficulty.High)
            {
                bonus.AddRange(highLevelBonus);
                bonus.AddRange(highLevelBonus);
                bonus.AddRange(highLevelBonus);
            }

            var bonusGameobject = bonus[UnityEngine.Random.Range(0, bonus.Count())];

            return bonusGameobject;
        }

        public GameObject SpawnBonus(Difficulty difficulty)
        {
            var bonusGameobject = GetBonus(difficulty);

            return InstantiateBonusPrefab(bonusGameobject);
        }

        public GameObject SpawnBonus(GameObject bonus)
        {
            return InstantiateBonusPrefab(bonus);
        }

        GameObject InstantiateBonusPrefab(GameObject bonusPrefab)
        {
            RemoveLastBonus();

            lastGameobject = Instantiate(bonusPrefab, transform.position, Quaternion.identity);

            AttachAutodestroyer(lastGameobject);

            SnapBonusAndHighlight(lastGameobject);

            return lastGameobject;
        }

        void RemoveLastBonus()
        {
            if (lastGameobject != null)
            {
                var lastAutodestroyer = lastGameobject.GetComponent<Autodestroy>();
                if (lastAutodestroyer != null)
                {
                    Destroy(lastGameobject);
                }
            }
        }

        void SnapBonusAndHighlight(GameObject bonus)
        {
            snapZone.GrabGrabbable(bonus.GetComponent<Grabbable>());

            audioSource.Play();
            StartCoroutine(FlashHighlight());
        }

        void AttachAutodestroyer(GameObject bonus)
        {
            var autodestroyer = bonus.GetComponent<Autodestroy>();
            if (autodestroyer == null)
            {
                autodestroyer = bonus.AddComponent<Autodestroy>();
                autodestroyer.Countdown = 10;
            }

        }


        void highLightIterator(float value)
        {
            //todo: to remove because is useless
           // highlight.intensity = value;
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
