using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.CommonRc.MeasuringUnits {
    public enum SensorType {
        Displacement = 0,
        Angular = 1,
        Humidity = 2,
        Temperature = 3,
        UnknownType = -1,
    }

    public struct Sensor {
        public SensorType Type { get; private set; }
        public string Name { get; private set; }
        public string UnitName { get; private set; }
        public double Resolution { get; private set; }
        public double Offset { get; private set; }

        public Sensor(SensorType type, string name = "", string unitName = "", double resolution = 1.0, double offset = 0.0) : this() {
            Type = type;
            Name = name;
            UnitName = unitName;
            Resolution = resolution;
            Offset = offset;
        }

        public double ToRealValue(double rowValue) {
            return rowValue * Resolution + Offset;
        }

        public double ToRowValue(double realValue) {
            return (realValue - Offset) / Resolution;
        }
    }
}
