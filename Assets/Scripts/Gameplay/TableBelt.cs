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

public class TableBelt : GnamGameplay 
{
    public List<GameObject> trays = new List<GameObject>();
    [SerializeField] GameObject[] foodbagsRepository;
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
    bool isCooking = false;

    protected override void Start()
    {
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

    protected override void GoToNextLevel(EaterDto eater)
    {
        base.GoToNextLevel(eater);
    }

    protected override void StartGame()
    {
        base.StartGame();
        SetSpeedByLevel(currentLevel);
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

    GameObject CloneRandomFoodbag(Difficulty difficulty)
    {
        var foodbags = foodbagsRepository.Select(f => f.GetComponent<Foodbag>());
        var availableFoodbags = foodbags.Where(f => f.difficulty <= difficulty).ToArray();
        var foodbag = availableFoodbags[Random.Range(0, availableFoodbags.Count())];
        //var foodbag = foodbagsRepository[4];
        var clone = Instantiate(foodbag, nodes[0].transform.position, Quaternion.identity);

        var cloneFoodbag = clone.GetComponentInChildren<Foodbag>();

        foreach (var eatable in cloneFoodbag.foods.SelectMany(f => f.eatableParts))
        {
            eatable.onEated += (eater) => {
                Score += 1;
            };
        }
        cloneFoodbag.onFoodEated += (eater, eated) =>
        {
            Score += 5;
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
        var newTray = CloneRandomFoodbag(currentDifficulty);
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
