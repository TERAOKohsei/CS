using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ports = System.IO.Ports;

namespace CS.Common.Communication {
    public class SerialPort : ICommunication {
        #region Fields
        Ports.SerialPort port = null;
        private const int readTimeout = 1000;
        private const int writeTimeout = 1000;
        #endregion // Fields

        #region Constructors

        public SerialPort(string portName = "COM1", int baudRate = 9600, int dataBits = 8, Ports.Parity parity = Ports.Parity.None, Ports.StopBits stopBits = Ports.StopBits.One,
            string newLine = "\r\n") {

            portNameValue = portName;
            baudRateValue = baudRate;
            dataBitsValue = dataBits;
            parityValue = parity;
            stopBitsValue = stopBits;
            newLineValue = newLine;
        }

        #endregion //Constructors

        #region Properites

        private string portNameValue = "COM1";
        public string PortName {
            get { return portNameValue; }
            set {
                portNameValue = value;
                if ( port != null ) { port.PortName = value; }
            }
        }

        private int baudRateValue = 9600;
        public int BaudRate {
            get { return baudRateValue; }
            set {
                baudRateValue = value;
                if ( port != null ) { port.BaudRate = value; }
            }
        }

        private int dataBitsValue = 8;
        public int DataBits {
            get { return dataBitsValue; }
            set {
                dataBitsValue = value;
                if ( port != null ) { port.DataBits = value; }
            }
        }

        private Ports.Parity parityValue = Ports.Parity.None;
        public Ports.Parity Parity {
            get { return parityValue; }
            set {
                if ( !Enum.IsDefined(typeof(Ports.Parity), value) ) {
                    throw new ArgumentOutOfRangeException("Parity", value, "Parity列挙のメンバを指定してください。");
                }

                parityValue = value;
                if ( port != null ) { port.Parity = value; }
            }
        }

        private Ports.StopBits stopBitsValue = Ports.StopBits.One;
        public Ports.StopBits StopBits {
            get { return stopBitsValue; }
            set {
                if ( !Enum.IsDefined(typeof(Ports.StopBits), value) ) {
                    throw new ArgumentOutOfRangeException("StopBits", value, "StopBits列挙のメンバを指定してください。");
                }

                stopBitsValue = value;
                if ( port != null ) { port.StopBits = value; }
            }
        }

        #endregion // Properties

        #region Methods

        // TODO: ポートが開かれていない場合、例外を送出します。を英語に!!
        private void ThrowException() {
            if ( port == null ) { throw new InvalidOperationException("ポートが開いていません。"); }
        }

        #endregion // Methods

        #region ICommunication メンバー

        public bool IsOpen {
            get {
                return port == null ? false : port.IsOpen;
            }
        }

        private string newLineValue = Environment.NewLine;
        public string NewLine {
            get { return newLineValue; }
            set {
                newLineValue = value;
                if ( port != null ) { port.NewLine = value; }
            }
        }

        public void Open() {
            if ( port == null ) {
                port = new Ports.SerialPort();
            }

            port.PortName = portNameValue;
            port.BaudRate = baudRateValue;
            port.DataBits = dataBitsValue;
            port.Parity = parityValue;
            port.StopBits = stopBitsValue;
            port.NewLine = newLineValue;
            port.ReadTimeout = readTimeout;
            port.WriteTimeout = writeTimeout;

            try {
                port.Open();
            } finally {
                if ( (port != null) && !port.IsOpen ) {
                    port.Dispose();
                    port = null;
                }
            }
        }

        public void Close() {
            if ( port != null ) {
                port.Close();
                port.Dispose();
                port = null;
            }
        }

        public int Read() {
            ThrowException();
            var c = new char[1];
            port.Read(c, 0, 1);
            return (int)c[0];
        }

        public string ReadLine() {
            ThrowException();
            return port.ReadLine();
        }

        public void Write(string value) {
            ThrowException();
            port.Write(value);
        }

        public void Write(string format, params object[] args) {
            port.Write(String.Format(format, args));
        }

        public void WriteLine(string value = "") {
            ThrowException();
            port.WriteLine(value);
        }

        public void WriteLine(string format, params object[] args) {
            ThrowException();
            port.WriteLine(String.Format(format, args));
        }

        #endregion

        #region IDisposable メンバー

        public void Dispose() {
            if ( IsOpen ) {
                Close();
            }
        }

        #endregion
    }
}
