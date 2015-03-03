using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.CommonRc.Stages {
    public enum StageType {
        Linear = 0,
        Virtical = 1,
        Rotary = 2,
        UnknownType = -1,
    }

    public struct Stage {
        public StageType Type { get; private set; }
        public string Name { get; private set; }
        public string UnitName { get; private set; }
        public double Resolution { get; private set; }
        public Range<double> Range { get; private set; }
    }
}
