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
        [SerializeField] protected GameObject nextLevelEatable;
        [SerializeField] Eatable startEatable;
        [SerializeField] protected Billboard billboard;
        [SerializeField] int levelDuration = 60;
        [SerializeField] protected int currentLevel = 1;
        [SerializeField] int foodToEat = 50;
        [SerializeField] protected MobSpawner mobSpawner;
        [SerializeField] protected AudioSource soundTrack;
        protected bool isPlaying = true;
        protected int eatedFoods = 0;

        protected float gameplayTime = 0;
        protected float totalGameplayTime = 0;


        protected virtual void Start()
        {
            totalGameplayTime = 0;
            isPlaying = false;
            
            startEatable.onEated += (eater) =>
            {
                this.StartGame();
            };

            InvokeRepeating("SpawnMobs", 5, UnityEngine.Random.Range(10, 20));
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

        protected void GoToNextLevel(EaterDto eater)
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
