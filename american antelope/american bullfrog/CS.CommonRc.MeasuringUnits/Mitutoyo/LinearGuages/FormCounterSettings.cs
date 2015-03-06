using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS.CommonRc.MeasuringUnits.Mitutoyo.LinearGuages {
    public partial class FormCounterSettings : Form {
        private Counter counter;
        
        private FormCounterSettings() {
            InitializeComponent();
            var portInfos = CS.Common.Communications.SerialPort.GetPortInformations();
            comboBoxSerialPort.DisplayMember = "DeviceName";
            comboBoxSerialPort.ValueMember = "PortName";
            comboBoxSerialPort.DataSource = portInfos;
            comboBoxSerialPort.SelectedIndex = 0;
        }

        public FormCounterSettings(Counter unit) : this() {
            counter = unit;
            Text = unit.ToString();
            labelMeasuringUnitInformation.Text = unit.ToLongString();
            if ( unit.Communication != null ) {
                comboBoxSerialPort.SelectedValue = ((CS.Common.Communications.SerialPort)unit.Communication).PortName;
            }
        }

        private void buttonOKCancelApply_Click(object sender, EventArgs e) {
            if ( (buttonOK.Equals(sender) || buttonApply.Equals(sender))
                && comboBoxSerialPort.SelectedValue.GetType().Equals(typeof(string)) && !String.IsNullOrEmpty((string)comboBoxSerialPort.SelectedValue) ) {

                counter.Communication = new CS.Common.Communications.SerialPort((string)comboBoxSerialPort.SelectedValue, 9600, 7, System.IO.Ports.Parity.Even, System.IO.Ports.StopBits.Two);
            }

            if ( buttonOK.Equals(sender) || buttonCancel.Equals(sender) ) {
                Close();
            }
        }
    }
}
