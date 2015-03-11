using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CS.CommonRc.MeasuringUnits;
using CS.CommonRc.Stages;
using CS.CommonRc.StageControllers;

using System.IO;
using System.Xml.Serialization;

namespace CS.Applications.AmericanBullfrog {
    public struct Inspector {
        public int Code { get; set; }
        public string Name { get; set; }
        public Inspector(int code, string name)
            : this() {
            Code = code;
            Name = name;
        }
        public override string ToString() {
            return String.Format("{0}: {1}", Code, Name);
        }
    }

    public struct InspectionConditions {
        public string SerialNumber { get; set; }
        public string ProductCode { get; set; }
        public Inspector Inspector { get; set; }
        public string Notes { get; set; }
    }

    public struct AmericanBullfrogSettings : IXmlSerializable {
        public static MeasuringUnit[] MeasuringUnits;
        public static Sensor[] Sensors;
        public static Inspector[] Inspectors;
        public MeasuringUnitCombination LengthUnit;
        public MeasuringUnitCombination HUnit;
        public MeasuringUnitCombination VUnit;
        public MeasuringUnitCombination YUnit;
        public MeasuringUnitCombination PUnit;
        public InspectionConditions Conditions;

        public StageController StageController;
        public Stage Stage;

        public static void LoadMeasuringUnitList(string filePath) {
            try {
                MeasuringUnits = MeasuringUnit.LoadFromFile(filePath).ToArray();
            } catch ( Exception e ) {
                FormMain.Logger.DebugException("測定機リスト読み込み中に例外が発生。", e);
                throw;
            }

            foreach ( var unit in MeasuringUnits ) {
                FormMain.Logger.Trace(String.Format("測定機{0}: {1} {2} ({3})を読み込み", unit.ID, unit.Manufacturer, unit.ProductType, unit.ManagementNumber));
            }
        }

        public static void LoadSensorList(string filePath) {
            try {
                Sensors = Sensor.LoadFromFile(filePath).ToArray();
            } catch ( Exception e ) {
                FormMain.Logger.DebugException("センサリスト読み込み中に例外が発生。", e);
                throw;
            }

            foreach ( var sensor in Sensors ) {
                FormMain.Logger.Trace(String.Format("センサ{0}: {1} {2} ({3})を読み込み", sensor.Id, sensor.Manufacturer, sensor.ProductType, sensor.ManagementNumber));
            }
        }

        public static void LoadInspectorsList(string filePath) {
            var inspectors = new List<Inspector>();

            try {
                using ( StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("shift-jis")) ) {
                    while ( !sr.EndOfStream ) {
                        var words = sr.ReadLine().Split(',');
                        int c;
                        Int32.TryParse(words[0], out c);
                        inspectors.Add(new Inspector(c, words[1]));
                    }
                }
            } catch ( Exception e ) {
                FormMain.Logger.DebugException("検査担当リスト読み込み中に例外が発生。", e);
                throw;
            }

            Inspectors = inspectors.ToArray();
            foreach ( var inspector in Inspectors ) {
                FormMain.Logger.Trace(String.Format("検査担当:{0}を読み込み", inspector.ToString()));
            }
        }

