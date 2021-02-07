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
    [SerializeField] float speed = .1f;
    [SerializeField] float nodePause = 3;
    [SerializeField] Animator cookAnimator;
    [SerializeField] AudioSource cookingAudio;
    [SerializeField] GameObject cookingParticle;
    [SerializeField] Transform cookingParticlePlaholder;
    bool isCooking = false;

    protected override void Start()
    {
        base.Start();
        base.onGameStarted += StartCooking;

        billboard.onObjectiveCompleted += (family, objectivesFamilies) =>
        {
            //IncreaseSpeed(.25f);
        };
        billboard.onGameCompleted += () =>
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

    void IncreaseSpeed(float quantity)
    {
        speed += speed * quantity;
        nodePause -= nodePause * quantity;
    }

    protected override void GoToNextLevel(EaterDto eater)
    {
        base.GoToNextLevel(eater);
        IncreaseSpeed(.2f);
        StartCooking();
    }

    void StartCooking()
    {
        isCooking = true;
        cookAnimator.SetBool("isCooking", true);
        cookingAudio.Play();
        InvokeRepeating("spawnCookingParticle", 0, 1.5f);
    }

    void StopCooking()
    {
        CancelInvoke();
        cookAnimator.SetBool("isCooking", false);
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
        cloneFoodbag.onClear += bonusSpawner.SpawnBonus;
        

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


    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPlaying) { 
            if (CanAddTray)
            {
                AddTrayToTable();
            }
        }
    }
}
