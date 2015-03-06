using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Threading;
using CS.CommonRc.MeasuringUnits;
using MySettings = CS.Applications.AmericanBullfrog.Properties.Settings;

namespace CS.Applications.AmericanBullfrog {
    public partial class FormMain : Form {
        private struct MeasuringUnitAxis {
            public MeasuringUnit Unit;
            public int Axis;
            public MeasuringUnitAxis(MeasuringUnit unit, int axis) {
                Unit = unit;
                Axis = axis;
            }
            public override string ToString() {
                return Unit.ToString(Axis);
            }
        }

        private struct Inspector {
            public int Code { get; private set; }
            public string Name { get; private set; }
            public Inspector(int code, string name) : this() {
                Code = code;
                Name = name;
            }
            public override string ToString() {
                return String.Format("{0}: {1}", Code, Name);
            }
        }

        private static NLog.Logger logger = null;
        private static IList<Sensor> sensors = null;
        private static IList<MeasuringUnit> measuringUnits = null;

        private static MeasuringUnitAxis LengthUnit;
        private static MeasuringUnitAxis HUnit;
        private static MeasuringUnitAxis VUnit;
        private static MeasuringUnitAxis YUnit;
        private static MeasuringUnitAxis PUnit;

        public FormMain() {
            InitializeComponent();
        }

        private void LoadSensorList() {
            try {
                sensors = Sensor.LoadFromFile(MySettings.Default.SensorListFilePath);
            } catch ( Exception e ) {
                logger.DebugException("センサリスト読み込み中に例外が発生。", e);
                throw;
            }

            foreach ( var sensor in sensors ) {
                logger.Trace(String.Format("センサ{0}: {1} {2} ({3})を読み込み", sensor.Id, sensor.Manufacturer, sensor.ProductType, sensor.ManagementNumber));
            }
        }

        private void LoadInspector() {
            listBoxInspectors.Items.Clear();
            try {
                using ( StreamReader sr = new StreamReader(MySettings.Default.InspecorListFilePath, Encoding.GetEncoding("shift-jis")) ) {
                    while ( !sr.EndOfStream ) {
                        var words = sr.ReadLine().Split(',');
                        int c;
                        Int32.TryParse(words[0], out c);
                        Inspector inspector = new Inspector(c, words[1]);
                        listBoxInspectors.Items.Add(inspector);
                        logger.Trace(String.Format("検査担当:{0}を読み込み", inspector.ToString()));
                    }
                }
            } catch ( Exception e ) {
                logger.DebugException("検査担当リスト読み込み中に例外が発生。", e);
                throw;
            }
        }

        private void LoadMeasuringUnitList() {
            listBoxMeasuringUnits.Items.Clear();
            try {
                measuringUnits = MeasuringUnit.LoadFromFile(MySettings.Default.MeasuringUnitListFilePath);
                foreach ( var m in measuringUnits ) {
                    for ( int i = 0; i < m.AxisCount; i++ ) {
                        listBoxMeasuringUnits.Items.Add(new MeasuringUnitAxis(m, i));
                    }
                }
            } catch ( Exception e ) {
                logger.DebugException("測定機リスト読み込み中に例外が発生。", e);
                throw;
            }

            foreach ( var unit in measuringUnits ) {
                logger.Trace(String.Format("測定機{0}: {1} {2} ({3})を読み込み", unit.ID, unit.Manufacturer, unit.ProductType, unit.ManagementNumber));
            }
        }

        private void LoadSerialPortList() {
            var portInfos = CS.Common.Communications.SerialPort.GetPortInformations();
            comboBoxPorts.DisplayMember = "DeviceName";
            comboBoxPorts.ValueMember = "PortName";
            comboBoxPorts.DataSource = portInfos;

            comboBoxPorts.SelectedIndex = 0;
        }

        private void SetMeasuringUnitButtonEnabled(bool displacement, bool angular) {
            buttonSetLengthMeasuringUnit.Enabled = displacement;
            buttonSetHMeasuringUnit.Enabled = displacement;
            buttonSetVMeasuringUnit.Enabled = displacement;
            buttonSetYMeasuringUnit.Enabled = angular;
            buttonSetPMeasuringUnit.Enabled = angular;
        }

#if DEBUG
        private void Test() {
            CS.CommonRc.Stages.Stage stage = new CommonRc.Stages.Stage();
            stage.LoadInformation(@"C:\Users\kohsei\Documents\LS-3047-C1.csv");
            //stage.LoadInformation(@"\\ageo.chuo.co.jp\ShareRoot\inspection_data\CTS検査データ\CTS-自動検査\検査パラメータ\LS-3047.dat");
        }
#endif

