using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets.Scripts;

public class Foodbag : MonoBehaviour
{
    public event Action onClear;
    public event Action<EaterDto,Food> onFoodEated;
    public List<Food> foods;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var food in foods.Where(f => f != null))
        {
            food.onEated += (eater) => Eated(eater, food);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Eated(EaterDto eater, Food eated)
    {
        Destroy(eated.gameObject);
        foods.Remove(eated);
        
        if(onFoodEated != null)
        {
            onFoodEated(eater, eated);
        }

        if (foods.Count() == 0 && onClear != null)
        {
            onClear();
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
