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
        [SerializeField] protected int CurrentLevel { get; private set; }

        Dictionary<int, int> levelScores = new Dictionary<int, int>();

        //scores
        protected const int eatableScore = 1;
        protected const int foodScore = 5;
        protected const int mobKillScore = 5;
        protected const int objectiveScore = 10;
        protected const int maxLevel = 9;


        protected Difficulty currentDifficulty { get {
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
        [SerializeField] int baseFoodToEat = 30;
        [SerializeField] MobSpawner mobSpawner;
        [SerializeField] AudioSource soundTrack;
        [SerializeField] AudioSource gameplaySound;
        [SerializeField] AudioClip winSound;
        [SerializeField] AudioClip loseSound;
        [SerializeField] int startLevel = 1;

        [SerializeField] GameObject gunPrefab;
        [SerializeField] GameObject gunClipPrefab;

        protected bool isPlaying { get; private set; }

        public event System.Action onGameStarted;

        int eatedFoods = 0;
        float gameplayTime = 0;
        float totalGameplayTime = 0;

        protected List<Food.FoodFamily> currentObjectiveFamilies = new List<Food.FoodFamily>();

        int TotalScore { get; set; }
        protected int CurrentLevelScore { get; set; }
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

                return heldedInLeftHand || heldedInRightHand || isInInventory;
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

                return heldedInLeftHand || heldedInRightHand || isInInventory;
            }
        }

        protected virtual void Start()
        {
            handsController = new VrifHandsControllerAdapter(handModelSelector);
            for (int i = 1; i <= maxLevel; i++)
            {
                Debug.Log($"level added {i}");
                levelScores.Add(i, 0);
            }

            isPlaying = false;
            CurrentLevel = startLevel;
            totalGameplayTime = 0;

            var bestScore = PlayerPrefs.GetInt(GnamConstants.bestScoreKey);
            starter.InitStarter(bestScore);
            starter.onStart += (eater) =>
            {
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
                starter.Hide();
                StartGame();
            };

            mobSpawner.OnMobDeath += () =>
            {
                SpawnBonus();
                billboard.AddTime(5);

                CurrentLevelScore += mobKillScore;
            };
            billboard.onObjectiveCompleted += (family, objectivesFamilies, bonus) =>
            {
             //   Debug.Log("OBJECTIVE COMPLETED FOR " + family); 
                SpawnBonus(bonus);
                billboard.AddTime(5);
                currentObjectiveFamilies = objectivesFamilies;

                CurrentLevelScore += objectiveScore;
                // SpawnMobs();
            };
            billboard.onObjectiveExpired += (family, objectivesFamilies) =>
            {
             //   Debug.Log("OBJECTIVE EXPIRED FOR " + family);
                currentObjectiveFamilies = objectivesFamilies;
            };
            billboard.onGameCompleted += (residueSeconds) =>
            {
                CurrentLevelScore += residueSeconds;
                TotalScore += CurrentLevelScore;

                StopGameplay();
                billboard.StopTimer();

                gameplaySound.PlayOneShot(winSound);

                GameObject bonus;
                if(CurrentLevel == 3) {
                    bonus = gunPrefab;
                }
                else {
                    bonus = DrawNewBonus(currentDifficulty);
                }

                SpawnBonus(bonus);

                UpdateLevelScore(CurrentLevel, CurrentLevelScore);
                var rate = GetGameRate(CurrentLevelScore, CurrentLevel);

                billboard.YouWin(CurrentLevelScore, rate, bonus);
                StartCoroutine(DelayedCallback(3, () => {
                    starter.Show();
                    starter.SpawnRetryEatable();
                }));
                StartCoroutine(DelayedCallback(3.2f, () => {
                    starter.SpawnNextLevelEatable();
                }));
            };
            billboard.onTimeExpired += () => {
                StopGameplay();

                billboard.GameOver(levelScores);

                gameplaySound.PlayOneShot(loseSound);
                starter.Show();
                starter.SpawnRetryEatable();

                CurrentLevelScore = 0;

                var isNewRecord = TotalScore > bestScore;
                if (isNewRecord)
                {
                    PlayerPrefs.SetInt(GnamConstants.bestScoreKey, TotalScore);
                }

                billboard.ShowResults(levelScores, isNewRecord);
            };

        }

        IEnumerator DelayedCallback(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }

        void UpdateLevelScore(int level, int score)
        {
            if (levelScores.Any(l => l.Key == level))
            {
                levelScores[level] = score;
            }
            else
            {
                levelScores.Add(level, score);
            }
        }

        int GetGameRate(int score, int level)
        {
            var foodPoints = GetFoodToEatByLevel(level) * foodScore;
            Debug.Log($"{score}/{foodPoints} = {Mathf.CeilToInt((float)score / (float)foodPoints)}");
            return Mathf.CeilToInt((float)score / (float)foodPoints);
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

            bonusSpawner.SpawnBonus(bonus);

            return bonus;
        }

        protected virtual void StopGameplay()
        {
            isPlaying = false;
            mobSpawner.RemoveMobs();
            soundTrack.Stop();
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
            soundTrack.Play();

            totalGameplayTime = gameplayTime;
            gameplayTime = 0;
            isPlaying = true;
            eatedFoods = 0;
            var level = GetLevel(CurrentLevel, eatedFoods);
            billboard.SetLevel(level);
            billboard.StartTimer();

            InvokeRepeating("SpawnMobs", 15, UnityEngine.Random.Range(15, 30));
            InvokeRepeating("AddNewObjective", 5, UnityEngine.Random.Range(5, 10));
        }

        protected virtual void GoToNextLevel(EaterDto eater)
        {
            CurrentLevelScore = 0;
            foreach (var levelScore in levelScores)
            {
                Debug.Log($"LIVELLO {levelScore.Key} PUNTI {levelScore.Value}");
            }

            if (CurrentLevel < maxLevel)
            {
                CurrentLevel++;
                this.StartGame();
            }
            else
            {
                var bestScore = PlayerPrefs.GetInt(GnamConstants.bestScoreKey);
                var isNewRecord = TotalScore > bestScore;
                if (isNewRecord)
                {
                    PlayerPrefs.SetInt(GnamConstants.bestScoreKey, TotalScore);
                }

                starter.Show();
                billboard.ShowResults(levelScores, isNewRecord);
            }
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
