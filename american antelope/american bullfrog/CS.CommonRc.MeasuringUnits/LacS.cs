using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.CommonRc.MeasuringUnits {
    class LacS : IMeasuringUnit {
        #region Constructors/Destructors
        public LacS(string portName = "COM1") {
        }
        #endregion // Constructors/Destructors
        #region IMeasuringUnit メンバー

        public string ProductName { get { return "LAC-S"; } }

        public Sensor<double>[] Sensors {
            get { throw new NotImplementedException(); }
        }

        public void Measure() {
            throw new NotImplementedException();
        }

        public double[] GetValues() {
            throw new NotImplementedException();
        }

        public double[] GetAngulars() {
            throw new NotImplementedException();
        }

        public double[] GetDisplacements() {
            throw new NotImplementedException();
        }

        public double[] GetHumidities() {
            throw new NotImplementedException();
        }

        public double[] GetTemperatures() {
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
        
        #region IDisposable members
        
        ~LacS() {
          Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            // TODO: リソース解放コード
        }

        #endregion // IDisposable members
    }
}
