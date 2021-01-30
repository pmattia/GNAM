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
        Quaternion[] prevWeaponsRotation;
        [SerializeField] Animator mobAnimator;
        Food[] foods;
        Transform player;
        
        public float speed = 10;
        public bool shootFoodFirst = true;

        private bool isReadyToShoot = true;
        [SerializeField] bool isShooterEnabled = true;
        private List<Transform> targets = new List<Transform>();
        public Transform currentTarget;
        private Damageable damageable;

        private void Start()
        {
            player = FindObjectOfType<Mouth>().transform;
            damageable = GetComponent<Damageable>();

            weapons = GetComponentsInChildren<RaycastWeapon>().Where(c => c.enabled == true).ToArray();
            prevWeaponsRotation = weapons.Select(w => w.transform.rotation).ToArray();
            if (!shootFoodFirst)
            { 
                targets.Add(player); 
            }
            SetNewTarget();

            InvokeRepeating("SetNewTarget", UnityEngine.Random.Range(2, 5), UnityEngine.Random.Range(2, 5));
        }

        void FixedUpdate()
        {
            if (isShooterEnabled) { 
                if (currentTarget != null)
                {
                    TakeAim(currentTarget);
                }
                else
                {
                    TakeAim(player);
                }

                if (isReadyToShoot)
                {
                    StartCoroutine(ShootAndRefil());
                }
            }
        }

        void SetNewTarget()
        {
            targets.Clear();
            var tableBelt = FindObjectOfType<TableBelt>();
            if (tableBelt != null)
            {
                if (tableBelt.trays.Count > 0)
                {
                    var foodbags = tableBelt.trays.Select(t => t.GetComponentInChildren<Foodbag>()).ToList();
                    var paths = foodbags.Select(t => t.GetComponent<PathNodesFollower>()).ToList();
                    var candidates = foodbags.Where(t => t.GetComponent<PathNodesFollower>().CurrentNode < 2);
                    targets.AddRange(candidates.SelectMany(c => c.foods.Select(f=>f.transform)));
                }
            }
            else {
                foods = FindObjectsOfType<Food>();
                targets.AddRange(foods.Select(e => e.transform));
                targets = targets.Where(t => t != null).ToList();
                
            }


            if (targets.Count() == 0 && shootFoodFirst)
            {
                //currentTarget = player;
            }
            else
            {
                currentTarget = targets[UnityEngine.Random.Range(0, targets.Count())];
            }
        }

        IEnumerator ShootAndRefil()
        {
            if(mobAnimator)
                mobAnimator.SetBool("shooting", true);
            isReadyToShoot = false;
            yield return new WaitForSeconds(.15f);
            foreach(var weapon in weapons)
            {
                if (currentTarget && mobAnimator) //solo per monokuma
                {
                    Vector3 relativePos = currentTarget.position - weapon.transform.position;
                    weapon.transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                    Debug.Log("rotation");
                }
                weapon.Shoot();
            }
            if(mobAnimator)
                mobAnimator.SetBool("shooting", false);
            yield return new WaitForSeconds(UnityEngine.Random.Range(2, 5));
            isReadyToShoot = true;
        }

        void TakeAim(Transform target)
        {
            transform.rotation = RotationLerpTo(transform, target.position);

            if (!mobAnimator)  //solo per gatti
            {
                foreach (var weapon in weapons)
                {
                    weapon.transform.rotation = RotationLerpTo(weapon.transform, target.position);
                }
            }
        }

        private Quaternion RotationLerpTo(Transform observer, Vector3 targetPoint)
        {
            Vector3 relativePos = targetPoint - observer.position;

            // the second argument, upwards, defaults to Vector3.up
            Quaternion toRotation = Quaternion.LookRotation(relativePos, Vector3.up);
            return Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);
        }
        public void EnableShooter()
        {
            isShooterEnabled = true;
        }
        public void DisableShooter()
        {
            isShooterEnabled = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.GetComponent<Grabbable>() != null)
            {
                damageable.DealDamage(100);
            }
        }
    }
}
