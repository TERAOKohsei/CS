using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CS.Common;

namespace CS.CommonRc.MeasuringUnits {
    public interface IMeasuringUnit : IUnit {
        string ProductName { get; }
        Sensor<double>[] Sensors { get; }
        void Measure();
        double[] GetValues();
        double[] GetAngulars();
        double[] GetDisplacements();
        double[] GetHumidities();
        double[] GetTemperatures();
    }
}
