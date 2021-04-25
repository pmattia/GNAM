using Assets.Scripts.AI;
using Assets.Scripts.Interfaces;
using Assets.Scripts.ScriptableObjects;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;

namespace Assets.Scripts.Gameplay
{
    public abstract class GnamGameplay : MonoBehaviour

    {
        [SerializeField] HandModelSelector handModelSelector;
        protected IHandsController handsController;
        [SerializeField] protected BonusSpawner bonusSpawner;
        [SerializeField] Starter starter;
        [SerializeField] protected Billboard billboard;
        [SerializeField] int levelDuration = 60;
        [SerializeField] Inventory inventory;
        [SerializeField] GameObject endgameParty;
        [SerializeField] protected int CurrentLevel { get; private set; }
        Dictionary<int, LevelResults> levelScores = new Dictionary<int, LevelResults>();
        [SerializeField] int baseFoodToEat = 30;
        [SerializeField] MobSpawner mobSpawner;
        [SerializeField] AudioSource soundTrack;
        [SerializeField] AudioSource gameplaySound;
        [SerializeField] AudioClip winSound;
        [SerializeField] AudioClip loseSound;
        [SerializeField] int startLevel = 1;
        [SerializeField] Transform player;
        [SerializeField] GameObject gunPrefab;
        [SerializeField] GameObject gunClipPrefab;
        [SerializeField] AudioClip waitingLoopClip;

        [SerializeField] protected GameObject[] foodRepository;
        [SerializeField] protected GameObject[] foodBagBonusesRepository;

        protected bool isPlaying { get; private set; }
        public int BestScore 
        { 
            get {
                return PlayerPrefs.GetInt(GnamConstants.bestScoreKey);
            }
        }
        protected Difficulty currentDifficulty
        {
            get
            {
                switch (CurrentLevel)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        return Difficulty.Low;
                        break;
                    case 4:
                    case 5:
                    case 6:
                        return Difficulty.Mid;
                        break;
                    case 7:
                    case 8:
                    case 9:
                    default:
                        return Difficulty.High;
                        break;
                }
            }
        }
        public event System.Action onGameStarted;

        int eatedFoods = 0;
        float gameplayTime = 0;
        float totalGameplayTime = 0;

        protected List<Food.FoodFamily> currentObjectiveFamilies = new List<Food.FoodFamily>();
        AudioSource waitingLoop;

        int TotalScore { get; set; }
        LevelResults currentLevelResults;
        protected LevelResults CurrentLevelResults { get { return currentLevelResults; } private set { currentLevelResults = value; } }
        protected bool HasGun { get {
                //var hasGun = ownedBonus.Any(b => b != null && b.GetComponent<GnamRaycastWeapon>() != null);
                //Debug.Log($"has gun {HasGun} in elements {ownedBonus.Count()} not null --> { ownedBonus.Count(b => b != null && b.GetComponent<GnamRaycastWeapon>() != null)}");
                var heldedInRightHand = false;
                if (handsController.RightGrabber.HeldGrabbable != null)
                {
                    heldedInRightHand = handsController.RightGrabber.HeldGrabbable.GetComponent<GnamRaycastWeapon>() != null;
                }
                var heldedInLeftHand = false;
                if (handsController.LeftGrabber.HeldGrabbable != null)
                {
                    heldedInLeftHand = handsController.LeftGrabber.HeldGrabbable.GetComponent<GnamRaycastWeapon>() != null;
                }
                var isInInventory = inventory.CheckObjectExistance<GnamRaycastWeapon>();
                var isInBonusSpawner = bonusSpawner.HeldItem!=null && bonusSpawner.HeldItem.GetComponent<GnamRaycastWeapon>() != null;

                Debug.Log($"has gun {heldedInLeftHand} {heldedInRightHand} {isInInventory}");
                return heldedInLeftHand || heldedInRightHand || isInInventory || isInBonusSpawner;
            } 
        }
        protected bool HasGunClip
        {
            get
            {
                var heldedInRightHand = false;
                if (handsController.RightGrabber.HeldGrabbable != null)
                {
                    heldedInRightHand = handsController.RightGrabber.HeldGrabbable.GetComponent<GnamPistolClip>() != null;
                }
                var heldedInLeftHand = false;
                if (handsController.LeftGrabber.HeldGrabbable != null)
                {
                    heldedInLeftHand = handsController.LeftGrabber.HeldGrabbable.GetComponent<GnamPistolClip>();
                }
                var isInInventory = inventory.CheckObjectExistance<GnamPistolClip>();
                var isInBonusSpawner = bonusSpawner.HeldItem != null && bonusSpawner.HeldItem.GetComponent<GnamRaycastWeapon>() != null;

                Debug.Log($"has clip {heldedInLeftHand} {heldedInRightHand} {isInInventory}");
                return heldedInLeftHand || heldedInRightHand || isInInventory || isInBonusSpawner;
            }
        }

