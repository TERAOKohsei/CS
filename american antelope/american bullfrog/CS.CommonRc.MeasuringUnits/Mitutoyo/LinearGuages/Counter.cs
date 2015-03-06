﻿using System;
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

    public class Counter : MeasuringUnit {
        private double[] currentDisplacement = null;

        #region コンストラクタ/デストラクタ
        public Counter(int id, string managementNumber, string manufacturer, string productName, string productType, string serialNumber, int axisCount,
            IEnumerable<string> axisNames, IEnumerable<int> sensorCodes) : base(id, managementNumber, manufacturer, productName, productType, serialNumber, axisCount, axisNames, sensorCodes) {
            
            currentDisplacement = new double[axisCount];
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

        #region MeasuringUnit メンバー

        public override void Measure() {
            foreach ( var s in Sensors.Select((v, i) => new { Value = v, Index = i }) ) {
                DisplayCurrentValue(s.Index);
                GetDisplacementToBuffer(s.Index);
            }
        }

        public override double[] GetValues() {
            return (double[])currentDisplacement.Clone();
        }

        public override double[] GetAngulars() { return null; }

        public override double[] GetDisplacements() {
            return (double[])currentDisplacement.Clone();
        }

        public override double[] GetHumidities() { return null; }

        public override double[] GetTemperatures() { return null; }

        public SensorType[] GetSensors() { return null; }

        public override void ShowSettingDialogue() {
            using ( var form = new FormCounterSettings(this) ) {
                form.ShowDialog();
            }
        }

        public override ICommunication Communication {
            get { return port; }
            set {
                if ( value.GetType().Equals(typeof(SerialPort)) ) {
                    var p = (SerialPort)value;
                    if ( (p.BaudRate != 9600) || (p.DataBits != 7) || (p.Parity != Ports.Parity.Even) || (p.StopBits != Ports.StopBits.Two) || (p.NewLine != "\r\n") ) {
                        throw new ArgumentException("ミツトヨEV、EHカウンタで、シリアル通信は現在、工場出荷時の設定のみサポートしています。");
                    }
                } else {
                    throw new ArgumentException("ミツトヨEV、EHカウンタで、指定されたICommunicationオブジェクトはサポートされていません。");
                }

                port = value;
            }
        }

        public override void Reset() {
            port.WriteLine("CR00");
            port.ReadLine();
        }
        #endregion // MeasuringUnitメンバ

        #region IUnit メンバー

        public override void Connect() {
            port.Open();
        }

        #endregion

        #region IDisposable メンバー

        protected override void Dispose(bool disposing) {
            if ( disposing && port != null ) {
                port.Dispose();
                port = null;
            }
        }

        #endregion
    }
}
