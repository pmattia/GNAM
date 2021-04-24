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
using Assets.Scripts.Gameplay;
using System;

public class TableBelt : GnamGameplay 
{
    public List<GameObject> trays = new List<GameObject>();
    [SerializeField] GameObject[] foodbagsRepository;
    [SerializeField] GameObject[] firstLevelfoodbags;

    [SerializeField] PathNode[] nodes;
    [SerializeField] int maxTrayOnTable;
    [SerializeField] float startFoodbagSpeed = .1f;
    float foodBagSpeed;
    [SerializeField] float startFoodbagPause = 3;
    float foodBagPause;
    [SerializeField] Animator destroyAnimator;
    [SerializeField] Animator cookAnimator;
    [SerializeField] AudioSource cookingAudio;
    [SerializeField] GameObject cookingParticle;
    [SerializeField] Transform cookingParticlePlaholder;

    [SerializeField] DynamicFoodbag dynamicFoodbag;

    bool isCooking = false;

    protected override void Start()
    {
        //var shuffled = foodRepository.OrderBy(n => Guid.NewGuid());
        //var randomFoods = shuffled.Take(5);
        //dynamicFoodbag.AddFoods(randomFoods);

        base.Start();

        foodBagSpeed = startFoodbagSpeed;
        foodBagPause = startFoodbagPause;

        base.onGameStarted += StartCooking;

        billboard.onObjectiveCompleted += (family, objectivesFamilies, bonus) =>
        {
            //IncreaseSpeed(.25f);
        };
        billboard.onGameCompleted += (residueSeconds) =>
        {
            trays.ForEach(t => Destroy(t.gameObject));
            trays.Clear();
            StopCooking();
        };
        billboard.onTimeExpired += () => {
            trays.ForEach(t => Destroy(t.gameObject));
            trays.Clear();
            StopCooking();
        };
    }

    protected override void StartGame()
    {
        base.StartGame();
        SetSpeedByLevel(CurrentLevel);
        StartCooking();
    }

    void SetSpeedByLevel(int level)
    {
        foodBagSpeed = startFoodbagSpeed + (level * .08f);
        foodBagPause = startFoodbagPause - (level * .2f);
    }

    void StartCooking()
    {
        isCooking = true;
        cookAnimator.SetBool("isCooking", true);
        destroyAnimator.SetBool("isDestroing", true);
        cookingAudio.Play();
        InvokeRepeating("spawnCookingParticle", 0, 1.5f);
    }

    void StopCooking()
    {
        CancelInvoke();
        cookAnimator.SetBool("isCooking", false);
        destroyAnimator.SetBool("isDestroing", false);
        cookingAudio.Stop();
    }

    void spawnCookingParticle()
    {
        var particle = Instantiate(cookingParticle, cookingParticlePlaholder.position, cookingParticlePlaholder.rotation);
        var autodestroyer = particle.AddComponent<Autodestroy>();
        autodestroyer.Countdown = 5;
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

    GameObject CloneRandomFoodbag(Difficulty difficulty, List<Food.FoodFamily> foodFamiliesSuggestion)
    {
        Foodbag[] availableFoodbags;
        if(CurrentLevel == 1)
        {
            availableFoodbags = firstLevelfoodbags.Select(f => f.GetComponent<Foodbag>()).ToArray();
        }
        else
        {
            var foodbags = foodbagsRepository.Select(f => f.GetComponent<Foodbag>());
            availableFoodbags = foodbags.Where(f => f.difficulty <= difficulty).ToArray();

            if (foodFamiliesSuggestion!= null 
                && foodFamiliesSuggestion.Count() > 0
                && !CheckFoodbagByFoodFamilies(foodFamiliesSuggestion))
            {
                Debug.Log($"suggestion");
                var rand = UnityEngine.Random.Range(0, 10);
                if(rand > 3)
                {
                    var tFoodbags = new List<Foodbag>();
                    foreach(var availableFoodbag in availableFoodbags)
                    {
                        if (availableFoodbag.foods.Select(f => f.foodFamily).Intersect(foodFamiliesSuggestion).Any())
                        {
                            tFoodbags.Add(availableFoodbag);
                        }
                    }

                    if (tFoodbags.Count() > 0)
                    {
                        availableFoodbags = tFoodbags.ToArray();
                    }
                    
                    foodFamiliesSuggestion.ForEach(f => Debug.Log($"suggestions {f}"));
                    availableFoodbags.ToList().ForEach(a=> Debug.Log($"selected suggestion {a.name}"));
                }
            }
        }
        var foodbag = availableFoodbags[UnityEngine.Random.Range(0, availableFoodbags.Count())];
        var clone = Instantiate(foodbag, nodes[0].transform.position, Quaternion.identity);

        var cloneFoodbag = clone.GetComponentInChildren<Foodbag>();

        // DEPRECATED
        //foreach (var eatable in cloneFoodbag.foods.SelectMany(f => f.eatableParts))
        //{
        //    eatable.onEated += (eater) => {
        //        CurrentLevelScore += GnamConstants.eatableScore;
        //    };
        //}
        cloneFoodbag.onFoodEated += (eater, eated) =>
        {
            CurrentLevelResults.FoodsCount++;
            billboard.AddFood(eated.foodFamily);
        };
       // cloneFoodbag.onClear += bonusSpawner.SpawnBonus;
        

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
        var newTray = CloneRandomFoodbag(currentDifficulty, currentObjectiveFamilies);
        trays.Add(newTray);
    }

    PathNodesFollower AttachFollowPath(Foodbag foodbag)
    {
        var followerComponent = foodbag.gameObject.AddComponent<PathNodesFollower>();
        followerComponent.SetNodes(this.nodes);
        followerComponent.SetSpeed(this.foodBagSpeed);
        followerComponent.SetNodePause(this.foodBagPause);
        followerComponent.onEndPath += OnTrayEndOfPath;
        followerComponent.StartMoving();

        return followerComponent;
    }

    bool CheckFoodbagByFoodFamilies(List<Food.FoodFamily> foodFamilies)
    {
        var nodeFollowers = FindObjectsOfType<PathNodesFollower>();
        var foodbags = nodeFollowers.Select(n => n.GetComponent<Foodbag>());
        return foodbags.Where(f => f.foods.Any(food => foodFamilies.Contains(food.foodFamily))).Any();
    }


    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isPlaying) { 
            if (CanAddTray)
            {
                AddTrayToTable();
            }
        }
    }

}
