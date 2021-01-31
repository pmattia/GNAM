using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{
    public class VrifTimeControllerAdapter : ITimeController
    {
        TimeController _timeController;

        public bool TimeSlowing { get { return _timeController.TimeSlowing; } }
        public float SlowTimeScale { get { return _timeController.SlowTimeScale; } }

        public VrifTimeControllerAdapter(TimeController timeController)
        {
            _timeController = timeController;
        }

        public void SlowTime()
        {
            _timeController.SlowTime();
        }

        public void ResumeTime()
        {
            _timeController.ResumeTime();
        }
    }
}