        protected virtual void Start()
        {
            if (FindObjectOfType<WaitingLoop>() != null)
            {
                waitingLoop = FindObjectOfType<WaitingLoop>().GetComponent<AudioSource>();
            }
            else
            {
                var waitingLoopObj = new GameObject("waitingLoop");
                
                var tObj = Instantiate(waitingLoopObj);
                waitingLoop = tObj.gameObject.AddComponent<AudioSource>();
                waitingLoop.clip = waitingLoopClip;
                waitingLoop.loop = true;
                waitingLoop.volume = 0.2f;
                waitingLoop.Play();
            }

            CurrentLevelResults = LevelResults.GetNewInstance();
            handsController = new VrifHandsControllerAdapter(handModelSelector);
            for (int i = 1; i <= GnamConstants.maxLevel; i++)
            {
                levelScores.Add(i, LevelResults.GetNewInstance());
            }

            isPlaying = false;
            CurrentLevel = startLevel;
            totalGameplayTime = 0;

            starter.InitStarter(BestScore);
            starter.onStart += (eater) =>
            {
                Analytics.CustomEvent("Game_start", new Dictionary<string, object>
                {
                    { "time_elapsed", Time.timeSinceLevelLoad }
                });
                starter.Hide();
                this.StartGame();
                if (onGameStarted != null)
                {
                    onGameStarted();
                }
            };
            starter.SpawnStartEatable();
            starter.onNextlevel += (eater) =>
            {
                starter.Hide();
                GoToNextLevel(eater);
            };
            starter.onRetry += (eater) =>
            {
                Analytics.CustomEvent("Game_retry", new Dictionary<string, object>
                {
                    { "time_elapsed", Time.timeSinceLevelLoad }
                });
                starter.Hide();
                StartGame();
            };
            starter.onFinish += OnFinish;

            mobSpawner.OnMobDeath += (mob) =>
            {
                SpawnBonus();
                billboard.AddTime(5);
                var bonusUi = Resources.Load<GameObject>("UiTimeBonus");
                var instancedUi = Instantiate(bonusUi, mob.transform.position, transform.rotation);
                instancedUi.transform.LookAt(player);
                Analytics.CustomEvent("Mob_killed", new Dictionary<string, object>
                {
                    { "time_elapsed", Time.timeSinceLevelLoad }
                });

                currentLevelResults.KillsCount++;
            };
            billboard.onObjectiveCompleted += (family, objectivesFamilies, bonus) =>
            {
             //   Debug.Log("OBJECTIVE COMPLETED FOR " + family); 
                SpawnBonus(bonus);
                billboard.AddTime(5);
                currentObjectiveFamilies = objectivesFamilies;

                currentLevelResults.ObjectivesCount++;
                // SpawnMobs();

                Analytics.CustomEvent("Objective_completed", new Dictionary<string, object>
                {
                    { "time_elapsed", Time.timeSinceLevelLoad }
                });
            };
            billboard.onObjectiveExpired += (family, objectivesFamilies) =>
            {
             //   Debug.Log("OBJECTIVE EXPIRED FOR " + family);
                currentObjectiveFamilies = objectivesFamilies;
            };
            billboard.onGameCompleted += (residueSeconds) =>
            {
                if (isPlaying)
                {
                    currentLevelResults.SecondsCount = residueSeconds;
                    TotalScore += currentLevelResults.TotalPoints;

                    StopGameplay();
                    billboard.StopTimer();

                    UpdateLevelScore(CurrentLevel, currentLevelResults);
                    var rate = GetGameRate(CurrentLevelResults, CurrentLevel);

                    if (CurrentLevel < GnamConstants.maxLevel)
                    {
                        gameplaySound.PlayOneShot(winSound);

                        StartCoroutine(DelayedCallback(3, () =>
                        {
                            starter.Show();
                            starter.SpawnRetryEatable();
                        }));

                        GameObject bonus;
                        if (CurrentLevel == 3)
                        {
                            bonus = gunPrefab;
                        }
                        else
                        {
                            bonus = DrawNewBonus(currentDifficulty);
                        }
                        billboard.YouWin(CurrentLevelResults, rate, bonus);
                        SpawnBonus(bonus);

                        StartCoroutine(DelayedCallback(3.2f, () =>
                        {
                            starter.SpawnNextLevelEatable();
                        }));
                    }
                    else
                    {
                        OnFinish(null);
                    }
                }
            };
            billboard.onTimeExpired += () => {
                if (isPlaying)
                {
                    StopGameplay();

                    billboard.GameOver();

                    gameplaySound.PlayOneShot(loseSound);
                    starter.Show();
                    starter.SpawnRetryEatable();

                    CurrentLevelResults = LevelResults.GetNewInstance();

                    var isNewRecord = TotalScore > BestScore;
                    if (isNewRecord)
                    {
                        PlayerPrefs.SetInt(GnamConstants.bestScoreKey, TotalScore);
                    }

                    billboard.ShowResults(levelScores, isNewRecord);

                    Analytics.CustomEvent("Game_lose", new Dictionary<string, object>
                    {
                        { "isNewRecord", isNewRecord },
                        { "time_elapsed", Time.timeSinceLevelLoad }
                    });
                }
            };

        }

