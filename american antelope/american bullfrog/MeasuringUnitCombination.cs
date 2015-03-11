using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace CS.Applications.AmericanBullfrog {
    public struct MeasuringUnitCombination : IXmlSerializable {
        public int UnitId;
        public int Axis;
        public int SensorId;
        public override string ToString() {
            return AmericanBullfrogSettings.MeasuringUnits[UnitId].ToString(Axis);
        }

        public string GetSensorName() {
            return AmericanBullfrogSettings.Sensors[SensorId].ToString();
        }

        public MeasuringUnitCombination(int unitId, int axis, int sensorId) {
            UnitId = unitId;
            Axis = axis;
            SensorId = sensorId;
        }

        #region IXmlSerializable メンバー

        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader) {
            UnitId = reader.ReadElementContentAsInt("UnitId", "");
            Axis = reader.ReadElementContentAsInt("Axis", "");
            SensorId = reader.ReadElementContentAsInt("SensorId", "");
        }

        public void WriteXml(System.Xml.XmlWriter writer) {
            writer.WriteElementString("UnitId", UnitId.ToString());
            writer.WriteElementString("Axis", Axis.ToString());
            writer.WriteElementString("SensorId", SensorId.ToString());
        }

        #endregion
    }
}
