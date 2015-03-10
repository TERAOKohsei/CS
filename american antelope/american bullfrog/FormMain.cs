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
using System.Xml.Serialization;
using CS.CommonRc.MeasuringUnits;
using CS.CommonRc.Stages;
using CS.CommonRc.StageControllers;
using MySettings = CS.Applications.AmericanBullfrog.Properties.Settings;

namespace CS.Applications.AmericanBullfrog {
    public partial class FormMain : Form {
        private struct MeasuringUnitChannel {
            public MeasuringUnit Unit;
            public int Axis;
            public MeasuringUnitChannel(MeasuringUnit unit, int axis) {
                Unit = unit;
                Axis = axis;
            }
            public override string ToString() {
                return Unit.ToString(Axis);
            }
        }

        private struct Inspector {
            public int Code { get; set; }
            public string Name { get; set; }
            public Inspector(int code, string name) : this() {
                Code = code;
                Name = name;
            }
            public override string ToString() {
                return String.Format("{0}: {1}", Code, Name);
            }
        }

        private struct InspectionConditions {
            public string SerialNumber { get; set; }
            public string ProductCode { get; set; }
            public Inspector Inspector { get; set; }
            public string Notes { get; set; }
        }

        public static NLog.Logger logger = null;
        private static IList<Sensor> sensors = null;

        private static AmericanBullfrogSettings settings;

        public FormMain() {
            InitializeComponent();
            int i = settings.HUnit.Axis;
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
            settings.LoadMeasuringUnitList(MySettings.Default.MeasuringUnitListFilePath);
            listBoxMeasuringUnits.Items.Clear();
            foreach ( var mu in settings.MeasuringUnits ) {
                for ( int i = 0; i < mu.AxisCount; i++ ) {
                    listBoxMeasuringUnits.Items.Add(new MeasuringUnitChannel(settings.MeasuringUnits[mu.ID], i));
                }
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
        private void TestReadXml() {
            using ( var sr = new StreamReader(@"C:\Users\kohsei\Documents\american bullfrog.xml", Encoding.GetEncoding("shift-jis")) ) {
                var xs = new XmlSerializer(typeof(AmericanBullfrogSettings));
                settings = (AmericanBullfrogSettings)xs.Deserialize(sr);
                LoadMeasuringUnitList();
            }

            comboBoxPorts.SelectedValue = ((CS.Common.Communications.SerialPort)settings.StageController.Communication).PortName;
        }

        private void TestWriteXml() {
            settings.StageController = new CsController(CsControllerType.QtAdm2);
            settings.StageController.Communication = new CS.Common.Communications.SerialPort("COM3");
            settings.MeasuringUnits[3].SetSensor(new int[] { 0 }, new Sensor[] { sensors[0] });
            settings.MeasuringUnits[1].SetSensor(new int[] { 0, 1 }, new Sensor[] { sensors[2], sensors[3] });
            settings.MeasuringUnits[0].SetSensor(new int[] { 0, 1 }, new Sensor[] { sensors[4], sensors[4] });
            settings.LengthUnit.UnitId = 3;
            settings.LengthUnit.Axis = 0;
            settings.HUnit.UnitId = 1;
            settings.HUnit.Axis = 0;
            settings.VUnit.UnitId = 1;
            settings.VUnit.Axis = 1;
            settings.YUnit.UnitId = 0;
            settings.YUnit.Axis = 0;
            settings.PUnit.UnitId = 0;
            settings.PUnit.Axis = 1;
            
            using ( var sw = new StreamWriter(@"C:\Users\kohsei\Documents\american bullfrog.xml", false, Encoding.GetEncoding("shift-jis")) ) {
                var xs = new XmlSerializer(typeof(AmericanBullfrogSettings));
                xs.Serialize(sw, settings);
            }

            settings.StageController.Dispose();
            settings.StageController = null;
        }

        private void TestStageController() {
            settings.StageController.Connect();
            settings.StageController.ReturnToOrigin();
            settings.StageController.Dispose();
            settings.StageController = null;
        }
#endif

        private void FormMain_Load(object sender, EventArgs e) {
            logger = NLog.LogManager.GetCurrentClassLogger();
            Text = String.Format("{0} Ver.{1}", Application.ProductName, Application.ProductVersion);
            try {
                LoadSerialPortList();
                LoadInspector();
                LoadSensorList();
                LoadMeasuringUnitList();
#if DEBUG
                TestWriteXml();
                TestReadXml();
                //TestStageController();
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
        }

        private void listBoxMeasuringUnits_SelectedIndexChanged(object sender, EventArgs e) {
            var sc = (MeasuringUnitChannel)listBoxMeasuringUnits.SelectedItem;

            listBoxSensors.Items.Clear();
            SetMeasuringUnitButtonEnabled(false, false);
            if ( settings.MeasuringUnits != null ) {
                foreach ( var s in sensors.Where(s => s.SensorCode == settings.MeasuringUnits[sc.Unit.ID].SensorCodes.ElementAt(sc.Axis)) ) {
                    listBoxSensors.Items.Add(s);
                }
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
            MeasuringUnitChannel uc = (MeasuringUnitChannel)listBoxMeasuringUnits.SelectedItem;
            MeasuringUnit unit = ((MeasuringUnitChannel)listBoxMeasuringUnits.SelectedItem).Unit;
            Sensor sensor = (Sensor)listBoxSensors.SelectedItem;
            TextBox tbm = null;
            TextBox tbs = null;
            settings.MeasuringUnits[unit.ID].SetSensor(new int[] { uc.Axis }, new Sensor[] { sensor });
            if ( buttonSetLengthMeasuringUnit.Equals(sender) ) {
                settings.LengthUnit = new MeasuringUnitAxis(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxLengthMeasuringUnit;
                tbs = textBoxLengthMeasuringSensor;
            } else if ( buttonSetHMeasuringUnit.Equals(sender) ) {
                settings.HUnit = new MeasuringUnitAxis(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxHMeasuringUnit;
                tbs = textBoxHMeasuringSensor;
            } else if ( buttonSetVMeasuringUnit.Equals(sender) ) {
                settings.VUnit = new MeasuringUnitAxis(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxVMeasuringUnit;
                tbs = textBoxVMeasuringSensor;
            } else if ( buttonSetYMeasuringUnit.Equals(sender) ) {
                settings.YUnit = new MeasuringUnitAxis(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxYMeasuringUnit;
                tbs = textBoxYMeasuringSensor;
            } else if ( buttonSetPMeasuringUnit.Equals(sender) ) {
                settings.PUnit = new MeasuringUnitAxis(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxPMeasuringUnit;
                tbs = textBoxPMeasuringSensor;
            }

            if ( tbm != null ) {
                tbm.Text = settings.MeasuringUnits[unit.ID].ToString(uc.Axis);
            }

            if ( tbs != null ) {
                tbs.Text = settings.MeasuringUnits[unit.ID].Sensors[uc.Axis].ToString();
            }
        }

        private void buttonShowMeasuringUnitSettingDialogue_Click(object sender, EventArgs e) {
            var unit = (MeasuringUnitChannel)listBoxMeasuringUnits.SelectedItem;

            if ( settings.MeasuringUnits[unit.Unit.ID] != null ) {
                settings.MeasuringUnits[unit.Unit.ID].ShowSettingDialogue();
            }
        }

        private void comboBoxPorts_SelectedIndexChanged(object sender, EventArgs e) {
            if ( settings.StageController != null ) {
                settings.StageController.Communication = new CS.Common.Communications.SerialPort((string)comboBoxPorts.SelectedValue);
            }
        }
    }
}