        IEnumerator DelayedCallback(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }

        void ClearFloor()
        {
            //TODO: CANCELLA ANCHE QUELLO CHE STA NELL'INVENTARIO!
            //var grabbables = FindObjectsOfType<GnamGrabbable>().Where(g => !g.BeingHeld);
            //grabbables.ToList().ForEach(g => Destroy(g.gameObject));
            //grabbables.ToList().ForEach(g => {
            //    Debug.Log($"{g.name} is helded {g.BeingHeld}");
            //    if (g.HeldByGrabbers!= null && g.HeldByGrabbers.Count() > 0)
            //    {
            //        g.HeldByGrabbers.ForEach(h => Debug.Log($"helded by {h.name}"));
            //    }
            //});
        }

        void OnFinish(EaterDto eater)
        {
            Analytics.CustomEvent("Game_finished", new Dictionary<string, object>
            {
                { "time_elapsed", Time.timeSinceLevelLoad }
            });
            var isNewRecord = TotalScore > BestScore;
            if (isNewRecord)
            {
                PlayerPrefs.SetInt(GnamConstants.bestScoreKey, TotalScore);
            }

            starter.ShowEndgameMessage();
            billboard.ShowResults(levelScores, isNewRecord);
            EndgameParty();

            StartCoroutine(DelayedCallback(3, () =>
            {
                starter.Show();
            }));
        }

        void UpdateLevelScore(int level, LevelResults results)
        {
            Analytics.CustomEvent("Level_score", new Dictionary<string, object>
            {
                { "level", level },
                { "results", results.TotalPoints },
                { "time_elapsed", Time.timeSinceLevelLoad }
            });
            if (levelScores.Any(l => l.Key == level))
            {
                levelScores[level] = results;
            }
            else
            {
                levelScores.Add(level, results);
            }
        }

        /// <summary>
        /// DEPRECATED NOT WORKING
        /// </summary>
        /// <param name="score"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        int GetGameRate(LevelResults score, int level)
        {
            var foodPoints = GetFoodToEatByLevel(level) * GnamConstants.foodScore;
            return Mathf.CeilToInt((float)score.TotalPoints / (float)foodPoints);
        }

        protected virtual GameObject DrawNewBonus(Difficulty difficulty)
        {
            GameObject bonus = null;
            if (!HasGun && difficulty == Difficulty.Mid)
            {
                bonus = gunPrefab;
            }
            else if (HasGun && !HasGunClip && difficulty == Difficulty.Mid)
            {
                bonus = gunClipPrefab;
            }
            else
            {
                bonus = bonusSpawner.GetBonus(currentDifficulty);
            }

            return bonus;
        }

        protected virtual GameObject SpawnBonus(GameObject bonus = null)
        {
            if (bonus ==null)
            {
                bonus = DrawNewBonus(currentDifficulty);
            }

            var foodBonus = GetFood(bonus);
            if (foodBonus != null)
            {
                bonusSpawner.SpawnFoodBonus(bonus, (eater) => {
                    if (isPlaying)
                    {
                        CurrentLevelResults.FoodsCount++;
                        billboard.AddFood(foodBonus.foodFamily);
                    }
                });
            }
            else
            {
                bonusSpawner.SpawnBonus(bonus);
            }

            return bonus;
        }

        Food GetFood(GameObject obj)
        {
            var bonusFood = obj.GetComponent<Food>();
            if (bonusFood == null)
            {
                bonusFood = obj.GetComponentInChildren<Food>();
            }
            return bonusFood;
        }

        protected virtual void StopGameplay()
        {
            isPlaying = false;
            mobSpawner.RemoveMobs();
            soundTrack.Stop();
            waitingLoop.Play();
            CancelInvoke("SpawnMobs");
            CancelInvoke("AddNewObjective");
        }

