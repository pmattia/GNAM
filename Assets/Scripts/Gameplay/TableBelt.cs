using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Assets.Scripts.AI;
using BNG;
using Assets.Scripts;
using static Assets.Scripts.Billboard;
using Assets.Scripts.Interfaces;

public class TableBelt : MonoBehaviour
{
    public MobSpawner mobSpawner;
    public List<GameObject> trays = new List<GameObject>();
    public GameObject[] foodbagsRepository;
    public PathNode[] nodes;
    public int maxTrayOnTable;
    public float speed = .1f;
    public float nodePause = 3;
    public GameObjectSpawner bonusSpawner;
    public GameObject nextLevelEatable;
    public Eatable startEatable;
    public Billboard billboard;
    public Timer timer;
    public int startupTimer = 60;
    bool isPlaying = true;
    public int currentLevel = 1;
    public int foodToEat = 50;
    int eatedFoods = 0;

    float gameplayTime = 0;
    float totalGameplayTime = 0;

    public AudioSource soundTrack;

    void Start()
    {
        totalGameplayTime = 0;
        isPlaying = false;
        billboard.onObjectiveCompleted += (family, objectivesFamilies) =>
        {
            Debug.Log("OBJECTIVE COMPLETED FOR " + family);
            speed += speed * .05f;
            nodePause -= nodePause * .05f;
            bonusSpawner.SpawnNewObject();
            timer.AddTime(15);

            //currentLevel = GetLevelIndex();
            billboard.AddObjective(GetNewObjective(currentLevel, objectivesFamilies));
            
            SpawnMobs();
        };
        billboard.onGameCompleted += () =>
        {
            isPlaying = false;
            billboard.YouWin();
            mobSpawner.RemoveMobs();
            trays.ForEach(t => Destroy(t.gameObject));
            trays.Clear();
            soundTrack.Stop();
            timer.StopTimer();
            bonusSpawner.SpawnPermanentObject(nextLevelEatable, GoToNextLevel);
        };
        timer.onExpired += () => {
            isPlaying = false;
            billboard.GameOver();
            mobSpawner.RemoveMobs();
            trays.ForEach(t => Destroy(t.gameObject));
            trays.Clear();
            soundTrack.Stop();
        };

        startEatable.onEated += (eater) =>
        {
            this.StartGame();
        };

        InvokeRepeating("SpawnMobs", 5, UnityEngine.Random.Range(10, 20));
    }

    void SpawnMobs()
    {
        //currentLevel = GetLevelIndex();

        mobSpawner.SpawnMob(currentLevel);
    }

    void StartGame()
    {
        soundTrack.Play();

        totalGameplayTime = gameplayTime;
        gameplayTime = 0;
        isPlaying = true;
        eatedFoods = 0;
        var level = GetLevel(currentLevel, eatedFoods);
        billboard.SetLevel(level);

        timer.SetTimer(startupTimer);
        timer.StartTimer();

        //mobSpawner.SpawnMob(5);

    }

    void GoToNextLevel(EaterDto eater)
    {
        currentLevel++;
        this.foodToEat = Mathf.CeilToInt(foodToEat * 1.5f);
        this.StartGame();
    }

    LevelDto GetLevel(int levelIndex, int eatedFoods)
    {
        var level = new LevelDto();
        level.foodToEat = foodToEat;
        level.foodsEated = eatedFoods;
        level.objectives.Add(new ObjectiveDto()
        {
            family = Food.FoodFamily.Candy,
            toEat = Mathf.CeilToInt((1f * levelIndex))
        });
        level.objectives.Add(new ObjectiveDto()
        {
            family = Random.Range(0, 5) < 2 ? Food.FoodFamily.Fruit : Food.FoodFamily.Vegetable,
            toEat = Mathf.CeilToInt((.5f * levelIndex))
        });
        level.objectives.Add(new ObjectiveDto()
        {
            family = Random.Range(0,5) < 2 ? Food.FoodFamily.Meat : Food.FoodFamily.Carbo,
            toEat = Mathf.CeilToInt((2f * levelIndex))
        });

        return level;
    }

    ObjectiveDto GetNewObjective(int levelIndex, List<Food.FoodFamily> excludedFamilies) {

        var ret = new ObjectiveDto();

        do
        {
            var rand = Random.Range(0, 5);
            ret.family = (Food.FoodFamily)rand;

        } while (excludedFamilies.Contains(ret.family));

        var toEat = Mathf.CeilToInt((Random.Range(1, levelIndex * 3)));
        ret.toEat = toEat;

        return ret;
    }

    bool CanAddTray
    {
        get
        {  
            return isPlaying 
                    && trays.All(t => t.GetComponent<PathNodesFollower>().GetCurrentNodeIndex() > (nodes.Length - maxTrayOnTable))
                    && trays.All(t => t.GetComponent<PathNodesFollower>().GetCurrentNodeIndex() > 0)
                    ;
        }
    }

    GameObject CloneRandomFoodbag()
    {
        var foodbag = foodbagsRepository[Random.Range(0, foodbagsRepository.Length)];
        //var foodbag = foodbagsRepository[4];
        var clone = Instantiate(foodbag, nodes[0].transform.position, Quaternion.identity);

        var cloneFoodbag = clone.GetComponentInChildren<Foodbag>();
       
        cloneFoodbag.onFoodEated += (eater, eated) =>
        {
            billboard.AddFood(eated.foodFamily);
        };
        cloneFoodbag.onClear += bonusSpawner.SpawnNewObject;
        

        AttachFollowPath(cloneFoodbag).StartMoving();

        return cloneFoodbag.gameObject;
    }

    void OnTrayEndOfPath(GameObject gameobject)
    {
        
        trays.Remove(gameobject);
        Destroy(gameobject);

    }

    void AddTrayToTable()
    {
        var newTray = CloneRandomFoodbag();
        trays.Add(newTray);
    }

    PathNodesFollower AttachFollowPath(Foodbag foodbag)
    {
        var followerComponent = foodbag.gameObject.AddComponent<PathNodesFollower>();
        followerComponent.SetNodes(this.nodes);
        followerComponent.SetSpeed(this.speed);
        followerComponent.SetNodePause(this.nodePause);
        followerComponent.onEndPath += OnTrayEndOfPath;
        followerComponent.StartMoving();

        return followerComponent;
    }

    //int GetLevelIndex() {
        
    //    var ret = Mathf.CeilToInt(totalGameplayTime / 30);
    //    return ret == 0 ? 1 : ret;
    //}


    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPlaying) { 
            gameplayTime += Time.deltaTime;
            totalGameplayTime += Time.deltaTime;

            if (CanAddTray)
            {
                AddTrayToTable();
            }
        }

        
    }
}
