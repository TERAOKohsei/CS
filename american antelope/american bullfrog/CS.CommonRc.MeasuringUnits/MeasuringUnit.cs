using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml.Serialization;
using CS.Common;

namespace CS.CommonRc.MeasuringUnits {
    public abstract class MeasuringUnit : IUnit, IXmlSerializable {
        public int ID { get; private set; }
        public string ManagementNumber { get; private set; }
        public string Manufacturer { get; private set; }
        public string ProductName { get; private set; }
        public string ProductType { get; private set; }
        public string SerialNumber { get; private set; }
        public int AxisCount { get; private set; }
        public IEnumerable<string> AxisNames { get; private set; }
        public IEnumerable<int> SensorCodes { get; private set; }

        protected CS.Common.Communications.ICommunication port;
        private Sensor[] innerSensors = null;
        public Sensor[] Sensors { get { return (Sensor[])innerSensors.Clone(); } }
        public abstract void Measure();
        public abstract void Reset();
        public abstract double[] GetValues();
        public abstract double[] GetAngulars();
        public abstract double[] GetDisplacements();
        public abstract double[] GetHumidities();
        public abstract double[] GetTemperatures();
        public abstract CS.Common.Communications.ICommunication Communication { get; set; }

        protected MeasuringUnit() {
        }

        public static IList<MeasuringUnit> LoadFromFile(string measuringUnitListPath) {
            var list = new List<MeasuringUnit>();

            using ( var sr = new StreamReader(measuringUnitListPath, Encoding.GetEncoding("shift-jis")) ) {
                sr.ReadLine();  // 項目名なので読み込むだけ。

                while ( !sr.EndOfStream ) {
                    var words = sr.ReadLine().Split(',');
                    if ( words.Count() < 9 ) {
                        throw new InvalidDataException(String.Concat("測定機リスト ファイル \"", measuringUnitListPath, "\"の内容が不正です。"));
                    }

                    int axisCount;
                    if ( !Int32.TryParse(words[6], out axisCount) ) {
                        throw new InvalidDataException(String.Concat("測定機リスト ファイル \"", measuringUnitListPath, "\"で軸数と軸リストの数が不一致です。"));
                    }

                    MeasuringUnit m;
                    int id;
                    Int32.TryParse(words[0], out id);
                    var sc = new int[axisCount];
                    var axisNames = new string[axisCount];
                    foreach ( var s in sc.Select((v, i) => new { Value = v, Index = i }) ) {
                        Int32.TryParse(words[8 + s.Index * 2], out sc[s.Index]);
                        axisNames[s.Index] = words[7 + s.Index * 2];
                    }
                    switch ( words[4] ) {
                    case "LAC-S":
                        m = new LacS(id, words[1], words[2], words[3], words[4], words[5], axisCount, axisNames, sc);
                        break;
                    case "EV-16A":
                    case "EV-16P":
                    case "EH-101P":
                        m = new CS.CommonRc.MeasuringUnits.Mitutoyo.LinearGuages.Counter(id, words[1], words[2], words[3], words[4], words[5], axisCount, axisNames, sc);
                        break;
                    default:
                        m = null;
                        break;
                    }

                    if ( m != null ) {
                        list.Add(m);
                    }
                }
            }

            return list;
        }

        public MeasuringUnit(int id, string managementNumber, string manufacturer, string productName, string productType, string serialNumber, int axisCount,
            IEnumerable<string> axisNames, IEnumerable<int> sensorCodes) {

            ID = id;
            ManagementNumber = managementNumber;
            Manufacturer = manufacturer;
            ProductName = productName;
            ProductType = productType;
            SerialNumber = serialNumber;
            AxisCount = axisCount;
            AxisNames = axisNames;
            SensorCodes = sensorCodes;

            innerSensors = new Sensor[axisCount];
        }

        public void SetSensor(IEnumerable<int> axes, IEnumerable<Sensor> sensors) {
            if ( (axes.Min() < 0) && AxisCount <= axes.Max() ) {
                throw new ArgumentOutOfRangeException("軸の指定に誤りがあります。");
            }

            foreach ( var pair in axes.Zip(sensors, (axis, sensor) => new { axis, sensor }) ) {
                if ( SensorCodes.ElementAt(pair.axis) == pair.sensor.SensorCode ) {
                    innerSensors[pair.axis] = pair.sensor;
                } else {
                    throw new ArgumentException("軸とセンサがマッチしません。");
                }
            }
        }

