using Assets.Scripts.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Machines
{
    public class MachinesGameplay : MonoBehaviour
    {
        public List<FoodDispenser> foodDispensers;
        public List<ShootAtTargets> shooters;
        public Timer timer;
        public TextMeshPro billboard;

        private int points = 0;

        private void Start()
        {
            timer.StartTimer();

            foreach(var spawner in foodDispensers)
            {
                spawner.onSpawned += (obj) =>
                {
                    var food = obj.GetComponent<Food>();
                    if(food != null)
                    {
                        food.onEated += (eater) =>
                        {
                            points++;
                            billboard.text = points.ToString();

                            if (points > 1)
                            {
                                var validShooters = shooters.Where(s => !s.gameObject.activeSelf).ToArray();
                                if (validShooters.Length > 0)
                                {
                                    var shooter = validShooters[UnityEngine.Random.Range(0, validShooters.Length)];
                                    shooter.gameObject.SetActive(true);
                                }
                            }
                        };
                    }
                };
            }
        }
    }
}
