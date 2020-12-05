using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class ShootAtPlayer : MonoBehaviour
    {
        public RaycastWeapon[] weapons;
        public Eatable[] eatables;

        public Transform target;
        public float speed = 10;
        public bool isRefiling = false;

        private List<Transform> targets = new List<Transform>();

        private void Start()
        {
            weapons = GetComponentsInChildren<RaycastWeapon>();
            targets.Add(target);
            targets.AddRange(eatables.Select(e => e.transform));

        }

        void Update()
        {
            // Rotate the camera every frame so it keeps looking at the target

            transform.LookAt(target);

            

            // Same as above, but setting the worldUp parameter to Vector3.left in this example turns the camera on its side
            //transform.LookAt(target, Vector3.left);

            //Quaternion toRotation = Quaternion.LookRotation(transform.forward, target.position);
            //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.time);

            if (!isRefiling)
            {
                var newTarget = targets[UnityEngine.Random.Range(0, targets.Count())];
                transform.LookAt(newTarget);
                StartCoroutine(ShootAndRefil());

            }
        }

        IEnumerator ShootAndRefil()
        {
            yield return new WaitForFixedUpdate();
            foreach(var weapon in weapons)
            {
                weapon.Shoot();
            }
            isRefiling = true;
            yield return new WaitForSeconds(3);
            isRefiling = false;
        }

    }
}
