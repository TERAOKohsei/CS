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
using CS.Common;
using CS.CommonRc;
using CS.CommonRc.Inspections;
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

        public static NLog.Logger Logger = null;

        private static AmericanBullfrogSettings Settings;

        public FormMain() {
            InitializeComponent();
            int i = Settings.HUnit.Axis;
        }

        private void LoadInspector() {
            listBoxInspectors.Items.Clear();
            AmericanBullfrogSettings.LoadInspectorsList(MySettings.Default.InspecorListFilePath);
            foreach ( var ins in AmericanBullfrogSettings.Inspectors ) {
                listBoxInspectors.Items.Add(ins);
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

        private void UpdateEnabledCondition() {
            InspectionItems testEnum = Settings.Stage.InspectionItems;

            checkBoxTravelRange.Checked = testEnum.HasFlag(InspectionItems.TravelRange);
            checkBoxPositioningAccuracy.Checked = testEnum.HasFlag(InspectionItems.PositioningAccuracy);
            checkBoxRepeatabilityOfPositioning.Checked = testEnum.HasFlag(InspectionItems.RepeatabilityOfPositioning);
            checkBoxLostMotion.Checked = testEnum.HasFlag(InspectionItems.LostMotion);
            checkBoxMotionAccuracyH.Checked = testEnum.HasFlag(InspectionItems.MotionAccuracyHorizontal);
            checkBoxMotionAccuracyV.Checked = testEnum.HasFlag(InspectionItems.MotionAccuracyVertical);
            checkBoxMotionAccuracyY.Checked = testEnum.HasFlag(InspectionItems.MotionAccuracyYaw);
            checkBoxMotionAccuracyP.Checked = testEnum.HasFlag(InspectionItems.MotionAccuracyPitch);
            checkBoxParallelismOfMotion.Checked = testEnum.HasFlag(InspectionItems.ParallelismOfMotion);

            textBoxTravelRange.Enabled = checkBoxTravelRange.Checked;
            textBoxLowerTravelRange.Enabled = checkBoxTravelRange.Checked;
            textBoxUpperTravelRange.Enabled = checkBoxTravelRange.Checked;
            textBoxPositioningAccuracy.Enabled = checkBoxPositioningAccuracy.Checked;
            textBoxRepeatabilityOfPositioning.Enabled = checkBoxRepeatabilityOfPositioning.Checked;
            textBoxLostMotion.Enabled = checkBoxLostMotion.Checked;
            textBoxMotionAccuracyH.Enabled = checkBoxMotionAccuracyH.Checked;
            textBoxMotionAccuracyV.Enabled = checkBoxMotionAccuracyV.Checked;
            textBoxMotionAccuracyY.Enabled = checkBoxMotionAccuracyY.Checked;
            textBoxMotionAccuracyP.Enabled = checkBoxMotionAccuracyP.Checked;
            textBoxParallelismOfMotion.Enabled = checkBoxParallelismOfMotion.Checked;

            groupBoxPositioninAccuracyConditions.Enabled
                = testEnum.HasFlag(InspectionItems.PositioningAccuracy) || testEnum.HasFlag(InspectionItems.MotionAccuracyHorizontal)
                || testEnum.HasFlag(InspectionItems.MotionAccuracyVertical) || testEnum.HasFlag(InspectionItems.MotionAccuracyYaw)
                || testEnum.HasFlag(InspectionItems.MotionAccuracyPitch) || testEnum.HasFlag(InspectionItems.ParallelismOfMotion);
            groupBoxRepeatabilityOfPositioning.Enabled = testEnum.HasFlag(InspectionItems.RepeatabilityOfPositioning) || testEnum.HasFlag(InspectionItems.LostMotion);
        }

        private void UpdateStageSpec() {
            textBoxProductName.Text = Settings.Stage.ProductName;
            textBoxProductType.Text = Settings.Stage.ProductType;
            textBoxTravelRange.Text = Settings.Stage.TravelRange.Difference.ToString();
            textBoxLowerTravelRange.Text = Settings.Stage.TravelRange.Lower.ToString();
            textBoxUpperTravelRange.Text = Settings.Stage.TravelRange.Upper.ToString();
            textBoxPositioningAccuracy.Text = Settings.Stage.PositioningAccuracy.ToString();
            textBoxRepeatabilityOfPositioning.Text = Settings.Stage.RepeatabilityOfPositioning.ToString();
            textBoxLostMotion.Text = Settings.Stage.LostMotion.ToString();
            textBoxMotionAccuracyH.Text = Settings.Stage.MotionAccuracyHorizontal.ToString();
            textBoxMotionAccuracyV.Text = Settings.Stage.MotionAccuracyVertical.ToString();
            textBoxMotionAccuracyY.Text = Settings.Stage.MotionAccuracyYaw.ToString();
            textBoxMotionAccuracyP.Text = Settings.Stage.MotionAccuracyPitch.ToString();
            textBoxParallelismOfMotion.Text = Settings.Stage.ParallelismOfMotion.ToString();

            textBoxMeasuringPointCount.Text = Settings.Stage.MeasuringPointCount.ToString();
            textBoxRepeatCount.Text = Settings.Stage.RepeatCount.ToString();
            listBoxMeasuringPositions.Items.Clear();
            foreach ( var p in Settings.Stage.MeasuringPositions ) {
                listBoxMeasuringPositions.Items.Add(p);
            }
            textBoxLowerSpeed.Text = Settings.Stage.LowerSpeedPps.ToString();
            textBoxUpperSpeed.Text = Settings.Stage.UpperSpeedPps.ToString();
            textBoxAccelerationTime.Text = Settings.Stage.AccelerationTimeMs.ToString();
            textBoxLostMotionCorrectValue.Text = Settings.Stage.LostMotionCorrectPps.ToString();
            textBoxWaitTimeAfterStopped.Text = Settings.Stage.WaitTimeMsAfterStopped.ToString();
            UpdateEnabledCondition();
        }

        private void LoadSettings() {
            using ( var sr = new StreamReader(@"C:\Users\kohsei\Documents\american bullfrog.xml", Encoding.GetEncoding("shift-jis")) ) {
                var xs = new XmlSerializer(typeof(AmericanBullfrogSettings));
                Settings = (AmericanBullfrogSettings)xs.Deserialize(sr);
            }

            comboBoxPorts.SelectedValue = ((CS.Common.Communications.SerialPort)Settings.StageController.Communication).PortName;
            textBoxLengthMeasuringUnit.Text = Settings.LengthUnit.ToString();
            textBoxLengthMeasuringSensor.Text = Settings.LengthUnit.GetSensorName();
            textBoxHMeasuringUnit.Text = Settings.HUnit.ToString();
            textBoxHMeasuringSensor.Text = Settings.HUnit.GetSensorName();
            textBoxVMeasuringUnit.Text = Settings.VUnit.ToString();
            textBoxVMeasuringSensor.Text = Settings.VUnit.GetSensorName();
            textBoxYMeasuringUnit.Text = Settings.YUnit.ToString();
            textBoxYMeasuringSensor.Text = Settings.YUnit.GetSensorName();
            textBoxPMeasuringUnit.Text = Settings.PUnit.ToString();
            textBoxPMeasuringSensor.Text = Settings.PUnit.GetSensorName();

            UpdateStageSpec();

            textBoxSerialNumber.Text = Settings.Conditions.SerialNumber;
            textBoxProductCode.Text = Settings.Conditions.ProductCode;
            listBoxInspectors.SelectedItem = Settings.Conditions.Inspector;
            textBoxNote.Text = Settings.Conditions.Notes;
        }

        private void SaveSettings() {
            using ( var sw = new StreamWriter(@"C:\Users\kohsei\Documents\american bullfrog.xml", false, Encoding.GetEncoding("shift-jis")) ) {
                var xs = new XmlSerializer(typeof(AmericanBullfrogSettings));
                xs.Serialize(sw, Settings);
            }
        }

#if DEBUG
        private void TestStageController() {
            Settings.StageController.Connect();
            Settings.StageController.ReturnToOrigin();
            Settings.StageController.Dispose();
            Settings.StageController = null;
        }
#endif

        private void FormMain_Load(object sender, EventArgs e) {
            Logger = NLog.LogManager.GetCurrentClassLogger();
            Text = String.Format("{0} Ver.{1}", Application.ProductName, Application.ProductVersion);
            try {
                LoadSerialPortList();
                LoadInspector();
                LoadSensorList();
                LoadMeasuringUnitList();
                LoadSettings();
#if DEBUG
                //SaveSettings();
                //TestStageController();
#endif
                
                // TODO: 前回の設定をロード
            } catch ( Exception exc ) {
                MessageBox.Show(String.Format("起動中に例外が発生しました。\r\n{0}", exc.Message));
                Logger.DebugException("起動中に例外が発生しました。\r\n", exc);
                // TODO: 操作関係のボタンなどをDisableにする。
                tabControlMain.SelectedTab = tabPageLog;
            }

            Logger.Trace("FromMainがロードされました。");
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
            SaveSettings();
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
                Settings.LengthUnit = new MeasuringUnitCombination(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxLengthMeasuringUnit;
                tbs = textBoxLengthMeasuringSensor;
            } else if ( buttonSetHMeasuringUnit.Equals(sender) ) {
                Settings.HUnit = new MeasuringUnitCombination(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxHMeasuringUnit;
                tbs = textBoxHMeasuringSensor;
            } else if ( buttonSetVMeasuringUnit.Equals(sender) ) {
                Settings.VUnit = new MeasuringUnitCombination(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxVMeasuringUnit;
                tbs = textBoxVMeasuringSensor;
            } else if ( buttonSetYMeasuringUnit.Equals(sender) ) {
                Settings.YUnit = new MeasuringUnitCombination(unit.ID, uc.Axis, sensor.Id);
                tbm = textBoxYMeasuringUnit;
                tbs = textBoxYMeasuringSensor;
            } else if ( buttonSetPMeasuringUnit.Equals(sender) ) {
                Settings.PUnit = new MeasuringUnitCombination(unit.ID, uc.Axis, sensor.Id);
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
            if ( listBoxMeasuringUnits.SelectedItem == null ) {
                return;
            }

            var unit = (MeasuringUnitChannel)listBoxMeasuringUnits.SelectedItem;

            if ( AmericanBullfrogSettings.MeasuringUnits[unit.Unit.ID] != null ) {
                AmericanBullfrogSettings.MeasuringUnits[unit.Unit.ID].ShowSettingDialogue();
            }
        }

        private void comboBoxPorts_SelectedIndexChanged(object sender, EventArgs e) {
            if ( Settings.StageController != null ) {
                Settings.StageController.Communication = new CS.Common.Communications.SerialPort((string)comboBoxPorts.SelectedValue);
            }
        }

        private void textBox_Validating(object sender, CancelEventArgs e) {
            var condition = Settings.Conditions;
            var stage = Settings.Stage;
            var tb = (TextBox)sender;
            try {
                if ( sender.Equals(textBoxSerialNumber) ) {
                    condition.SerialNumber = tb.Text;
                } else if ( sender.Equals(textBoxProductCode) ) {
                    condition.ProductCode = tb.Text;
                } else if ( sender.Equals(textBoxNote) ) {
                    condition.Notes = tb.Text;
                } else if ( sender.Equals(textBoxProductName) ) {
                    stage.ProductName = tb.Text;
                } else if ( sender.Equals(textBoxProductType) ) {
                    stage.ProductType = tb.Text;
                } else if ( sender.Equals(textBoxTravelRange) ) {
                    var r = Double.Parse(tb.Text);
                    stage.TravelRange = new CommonRc.Range<double>(r * 0.5, -r * 0.5);
                    textBoxLowerTravelRange.Text = ((double)(-r * 0.5)).ToString();
                    textBoxUpperTravelRange.Text = ((double)r * 0.5).ToString();
                } else if ( sender.Equals(textBoxLowerTravelRange) ) {
                    stage.TravelRange = new CommonRc.Range<double>(stage.TravelRange.Upper, Double.Parse(tb.Text));
                    textBoxTravelRange.Text = stage.TravelRange.Difference.ToString();
                } else if ( sender.Equals(textBoxUpperTravelRange) ) {
                    stage.TravelRange = new Range<double>(Double.Parse(tb.Text), stage.TravelRange.Lower);
                    textBoxTravelRange.Text = stage.TravelRange.Difference.ToString();
                } else if ( sender.Equals(textBoxPositioningAccuracy) ) {
                    stage.PositioningAccuracy = Double.Parse(tb.Text);
                } else if ( sender.Equals(textBoxRepeatabilityOfPositioning) ) {
                    stage.RepeatabilityOfPositioning = Double.Parse(tb.Text);
                } else if ( sender.Equals(textBoxLostMotion) ) {
                    stage.LostMotion = Double.Parse(tb.Text);
                } else if ( sender.Equals(textBoxMotionAccuracyH) ) {
                    stage.MotionAccuracyHorizontal = Double.Parse(tb.Text);
                } else if ( sender.Equals(textBoxMotionAccuracyV) ) {
                    stage.MotionAccuracyVertical = Double.Parse(tb.Text);
                } else if ( sender.Equals(textBoxMotionAccuracyY) ) {
                    stage.MotionAccuracyYaw = Double.Parse(tb.Text);
                } else if ( sender.Equals(textBoxMotionAccuracyP) ) {
                    stage.MotionAccuracyPitch = Double.Parse(tb.Text);
                } else if ( sender.Equals(textBoxParallelismOfMotion) ) {
                    stage.ParallelismOfMotion = Double.Parse(tb.Text);
                } else if ( sender.Equals(textBoxMeasuringPointCount) ) {
                    stage.MeasuringPointCount = Int32.Parse(tb.Text);
                } else if ( sender.Equals(textBoxRepeatCount) ) {
                    stage.RepeatCount = Int32.Parse(tb.Text);
                } else if ( sender.Equals(textBoxMeasuringPosition) ) {
                    int i;
                    buttonAddMeasuringPosition.Enabled = Int32.TryParse(tb.Text, out i);
                } else if ( sender.Equals(textBoxLowerSpeed) ) {
                    stage.LowerSpeedPps = Int32.Parse(tb.Text);
                } else if ( sender.Equals(textBoxUpperSpeed) ) {
                    stage.UpperSpeedPps = Int32.Parse(tb.Text);
                } else if ( sender.Equals(textBoxAccelerationTime) ) {
                    stage.AccelerationTimeMs = Int32.Parse(tb.Text);
                } else if ( sender.Equals(textBoxLostMotionCorrectValue) ) {
                    stage.LostMotionCorrectPps = Int32.Parse(tb.Text);
                } else if ( sender.Equals(textBoxWaitTimeAfterStopped) ) {
                    stage.WaitTimeMsAfterStopped = Int32.Parse(tb.Text);
                }
            } catch ( Exception exc) {
                Logger.DebugException("テキストの入力が正しくありません。", exc);
                e.Cancel = true;
                tb.SelectAll();
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e) {
            var stage = Settings.Stage;
            var cb = (CheckBox)sender;
            InspectionItems item;

            if ( sender.Equals(checkBoxTravelRange) ) {
                item = InspectionItems.TravelRange;
            } else if ( sender.Equals(checkBoxPositioningAccuracy) ) {
                item = InspectionItems.PositioningAccuracy;
            } else if ( sender.Equals(checkBoxRepeatabilityOfPositioning) ) {
                item = InspectionItems.RepeatabilityOfPositioning;
            } else if ( sender.Equals(checkBoxLostMotion) ) {
                item = InspectionItems.LostMotion;
            } else if ( sender.Equals(checkBoxMotionAccuracyH) ) {
                item = InspectionItems.MotionAccuracyHorizontal;
            } else if ( sender.Equals(checkBoxMotionAccuracyV) ) {
                item = InspectionItems.MotionAccuracyVertical;
            } else if ( sender.Equals(checkBoxMotionAccuracyY) ) {
                item = InspectionItems.MotionAccuracyYaw;
            } else if ( sender.Equals(checkBoxMotionAccuracyP) ) {
                item = InspectionItems.MotionAccuracyPitch;
            } else if ( sender.Equals(checkBoxParallelismOfMotion) ) {
                item = InspectionItems.ParallelismOfMotion;
            } else {
                item = InspectionItems.Nothing;
            }

            if ( item != InspectionItems.Nothing ) {
                if ( cb.Checked ) {
                    Settings.Stage.InspectionItems |= item;
                } else {
                    Settings.Stage.InspectionItems &= ~item;
                }
            }

            UpdateEnabledCondition();
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e) {
            var lb = (ListBox)sender;

            if ( sender.Equals(listBoxInspectors) ) {
                Settings.Conditions.Inspector = (Inspector)lb.SelectedItem;
            } else if ( sender.Equals(listBoxMeasuringPositions) ) {
                buttonDeleteMeasuringPosition.Enabled = (lb.SelectedItem != null);
            }
        }

        private void listBoxMeasuringPositions_KeyDown(object sender, KeyEventArgs e) {
            if ( e.KeyCode == Keys.Escape ) {
                listBoxMeasuringPositions.SelectedIndex = -1;
            }
        }

        private void buttonMeasuringPositions_Click(object sender, EventArgs e) {
            var bt = (Button)sender;

            if ( sender.Equals(buttonAddMeasuringPosition) ) {
                List<double> positions = new List<double>(listBoxMeasuringPositions.Items.Cast<double>());
                positions.Add(Double.Parse(textBoxMeasuringPosition.Text));
                Settings.Stage.MeasuringPositions = positions.OrderBy(a => a).ToArray();
                listBoxMeasuringPositions.Items.Clear();
                foreach ( var position in Settings.Stage.MeasuringPositions ) {
                    listBoxMeasuringPositions.Items.Add(position);
                }
            } else if ( sender.Equals(buttonDeleteMeasuringPosition) ) {
                var positions = new List<double>();
                foreach ( var position in Settings.Stage.MeasuringPositions.Where(c => c != (double)listBoxMeasuringPositions.SelectedItem).OrderBy(a => a) ) {
                    positions.Add(position);
                }
                Settings.Stage.MeasuringPositions = positions.ToArray();
                listBoxMeasuringPositions.Items.Clear();
                foreach ( var position in positions ) {
                    listBoxMeasuringPositions.Items.Add(position);
                }
            }
        }

        private void buttonLoadStageSpec_Click(object sender, EventArgs e) {
            if ( openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK ) {
                Settings.Stage.LoadInformation(openFileDialog1.FileName);
                UpdateStageSpec();
            }
        }

        private void buttonSaveStageSpec_Click(object sender, EventArgs e) {
            if ( saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK ) {
                Settings.Stage.SaveInformation(saveFileDialog1.FileName);
            }
        }
    }
}
