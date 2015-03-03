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

    public struct Sensor<T> where T:IComparable<T> {
        public SensorType Type { get; private set; }
        public string Name { get; private set; }
        public string UnitName { get; private set; }
        public Range<T> Range { get; private set; }
        public T Resolution { get; private set; }

        public Sensor(SensorType type, string name = "", string unitName = "", T upper = default(T), T lower = default(T), T offset = default(T), T resolution = default(T) ) : this() {
            Type = type;
            Name = name;
            UnitName = unitName;
            Range = new Range<T>(upper, lower, offset);
            Resolution = resolution;
        }
    }
}
