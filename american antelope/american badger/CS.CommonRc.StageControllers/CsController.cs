using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CS.Common.Communications;
using System.Threading;
using System.Xml.Serialization;
using Ports = System.IO.Ports;

namespace CS.CommonRc.StageControllers {
    public enum CsControllerType {
        Cpc1b, Cpc2b, Cpc3b, Cpc1c, Cpc2c, Cpc3c, Cpc1bt, Cpc1ct,
        Cpc1cs, Cpc2cs, Cpc3cs, Cpc1ch, Cpc2ch, Cpc3ch,
        Cpc1d, Cpc2d, Cpc3d, Cpc1dn, Cpc2dn, Cpc3dn,
        CatI, CatII, CatE1, CatE2, CatE3, CatEd1, CatEd2, CatEd3,
        CatC1, CatC2, CatC3, CatD1, CatD2, CatD3, Mmc2, MmcXp, MsC2, MsP4, 
        QtCd1, QtCm2, QtCn6, QtAdl1, QtAdm2, QtAdm3, QtAmh2, QtAmh2a,
        /// <summary>
        /// 内部処理用メンバ、コントローラのタイプとしては選択不可です。
        /// </summary>
        Count,
    }

    public struct ControllerSpec {
        public string ProductName {get; set; }
        public int AxisCount { get; set; }
    }

    public class CsController : StageController {
        #region Fields
        private static ControllerSpec[] specList = new ControllerSpec[(int)CsControllerType.Count];
        private CancellationTokenSource disposeCts =  new CancellationTokenSource();
        private int[] usableAxes = null;
        #endregion // Fields

        #region Constructors/Destructors
        static CsController() {
            var spstr = Properties.Resources.ControllerSpec.Split(new string[] { ",", "\r\n" }, StringSplitOptions.None);
            for ( int i = 0; i < (int)CsControllerType.Count; ++i ) {
                if ( !String.IsNullOrEmpty(spstr[i * 2]) ) {
                    specList[i].ProductName = spstr[i * 2];
                    specList[i].AxisCount = int.Parse(spstr[i * 2 + 1]);
                }
            }

            foreach ( var sp in specList ) {
                System.Diagnostics.Debug.WriteLine("{0}:{1}", sp.ProductName, sp.AxisCount);
            }
        }

        private CsController() : base() { }

        public CsController(CsControllerType type = CsControllerType.QtAdm2) : base() {
            if ( (type < CsControllerType.QtCd1) || !Enum.IsDefined(typeof(CsControllerType), type) ) {
                throw new ArgumentOutOfRangeException("type", type, "未実装のコントローラです。QT-CD1以降の製品を指定してください。");
            } else {
                ProductName = specList[(int)type].ProductName;
                AxisCount = specList[(int)type].AxisCount;
            }
        }
        #endregion // Constructors

        #region Properties
        public CsControllerType ControllerType { get; private set; }
        #endregion // Properties

        #region Methods

        private string AddAxisList(string source, params int[] axes) {
            string value = source;

            foreach ( var axis in axes.OrderBy(axis => axis) ) {
                value += GetAxisName(axis);
            }

            return value;
        }

        private string AddAxesAndParameters(string source, int[] axes, int[] args) {
            string value = source;

            int[] axisList = axes.OrderBy(axis => axis).ToArray();
            foreach ( var axis in axisList.Select((v, i)=> new {Value = v, Index = i}) ) {
                value += GetAxisName(axis.Value) + args[axis.Index].ToString();
            }

            return value;
        }

        public override string GetAxisName(int axisNumber) {
            return Convert.ToString((char)(axisNumber + 0x41));
        }

        private int[] GetParameter(int prameterNumber) {
            port.WriteLine("P:" + prameterNumber.ToString("D2")+"R");
            string[] r = port.ReadLine().Split(',');
            var result = new int[r.Length];

            foreach ( var s in r.Select((v, i) => new { Value = v, Index = i }) ) {
                result[s.Index] = Int32.Parse(s.Value);
            }

            return result;
        }

        protected override void MoveCore(bool isAbsoluteMode, params MovingCommand[] commands) {
            string cmdstr;

            if ( isAbsoluteMode == true ) {
                cmdstr = "AGO:";
            } else {
                cmdstr = "MGO:";
            }

            foreach ( var c in commands.OrderBy(elem => elem.Axis) ) {
                cmdstr += GetAxisName(c.Axis) + c.Pulse.ToString();
            }

            port.WriteLine(cmdstr);
            port.ReadLine();
        }

