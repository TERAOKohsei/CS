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
    }

    public class CsController : IStageController {
        private static string[] productName
            = new string[] { "CPC-1B", "CPC-2B", "CPC-3B", "CPC-1C", "CPC-2C", "CPC-3C", "CPC-1BT", "CPC-1CT",
            "CPC-1CS", "CPC-2CS", "CPC-3CS", "CPC-1CH", "CPC-2CH", "CPC-3CH", "CPC-1D", "CPC-2D", "CPC-3D",
            "CPC-1DN", "CPC-2DN", "CPC-3DN", "CAT-I", "CAT-II", "CAT-E1", "CAT-E2", "CAT-E3", "CAT-ED1", "CAT-ED2", "CAT-ED3",
            "CAT-C1", "CAT-C2", "CAT-C3", "CAT-D1", "CAT-D2", "CAT-D3","MMC-2", "MMC-XP", "MS-C2", "MS-P4",
            "QT-CD1/QT-CD1-35", "QT-CM2/QT-CM2-35", "QT-CN6",
            "QT-ADL1/QT-ADL1-35", "QT-ADM2/QT-ADM2-35", "QT-ADM3/QT-ADM3-35", "QT-AMH2/QT-AMH2-35", "QT-AMH2A/QT-AMH2A-35", };

        #region Fields
        private ICommunication port = null;
        #endregion // Fields

        #region Constructors
        public CsController(CsControllerType type = CsControllerType.QtAdm2, string portName = "COM1", int baudRate = 9600, int dataBits = 8,
            Ports.Parity parity = Ports.Parity.None, Ports.StopBits stopBits = Ports.StopBits.One, string delimiter = "\r\n") {

            if ( (type < CsControllerType.QtCd1) || !Enum.IsDefined(typeof(CsControllerType), type) ) {
                throw new ArgumentOutOfRangeException("type", type, "未実装のコントローラです。QT-CD1以降の製品を指定してください。");
            }

            if ( port != null ) {
                port.Dispose();
                port = null;
            }

            port = new CS.Common.Communication.SerialPort(portName, baudRate, dataBits, parity, stopBits, delimiter);
        }
        #endregion // Constructors

        #region IStageController メンバー

        public int AxisCount {
            get { throw new NotImplementedException(); }
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
