using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RushGameplay : MonoBehaviour
{
    public Foodbag[] foodbags;
    public Transform foodbagSpawnTransform;


    public float foodbagCooldown = 5000f;
    private float realtimeFoodbagCooldown;
    private int currentfoodbagIndex = 0;
    public TextMeshPro timer;

    public Eatable starter;
    public Eatable restarter;
    public bool gamePaused = true;

    // Start is called before the first frame update
    void Start()
    {
        realtimeFoodbagCooldown = foodbagCooldown;
        

        foreach(var foodbag in foodbags)
        {
            foodbag.onClear += () => realtimeFoodbagCooldown = 0;
        }

        starter.onEated += (mouth) => StartRush(); 
        restarter.onEated += (mouth) => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer != null)
        {
            timer.text = Mathf.Round(realtimeFoodbagCooldown).ToString();
        }
        if (!gamePaused)
        {
           
            if (realtimeFoodbagCooldown > 0)
            {
                //Debug.Log(realtimeFoodbagCooldown);
                realtimeFoodbagCooldown -= Time.deltaTime;
            }
            else if (realtimeFoodbagCooldown <= 0)
            {
                
                PauseRush();

                if (currentfoodbagIndex < foodbags.Length - 1)
                {
                    currentfoodbagIndex++;
                    StartCoroutine(WaitForNewFoodbag());
                    StartRush();
                }
                else
                {
                    Debug.Log("END GAME!");
                }
            }
        }
    }

    private void StartRush()
    {
        gamePaused = false;
        foodbags[currentfoodbagIndex].gameObject.transform.SetPositionAndRotation(foodbagSpawnTransform.position,foodbagSpawnTransform.rotation);
        foodbags[currentfoodbagIndex].Show();
    }

    private void PauseRush()
    {
        gamePaused = true;
        Debug.Log($"stop rush {currentfoodbagIndex}");
        foodbags[currentfoodbagIndex].Hide();
    }

    IEnumerator WaitForNewFoodbag()
    {
        Debug.Log("2 sec pause");
        yield return new WaitForSeconds(2);

        realtimeFoodbagCooldown = foodbagCooldown;
        
    }
}
