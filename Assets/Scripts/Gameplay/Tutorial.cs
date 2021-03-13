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

namespace Assets.Scripts.Gameplay
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] Foodbag foods;
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

        public event Action onStep1Completed;
        public event Action onStep2Completed;
        public event Action onStep3Completed;
        public event Action onStep4Completed;
        public event Action<GnamRaycastWeapon> onStep5Completed;
        public event Action onStep6Completed;
        public event Action onStep7Completed;
        List<GameObject> spawnedItems = new List<GameObject>();

        float stepCompletedDelay = .5f;

        private void Start()
        {
            InitTutorial();
        }

        void InitTutorial()
        {
            InitStepCompletedActions();

            ClearStarter();
            spawnedItems.Add(commandSpawner.SpawnObject(startEatable, (eater) =>
            {
                CompletedStep();
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
                var foodbagObj = Instantiate(modifiedFoods, chipPlate.transform.parent);
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
                Debug.Log("STEP 2 COMPLETED MODIFIED FOOD");

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
                        onStep3Completed();
                    }));

                    var onGunGrab = new GrabberEvent();
                    var gunEvents = gunBonus.GetComponent<GrabbableUnityEvents>();

                    onGunGrab.AddListener((grabber) => {
                        CompletedStep();
                        StartCoroutine(DelayedCallback(stepCompletedDelay, () => {
                            gunEvents.onGrab.RemoveAllListeners();
                            onStep4Completed();
                        }));
                    });

                    gunEvents.onGrab = onGunGrab;
                };
                
            };

            onStep3Completed += () =>
            {
                Debug.Log("STEP 3 COMPLETED OBJECTIVE");

                InitStep(3);

                FindObjectsOfType<Foodbag>().ToList().ForEach(f => Destroy(f.gameObject));
                inventory.ShowItems();
                
            };

            onStep4Completed += () =>
            {
                Debug.Log("STEP 4 COMPLETED GUN TAKEN");

                InitStep(4);

                inventory.ShowItems();
                inventory.onSnap += OnGunSnappedToInventory;

            };

            onStep5Completed += (GnamRaycastWeapon gun) =>
            {
                Debug.Log("STEP 5 COMPLETED GUN SNAPPED TO INVENTORY");

                InitStep(5);

                inventory.SnapEmptyHolders(gunClip.GetComponent<GnamGrabbable>());

                var onGunCharged = new UnityEvent();
                onGunCharged.AddListener(() => {
                    CompletedStep();
                    StartCoroutine(DelayedCallback(stepCompletedDelay, () => {
                        gun.onWeaponChargedEvent.RemoveAllListeners();
                        onStep6Completed();
                    }));
                });

                gun.onWeaponChargedEvent = onGunCharged;
            };

            onStep6Completed += () =>
            {
                Debug.Log("STEP 6 COMPLETED GUN CHARGED");

                InitStep(6);

                mobs.ForEach(m => {
                    m.SetActive(true);
                    var shoter = m.GetComponent<ShootAtTargets>();
                    shoter.onDeath += () =>
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
                                    onStep7Completed();
                                }));
                            }
                        }));
                        
                    };
                });
            };

            onStep7Completed += () =>
            {
                Debug.Log("STEP 7 COMPLETED MOB KILLED");
                panels.ForEach(p => p.SetActive(false));
                panelAnimator.SetBool("show", true);
                finishPanel.SetActive(true);

                FindObjectsOfType<GnamGrabbable>().ToList().ForEach(f => Destroy(f.gameObject));
                inventory.HideItems();

                chipPlate.SetActive(true);
                ClearStarter();
                spawnedItems.Add(commandSpawner.SpawnObject(startEatable, (eater) =>
                {
                    SceneManager.LoadSceneAsync("SampleSceneRush");
                }));
            };
        }

        void CompletedStep()
        {
            panelAnimator.SetBool("show", false);
            VRUtils.Instance.PlaySpatialClipAt(stepCompletedAudio, transform.position, 1f, 0.5f);
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

                onStep5Completed(gnammable.GetComponent<GnamRaycastWeapon>());
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
