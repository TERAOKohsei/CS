using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;

namespace CS.Applications.AmericanBullfrog {
    public partial class FormMain : Form {
        private static NLog.Logger logger = null;

        public FormMain() {
            InitializeComponent();
        }

        private struct PortInfo {
            public string DeviceName { get; set; }
            public string PortName { get; set; }
        }

        private void LoadSerialPortList() {
            var deviceNames = CS.Common.Communications.SerialPort.GetDeviceNames();
            var portNames = System.IO.Ports.SerialPort.GetPortNames();
            var portInfos = new PortInfo[deviceNames.Count()];
            foreach ( var port in portNames.Select((v, i) => new { Value = v, Index = i }) ) {
                portInfos[port.Index].DeviceName = deviceNames[port.Index];
                portInfos[port.Index].PortName = port.Value;
                logger.Trace("Detected serial port : {0}", deviceNames[port.Index]);
            }
            comboBoxPorts.DisplayMember = "DeviceName";
            comboBoxPorts.ValueMember = "PortName";
            comboBoxPorts.DataSource = portInfos;

            comboBoxPorts.SelectedIndex = 0;
        }

        private void FormMain_Load(object sender, EventArgs e) {
            logger = NLog.LogManager.GetCurrentClassLogger();
            Text = String.Format("{0} Ver.{1}", Application.ProductName, Application.ProductVersion);
            LoadSerialPortList();
            logger.Trace("FromMain is loaded.");
        }
    }
}
