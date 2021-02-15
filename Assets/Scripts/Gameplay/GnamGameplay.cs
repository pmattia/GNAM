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

        protected bool isPlaying { get; private set; }

        public event System.Action onGameStarted;

        int eatedFoods = 0;
        float gameplayTime = 0;
        float totalGameplayTime = 0;

        List<Food.FoodFamily> currentObjectiveFamilies = new List<Food.FoodFamily>();
        int completedObjectsStack = 0;
        float objectiveCooldown = 5;

        protected int Score { get; set; }

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
                bonusSpawner.SpawnBonus(currentDifficulty);
                billboard.AddTime(5);

                Score += 5;
            };
            billboard.onObjectiveCompleted += (family, objectivesFamilies) =>
            {
                Debug.Log("OBJECTIVE COMPLETED FOR " + family);
                bonusSpawner.SpawnBonus(currentDifficulty);
                billboard.AddTime(5);
                currentObjectiveFamilies = objectivesFamilies;
                completedObjectsStack++;

                Score += 10;
                // SpawnMobs();
            };
            billboard.onGameCompleted += (residueSeconds) =>
            {
                Score += residueSeconds;
                StopGameplay();
                billboard.YouWin(Score);
                billboard.StopTimer();
                commandSpawner.SpawnObject(nextLevelEatable, GoToNextLevel);
                gameplaySound.PlayOneShot(winSound);
                //todo: gestire bonus obbligati in base al livello
                //if(currentLevel == 3)
                //{
                //    bonusSpawner.SpawnGun();
                //}
                //else
                //{
                    bonusSpawner.SpawnBonus(currentDifficulty);
                //}
            };
            billboard.onTimeExpired += () => {
                StopGameplay();
                billboard.GameOver();
                gameplaySound.PlayOneShot(loseSound);

                Score = 0;
            };

        }

        protected virtual void StopGameplay()
        {
            isPlaying = false;
            mobSpawner.RemoveMobs();
            soundTrack.Stop();
            completedObjectsStack = 0;
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
            if (completedObjectsStack > 0)
            {
                billboard.AddObjective(GetNewObjective(currentLevel, currentObjectiveFamilies));
                completedObjectsStack--;
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

            var objCount = Mathf.FloorToInt((float)level / 3f);
            for(int i=0; i<objCount; i++)
            {
                levelDto.objectives.Add(GetNewObjective(level, null));
            }

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
            var toEat = Mathf.CeilToInt((Random.Range(1, 5)));

            ret.toEat = toEat;

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
