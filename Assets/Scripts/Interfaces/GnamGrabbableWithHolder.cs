using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public class GnamGrabbableWithHolder: GnamGrabbable
    {
        public Eatable holder;
        public List<Transform> holderGrabPoints;
        private List<Transform> grabpoints = new List<Transform>();

        private void Start()
        {
            grabpoints.AddRange(GrabPoints);
            holderGrabPoints.ForEach(p => grabpoints.Remove(p));

            GrabPoints.Clear();
            GrabPoints.AddRange(holderGrabPoints);

            holder.onEated += (eater) =>
            {
                GrabPoints = grabpoints;
            };
        }
    }
}
