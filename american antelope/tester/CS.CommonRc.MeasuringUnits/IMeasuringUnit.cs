using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CS.Common;

namespace CS.CommonRc.MeasuringUnits {
    interface IMeasuringUnit : IUnit {
        int AngularCount { get; }
        int DisplacementCount { get; }
        int HumidityCount { get; }
        int TemperatureCount { get; }
        string[] GetAngularNames();
        string[] GetDisplacementNames();
        string[] GetHumidityNames();
        string[] GetTemperatureNames();
        void Measure();
        double[] GetValues();
        double[] GetAngulars();
        double[] GetDisplacements();
        double[] GetHumidities();
        double[] GetTemperatures();
        void SetMeasurementItems(int displacementCount = 0, int angularCount = 0, int temeratureCount = 0, int humidityCount = 0);
    }
}
