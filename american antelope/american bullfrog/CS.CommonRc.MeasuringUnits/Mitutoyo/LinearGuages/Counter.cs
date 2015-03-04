using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ports = System.IO.Ports;
using CS.Common.Communications;

namespace CS.CommonRc.MeasuringUnits.Mitutoyo.LinearGuages {
    public enum CounterType {
        Ev, Eh101p,
        /// <summary>
        /// 内部処理用メンバ、コントローラのタイプとしては選択不可です。
        /// </summary>
        Count,
    }

    public struct SerialPortSettings {
        public string PortName;
        public int BaudRate;
        public Ports.Parity Parity;
        public int DataBits;
        public SerialPortSettings(string portName = "COM1", int baudRate = 9600, Ports.Parity parity = Ports.Parity.Even, int dataBits = 7) {
            PortName = portName;
            BaudRate = baudRate;
            Parity = parity;
            DataBits = dataBits;
        }
    }

    public class Counter : IMeasuringUnit {
        private ICommunication port = null;
        private double[] currentDisplacement;

        #region コンストラクタ/デストラクタ
        /// <summary>
        /// 初期化時に使用するセンサを指定する。
        /// </summary>
        /// <param namePropertyValue="sensors"></param>
        public Counter(CounterType type, SerialPortSettings portSettings, params Sensor<double>[] sensors) {
            switch ( type ) {
            case CounterType.Ev:
                ProductName = "EV";
                displacementCountValue = 10;
                break;
            case CounterType.Eh101p:
                ProductName = "EH-101P";
                displacementCountValue = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, "未実装/未知のカウンタです。EVもしくはEH-101Pのいずれかを指定してください。");
            }

            if ( displacementCountValue < sensors.Count() ) {
                throw new ArgumentException(String.Format("指定したセンサの数が多すぎます。{0}以下で指定してください。", DisplacementCount), "sensors");
            }

            port = new SerialPort(portSettings.PortName, portSettings.BaudRate, portSettings.DataBits, portSettings.Parity, Ports.StopBits.Two);

            if ( 0 <sensors.Where(sensor => sensor.Type != SensorType.Displacement).Count() ) {
                throw new ArgumentException("クラスEvに割り当て可能なSensorTypeはSensorType.Displacementのみです。");
            }

            sensorsValue = new Sensor<double>[sensors.Length];
            displacementCountValue = sensors.Where(sensor => sensor.Type == SensorType.Displacement).Count();
            currentDisplacement = new double[DisplacementCount];
            
            foreach ( var s in sensors.Select((v, i) => new { Value = v, Index = i }) ) {
                sensorsValue[s.Index] = s.Value;                
            }
        }
        
        #endregion // コンストラクタ/デストラクタ

        #region Methods

        private void DisplayCurrentValue(int channel) {
            string cmd = String.Format("CN{0:D2}", channel);
            port.WriteLine(cmd);
            port.ReadLine();
        }

        private void GetDisplacementToBuffer(int channel) {
            string cmd = String.Format("GA{0:D2}", channel);
            port.WriteLine(cmd);
            string data = port.ReadLine();
            if ( String.Compare(data, 0, "GN", 0, 2) != 0 ) {
                throw new InvalidOperationException("想定外の返答がミツトヨカウンタから帰ってきました。");
            } else {
                currentDisplacement[channel] = double.Parse(data.Split(',')[1]);
            }
        }

        #endregion // Methods

        #region IMeasuringUnit メンバー

        public string ProductName { get; private set; }

        public int AngularCount { get { return 0; } }

        int displacementCountValue = 0;
        public int DisplacementCount { get { return displacementCountValue; } }

        public int HumidityCount { get { return 0; } }

        public int TemperatureCount { get { return 0; } }

        Sensor<double>[] sensorsValue = null;
        public Sensor<double>[] Sensors { get { return (Sensor<double>[])sensorsValue.Clone(); } }

        public void Measure() {
            foreach ( var s in Sensors.Select((v, i) => new { Value = v, Index = i }) ) {
                DisplayCurrentValue(s.Index);
                GetDisplacementToBuffer(s.Index);
            }
        }

        public double[] GetValues() {
            return (double[])currentDisplacement.Clone();
        }

        public double[] GetAngulars() { return null; }

        public double[] GetDisplacements() {
            return (double[])currentDisplacement.Clone();
        }

        public double[] GetHumidities() { return null; }

        public double[] GetTemperatures() { return null; }

        public SensorType[] GetSensors() { return null; }

        #endregion

        #region IUnit メンバー

        public bool IsConnected { get { return port == null ? port.IsOpen : false; } }

        public void Connect() {
            port.Open();
        }

        #endregion

        #region IDisposable メンバー

        ~Counter() {
            Dispose(false);
        }
        
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if ( disposing && port != null ) {
                port.Dispose();
                port = null;
            }
        }

        #endregion
    }
}
