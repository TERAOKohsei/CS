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
    public partial class FormMain : Form, IXmlSerializable {
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

        private static NLog.Logger logger = null;
        private static IList<Sensor> sensors = null;
        private static IList<MeasuringUnit> measuringUnits = null;

        private static MeasuringUnitAxis lengthUnit;
        private static MeasuringUnitAxis hUnit;
        private static MeasuringUnitAxis vUnit;
        private static MeasuringUnitAxis yUnit;
        private static MeasuringUnitAxis pUnit;

        private static StageController stageController;
        private static Stage stage;
        

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
        private void TestReadXml() {
            using ( var sr = new StreamReader(@"C:\Users\kohsei\Documents\american bullfrog.xml", Encoding.GetEncoding("shift-jis")) ) {
                var xs = new XmlSerializer(typeof(FormMain));
                xs.Deserialize(sr);
            }
        }

        private void TestWriteXml() {
            stageController = new CsController(CsControllerType.QtAdm2);
            stageController.Communication = new CS.Common.Communications.SerialPort("COM3");
            using ( var sw = new StreamWriter(@"C:\Users\kohsei\Documents\american bullfrog.xml", false, Encoding.GetEncoding("shift-jis")) ) {
                var xs = new XmlSerializer(typeof(FormMain));
                xs.Serialize(sw, this);
            }

            stageController.Dispose();
            stageController = null;
        }

        private void TestStageController() {
            stageController.Connect();
            stageController.ReturnToOrigin();
            stageController.Dispose();
            stageController = null;
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
                // TestWriteXml();
                TestReadXml();
                TestStageController();
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
                lengthUnit = unit;
                tbm = textBoxLengthMeasuringUnit;
                tbs = textBoxLengthMeasuringSensor;
            } else if ( buttonSetHMeasuringUnit.Equals(sender) ) {
                hUnit = unit;
                tbm = textBoxHMeasuringUnit;
                tbs = textBoxHMeasuringSensor;
            } else if ( buttonSetVMeasuringUnit.Equals(sender) ) {
                vUnit = unit;
                tbm = textBoxVMeasuringUnit;
                tbs = textBoxVMeasuringSensor;
            } else if ( buttonSetYMeasuringUnit.Equals(sender) ) {
                yUnit = unit;
                tbm = textBoxYMeasuringUnit;
                tbs = textBoxYMeasuringSensor;
            } else if ( buttonSetPMeasuringUnit.Equals(sender) ) {
                pUnit = unit;
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

        #region IXmlSerializable メンバー

        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader) {
            reader.ReadStartElement("FormMain");
            reader.ReadElementContentAsString("StageControllerObject", "");
            XmlSerializer xs;
            // TODO: 現段階ではStageControllerの実装はCsControllerのみ
            xs = new XmlSerializer(typeof(CsController));
            stageController = (CsController)xs.Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer) {
            writer.WriteElementString("StageControllerObject", stageController.GetType().ToString());
            XmlSerializer xs;
            // TODO: 現段階ではStageControllerの実装はCsControllerのみ
            xs = new XmlSerializer(typeof(CsController));
            xs.Serialize(writer, stageController);
        }

        #endregion

        private void comboBoxPorts_SelectedIndexChanged(object sender, EventArgs e) {
            
        }
    }
}
