using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.CommonRc.MeasuringUnits {
    class Ev : IMeasuringUnit {
        #region コンストラクタ/デストラクタ
        /// <summary>
        /// 初期化時に使用するセンサを指定する。
        /// </summary>
        /// <param name="sensors"></param>
        public Ev(params Sensor[] sensors) {
            if ( 0 <sensors.Where(sensor => sensor.Type != SensorType.Displacement).Count() ) {
                throw new ArgumentException("クラスEvに割り当て可能なSensorTypeはSensorType.Displacementのみです。");
            }

            sensorsValue = new Sensor[sensors.Length];
            displacementCountValue = sensors.Where(sensor => sensor.Type == SensorType.Displacement).Count();
            
            foreach ( var s in sensors.Select((v, i) => new { Value = v, Index = i }) ) {
                sensorsValue[s.Index] = s.Value;                
            }
        }
        
        ~Ev() {
            Dispose(false);
        }
        #endregion // コンストラクタ/デストラクタ

        #region IMeasuringUnit メンバー

        public int AngularCount { get { return 0; } }

        int displacementCountValue = 0;
        public int DisplacementCount { get { return displacementCountValue; } }

        public int HumidityCount { get { return 0; } }

        public int TemperatureCount { get { return 0; } }

        Sensor[] sensorsValue = null;
        public Sensor[] Sensors { get { return (Sensor[])sensorsValue.Clone(); } }

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

        public SensorType[] GetSensors() { return null; }

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
            // TODO: リソース解放コード
        }

        #endregion
    }
}
