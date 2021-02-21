using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNodesFollower : MonoBehaviour
{
    PathNode[] nodes;
    float speed;
    float timer;
    float nodePause;
    int currentNode;
    Vector3 currentPosition;
    public bool IsMoving { get; private set; }
    public event Action<GameObject> onEndPath;
    public int CurrentNode { get { return currentNode; } }

    void Start()
    {
        CheckNode();
    }

    void CheckNode()
    {
        timer = 0;
        currentPosition = nodes[currentNode].transform.position;
    }

    public void StartMoving()
    {
        IsMoving = true;
    }

    public void StopMoving()
    {
        IsMoving = false;
    }

    public void SetNodes(PathNode[] nodes)
    {
        this.nodes = nodes;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetNodePause(float pause)
    {
        this.nodePause = pause;
    }

    public int GetCurrentNodeIndex()
    {
        return currentNode;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsMoving)
        {
            timer += Time.deltaTime * speed;
            if (transform.position != currentPosition)
            {
                transform.position = Vector3.Lerp(transform.position, currentPosition, timer);
            }
            else
            {
                if (currentNode < nodes.Length - 1)
                {
                    currentNode++;
                    StartCoroutine(Pause());
                }
                else
                {
                    if (onEndPath != null)
                    {
                        onEndPath(gameObject);
                    }

                }
                CheckNode();
            }
        }
    }

    IEnumerator Pause()
    {
        IsMoving = false;
        yield return new WaitForSeconds(nodePause);
        IsMoving = true;
    }
}
