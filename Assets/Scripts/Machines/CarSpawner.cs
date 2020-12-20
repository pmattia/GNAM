using Assets.Scripts.Machines;
using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class CarSpawner : MonoBehaviour
    {
        public event Action<GameObject> onSpawned;
        [SerializeField]
        private GameObject gameObjectToSpawn;
        [SerializeField]
        private Lever wheel;
        [SerializeField]
        private Lever speed;


        public void SpawnObject()
        {
            var clone = Instantiate(gameObjectToSpawn, transform.position, Quaternion.identity);
            var damagable = clone.GetComponent<Damageable>();
            if(damagable != null)
            {
                damagable.onDestroyed.AddListener(SpawnObject);
                damagable.Health = 10;
            }

            var carController = clone.GetComponent<CarController>();
            wheel.onLeverChange.AddListener(carController.SetSteering);
            speed.onLeverChange.AddListener(carController.SetSpeed);
        }

    }
}