        private void FormMain_Load(object sender, EventArgs e) {
            logger = NLog.LogManager.GetCurrentClassLogger();
            Text = String.Format("{0} Ver.{1}", Application.ProductName, Application.ProductVersion);
            try {
                LoadSerialPortList();
                LoadSensorList();
                LoadMeasuringUnitList();
                LoadInspector();
#if DEBUG
                Test();
#endif
                
                // TODO: 前回の設定をロード
            } catch ( Exception exc ) {
                MessageBox.Show(String.Format("起動中に例外が発生しました。\r\n{0}", exc.Message));
                logger.DebugException("起動中に例外が発生しました。\r\n", exc);
                // TODO: 操作関係のボタンなどをDisableにする。
                tabControlMain.SelectedTab = tabPageLog;
            }

            logger.Trace("FromMainがロードされました。");
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
            // TODO : 設定を保存
        }

        private void listBoxMeasuringUnits_SelectedIndexChanged(object sender, EventArgs e) {
            var sc = (MeasuringUnitAxis)listBoxMeasuringUnits.SelectedItem;

            listBoxSensors.Items.Clear();
            SetMeasuringUnitButtonEnabled(false, false);
            foreach ( var s in sensors.Where(s => s.SensorCode == sc.Unit.SensorCodes.ElementAt(sc.Axis)) ) {
                listBoxSensors.Items.Add(s);
            }

            if ( listBoxSensors.Items.Count == 1 ) {
                listBoxSensors.SelectedIndex = 0;
            }
        }

        private void listBoxSensors_SelectedIndexChanged(object sender, EventArgs e) {
            var sensor = (Sensor)listBoxSensors.SelectedItem;

            switch ( sensor.SensorType ) {
            case SensorType.Displacement:
                SetMeasuringUnitButtonEnabled(true, false);
                break;
            case SensorType.Angular:
                SetMeasuringUnitButtonEnabled(false, true);
                break;
            default:
                SetMeasuringUnitButtonEnabled(false, false);
                break;
            }
        }

        private void buttonSetMeasuringUnit_Click(object sender, EventArgs e) {
            MeasuringUnitAxis unit = (MeasuringUnitAxis)listBoxMeasuringUnits.SelectedItem;
            Sensor sensor = (Sensor)listBoxSensors.SelectedItem;
            TextBox tbm = null;
            TextBox tbs = null;
            unit.Unit.SetSensor(new int[] { unit.Axis }, new Sensor[] { sensor });
            if ( buttonSetLengthMeasuringUnit.Equals(sender) ) {
                LengthUnit = unit;
                tbm = textBoxLengthMeasuringUnit;
                tbs = textBoxLengthMeasuringSensor;
            } else if ( buttonSetHMeasuringUnit.Equals(sender) ) {
                HUnit = unit;
                tbm = textBoxHMeasuringUnit;
                tbs = textBoxHMeasuringSensor;
            } else if ( buttonSetVMeasuringUnit.Equals(sender) ) {
                VUnit = unit;
                tbm = textBoxVMeasuringUnit;
                tbs = textBoxVMeasuringSensor;
            } else if ( buttonSetYMeasuringUnit.Equals(sender) ) {
                YUnit = unit;
                tbm = textBoxYMeasuringUnit;
                tbs = textBoxYMeasuringSensor;
            } else if ( buttonSetPMeasuringUnit.Equals(sender) ) {
                PUnit = unit;
                tbm = textBoxPMeasuringUnit;
                tbs = textBoxPMeasuringSensor;
            }

            if ( tbm != null ) {
                tbm.Text = unit.Unit.ToString(unit.Axis);
            }

            if ( tbs != null ) {
                tbs.Text = unit.Unit.Sensors[unit.Axis].ToString();
            }
        }

        private void buttonShowMeasuringUnitSettingDialogue_Click(object sender, EventArgs e) {
            var unit = (MeasuringUnitAxis)listBoxMeasuringUnits.SelectedItem;

            if ( unit.Unit != null ) {
                unit.Unit.ShowSettingDialogue();
            }
        }
    }
}
