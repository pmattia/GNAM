using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Inventory: MonoBehaviour
    {
        public virtual void OnSnap(Grabbable gnabbable)
        {
            var autodestroyer = gnabbable.GetComponent<Autodestroy>();
            if (autodestroyer != null)
            {
                Destroy(autodestroyer);
            }
        }
    }
}
