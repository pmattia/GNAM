using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.AI
{
    public class ShootAtTargets : MonoBehaviour
    {
        RaycastWeapon[] weapons;
        Quaternion[] prevWeaponsRotation;
        [SerializeField] Animator mobAnimator;
        Food[] foods;
        Transform player;
        [SerializeField] AudioClip[] voices;
        [SerializeField] AudioClip spawn;
        [SerializeField] AudioClip stun;
        public event Action<ShootAtTargets> onDeath;

        public float speed = 10;
        public bool shootFoodFirst = true;

        private bool isReadyToShoot = true;
        [SerializeField] bool isShooterEnabled = true;
        private List<Transform> targets = new List<Transform>();
        public Transform currentTarget;
        private Damageable damageable;
        private int coolDownMultiplier;
        public int Type = 0; //0 -> modifier 1 -> killer

        private void Start()
        {
            VRUtils.Instance.PlaySpatialClipAt(spawn, transform.position, 1f, 0.5f);
            player = FindObjectOfType<Mouth>().transform;
            damageable = GetComponent<Damageable>();
          
            weapons = GetComponentsInChildren<RaycastWeapon>().Where(c => c.enabled == true).ToArray();
            prevWeaponsRotation = weapons.Select(w => w.transform.rotation).ToArray();
            if (!shootFoodFirst)
            { 
                //targets.Add(player); 
            }
            SetNewTarget();

            UnityEvent onMobDeath = new UnityEvent();
            onMobDeath.AddListener(()=> {
                if (onDeath != null)
                {
                    onDeath(this);
                }
            });
            damageable.onDestroyed = onMobDeath;

            InvokeRepeating("SetNewTarget", UnityEngine.Random.Range(2, 5), UnityEngine.Random.Range(1, 3));
            InvokeRepeating("SaySomething", UnityEngine.Random.Range(2, 5), UnityEngine.Random.Range(10, 30));
        }

        public void StopTalking()
        {
            CancelInvoke("SaySomething");
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

                if (isReadyToShoot && currentTarget!=null)
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
                    var candidates = foodbags.Where(t => t.foods!= null 
                                                        && t.foods.Count()>0 && t.GetComponent<PathNodesFollower>().CurrentNode < 2
                                                        && (t.foods.Count() > 0 && t.GetComponent<PathNodesFollower>().CurrentNode > 0 && t.GetComponent<PathNodesFollower>().IsMoving)
                                                        );
                    foreach(var foodbag in candidates)
                    {
                        var foods = foodbag.foods.Where(f => f != null);
                        targets.AddRange(foods.Select(food => food.transform));
                    }
                    //targets.AddRange(candidates.SelectMany(c => c.foods.Select(f=>f.transform)));
                }
            }
            else {
                foods = FindObjectsOfType<Food>().Where(f => !f.HasModifiers).ToArray();
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

        void SaySomething()
        {
            var randomAudio = voices[UnityEngine.Random.Range(0, voices.Length)];

            VRUtils.Instance.PlaySpatialClipAt(randomAudio, transform.position, 1f, 0.5f);
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
                }
                weapon.Shoot();
            }
            if(mobAnimator)
                mobAnimator.SetBool("shooting", false);
            yield return new WaitForSeconds(GetRandomCoolDown());
            isReadyToShoot = true;
        }

        public void SetShootingSpeed(ShootingSpeed speed)
        {
            if (speed == ShootingSpeed.Standard)
            {
                coolDownMultiplier = 1;
            }
            else if (speed == ShootingSpeed.Fast)
            {
                coolDownMultiplier = 2;
            }
        }

        int GetRandomCoolDown()
        {
            return UnityEngine.Random.Range(Mathf.FloorToInt(2/ coolDownMultiplier), Mathf.FloorToInt(5/coolDownMultiplier));
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

        public void  OnDamaged(float damage)
        {
       //     Debug.Log($"{name} danneggiato di {damage}");
        }

        private void OnCollisionEnter(Collision collision)
        {
       //     Debug.Log($"{name} colpito da {collision.gameObject.name}");
            if(collision.gameObject.GetComponent<Grabbable>() != null)
            {
                VRUtils.Instance.PlaySpatialClipAt(stun, transform.position, 1f, 0.5f);
                //damageable.DealDamage(5000);
                var isDead = mobAnimator.GetBool("die");
                if (!isDead)
                {
                    mobAnimator.SetBool("die", true);
                    isShooterEnabled = false;

                    StartCoroutine(DelayedCallback(UnityEngine.Random.Range(5,7), () =>
                    {
                        mobAnimator.SetBool("die", false);
                        isShooterEnabled = true;
                    }));
                }
            }
        }
        IEnumerator DelayedCallback(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }
    }


    public enum ShootingSpeed
    {
        Standard,
        Fast
    }
}
