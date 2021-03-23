using Assets.Scripts.AI;
using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class SupriseMob : MonoBehaviour
    {
        [SerializeField] ShootAtTargets mob;
        [SerializeField] SnapZone cup;
        private void Start()
        {
            mob.gameObject.SetActive(false);
        }

        public void Surprise(Grabbable grabbable)
        {
            Debug.Log("detach");
            if (cup.HeldItem == null)
            {
                mob.gameObject.SetActive(true);
                foreach (var col in mob.GetComponents<Collider>())
                {
                    col.enabled = false;
                }
                foreach (var col in mob.GetComponentsInChildren<Collider>())
                {
                    col.enabled = false;
                }
            }
        }
    }
}
