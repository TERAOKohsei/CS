using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CS.Common;
using CS.Common.Communications;
using System.Threading;
using System.Xml.Serialization;
using Ports = System.IO.Ports;

namespace CS.CommonRc.StageControllers {
    [Flags()]
    public enum StageStates : uint {
        Stopped = 0x1,
        Running = 0x2,
        DetectedLimit = 0x10,
        DetectedReturnError = 0x100,
        DetectedEmergencyError = 0x200,
        UnknownState = 0xffff,
    }
    
    public abstract class StageController : IUnit, IXmlSerializable {
        protected struct MovingCommand {
            public int Axis;
            public int Pulse;
            public MovingCommand(int axis, int pulse) {
                Axis = axis;
                Pulse = pulse;
            }
        }

        #region Fields
        protected ICommunication port = null;
        // private CancellationTokenSource disposeCts = new CancellationTokenSource();
        // private int[] usableAxes = null;
        #endregion // Fields

        #region Constructors/Destructors

        protected StageController() { }

        #endregion // Constructors

        #region Properties

        public string ProductName { get; protected set; }
        public abstract CS.Common.Communications.ICommunication Communication { get; set; }

        #endregion // Properties

        public virtual string GetAxisName(int axisNumber) {
            return axisNumber.ToString();
        }

        protected abstract void MoveCore(bool isAbsoluteMode, params MovingCommand[] commands);

        public abstract int[] GetPositions();

        public abstract StageStates[] GetStates();

        public abstract int GetPosition(int axis);

        public abstract StageStates GetState(int axis);

        public int AxisCount { get; protected set; }

        public void Move(int[] axes, int[] travels) {
            var cs = new MovingCommand[axes.Length];

            foreach ( var a in axes.Select((v, i) => new { Value = v, Index = i }) ) {
                cs[a.Index].Axis = a.Value;
                cs[a.Index].Pulse = travels[a.Index];
            }

            MoveCore(false, cs);
        }

        public void Move(params int[] travels) {
            var cs = new MovingCommand[travels.Length];

            foreach ( var t in travels.Select((v, i) => new { Value = v, Index = i }) ) {
                cs[t.Index].Axis = t.Index;
                cs[t.Index].Pulse = t.Value;
            }

            MoveCore(false, cs);
        }

        public void MoveTo(int[] axes, int[] positions) {
            var cs = new MovingCommand[axes.Length];

            foreach ( var a in axes.Select((v, i) => new { Value = v, Index = i }) ) {
                cs[a.Index].Axis = a.Value;
                cs[a.Index].Pulse = positions[a.Index];
            }

            MoveCore(true, cs);
        }

        public void MoveTo(params int[] positions) {
            var cs = new MovingCommand[positions.Length];

            foreach ( var p in positions.Select((v, i) => new { Value = v, Index = i }) ) {
                cs[p.Index].Axis = p.Index;
                cs[p.Index].Pulse = p.Value;
            }

            MoveCore(true, cs);
        }

        public abstract void ReturnToOrigin();

        public abstract void ReturnToOrigin(params int[] axes);

        public abstract void Stop();

        public abstract void Stop(params int[] axes);

        public void Wait(StageStates state = StageStates.Stopped, int waitTime = -1) {
            bool detectedState = false;

            while ( !detectedState ) {
                detectedState = true;
                StageStates[] ss = GetStates();

                foreach ( var s in ss ) {
                    if ( (s & state) != state ) {
                        detectedState = false;
                        break;
                    }
                }
            }
        }

        #region IUnit メンバー

        public bool IsConnected {
            get { return port == null ? port.IsOpen : false; }
        }

        public virtual void Connect() {
            port.Open();
        }

        #endregion

        #region IDisposable メンバー

        ~StageController() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if ( disposing && port != null && port.IsOpen ) {
                port.Dispose();
                port = null;
            }
        }

        #endregion

        #region IXmlSerializable メンバー

        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader) {
            ProductName = reader.ReadElementContentAsString("ProductName", "");
            AxisCount = reader.ReadElementContentAsInt("AxisCount", "");
            // 現バージョンではシリアルポートのみサポートしている。
            var sss = reader.ReadElementContentAsString("PortObject", "");
            if ( port == null ) {
                port = new SerialPort();
            }
            port.ReadXml(reader);
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer) {
            writer.WriteElementString("ProductName", ProductName);
            writer.WriteElementString("AxisCount", AxisCount.ToString());
            writer.WriteElementString("PortObject", port.GetType().ToString());
            if ( port != null ) {
                port.WriteXml(writer);
            }
        }

        #endregion
    }
}
