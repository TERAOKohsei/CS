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

        public static NLog.Logger logger = null;

        private static AmericanBullfrogSettings settings;

        public FormMain() {
            InitializeComponent();
            int i = settings.HUnit.Axis;
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
            AmericanBullfrogSettings.LoadMeasuringUnitList(MySettings.Default.MeasuringUnitListFilePath);
            listBoxMeasuringUnits.Items.Clear();
            foreach ( var mu in AmericanBullfrogSettings.MeasuringUnits ) {
                for ( int i = 0; i < mu.AxisCount; i++ ) {
                    listBoxMeasuringUnits.Items.Add(new MeasuringUnitChannel(AmericanBullfrogSettings.MeasuringUnits[mu.ID], i));
                }
            }
        }

        private void LoadSensorList() {
            AmericanBullfrogSettings.LoadSensorList(MySettings.Default.SensorListFilePath);
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
            }

            comboBoxPorts.SelectedValue = ((CS.Common.Communications.SerialPort)settings.StageController.Communication).PortName;
            textBoxLengthMeasuringUnit.Text = settings.LengthUnit.ToString();
            textBoxLengthMeasuringSensor.Text = settings.LengthUnit.GetSensorName();
            textBoxHMeasuringUnit.Text = settings.HUnit.ToString();
            textBoxHMeasuringSensor.Text = settings.HUnit.GetSensorName();
            textBoxVMeasuringUnit.Text = settings.VUnit.ToString();
            textBoxVMeasuringSensor.Text = settings.VUnit.GetSensorName();
            textBoxYMeasuringUnit.Text = settings.YUnit.ToString();
            textBoxYMeasuringSensor.Text = settings.YUnit.GetSensorName();
            textBoxPMeasuringUnit.Text = settings.PUnit.ToString();
            textBoxPMeasuringSensor.Text = settings.PUnit.GetSensorName();

            textBoxProductName.Text = settings.Stage.ProductName;
            textBoxProductType.Text = settings.Stage.ProductType;
            textBoxTravelRange.Text = settings.Stage.TravelRange.Difference.ToString();
            textBoxLowerTravelRange.Text = settings.Stage.TravelRange.Lower.ToString();
            textBoxUpperTravelRange.Text = settings.Stage.TravelRange.Upper.ToString();
            textBoxPositioningAccuracy.Text = settings.Stage.PositionAccuracy.ToString();
            textBoxRepeatabilityOfPositioning.Text = settings.Stage.RepeatabilityOfPositioning.ToString();
            textBoxLostMotion.Text = settings.Stage.LostMotion.ToString();
            textBoxMotionAccuracyH.Text = settings.Stage.MotionAccuracyHorizontal.ToString();
            textBoxMotionAccuracyV.Text = settings.Stage.MotionAccuracyVertical.ToString();
            textBoxMotionAccuracyY.Text = settings.Stage.MotionAccuracyYaw.ToString();
            textBoxMotionAccuracyP.Text = settings.Stage.MotionAccuracyPitch.ToString();
            textBoxParallelismOfMotion.Text = settings.Stage.ParallelismOfMotion.ToString();

            textBoxMeasuringPointCount.Text = settings.Stage.MeasuringPointCount.ToString();
            textBoxRepeatCount.Text = settings.Stage.RepeatCount.ToString();
            listBoxMeasuringPositions.Items.Clear();
            foreach ( var p in settings.Stage.MeasuringPositions ) {
                listBoxMeasuringPositions.Items.Add(p);
            }
            textBoxLowerSpeed.Text = settings.Stage.LowerSpeedPps.ToString();
            textBoxUpperSpeed.Text = settings.Stage.UpperSpeedPps.ToString();
            textBoxAccelerationTime.Text = settings.Stage.AccelerationTimeMs.ToString();
            textBoxLostMotionCorrectValue.Text = settings.Stage.LostMotionCorrectPps.ToString();
            textBoxWaitTimeAfterStopped.Text = settings.Stage.WaitTimeMsAfterStopped.ToString();
        }

        private void TestWriteXml() {
            settings.StageController = new CsController(CsControllerType.QtAdm2);
            settings.StageController.Communication = new CS.Common.Communications.SerialPort("COM3");
            AmericanBullfrogSettings.MeasuringUnits[3].SetSensor(new int[] { 0 }, new Sensor[] { AmericanBullfrogSettings.Sensors[0] });
            AmericanBullfrogSettings.MeasuringUnits[3].Communication = new CS.Common.Communications.SerialPort("COM2", 9600, 7, System.IO.Ports.Parity.Even, System.IO.Ports.StopBits.Two);
            AmericanBullfrogSettings.MeasuringUnits[1].SetSensor(new int[] { 0, 1 }, new Sensor[] { AmericanBullfrogSettings.Sensors[2], AmericanBullfrogSettings.Sensors[3] });
            AmericanBullfrogSettings.MeasuringUnits[1].Communication = new CS.Common.Communications.SerialPort("COM4", 9600, 7, System.IO.Ports.Parity.Even, System.IO.Ports.StopBits.Two);
            AmericanBullfrogSettings.MeasuringUnits[0].SetSensor(new int[] { 0, 1 }, new Sensor[] { AmericanBullfrogSettings.Sensors[4], AmericanBullfrogSettings.Sensors[4] });
            AmericanBullfrogSettings.MeasuringUnits[0].Communication = new CS.Common.Communications.SerialPort("COM5", 9600, 8, System.IO.Ports.Parity.Even, System.IO.Ports.StopBits.Two);
            settings.LengthUnit.UnitId = 3;
            settings.LengthUnit.Axis = 0;
            settings.LengthUnit.SensorId = 0;
            settings.HUnit.UnitId = 1;
            settings.HUnit.Axis = 0;
            settings.HUnit.SensorId = 2;
            settings.VUnit.UnitId = 1;
            settings.VUnit.Axis = 1;
            settings.VUnit.SensorId = 3;
            settings.YUnit.UnitId = 0;
            settings.YUnit.Axis = 0;
            settings.YUnit.SensorId = 4;
            settings.PUnit.UnitId = 0;
            settings.PUnit.Axis = 1;
            settings.PUnit.SensorId = 4;
            settings.Stage.LoadInformation(@"C:\Users\kohsei\Documents\LS-3047-C1.csv");
            
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
            if ( AmericanBullfrogSettings.MeasuringUnits != null ) {
                foreach ( var s in AmericanBullfrogSettings.Sensors.Where(s => s.SensorCode == AmericanBullfrogSettings.MeasuringUnits[sc.Unit.ID].SensorCodes.ElementAt(sc.Axis)) ) {
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
            AmericanBullfrogSettings.MeasuringUnits[unit.ID].SetSensor(new int[] { uc.Axis }, new Sensor[] { sensor });
            if ( buttonSetLengthMeasuringUnit.Equals(sender) ) {
                settings.LengthUnit = new MeasuringUnitCombination(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxLengthMeasuringUnit;
                tbs = textBoxLengthMeasuringSensor;
            } else if ( buttonSetHMeasuringUnit.Equals(sender) ) {
                settings.HUnit = new MeasuringUnitCombination(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxHMeasuringUnit;
                tbs = textBoxHMeasuringSensor;
            } else if ( buttonSetVMeasuringUnit.Equals(sender) ) {
                settings.VUnit = new MeasuringUnitCombination(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxVMeasuringUnit;
                tbs = textBoxVMeasuringSensor;
            } else if ( buttonSetYMeasuringUnit.Equals(sender) ) {
                settings.YUnit = new MeasuringUnitCombination(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxYMeasuringUnit;
                tbs = textBoxYMeasuringSensor;
            } else if ( buttonSetPMeasuringUnit.Equals(sender) ) {
                settings.PUnit = new MeasuringUnitCombination(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxPMeasuringUnit;
                tbs = textBoxPMeasuringSensor;
            }

            if ( tbm != null ) {
                tbm.Text = AmericanBullfrogSettings.MeasuringUnits[unit.ID].ToString(uc.Axis);
            }

            if ( tbs != null ) {
                tbs.Text = AmericanBullfrogSettings.MeasuringUnits[unit.ID].Sensors[uc.Axis].ToString();
            }
        }

        private void buttonShowMeasuringUnitSettingDialogue_Click(object sender, EventArgs e) {
            var unit = (MeasuringUnitChannel)listBoxMeasuringUnits.SelectedItem;

            if ( AmericanBullfrogSettings.MeasuringUnits[unit.Unit.ID] != null ) {
                AmericanBullfrogSettings.MeasuringUnits[unit.Unit.ID].ShowSettingDialogue();
            }
        }

        private void comboBoxPorts_SelectedIndexChanged(object sender, EventArgs e) {
            if ( settings.StageController != null ) {
                settings.StageController.Communication = new CS.Common.Communications.SerialPort((string)comboBoxPorts.SelectedValue);
            }
        }
    }
}
