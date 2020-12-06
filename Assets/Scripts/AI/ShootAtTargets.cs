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
    public class ShootAtTargets : MonoBehaviour
    {
        RaycastWeapon[] weapons;
        Eatable[] eatables;
        Transform player;
        
        public float speed = 10;
        public bool shootFoodFirst = true;

        private bool isReadyToShoot = true;
        private List<Transform> targets = new List<Transform>();
        private Transform currentTarget;

        private void Start()
        {
            player = FindObjectOfType<Mouth>().transform;
            eatables = FindObjectsOfType<Eatable>();

            weapons = GetComponentsInChildren<RaycastWeapon>().Where(c => c.enabled == true).ToArray();
            if (!shootFoodFirst)
            { 
                targets.Add(player); 
            }
            targets.AddRange(eatables.Select(e => e.transform));
            SetNewTarget();

            InvokeRepeating("SetNewTarget", 2.0f, 2f);
        }

        void FixedUpdate()
        {
            if (currentTarget != null)
            {
                TakeAim(currentTarget);
            }

            if (isReadyToShoot)
            {
                StartCoroutine(ShootAndRefil());
            }
        }

        void SetNewTarget()
        {
            targets = targets.Where(t => t != null).ToList();
            if (targets.Count() == 0 && shootFoodFirst)
            {
                currentTarget = player;
            }
            else
            {
                currentTarget = targets[UnityEngine.Random.Range(0, targets.Count())];
            }
        }

        IEnumerator ShootAndRefil()
        {
            yield return new WaitForFixedUpdate();
            foreach(var weapon in weapons)
            {
                weapon.Shoot();
            }
            isReadyToShoot = false;
            yield return new WaitForSeconds(3);
            isReadyToShoot = true;
        }

        void TakeAim(Transform target)
        {
            transform.rotation = RotationLerpTo(transform, target.position);

            foreach (var weapon in weapons)
            {
                weapon.transform.rotation = RotationLerpTo(weapon.transform, target.position);
            }
        }

        private Quaternion RotationLerpTo(Transform observer, Vector3 targetPoint)
        {
            Vector3 relativePos = targetPoint - observer.position;

            // the second argument, upwards, defaults to Vector3.up
            Quaternion toRotation = Quaternion.LookRotation(relativePos, Vector3.up);
            return Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);
        }

    }
}
