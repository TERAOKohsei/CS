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
using CS.CommonRc.MeasuringUnits;
using MySettings = CS.Applications.AmericanBullfrog.Properties.Settings;

namespace CS.Applications.AmericanBullfrog {
    public partial class FormMain : Form {
        private static NLog.Logger logger = null;
        private static IList<Sensor> sensors = null;
        private static IList<MeasuringUnit> measuringUnits = null;

        public FormMain() {
            InitializeComponent();
        }

        private struct PortInfo {
            public string DeviceName { get; set; }
            public string PortName { get; set; }
        }

        private void LoadSensorList() {
            try {
                sensors = Sensor.LoadFromFile(MySettings.Default.SensorListFilePath);
            } catch ( Exception e ) {
                logger.TraceException("センサリスト読み込み中に例外が発生。", e);
                throw e;
            }

            foreach ( var sensor in sensors ) {
                logger.Trace(String.Format("センサ{0}: {1} {2} ({3})を読み込み", sensor.Id, sensor.Manufacturer, sensor.ProductType, sensor.ManagementNumber));
            }
        }

        private void LoadMeasuringUnitList() {
            try {
                measuringUnits = MeasuringUnit.LoadFromFile(MySettings.Default.MeasuringUnitListFilePath);
            } catch ( Exception e ) {
                logger.TraceException("測定機リスト読み込み中に例外が発生。", e);
                throw e;
            }

            foreach ( var unit in measuringUnits ) {
                logger.Trace(String.Format("測定機{0}: {1} {2} ({3})を読み込み", unit.ID, unit.Manufacturer, unit.ProductType, unit.ManagementNumber));
            }
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
            try {
                LoadSerialPortList();
                LoadSensorList();
                LoadMeasuringUnitList();
            } catch ( Exception exc ) {
                MessageBox.Show(String.Format("起動中に例外が発生しました。\r\n{0}", exc.Message));
                // TODO: 操作関係のボタンなどをDisableにする。
            }

            logger.Trace("FromMain is loaded.");
        }
    }
}
