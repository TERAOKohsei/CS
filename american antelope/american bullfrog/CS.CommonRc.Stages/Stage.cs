using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CS.CommonRc.Inspections;

namespace CS.CommonRc.Stages {
    public enum StageType {
        Linear = 0,
        Virtical = 1,
        Rotary = 2,
        UnknownType = -1,
    }

    public struct Stage {
        public string ProductName { get; private set; }
        public StageType ProductType { get; private set; }
        public StageSpec Spec { get; private set; }
        public InspectionItems InspectionItems { get; private set; }
    }

    public struct StageSpec {
        public GeneralSpec General { get; private set; }
        public AccuracySpec Accuracy { get; private set; }
    }

    public struct AccuracySpec {
        public double PositionAccuracy { get; private set; }
        public double RepeatabilityOfPositioning { get; private set; }
        public double LostMotion { get; private set; }
        public double MotionAccuracyHorizontal { get; private set; }
        public double MotionAccuracyVertical { get; private set; }
        public double MotionAccuracyYaw { get; private set; }
        public double MotionAccuracyPitch { get; private set; }
        public double ParallelismOfMotion { get; private set; }
    }

    public struct GeneralSpec {
        public bool HaveOriginSensor { get; private set; }
        public bool HaveLimitSensors { get; private set; }
        public string UnitName { get; private set; }
        public double Resolution { get; private set; }
        public Range<double> Travels { get; private set; }
    }
}
