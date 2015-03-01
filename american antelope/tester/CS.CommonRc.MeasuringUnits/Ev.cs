using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.CommonRc.MeasuringUnits {
    class Ev : IMeasuringUnit {

        #region コンストラクタ/デストラクタ
        private Ev() : this(null) {
        }

        public Ev(params string[] sensorNames) {
            sensorNames = new string[displacementCountValue];
            displacementCountValue = sensorNames.Length;
            foreach ( var name in sensorNames.Select((v, i) => new { Value = v, Index = i }) ) {
                sensorNames[name.Index] = name.Value;
            }
        }
        
        ~Ev() {
            Dispose(false);
        }
        #endregion // コンストラクタ/デストラクタ

        #region IMeasuringUnit メンバー

        public int AngularCount { get { return 0; } }

        int displacementCountValue;
        public int DisplacementCount { get { return displacementCountValue; } }

        public int HumidityCount { get { return 0; } }

        public int TemperatureCount { get { return 0; } }

        public string[] GetAngularNames() { return null; }

        string[] sensorNames;
        public string[] GetDisplacementNames() { return sensorNames; }

        public string[] GetHumidityNames() { return null; }

        public string[] GetTemperatureNames() { return null; }

        public void Measure() {
            throw new NotImplementedException();
        }

        public double[] GetValues() {
            throw new NotImplementedException();
        }

        public double[] GetAngulars() { return null; }

        public double[] GetDisplacements() {
            throw new NotImplementedException();
        }

        public double[] GetHumidities() { return null; }

        public double[] GetTemperatures() { return null; }

        public void SetMeasurementItems(int displacementCount = 0, int angularCount = 0, int temeratureCount = 0, int humidityCount = 0) {
            throw new NotImplementedException();
        }

        #endregion

        #region IUnit メンバー

        public bool IsConnected {
            get { throw new NotImplementedException(); }
        }

        public void Connect() {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable メンバー

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
        }

        #endregion
    }
}
