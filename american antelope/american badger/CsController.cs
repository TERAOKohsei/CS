using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CS.Common.Communication;
using Ports = System.IO.Ports;

namespace CS.Common.StageController {
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
        public string ProductName;
        public int AxesCount;
        public ControllerSpec(string productName, int axesCount) {
            ProductName = productName;
            AxesCount = axesCount;
        }
    }

    public class CsController : IStageController {
        private static ControllerSpec[] specList;
        private ControllerSpec spec;

        #region Fields
        private ICommunication port = null;
        #endregion // Fields

        #region Constructors
        static CsController() {
            specList = new ControllerSpec[(int)CsControllerType.Count];

            var spstr = Properties.Resources.ControllerSpec.Split(new string[] {",", "\r\n"}, StringSplitOptions.None);
            for ( int i = 0; i < (int)CsControllerType.Count; ++i ) {
                if ( !String.IsNullOrEmpty(spstr[i * 2]) ) {
                    specList[i].ProductName = spstr[i * 2];
                    specList[i].AxesCount = int.Parse(spstr[i * 2 + 1]);
                }
            }

            foreach ( var sp in specList ) {
                System.Diagnostics.Debug.WriteLine("{0}:{1}", sp.ProductName, sp.AxesCount);
            }
        }

        public CsController(CsControllerType type = CsControllerType.QtAdm2, string portName = "COM1", int baudRate = 9600, int dataBits = 8,
            Ports.Parity parity = Ports.Parity.None, Ports.StopBits stopBits = Ports.StopBits.One, string delimiter = "\r\n") {

            if ( (type < CsControllerType.QtCd1) || !Enum.IsDefined(typeof(CsControllerType), type) ) {
                throw new ArgumentOutOfRangeException("type", type, "未実装のコントローラです。QT-CD1以降の製品を指定してください。");
            } else {
                spec = specList[(int)type];
            }

            if ( port != null ) {
                port.Dispose();
                port = null;
            }

            port = new CS.Common.Communication.SerialPort(portName, baudRate, dataBits, parity, stopBits, delimiter);
        }
        #endregion // Constructors

        #region Properties
        public CsControllerType ControllerType { get; private set; }
        #endregion // Properties

        #region IStageController メンバー

        public int AxisCount {
            get { return 0; }
        }

        public bool IsConnect {
            get { throw new NotImplementedException(); }
        }

        public void Connect() {
            port.Open();
        }

        public void Disconnect() {
            port.Close();
        }

        public void Move(int[] axes, int[] travels) {
            throw new NotImplementedException();
        }

        public void Move(params int[] travels) {
            throw new NotImplementedException();
        }

        public void MoveTo(int[] axes, int[] positions) {
            throw new NotImplementedException();
        }

        public void MoveTo(params int[] positions) {
            throw new NotImplementedException();
        }

        public void ReturnToOrigin() {
            throw new NotImplementedException();
        }

        public void ReturnToOrigin(params int[] axes) {
            throw new NotImplementedException();
        }

        public void Stop() {
            throw new NotImplementedException();
        }

        public void Stop(params int[] axes) {
            throw new NotImplementedException();
        }

        public void Wait(StageStates state = StageStates.Stopped, int waitTime = -1) {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable メンバー

        public void Dispose() {
            port.Close();
            port.Dispose();
        }

        #endregion
    }
}
