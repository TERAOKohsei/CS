using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.CommonRc.Inspections {
    [Flags()]
    public enum InspectionItems : uint {
        Nothing = 0x0,
        TravelRange = 0x1,
        PositioningAccuracy = 0x2,
        RepeatabilityofPositioning = 0x4,
        LostMotion = 0x8,
        MotionAccuracyHorizontal = 0x10,
        MotionAccuracyVertical = 0x20,
        MotionAccuracyYaw = 0x40,
        MotionAccuracyPitch = 0x80,
        ParallelismOfMotion = 0x100,
    }

    public struct InspectionCondition {
        public InspectionItems InspectionItems { get; private set; }
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
