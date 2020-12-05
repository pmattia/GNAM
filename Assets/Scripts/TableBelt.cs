using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class TableBelt : MonoBehaviour
{
    public List<GameObject> trays = new List<GameObject>();
    public Foodbag[] foodbagsRepository;
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
            return isPlaying && trays.All(t => t.GetComponent<PathNodesFollower>().GetCurrentNodeIndex() > (nodes.Length - maxTrayOnTable));
        }
    }

    GameObject CloneRandomFoodbag()
    {
        //var foodbag = foodbagsRepository[Random.Range(0, foodbagsRepository.Length)];
        var foodbag = foodbagsRepository[4];
        var clone = Instantiate(foodbag.gameObject, nodes[0].transform.position, Quaternion.identity);
        var cloneFollower = AttachFollowPath(clone);
        cloneFollower.StartMoving();

        var cloneFoodbag = clone.GetComponent<Foodbag>();

        cloneFoodbag.onClear += () => { 
            speed += speed * .1f;
            nodePause -= nodePause * .1f;
            var points = int.Parse(billboard.text);
            billboard.text = (points + 10).ToString();
        };
        cloneFoodbag.onClear += modifierSpawner.SpawnNewBonus;

        return clone;
    }

    void OnTrayEndOfPath(GameObject gameobject)
    {
        
        trays.Remove(gameobject);
        Destroy(gameobject);

    }

    void AddTrayToTable()
    {
        trays.Add(CloneRandomFoodbag());
    }

    PathNodesFollower AttachFollowPath(GameObject clone)
    {
        var followerComponent = clone.AddComponent<PathNodesFollower>();
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
