using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CS.CommonRc;
using CS.Common.Communications;
using CS.CommonRc.MeasuringUnits;
using CS.CommonRc.MeasuringUnits.Mitutoyo.LinearGuages;
using CS.Common.StageController;

using MCounter = CS.CommonRc.MeasuringUnits.Mitutoyo.LinearGuages.Counter;

namespace CS.Applications.tester {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, IDisposable {
        private IMeasuringUnit counter;
        private MCounter ev;
        private MCounter eh101p;

        public MainWindow() {
            InitializeComponent();
            ev = new MCounter(CounterType.Ev, new SerialPortSettings("COM3"),
                new Sensor<double>(SensorType.Displacement, "MCH-335 <ASH-017B>", "mm", 1.5, -1.5, 0, 0.001),
                new Sensor<double>(SensorType.Displacement, "MCH-335 <ASH-017A>", "mm", 1.5, -1.5, 0, 0.001));
            eh101p = new MCounter(CounterType.Eh101p, new SerialPortSettings("COM3"),
                new Sensor<double>(SensorType.Displacement, "LG-01100C <ASH-004R>", "mm", 100, 0, 0, 0.001));
            counter = ev;
        }

        private void TestMeasure() {
            Action Measure = new Action(() => {
                if ( !counter.IsConnected ) {
                    counter.Connect();
                }
                counter.Measure();
                this.Dispatcher.BeginInvoke(new Action(() => {
                    textBoxCount.Text = counter.ProductName;
                    foreach ( var d in counter.GetDisplacements() ) {
                        textBoxCount.Text += String.Format(",{0}", d);
                    }
                }));
            });

            buttonStart.IsEnabled = false;
            Task.Run(() => {
                Measure();
                this.Dispatcher.BeginInvoke(new Action(() => {
                    buttonStop.IsEnabled = false;
                    buttonStart.IsEnabled = true;
                }));
            });
            buttonStop.IsEnabled = true;   
        }

        private void TestQt() {
            using ( IStageController sc = new CsController(CsControllerType.QtAdm2, "COM3") ) {
                sc.Connect();
                sc.ReturnToOrigin(0);
                sc.Wait();
            }
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e) {
            TestQt();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e) {
            if ( radioButtonEv.Equals(sender) ) {
                counter = ev;
            } else if ( radioButtonEh101p.Equals(sender) ) {
                counter = eh101p;
            }
        }

        #region IDisposable members
        
        ~MainWindow() {
          Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if ( disposing ) {
                ev.Dispose();
                eh101p.Dispose();
            }
        }

        #endregion // IDisposable members

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Dispose(true);
        }
    }
}