        public string ToLongString() {
            string str = String.Join(" ", Manufacturer, ProductName, ProductType);

            if ( !String.IsNullOrEmpty(str) ) {
                str += " ";
            }
            if ( !String.IsNullOrEmpty(SerialNumber) ) {
                str += "(" + SerialNumber + ")";
            }

            if ( !String.IsNullOrEmpty(str) ) {
                str += " ";
            }
            if ( !String.IsNullOrEmpty(ManagementNumber) ) {
                str += "- " + ManagementNumber;
            }

            return str;
        }

        public string ToString(int axis) {
            string str = String.Join(" ", Manufacturer, ProductType);


            if ( !String.IsNullOrEmpty(str) ) {
                str += " ";
            }
            if ( !String.IsNullOrEmpty(SerialNumber) ) {
                str += "(" + SerialNumber + ")";
            }

            if ( !String.IsNullOrEmpty(str) ) {
                str += " ";
            }
            if ( !String.IsNullOrEmpty(ManagementNumber) ) {
                str += "- " + ManagementNumber;
            }

            str += " : " + AxisNames.ElementAt(axis);

            return str;
        }

        public abstract void ShowSettingDialogue();

        #region IDisposable members
        
        ~MeasuringUnit() {
          Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
        }

        #endregion // IDisposable members

        #region IUnit メンバー

        public bool IsConnected { get; private set; }

        public abstract void Connect();

        #endregion

        #region IXmlSerializable メンバー

        public virtual System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader) {
            reader.Read();
            ID = reader.ReadElementContentAsInt("ID", "");
            ManagementNumber = reader.ReadElementContentAsString("ManagementNumber", "");
            Manufacturer = reader.ReadElementContentAsString("Manufacturer", "");
            ProductName = reader.ReadElementContentAsString("ProductName", "");
            ProductType = reader.ReadElementContentAsString("ProductType", "");
            SerialNumber = reader.ReadElementContentAsString("SerialNumber", "");
            AxisCount = reader.ReadElementContentAsInt("AxisCount", "");

            reader.ReadStartElement("AxisNames");
            var axisNames = new string[AxisCount];
            for ( int i = 0; i < AxisCount; i++ ) {
                axisNames[i] = reader.ReadElementContentAsString("string", "");
            }
            reader.ReadEndElement();
            AxisNames = axisNames;

            reader.ReadStartElement("SensorCodes");
            var codes = new int[AxisCount];
            for ( int i = 0; i < AxisCount; i++ ) {
                codes[i] = reader.ReadElementContentAsInt("int", "");
            }
            reader.ReadEndElement();
            SensorCodes = codes;

            reader.ReadStartElement("Sensors");
            innerSensors = new Sensor[AxisCount];
            for ( int i = 0; i < AxisCount; i++ ) {
                var xs = new XmlSerializer(typeof(Sensor));
                innerSensors[i] = (Sensor)xs.Deserialize(reader);
            }
            reader.ReadEndElement();
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer) {
            writer.WriteElementString("ID", ID.ToString());
            writer.WriteElementString("ManagementNumber", ManagementNumber);
            writer.WriteElementString("Manufacturer", Manufacturer);
            writer.WriteElementString("ProductName", ProductName);
            writer.WriteElementString("ProductType", ProductType);
            writer.WriteElementString("SerialNumber", SerialNumber);
            writer.WriteElementString("AxisCount", AxisCount.ToString());
            writer.WriteStartElement("AxisNames");
            foreach ( var s in AxisNames ) {
                writer.WriteElementString("string", s);
            }
            writer.WriteEndElement();
            writer.WriteStartElement("SensorCodes");
            foreach ( var code in SensorCodes ) {
                writer.WriteElementString("int", code.ToString());
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Sensors");
            foreach ( var sens in innerSensors ) {
                var xs = new XmlSerializer(typeof(Sensor));
                xs.Serialize(writer, sens);
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
