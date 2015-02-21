using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using Ports = System.IO.Ports;
using System.Threading;

namespace CS.Common.Communications {
    public delegate ReceivedCharacterEventArgs ReceivedCharacterEventHandler(object sender, ReceivedCharacterEventArgs e);
    public delegate ReceivedLineEventArgs ReceivedLineEventHandler(object sender, ReceivedLineEventArgs e);

    public class SerialPort : ICommunication {
        #region Events
        public ReceivedCharacterEventHandler ReceivedCharaceter;
        public ReceivedLineEventHandler ReceivedLine;
        #endregion // Events

        #region Fields
        Task mainTask = null;
        CancellationTokenSource cts = null;
        private const int readTimeout = 1000;
        private const int writeTimeout = 1000;
        Exception innerException = null;
        Queue<string> inbuffer = new Queue<string>();
        Queue<string> outbuffer = new Queue<string>();
        string linebuffer = "";
        ManualResetEventSlim loopReset = new ManualResetEventSlim();
        AutoResetEvent receivedEvent = new AutoResetEvent(false);
        #endregion // Fields

        #region Constructors/Destructor

        public SerialPort(string portName = "COM1", int baudRate = 9600, int dataBits = 8, Ports.Parity parity = Ports.Parity.None, Ports.StopBits stopBits = Ports.StopBits.One,
            string newLine = "\r\n") {

            portNameValue = portName;
            baudRateValue = baudRate;
            dataBitsValue = dataBits;
            parityValue = parity;
            stopBitsValue = stopBits;
            newLineValue = newLine;
        }

        ~SerialPort() {
            Dispose(false);
        }

        #endregion //Constructors

        #region Properites

        private string portNameValue = "COM1";
        public string PortName {
            get { return portNameValue; }
            set { portNameValue = value; }
        }

        private int baudRateValue = 9600;
        public int BaudRate {
            get { return baudRateValue; }
            set { baudRateValue = value; }
        }

        private int dataBitsValue = 8;
        public int DataBits {
            get { return dataBitsValue; }
            set { dataBitsValue = value; }
        }

        private Ports.Parity parityValue = Ports.Parity.None;
        public Ports.Parity Parity {
            get { return parityValue; }
            set {
                if ( !Enum.IsDefined(typeof(Ports.Parity), value) ) {
                    throw new ArgumentOutOfRangeException("Parity", value, "Parity列挙のメンバを指定してください。");
                }

                parityValue = value;
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
            }
        }

        #endregion // Properties

        #region Methods

        private void Maintask() {
            Ports.SerialPort port = null;
            try {
                port = new Ports.SerialPort(portNameValue, baudRateValue, parityValue, dataBitsValue, stopBitsValue);
                port.ReadTimeout = readTimeout;
                port.WriteTimeout = writeTimeout;
                port.DataReceived += (sender, e) => { loopReset.Set(); };
                port.Open();
                loopReset.Reset();
                inbuffer.Clear();
                outbuffer.Clear();
                linebuffer = "";
                Debug.WriteLine(String.Format("Enter loop for serial communication in {0}", mainTask.Id));
                while ( !cts.IsCancellationRequested ) {
                    loopReset.Wait(cts.Token);
                    loopReset.Reset();
                    while ( 0 < outbuffer.Count() ) {
                        var command = outbuffer.Dequeue();
                        port.Write(command);
                        Debug.WriteLine("Write `{0}' to {1} - buffer {2}", command, port.PortName, outbuffer.Count());
                    }
                    while ( 0 < port.BytesToRead ) {
                        char rc = (char)port.ReadByte();
                        linebuffer += rc;
                        Debug.WriteLine("Read `{0}' from {1}", rc, port.PortName);
                        if ( ReceivedCharaceter != null ) {
                            ReceivedCharaceter(this, new ReceivedCharacterEventArgs(rc));
                        }
                        if ( linebuffer.Contains(newLineValue) ) {
                            string data = linebuffer.Replace(newLineValue, "");
                            inbuffer.Enqueue(data);
                            linebuffer = "";
                            Debug.WriteLine("Readline `{0}' to index{1} from {2}", data, inbuffer.Count, port.PortName);
                            if ( ReceivedLine != null ) {
                                ReceivedLine(this, new ReceivedLineEventArgs(data));
                            }
                        }
                    }
                }
                port.Close();
            } catch ( OperationCanceledException ) {
                // 何もしない。                        
            } catch ( Exception e ) {
                innerException = e;
                Debug.WriteLine(e.Message, e.GetType().ToString());
                return;
            }
        }

        #endregion // Methods

        #region ICommunication メンバー

        public bool IsOpen {
            get {
                return mainTask != null;
            }
        }

        private string newLineValue = Environment.NewLine;
        public string NewLine {
            get { return newLineValue; }
            set { newLineValue = value; }
        }

        public void Open() {
            if ( mainTask == null ) {
                cts = new CancellationTokenSource();
                mainTask = new Task(new Action(Maintask), cts.Token);
                mainTask.Start();
                Thread.Sleep(100);
                if ( mainTask.IsCompleted ) {
                    mainTask.Dispose();
                    mainTask = null;
                    throw innerException;
                }
            }
        }

        public void Close() {
            if ( mainTask != null ) {
                cts.Cancel();
                mainTask.Wait(cts.Token);
                mainTask.Dispose();
                mainTask = null;
                cts.Dispose();
                cts = null;
            }
        }

        public int Read() {
            throw new NotImplementedException();
        }

        public string ReadLine() {
            DateTime endTime = DateTime.Now.AddMilliseconds(readTimeout);
            string value = String.Empty;

            bool eol = false;
            while ( !eol ) {
                Thread.Sleep(0);
                if ( endTime < DateTime.Now ) {
                    throw new TimeoutException(String.Format("通信ポート{0}の読み込み時にタイムアウト・エラーが発生しました。", PortName));
                }

                if ( 0 < inbuffer.Count ) {
                    value = inbuffer.Dequeue();
                    eol = true;
                }
            }

            return value;
        }

        public void Write(string value) {
            outbuffer.Enqueue(value);
            loopReset.Set();
        }

        public void Write(string format, params object[] args) {
            Write(String.Format(format, args));
        }

        public void WriteLine(string value = "") {
            Write(String.Concat(value, newLineValue));
        }

        public void WriteLine(string format, params object[] args) {
            Write(String.Concat(String.Format(format, args), newLineValue));
        }

        #endregion

        #region IDisposable メンバー

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if ( disposing ) {
                if ( IsOpen ) {
                    Close();
                }
                receivedEvent.Dispose();
                loopReset.Dispose();
            }
        }

        #endregion
    }
}
