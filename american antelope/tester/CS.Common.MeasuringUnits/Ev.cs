using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common.MeasuringUnits {
    class Ev : IMeasuringUnit {
        #region IMeasuringUnit メンバー

        public void Measure() {
            System.Diagnostics.Debug.WriteLine("Measure");
        }

        public double[] GetValues() {
            var values = new double[6];

            for ( int i = 0; i < 6; ++i ) {
                values[i] = i;
            }

            System.Diagnostics.Debug.WriteLine("GetValues");
            return values;
        }

        #endregion

        #region IUnit メンバー

        public bool IsConnected { get; private set; }

        public void Connect() {
            IsConnected = true;
        }

        #endregion

        #region IDisposable メンバー

        public void Dispose() {
        }

        #endregion
    }
}