        #endregion // Methods

        #region IStageController メンバー

        public override int[] GetPositions() {
            var p = Enumerable.Repeat<int>(1, usableAxes.Count()).ToArray();
            port.WriteLine(AddAxesAndParameters("Q:", usableAxes, p));

            var r = new int[usableAxes.Count()];
            foreach ( var line in port.ReadLine().Split(',').Select((v, i) => new { Value = v, Index = i }) ) {
                r[line.Index] = Int32.Parse(line.Value);
            }

            return r;
        }

        public override StageStates[] GetStates() {
            var p = Enumerable.Repeat<int>(2, usableAxes.Count()).ToArray();
            port.WriteLine(AddAxesAndParameters("Q:", usableAxes, p));

            var r = new StageStates[usableAxes.Count()];
            foreach ( var line in port.ReadLine().Split(',').Select((v, i) => new { Value = v, Index = i }) ) {
                r[line.Index] = GetStateByCharacter(line.Value);
            }

            return r;
        }

        public override int GetPosition(int axis) {
            port.WriteLine("Q:{0}1", GetAxisName(axis));
            return int.Parse(port.ReadLine());
        }

        public override StageStates GetState(int axis) {
            port.WriteLine("Q:{0}2", GetAxisName(axis));
            return GetStateByCharacter(port.ReadLine());
        }

        private StageStates GetStateByCharacter(string character) {
            switch ( character ) {
            case "D":
                return StageStates.Running;
            case "K":
                return StageStates.Stopped;
            case "E":
                return StageStates.Stopped | StageStates.DetectedEmergencyError;
            case "H":
                return StageStates.Stopped | StageStates.DetectedReturnError;
            case "L":
                return StageStates.Stopped | StageStates.DetectedLimit;
            default:
                return StageStates.UnknownState;
            }
        }

        public override void ReturnToOrigin() {
            ReturnToOrigin(usableAxes);
        }

        public override void ReturnToOrigin(params int[] axes) {
            string cmd = "H:";

            cmd = AddAxisList(cmd, axes);

            port.WriteLine(cmd);
            port.ReadLine();
        }

        public override void Stop() {
            port.WriteLine("L:");
            port.ReadLine();
        }

        public override void Stop(params int[] axes) {
            string cmd = "L:";

            cmd = AddAxisList(cmd, axes);

            port.WriteLine(cmd);
            port.ReadLine();
        }

        public override Common.Communications.ICommunication Communication {
            get { return port; }
            set {
                if ( value.GetType().Equals(typeof(SerialPort)) ) {
                    var p = (SerialPort)value;
                    if ( (p.BaudRate != 9600) || (p.DataBits != 8) || (p.Parity != Ports.Parity.None) || (p.StopBits != Ports.StopBits.One) || (p.NewLine != "\r\n") ) {
                        throw new ArgumentException("QT/QT-Aシリーズのシリアル通信は現在、工場出荷時の設定のみサポートしています。");
                    }
                } else {
                    throw new ArgumentException("QT/QT-Aでは指定されたICommunicationオブジェクトはサポートされていません。");
                }

                port = value;
            }
        }

        #endregion

        #region IUnit メンバー

        public override void Connect() {
            port.Open();
            port.WriteLine("X:1");
            port.ReadLine();
            int[] rd = GetParameter(6);

            int c = rd.Where(v => v == 1).Count();
            usableAxes = new int[c];
            int id = 0;
            for ( int i = 0; i < rd.Length; ++i ) {
                if ( rd[i] == 1 ) {
                    usableAxes[id] = i;
                    ++id;
                }
            }
        }

        #endregion

        #region IDisposable メンバー

        //~CsController() {
        //    Dispose(false);
        //}

        //public void Dispose() {
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if ( disposing && disposeCts != null ) {
                disposeCts.Dispose();
            }
            //if ( disposing && port != null && port.IsOpen ) {
            //    port.Dispose();
            //    port = null;
            //    disposeCts.Dispose();
            //}
        }

        #endregion

        #region IXmlSerializable メンバー
        public override void ReadXml(System.Xml.XmlReader reader) {
            reader.ReadStartElement("CsController", "");
            base.ReadXml(reader);
            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer) {
            writer.WriteStartElement("CsController");
            base.WriteXml(writer);
            writer.WriteEndElement();
        }
        #endregion
    }
}
