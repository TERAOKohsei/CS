using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CS.CommonRc.MeasuringUnits;
using CS.Common.Communications;

namespace CS.CommonRc.MeasuringUnits {
    public partial class FormLacsSettings : Form {
        private LacS counter;
        private int waitTime;

        private FormLacsSettings() {
            InitializeComponent();
            var portInfos = SerialPort.GetPortInformations();
            comboBoxPortName.DisplayMember = "DeviceName";
            comboBoxPortName.ValueMember = "PortName";
            comboBoxPortName.DataSource = portInfos;
        }

        public FormLacsSettings(LacS unit) : this() {
            counter = unit;
            waitTime = unit.WaitTimeMs;
            Text = unit.ToString();
            labelMeasuringUnitInformation.Text = unit.ToLongString();
            if ( unit.Communication != null ) {
                comboBoxPortName.SelectedValue = ((SerialPort)unit.Communication).PortName;
            }
            textBoxWaitTimeMs.Text = waitTime.ToString();
        }

        private void buttonOKCancelApply_Click(object sender, EventArgs e) {
            if ( (buttonOK.Equals(sender) || buttonApply.Equals(sender))
                && comboBoxPortName.SelectedValue.GetType().Equals(typeof(string)) && !String.IsNullOrEmpty((string)comboBoxPortName.SelectedValue) ) {

                counter.Communication = new CS.Common.Communications.SerialPort((string)comboBoxPortName.SelectedValue, 9600, 8, System.IO.Ports.Parity.Even, System.IO.Ports.StopBits.Two);
                counter.WaitTimeMs = waitTime;
            }

            if ( buttonOK.Equals(sender) || buttonCancel.Equals(sender) ) {
                Close();
            }
        }

        private void textBoxWaitTimeMs_Click(object sender, EventArgs e) {
            ((TextBox)sender).SelectAll();
        }

        private void textBoxWaitTimeMs_Enter(object sender, EventArgs e) {
            ((TextBox)sender).SelectAll();
        }

        private void textBoxWaitTimeMs_Validating(object sender, CancelEventArgs e) {
            try {
                int c = Int32.Parse(textBoxWaitTimeMs.Text);
                if ( c < 0 ) {
                    throw new InvalidOperationException("測定待ち時間[ms]は0以上で指定してください。");
                }
                waitTime = c;
            } catch ( Exception ) {
                MessageBox.Show("測定待ち時間[ms]は0以上で指定してください。");
                e.Cancel = true;
                textBoxWaitTimeMs.SelectAll();
            }
        }
    }
}
