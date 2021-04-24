using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets.Scripts;
using Assets.Scripts.Gameplay;
using BNG;
using Assets.Scripts.Interfaces;

public class DynamicFoodbag : MonoBehaviour
{
    List<SnapZone> snapZones;
    public event Action onClear;
    public event Action<EaterDto, Food> onFoodEated;
    List<Food> foods;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        snapZones = GetComponentsInChildren<SnapZone>().ToList();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Eated(EaterDto eater, Food eated)
    {
        Debug.Log($"mangiato {eated.name}");
        Destroy(eated.gameObject);
        foods.Remove(eated);

        if (onFoodEated != null)
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
        foreach (var food in foods)
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

    public void AddFoods(IEnumerable<GameObject> foodGameObjects)
    {
        foreach(var food in foodGameObjects)
        {
            var availableSnapZones = snapZones.Where(s => s.HeldItem == null).ToList();
            var randomSnapZone = availableSnapZones[UnityEngine.Random.Range(0, availableSnapZones.Count())];
            var clone = Instantiate(food, transform);
            clone.transform.localPosition = randomSnapZone.transform.localPosition;
            clone.transform.localRotation = randomSnapZone.transform.localRotation;
            randomSnapZone.GrabGrabbable(clone.GetComponent<Grabbable>());

            var foodComponent = clone.GetComponent<Food>();
            if(foodComponent == null)
            {
                foodComponent = clone.GetComponentInChildren<Food>();
            }
            foods = new List<Food>();
            foods.Add(foodComponent);
            foodComponent.onEated += (eater) => Eated(eater, foodComponent);
        }
    }
}

