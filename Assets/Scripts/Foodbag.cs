using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets.Scripts;

public class Foodbag : MonoBehaviour
{
    public event Action onClear;
    public List<Food> foods;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var food in foods)
        {
            food.onEated += (mouth) => Eated(mouth, food);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Eated(Mouth mouth, Food eated)
    {
        Destroy(eated.gameObject);
        foods.Remove(eated);
        Debug.Log($"foods: {foods.Count()}");
        if (foods.Count() == 0)
        {
            onClear();
            Debug.Log("TUTTO MANGIATO");
        }
    }

    public void Hide()
    {
        foreach(var food in foods)
        {
            //food.gameObject.SetActive(false);
            
        }
        gameObject.SetActive(false);
    }

    public void Show()
    {
        foreach (var food in foods)
        {
            food.gameObject.SetActive(true);
            
        }
        gameObject.SetActive(true);
    }
}