        protected virtual void FixedUpdate()
        {
            if (isPlaying)
            {
                //Debug.Log($"update gameplay audio {Time.timeScale}");
                gameplayTime += Time.deltaTime;
                totalGameplayTime += Time.deltaTime;
                soundTrack.pitch = Time.timeScale;
            }
        }

        protected void SpawnMobs()
        {
            mobSpawner.SpawnMob(CurrentLevel);
        }

        void AddNewObjective()
        {
            var objCount = Mathf.FloorToInt((float)CurrentLevel / 3f);
            var currentObjCount = billboard.GetObjectives().Count();
            for (int i = 0; i < objCount - currentObjCount; i++)
            {
                var newObjective = GetNewObjective(currentObjectiveFamilies);
                if (!currentObjectiveFamilies.Contains(newObjective.family))
                {
                    currentObjectiveFamilies.Add(newObjective.family);
                }
                billboard.AddObjective(newObjective);
            }
        }

        protected virtual void StartGame()
        {
            ClearFloor();
            soundTrack.Play();
            waitingLoop.Stop();

            totalGameplayTime = gameplayTime;
            gameplayTime = 0;
            isPlaying = true;
            eatedFoods = 0;
            var level = GetLevel(CurrentLevel, eatedFoods);
            billboard.SetLevel(level);
            billboard.StartTimer();

            int respawntime;
            switch (CurrentLevel)
            {
                case 7:
                    respawntime = UnityEngine.Random.Range(25, 30);
                    break;
                case 8:
                    respawntime = UnityEngine.Random.Range(20, 25);
                    break;
                case 9:
                    respawntime = UnityEngine.Random.Range(15, 20);
                    break;
                default:
                    respawntime = UnityEngine.Random.Range(15, 30);
                    break;
            }

            InvokeRepeating("AddNewObjective", 5, UnityEngine.Random.Range(5, 10));
            InvokeRepeating("SpawnMobs", 15, respawntime);
        }

        protected virtual void GoToNextLevel(EaterDto eater)
        {
            CurrentLevelResults = LevelResults.GetNewInstance();
            Analytics.CustomEvent("Game_next_level", new Dictionary<string, object>
            {
                { "Level", CurrentLevel },
                { "time_elapsed", Time.timeSinceLevelLoad }
            });

            CurrentLevel++;
            this.StartGame();            
        }

        void EndgameParty()
        {
            endgameParty.SetActive(true);
            foreach(var mob in endgameParty.GetComponentsInChildren<ShootAtTargets>())
            {
                mob.GetComponents<Collider>().ToList().ForEach(c => c.enabled = false);
                mob.GetComponentInChildren<Animator>().SetBool("exult", true);
            }
          //  mobSpawner.Party();
        }

        protected LevelDto GetLevel(int level, int eatedFoods)
        {
            var levelDto = new LevelDto();
            levelDto.levelIndex = level;
            levelDto.foodToEat = GetFoodToEatByLevel(level);
            levelDto.foodsEated = eatedFoods;
            levelDto.time = levelDuration;

            return levelDto;
        }

        int GetFoodToEatByLevel(int level)
        {
            return Mathf.CeilToInt(baseFoodToEat + ((level - 1) * baseFoodToEat * .30f));
        }

        protected ObjectiveDto GetNewObjective(List<Food.FoodFamily> excludedFamilies)
        {
            excludedFamilies = excludedFamilies == null ? new List<Food.FoodFamily>() : excludedFamilies;
            var ret = new ObjectiveDto();

            do
            {
                var rand = UnityEngine.Random.Range(0, 6);
                ret.family = (Food.FoodFamily)rand;

            } while (excludedFamilies.Contains(ret.family));

            var toEat = Mathf.CeilToInt((UnityEngine.Random.Range(2, 5)));

            ret.toEat = toEat;
            ret.bonus = DrawNewBonus(currentDifficulty);
            switch (currentDifficulty)
            {
                case Difficulty.Low:
                    ret.cooldown = 15;
                    break;
                case Difficulty.Mid:
                    ret.cooldown = 15;
                    break;
                case Difficulty.High:
                    ret.cooldown = 10;
                    break;
            }
            return ret;
        }
    }

    public class LevelDto
    {
        public int levelIndex;
        public int foodToEat;
        public int foodsEated;
        public float time;
        public List<ObjectiveDto> objectives = new List<ObjectiveDto>();
    }
    public class ObjectiveDto
    {
        public Food.FoodFamily family;
        public int toEat;
        public int eated;
        public GameObject bonus;
        public float cooldown;
        public bool IsCompleted
        {
            get
            {
                return eated >= toEat && toEat > 0;
            }
        }
    }

    public enum Difficulty
    {
        Low,
        Mid,
        High
    }
}
