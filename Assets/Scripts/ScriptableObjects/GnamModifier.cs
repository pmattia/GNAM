using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    public abstract class GnamModifier : ScriptableObject
    {
        public abstract void Activate(EaterDto eater);
    }
}
