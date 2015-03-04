using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.CommonRc.Inspections {
    [Flags()]
    public enum InspectionItems : uint {
        PositioningAccuracy = 0x1,
        RepeatabilityofPositioning = 0x2,
        LostMotion = 0x4,
        MotionAccuracyHorizontal = 0x8,
        MotionAccuracyVertical = 0x10,
        MOtionAccuracyYaw = 0x20,
        MotionAccuracyPitch = 0x40,
        ParallelismOfMotion = 0x80,
    }

    public struct InspectionCondition {
        public int LowerSpeed { get; private set; }
        public int UppeerSpeed { get; private set; }
        public int AccelerationMilliSecond { get; private set; }
        public double LostMotionCorrectValue { get; private set; }
        public int WaitMilliSecondAfterStopped { get; private set; }
        public int MeasuringPoints { get; private set; }
        public int RepeatCount { get; private set; }
        public double[] MeasuringPositions { get; private set; }
    }
}