        #region IXmlSerializable メンバー

        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        private void ReadXmlUsingMeasuringUnits(System.Xml.XmlReader reader) {
            reader.ReadStartElement("UsingMeasuringUnits");

            reader.ReadStartElement("LengthUnit");
            LengthUnit.ReadXml(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("HUnit");
            HUnit.ReadXml(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("VUnit");
            VUnit.ReadXml(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("YUnit");
            YUnit.ReadXml(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("PUnit");
            PUnit.ReadXml(reader);
            reader.ReadEndElement();

            reader.ReadEndElement();
        }

        private void WriteXmlUsingMeasuringUnits(System.Xml.XmlWriter writer) {
            writer.WriteStartElement("UsingMeasuringUnits");

            writer.WriteStartElement("LengthUnit");
            LengthUnit.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("HUnit");
            HUnit.WriteXml(writer);
            writer.WriteEndElement();
            
            writer.WriteStartElement("VUnit");
            VUnit.WriteXml(writer);
            writer.WriteEndElement();
            
            writer.WriteStartElement("YUnit");
            YUnit.WriteXml(writer);
            writer.WriteEndElement();
            
            writer.WriteStartElement("PUnit");
            PUnit.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private void ReadXmlStageControllers(System.Xml.XmlReader reader) {
            reader.ReadStartElement("StageControllers");

            // TODO: ステージコントローラは今のところCsControllerのみ
            reader.ReadElementContentAsString("StageControllerObject", "");
            if ( StageController == null ) {
                StageController = new CsController();
            }
            StageController.ReadXml(reader);

            reader.ReadEndElement();
        }

        private void WriteXmlStageControllers(System.Xml.XmlWriter writer) {
            writer.WriteStartElement("StageControllers");

            writer.WriteElementString("StageControllerObject", StageController.GetType().ToString());
            StageController.WriteXml(writer);
            
            writer.WriteEndElement();
        }

        private void ReadXmlMeasuringUnits(System.Xml.XmlReader reader) {
            reader.ReadStartElement("MeasuringUnits");

            int c = reader.ReadElementContentAsInt("Count", "");
            for ( int i = 0; i < c; i++ ) {
                MeasuringUnit m;
                switch ( reader.ReadElementContentAsString("MeasuringUnitObject", "") ) {
                case "CS.CommonRc.MeasuringUnits.LacS":
                    m = new LacS();
                    break;
                case "CS.CommonRc.MeasuringUnits.Mitutoyo.LinearGuages.Counter":
                    m = new CS.CommonRc.MeasuringUnits.Mitutoyo.LinearGuages.Counter();
                    break;
                default:
                    m = null;
                    break;
                }
                if ( m != null) {
                    m.ReadXml(reader);
                    if ( m.Communication != null ) {
                        MeasuringUnits[m.ID].Communication = m.Communication;
                    }
                }
            }

            reader.ReadEndElement();
        }

        private void WriteXmlMeasuringUnits(System.Xml.XmlWriter writer) {
            writer.WriteStartElement("MeasuringUnits");

            writer.WriteElementString("Count", MeasuringUnits.Count().ToString());
            foreach ( var m in MeasuringUnits ) {
                writer.WriteElementString("MeasuringUnitObject", m.GetType().ToString());
                m.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        private void ReadXmlStage(System.Xml.XmlReader reader) {
            var xs = new XmlSerializer(typeof(Stage));
            Stage = (Stage)xs.Deserialize(reader);
        }

        private void WriteXmlStage(System.Xml.XmlWriter writer) {
            var xs = new XmlSerializer(typeof(Stage));
            xs.Serialize(writer, Stage);
        }

        private void ReadXmlInspectionConditions(System.Xml.XmlReader reader) {
            var xs = new XmlSerializer(typeof(InspectionConditions));
            Conditions = (InspectionConditions)xs.Deserialize(reader);
        }

        private void WriteXmlInspectionConditions(System.Xml.XmlWriter writer) {
            var xs = new XmlSerializer(typeof(InspectionConditions));
            xs.Serialize(writer, Conditions);
        }

        public void ReadXml(System.Xml.XmlReader reader) {
            reader.ReadStartElement("AmericanBullfrogSettings");

            ReadXmlUsingMeasuringUnits(reader);
            ReadXmlStageControllers(reader);
            ReadXmlMeasuringUnits(reader);
            ReadXmlStage(reader);
            ReadXmlInspectionConditions(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer) {
            WriteXmlUsingMeasuringUnits(writer);
            WriteXmlStageControllers(writer);
            WriteXmlMeasuringUnits(writer);
            WriteXmlStage(writer);
            WriteXmlInspectionConditions(writer);
        }

        #endregion
    }

}
