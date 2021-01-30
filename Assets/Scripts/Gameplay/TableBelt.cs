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
    public GameObject[] foodbagsRepository;
    public PathNode[] nodes;
    public int maxTrayOnTable;
    public float speed = .1f;
    public float nodePause = 3;

    protected override void Start()
    {
        base.Start();
        billboard.onObjectiveCompleted += (family, objectivesFamilies) =>
        {
            Debug.Log("OBJECTIVE COMPLETED FOR " + family);
            speed += speed * .05f;
            nodePause -= nodePause * .05f;
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
            trays.ForEach(t => Destroy(t.gameObject));
            trays.Clear();
            soundTrack.Stop();
            billboard.StopTimer();
            bonusSpawner.SpawnObject(nextLevelEatable, GoToNextLevel);
        };
        billboard.onTimeExpired += () => {
            isPlaying = false;
            billboard.GameOver();
            mobSpawner.RemoveMobs();
            trays.ForEach(t => Destroy(t.gameObject));
            trays.Clear();
            soundTrack.Stop();
        };
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
            gameplayTime += Time.deltaTime;
            totalGameplayTime += Time.deltaTime;

            if (CanAddTray)
            {
                AddTrayToTable();
            }
        }

        
    }
}
