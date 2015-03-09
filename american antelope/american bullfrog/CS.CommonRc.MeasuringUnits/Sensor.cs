using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace CS.CommonRc.MeasuringUnits {
    public enum SensorType {
        Displacement = 0,
        Angular = 1,
        Humidity = 2,
        Temperature = 3,
        UnknownType = -1,
    }

    public struct Sensor {
        public int Id { get; set; }
        public string ManagementNumber { get; set; }
        public string Manufacturer { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string SerialNumber { get; set; }
        public SensorType SensorType { get; set; }
        public string UnitName { get; set; }
        public Range<double> Range { get; set; }
        public double Resolution { get; set; }
        public int SensorCode { get; set; }

        public Sensor(int id, string managementNumber, string manufacturer, string productName, string productType, string serialNumber, SensorType sensorType, string unitName,
            double upper, double lower, double offset, double resolution, int sensorCode)
            : this() {

            Id = id;
            ManagementNumber = managementNumber;
            Manufacturer = manufacturer;
            ProductName = productName;
            ProductType = productType;
            SerialNumber = serialNumber;
            SensorType = sensorType;
            UnitName = unitName;
            Range = new Range<double>(upper, lower, offset);
            Resolution = resolution;
            SensorCode = sensorCode;
        }

        public static IList<Sensor> LoadFromFile(string sensorListPath) {
            var list = new List<Sensor>();

            using ( var sr = new StreamReader(sensorListPath, Encoding.GetEncoding("shift-jis")) ) {
                string[] words = sr.ReadLine().Split(',');

                while ( !sr.EndOfStream ) {
                    words = sr.ReadLine().Split(',');
                    if ( words.Count() < 14 ) {
                        throw new InvalidDataException(String.Concat("センサリスト ファイル \"", sensorListPath, "\"の内容が不正です。"));
                    }
                    SensorType st;
                    if ( !Enum.TryParse<SensorType>(words[6], out st) ) {
                        st = SensorType.UnknownType;
                    }
                    int id, sc;
                    int.TryParse(words[0], out id);
                    int.TryParse(words[13], out sc);
                    var dlist = new double[4];
                    foreach ( var d in dlist.Select((v, i) => new { Value = v, Index = i }) ) {
                        double.TryParse(words[d.Index + 9], out dlist[d.Index]);
                    }
                    list.Add(new Sensor(id, words[1], words[2], words[3], words[4], words[5], st, words[7], dlist[0], dlist[1], dlist[2], dlist[3], sc));
                }
            }

            return list;
        }

        public override string ToString() {
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

            return str;
        }
    }
}
