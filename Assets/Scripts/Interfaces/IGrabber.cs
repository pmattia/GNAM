using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ScriptableObjects
{
    public interface IGrabber
    {
        bool ForceGrab { get; set; }
        bool Enabled { get; set; }
        void TryRelease();
    }
}
