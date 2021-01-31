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
        [SerializeField] int foodToEat = 30;
        [SerializeField] MobSpawner mobSpawner;
        [SerializeField] AudioSource soundTrack;
        [SerializeField] AudioSource gameplaySound;
        [SerializeField] AudioClip winSound;
        [SerializeField] AudioClip loseSound;
        protected bool isPlaying { get; private set; }

        public event System.Action onGameStarted;

        int eatedFoods = 0;
        float gameplayTime = 0;
        float totalGameplayTime = 0;


        protected virtual void Start()
        {
            isPlaying = false;
            currentLevel = 1;
            totalGameplayTime = 0;
            
            startEatable.onEated += (eater) =>
            {
                this.StartGame();
                if (onGameStarted != null)
                {
                    onGameStarted();
                }
            };
            billboard.onObjectiveCompleted += (family, objectivesFamilies) =>
            {
                Debug.Log("OBJECTIVE COMPLETED FOR " + family);
                bonusSpawner.SpawnBonus();
                billboard.AddTime(15);

                //currentLevel = GetLevelIndex();
                billboard.AddObjective(GetNewObjective(currentLevel, objectivesFamilies));

                SpawnMobs();
            };
            billboard.onGameCompleted += () =>
            {
                isPlaying = false;
                billboard.YouWin();
                mobSpawner.RemoveMobs();
                billboard.StopTimer();
                commandSpawner.SpawnObject(nextLevelEatable, GoToNextLevel);
                soundTrack.Stop();
                gameplaySound.PlayOneShot(winSound);
            };
            billboard.onTimeExpired += () => {
                isPlaying = false;
                billboard.GameOver();
                mobSpawner.RemoveMobs();
                soundTrack.Stop();
                gameplaySound.PlayOneShot(loseSound);
            };

            InvokeRepeating("SpawnMobs", 5, UnityEngine.Random.Range(10, 20));
        }

        void FixedUpdate()
        {
            if (isPlaying)
            {
                gameplayTime += Time.deltaTime;
                totalGameplayTime += Time.deltaTime;
            }
        }

        protected void SpawnMobs()
        {
            mobSpawner.SpawnMob(currentLevel);
        }

        protected void StartGame()
        {
            soundTrack.Play();

            totalGameplayTime = gameplayTime;
            gameplayTime = 0;
            isPlaying = true;
            eatedFoods = 0;
            var level = GetLevel(currentLevel, eatedFoods);
            billboard.SetLevel(level);
            billboard.StartTimer();

            //mobSpawner.SpawnMob(5);

        }

        protected virtual void GoToNextLevel(EaterDto eater)
        {
            currentLevel++;
            this.foodToEat = Mathf.CeilToInt(foodToEat * 1.5f);
            this.StartGame();
        }

        protected LevelDto GetLevel(int level, int eatedFoods)
        {
            var levelDto = new LevelDto();
            levelDto.foodToEat = foodToEat;
            levelDto.foodsEated = eatedFoods;
            levelDto.time = levelDuration;

            var objCount = Mathf.FloorToInt((float)level / 2f);
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
                var rand = UnityEngine.Random.Range(0, 5);
                ret.family = (Food.FoodFamily)rand;

            } while (excludedFamilies.Contains(ret.family));

            var toEat = Mathf.CeilToInt((Random.Range(1, levelIndex * 3)));
            ret.toEat = toEat;

            return ret;
        }
    }

    public class LevelDto
    {
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
}
