using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using CS.Common.Communications;
using Ports = System.IO.Ports;

namespace CS.CommonRc.MeasuringUnits {
    public class LacS : MeasuringUnit {
        double[] currentAngle = new double[2];

        protected LacS()
            : base() {
        }

        public LacS(int id, string managementNumber, string manufacturer, string productName, string productType, string serialNumber, int axisCount,
            IEnumerable<string> axisNames, IEnumerable<int> sensorCodes) : base(id, managementNumber, manufacturer, productName, productType, serialNumber, axisCount, axisNames, sensorCodes) {
        }

        protected override void MeasureImmediately() {
            port.WriteLine("D");
            var words = port.ReadLine().Split(',');
            foreach ( var w in words.Select((v, i) => new { v, i }) ) {
                if ( w.v == "<" ) {
                    currentAngle[w.i] = double.NaN;
                } else {
                    double.TryParse(w.v, out currentAngle[w.i]);
                }
            }
        }

        public override double[] GetValues() {
            return GetAngulars();
        }

        public override double[] GetAngulars() {
            return (double[])currentAngle.Clone();
        }

        public override double[] GetDisplacements() {
            return null;
        }

        public override double[] GetHumidities() {
            return null;
        }

        public override double[] GetTemperatures() {
            return null;
        }

        public override void ShowSettingDialogue() {
            using ( var form = new FormLacsSettings(this) ) {
                form.ShowDialog();
            }
        }

        public override void Connect() {
            port.Open();
        }

        public override void Reset() {
            port.WriteLine("I");
            port.Read();
            port.WriteLine("RX");
            port.Read();
            port.WriteLine("RY");
            port.Read();
        }

        protected override void Dispose(bool disposing) {
            if ( disposing && (port != null) ) {
                port.Dispose();
                port = null;
            }
        }

        public override Common.Communications.ICommunication Communication {
            get { return port; }
            set {
                if ( value.GetType().Equals(typeof(SerialPort)) ) {
                    var p = (SerialPort)value;
                    if ( (p.BaudRate != 9600) || (p.DataBits != 8) || (p.Parity != Ports.Parity.Even) || (p.StopBits != Ports.StopBits.Two) || (p.NewLine != "\r\n") ) {
                        throw new ArgumentException("LAC-Sのシリアル通信は現在、工場出荷時の設定のみサポートしています。");
                    }
                } else {
                    throw new ArgumentException("LAC-Sでは指定されたICommunicationオブジェクトはサポートされていません。");
                }

                port = value;
            }
        }

        #region IXmlSerializable メンバー

        public override void ReadXml(System.Xml.XmlReader reader) {
            base.ReadXml(reader);
            switch ( reader.ReadElementContentAsString("Communication", "") ) {
            case "SerialPort":
                if ( port == null ) {
                    port = new SerialPort();
                }
                port.ReadXml(reader);
                break;
            case "Null":
            default:
                if ( port != null ) {
                    port.Dispose();
                    port = null;
                }
                break;
            }
        }

        public override void WriteXml(System.Xml.XmlWriter writer) {
            base.WriteXml(writer);
            if ( port == null ) {
                writer.WriteElementString("Communication", "Null");
            } else {
                writer.WriteElementString("Communication", "SerialPort");
                port.WriteXml(writer);
            }
        }

        #endregion
    }
}
