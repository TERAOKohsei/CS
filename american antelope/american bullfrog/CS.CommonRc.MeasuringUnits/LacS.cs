using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.CommonRc.MeasuringUnits {
    class LacS : MeasuringUnit  {
        double[] currentAngle = new double[2];

        public LacS(int id, string managementNumber, string manufacturer, string productName, string productType, string serialNumber, int axisCount,
            IEnumerable<string> axisNames, IEnumerable<int> sensorCodes) : base(id, managementNumber, manufacturer, productName, productType, serialNumber, axisCount, axisNames, sensorCodes) {
        }

        public override void Measure() {
            throw new NotImplementedException();
        }

        public override double[] GetValues() {
            return GetAngulars();
        }

        public override double[] GetAngulars() {
            return (double[])currentAngle.Clone();
        }

        public override double[] GetDisplacements() {
            throw new NotImplementedException();
        }

        public override double[] GetHumidities() {
            throw new NotImplementedException();
        }

        public override double[] GetTemperatures() {
            throw new NotImplementedException();
        }

        public override void Connect() {
            throw new NotImplementedException();
        }
    }
}
