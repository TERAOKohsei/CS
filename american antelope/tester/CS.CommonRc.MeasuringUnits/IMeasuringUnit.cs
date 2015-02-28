using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CS.Common;

namespace CS.CommonRc.MeasuringUnits {
    interface IMeasuringUnit : IUnit {
        void Measure();
        double[] GetValues();
    }
}
