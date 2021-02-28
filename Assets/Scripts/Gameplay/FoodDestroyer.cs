using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class FoodDestroyer: MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"collision {collision.collider.name}");
            var foodbag = collision.rigidbody.GetComponent<Foodbag>();
            if (foodbag != null)
            {
                var particle = Resources.Load<GameObject>("GnamAutodestroy");
                Instantiate(particle, transform.position, transform.rotation);
                Destroy(foodbag.gameObject);
            }
        }
    }
}
