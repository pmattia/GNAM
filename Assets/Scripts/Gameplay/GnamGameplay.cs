using Assets.Scripts.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public abstract class GnamGameplay : MonoBehaviour

    {
        [SerializeField] protected BonusSpawner bonusSpawner;
        [SerializeField] GameObjectSpawner commandSpawner;
        [SerializeField] GameObject nextLevelEatable;
        [SerializeField] Eatable startEatable;
        [SerializeField] protected Billboard billboard;
        [SerializeField] int levelDuration = 60;
        List<GameObject> ownedBonus = new List<GameObject>();
        [SerializeField] protected int currentLevel { get; private set; }
        protected Difficulty currentDifficulty { get {
                switch (currentLevel)
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

        List<Food.FoodFamily> currentObjectiveFamilies = new List<Food.FoodFamily>();

        protected int Score { get; set; }
        protected bool HasGun { get {
                return ownedBonus.Any(b => b != null && b.GetComponent<GnamRaycastWeapon>() != null);
            } 
        }
        protected bool HasGunClip
        {
            get
            {
                return ownedBonus.Any(b => b != null && b.GetComponent<GnamPistolClip>() != null);
            }
        }

        protected virtual void Start()
        {
            isPlaying = false;
            currentLevel = startLevel;
            totalGameplayTime = 0;
            
            startEatable.onEated += (eater) =>
            {
                this.StartGame();
                if (onGameStarted != null)
                {
                    onGameStarted();
                }
            };
            mobSpawner.OnMobDeath += () =>
            {
                SpawnBonus();
                billboard.AddTime(5);

                Score += 5;
            };
            billboard.onObjectiveCompleted += (family, objectivesFamilies, bonus) =>
            {
                Debug.Log("OBJECTIVE COMPLETED FOR " + family); 
                SpawnBonus(bonus);
                billboard.AddTime(5);
                currentObjectiveFamilies = objectivesFamilies;

                Score += 10;
                // SpawnMobs();
            };
            billboard.onObjectiveExpired += (family, objectivesFamilies) =>
            {
                Debug.Log("OBJECTIVE EXPIRED FOR " + family);
                currentObjectiveFamilies = objectivesFamilies;
            };
            billboard.onGameCompleted += (residueSeconds) =>
            {
                Score += residueSeconds;
                StopGameplay();
                billboard.YouWin(Score);
                billboard.StopTimer();
                commandSpawner.SpawnObject(nextLevelEatable, GoToNextLevel);
                gameplaySound.PlayOneShot(winSound);

                if (!HasGun && currentLevel >= 3)
                {
                    SpawnBonus(gunPrefab);
                }
                else if (HasGun && !HasGunClip && currentLevel >= 3)
                {
                    SpawnBonus(gunClipPrefab);
                }
                else
                {
                    SpawnBonus();
                }
            };
            billboard.onTimeExpired += () => {
                StopGameplay();
                billboard.GameOver();
                gameplaySound.PlayOneShot(loseSound);

                Score = 0;
            };

        }

        protected virtual GameObject DrawNewBonus(int level)
        {
            GameObject bonus = null;
            if (!HasGun && level >= 3)
            {
                bonus = gunPrefab;
            }
            else if (HasGun && !HasGunClip && currentLevel >= 3)
            {
                bonus = gunClipPrefab;
            }
            else
            {
                bonus = bonusSpawner.GetBonus(currentDifficulty);
            }

            return bonus;
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

        protected virtual void SpawnBonus(GameObject bonus = null)
        {
            if (bonus ==null)
            {
                bonus = DrawNewBonus(currentLevel);
            }

            if (bonus != null)
            {
                Debug.Log($"SPAWN BONUS {bonus.name}");

                var bonusAutodestroyer = bonus.GetComponent<Autodestroy>();
                if (bonusAutodestroyer != null)
                {
                    bonusAutodestroyer.onDestroy += () =>
                    {
                        ownedBonus.Remove(bonus);
                    };
                }

                var bonusEatable = bonus.GetComponent<Eatable>();
                if (bonusEatable != null)
                {
                    bonusEatable.onEated += (eater) =>
                    {
                        ownedBonus.Remove(bonus);
                    };
                }

                var bonusFood = bonus.GetComponent<Food>();
                if (bonusFood != null)
                {
                    bonusFood.onEated += (eater) =>
                    {
                        billboard.AddFood(bonusFood.foodFamily);
                        ownedBonus.Remove(bonus);
                    };
                }

                ownedBonus.Add(bonusSpawner.SpawnBonus(bonus));
            }

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
            mobSpawner.SpawnMob(currentLevel);
        }

        void AddNewObjective()
        {
            var objCount = Mathf.FloorToInt((float)currentLevel / 3f);
            var currentObjCount = billboard.GetObjectives().Count();
            for (int i = 0; i < objCount - currentObjCount; i++)
            {
                billboard.AddObjective(GetNewObjective(currentLevel, currentObjectiveFamilies));
            }
        }

        protected virtual void StartGame()
        {
            soundTrack.Play();

            totalGameplayTime = gameplayTime;
            gameplayTime = 0;
            isPlaying = true;
            eatedFoods = 0;
            var level = GetLevel(currentLevel, eatedFoods);
            billboard.SetLevel(level);
            billboard.StartTimer();

            InvokeRepeating("SpawnMobs", 15, UnityEngine.Random.Range(15, 30));
            InvokeRepeating("AddNewObjective", 5, UnityEngine.Random.Range(5, 10));
        }

        protected virtual void GoToNextLevel(EaterDto eater)
        {
            currentLevel++;
            this.StartGame();
        }

        protected LevelDto GetLevel(int level, int eatedFoods)
        {
            var levelDto = new LevelDto();
            levelDto.levelIndex = level;
            levelDto.foodToEat = Mathf.CeilToInt(baseFoodToEat + ((currentLevel -1) * baseFoodToEat * .25f));
            Debug.Log($"{currentLevel} - {baseFoodToEat} - {(currentLevel - 1 * baseFoodToEat * .25f)}");
            levelDto.foodsEated = eatedFoods;
            levelDto.time = levelDuration;

            //var objCount = Mathf.FloorToInt((float)level / 3f);
            //for(int i=0; i<objCount; i++)
            //{
            //    levelDto.objectives.Add(GetNewObjective(level, null));
            //}

            return levelDto;
        }

        protected ObjectiveDto GetNewObjective(int levelIndex, List<Food.FoodFamily> excludedFamilies)
        {
            excludedFamilies = excludedFamilies == null ? new List<Food.FoodFamily>() : excludedFamilies;
            var ret = new ObjectiveDto();

            do
            {
                var rand = UnityEngine.Random.Range(0, 6);
                ret.family = (Food.FoodFamily)rand;

            } while (excludedFamilies.Contains(ret.family));

            //var toEat = Mathf.CeilToInt((Random.Range(1, 2 + levelIndex * 2)));
            var toEat = Mathf.CeilToInt((Random.Range(2, 5)));

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
