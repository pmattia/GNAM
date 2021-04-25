using Assets.Scripts.AI;
using Assets.Scripts.Interfaces;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

namespace Assets.Scripts.Gameplay
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] Foodbag foods;
        [SerializeField] Foodbag moreBitesFoods;
        [SerializeField] Foodbag modifiedFoods;
        [SerializeField] Foodbag objectiveFoods;
        [SerializeField] ObjectiveDto objective;
        [SerializeField] GameObject gun;
        [SerializeField] GameObject gunClip;
        [SerializeField] List<GameObject> mobs;
        [SerializeField] List<GameObject> randomBonuses;

        [SerializeField] List<GameObject> panels;
        [SerializeField] Animator panelAnimator;

        [SerializeField] BonusSpawner bonusSpawner;
        [SerializeField] Inventory inventory;
        [SerializeField] Billboard billboard;
        [SerializeField] GameObject instructionsPanel;
        [SerializeField] GameObject finishPanel;
        [SerializeField] GameObject startEatable;
        [SerializeField] GameObject chipPlate;
        [SerializeField] GameObjectSpawner commandSpawner;
        [SerializeField] AudioClip stepCompletedAudio;
        [SerializeField] AudioClip tutorialCompletedAudio;

        [SerializeField] bool skipTutorial = false;
        [SerializeField] bool forceTutorial = false;

        public event Action onStep1Completed;
        public event Action onStep2Completed;
        public event Action onStep3Completed;
        public event Action onStep4Completed;
        public event Action onStep5Completed;
        public event Action<GnamRaycastWeapon> onStep6Completed;
        public event Action onStep7Completed;
        public event Action onStep8Completed;
        List<GameObject> spawnedItems = new List<GameObject>();

        float stepCompletedDelay = .5f;


        private void Awake()
        {
          //  PlayerPrefs.SetInt(bestScoreKey, 0);


            var tutorialDone = PlayerPrefs.GetInt(GnamConstants.tutorialDoneKey);
            Debug.Log($"tutorial done = {tutorialDone}");
            if((tutorialDone > 0 || skipTutorial) && !forceTutorial)
            {
                GoToGameplay();
            }
        }

        private void Start()
        {
            InitTutorial();
        }

        void InitTutorial()
        {
            chipPlate.SetActive(true);
            instructionsPanel.SetActive(true);
            panels.ForEach(p => p.SetActive(false));

            InitStepCompletedActions();

            ClearStarter();
            spawnedItems.Add(commandSpawner.SpawnObject(startEatable, (eater) =>
            {
                CompletedStep(false);
                StartCoroutine(DelayedCallback(stepCompletedDelay, () => {
                    instructionsPanel.SetActive(false);

                    InitStep(0);

                    chipPlate.SetActive(false);
                    var foodbagObj = Instantiate(foods, chipPlate.transform.parent);
                    foodbagObj.transform.localPosition = Vector3.zero;
                    foodbagObj.transform.localRotation = Quaternion.identity;
                    foodbagObj.GetComponentInChildren<Foodbag>().onClear += () =>
                    {
                        CompletedStep();
                        StartCoroutine(DelayedCallback(stepCompletedDelay, () => {
                            onStep1Completed();
                        }));
                    };
                }));
                
            }));

            inventory.HideItems();
            panelAnimator.SetBool("show", true);
        }

        void InitStepCompletedActions()
        {
            onStep1Completed += () =>
            {
                Debug.Log("STEP 1 COMPLETED NORMAL FOOD");

                InitStep(1);

                FindObjectsOfType<Foodbag>().ToList().ForEach(f => Destroy(f.gameObject));
                var foodbagObj = Instantiate(moreBitesFoods, chipPlate.transform.parent);
                foodbagObj.transform.localPosition = Vector3.zero;
                foodbagObj.transform.localRotation = Quaternion.identity;
                foodbagObj.GetComponentInChildren<Foodbag>().onClear += () =>
                {
                    CompletedStep();
                    StartCoroutine(DelayedCallback(stepCompletedDelay, () => {
                        onStep2Completed();
                    }));
                };
            };

            onStep2Completed += () =>
            {
                Debug.Log("STEP 2 COMPLETED NORMAL FOOD");

                InitStep(1);

                FindObjectsOfType<Foodbag>().ToList().ForEach(f => Destroy(f.gameObject));
                var foodbagObj = Instantiate(modifiedFoods, chipPlate.transform.parent);
                foodbagObj.transform.localPosition = Vector3.zero;
                foodbagObj.transform.localRotation = Quaternion.identity;
                foodbagObj.GetComponentInChildren<Foodbag>().onClear += () =>
                {
                    CompletedStep();
                    StartCoroutine(DelayedCallback(stepCompletedDelay, () => {
                        onStep3Completed();
                    }));
                };
            };

            onStep3Completed += () =>
            {
                Debug.Log("STEP 3 COMPLETED MODIFIED FOOD");

                InitStep(2);

                FindObjectsOfType<Foodbag>().ToList().ForEach(f => Destroy(f.gameObject));
                var foodbagObj = Instantiate(objectiveFoods, chipPlate.transform.parent);
                foodbagObj.transform.localPosition = Vector3.zero;
                foodbagObj.transform.localRotation = Quaternion.identity;
                foodbagObj.GetComponentInChildren<Foodbag>().onFoodEated += (foodbagEater, food) =>
                {
                    billboard.AddFood(food.foodFamily);
                };

                var ret = new ObjectiveDto();
                ret.family = Food.FoodFamily.Candy;
                ret.toEat = 3;
                ret.bonus = gun;
                ret.cooldown = 999999999;

                billboard.AddObjective(ret);

                billboard.onObjectiveCompleted += (family, objectivesFamilies, bonus) =>
                {
                    CompletedStep();
                    var gunBonus = bonusSpawner.SpawnBonus(bonus);
                    StartCoroutine(DelayedCallback(stepCompletedDelay, () => {
                        onStep4Completed();
                    }));

                    var onGunGrab = new GrabberEvent();
                    var gunEvents = gunBonus.GetComponent<GrabbableUnityEvents>();

                    onGunGrab.AddListener((grabber) => {
                        CompletedStep();
                        StartCoroutine(DelayedCallback(stepCompletedDelay, () => {
                            gunEvents.onGrab.RemoveAllListeners();
                            onStep5Completed();
                        }));
                    });

                    gunEvents.onGrab = onGunGrab;
                };
            };

            onStep4Completed += () =>
            {
                Debug.Log("STEP 4 COMPLETED OBJECTIVE");

                InitStep(4);

                FindObjectsOfType<Foodbag>().ToList().ForEach(f => Destroy(f.gameObject));
                inventory.ShowItems();
            };

            onStep5Completed += () =>
            {
                Debug.Log("STEP 5 COMPLETED GUN TAKEN");

                InitStep(5);

                inventory.ShowItems();
                inventory.onSnap += OnGunSnappedToInventory;
            };

            onStep6Completed += (GnamRaycastWeapon gun) =>
            {
                Debug.Log("STEP 5 COMPLETED GUN SNAPPED TO INVENTORY");

                InitStep(6);

                inventory.SnapEmptyHolders(gunClip.GetComponent<GnamGrabbable>());

                var onGunCharged = new UnityEvent();
                onGunCharged.AddListener(() => {
                    CompletedStep();
                    StartCoroutine(DelayedCallback(stepCompletedDelay, () => {
                        gun.onWeaponChargedEvent.RemoveAllListeners();
                        onStep7Completed();
                    }));
                });

                gun.onWeaponChargedEvent = onGunCharged;
            };

            onStep7Completed += () =>
            {
                Debug.Log("STEP 7 COMPLETED GUN CHARGED");

                InitStep(7);

                mobs.ForEach(m => {
                    m.SetActive(true);
                    m.GetComponents<Collider>().ToList().ForEach(c => c.enabled = true);
                    var shoter = m.GetComponent<ShootAtTargets>();
                    shoter.onDeath += (killedmob) =>
                    {
                        bonusSpawner.SpawnBonus(randomBonuses[UnityEngine.Random.Range(0, randomBonuses.Count())]);

                        StartCoroutine(DelayedCallback(.1f, () =>
                        {
                            Debug.Log($"uccisi { mobs.Where(mob => mob == null).Count()} / {mobs.Count()}");

                            if (mobs.Where(mob => mob != null).Count() == 0)
                            {
                                CompletedStep();
                                StartCoroutine(DelayedCallback(stepCompletedDelay, () =>
                                {
                                    onStep8Completed();
                                }));
                            }
                        }));

                    };
                });
            };

            onStep8Completed += () =>
            {
                Debug.Log("STEP 8 COMPLETED MOB KILLED");
                panels.ForEach(p => p.SetActive(false));
                panelAnimator.SetBool("show", true);
                finishPanel.SetActive(true);

                FindObjectsOfType<GnamGrabbable>().ToList().ForEach(f => Destroy(f.gameObject));
                inventory.HideItems();

                chipPlate.SetActive(true);
                ClearStarter();
                spawnedItems.Add(commandSpawner.SpawnObject(startEatable, (eater) =>
                {
                    GoToGameplay();
                }));

                PlayerPrefs.SetInt(GnamConstants.tutorialDoneKey, 1);
            };
        }

        void GoToGameplay()
        {
            Analytics.CustomEvent("Tutorial_completed", new Dictionary<string, object>
            {
                { "time_elapsed", Time.timeSinceLevelLoad }
            });
            SceneManager.LoadSceneAsync(GnamConstants.rushScene);
        }

        void CompletedStep(bool playsound = true)
        {
            panelAnimator.SetBool("show", false);
            if (playsound)
            {
                VRUtils.Instance.PlaySpatialClipAt(stepCompletedAudio, transform.position, 1f, 0.5f);
            }
        }

        void InitStep(int stepIndex)
        {
            Debug.Log($"active step {stepIndex}");
            panels.ForEach(p => p.SetActive(false));
            panels[stepIndex].SetActive(true);
            panelAnimator.SetBool("show", true);
        }

        void OnGunSnappedToInventory(GnamGrabbable gnammable)
        {
            inventory.onSnap -= OnGunSnappedToInventory;
            CompletedStep();
            StartCoroutine(DelayedCallback(stepCompletedDelay, () => {

                onStep6Completed(gnammable.GetComponent<GnamRaycastWeapon>());
            }));
        }

        IEnumerator DelayedCallback(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }

        void ClearStarter()
        {
            foreach (var item in spawnedItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            spawnedItems.Clear();
        }
    }
}
