using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Assets.Scripts.AI;
using BNG;

public class TableBelt : MonoBehaviour
{
    public List<ShootAtTargets> shooters;
    public List<GameObject> trays = new List<GameObject>();
    public GameObject[] foodbagsRepository;
    public PathNode[] nodes;
    public int maxTrayOnTable;
    public float speed;
    public float nodePause = 4;
    public ModifierSpawner modifierSpawner;
    public TextMeshPro billboard;
    public Timer timer;
    bool isPlaying = true;

    void Start()
    {
        timer.onExpired += () => { isPlaying = false; };
        timer.SetTimer(60);
        timer.StartTimer();
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
        cloneFoodbag.onClear += () =>
        {
            speed += speed * .1f;
            nodePause -= nodePause * .1f;
            var points = int.Parse(billboard.text);
            billboard.text = (points + 10).ToString();
            var validShooters = shooters.Where(s => !s.gameObject.activeSelf).ToArray();
            if (validShooters.Length > 0)
            {
                var shooter = validShooters[Random.Range(0, validShooters.Length)];
                shooter.gameObject.SetActive(true);
            }
        };
        cloneFoodbag.onClear += modifierSpawner.SpawnNewBonus;

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
        if (CanAddTray)
        {

            AddTrayToTable();
        }
    }
}
