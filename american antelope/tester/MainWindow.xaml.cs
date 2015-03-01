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

using CS.Common.Communications;
using CS.CommonRc.MeasuringUnits;
using CS.Common.StageController;

namespace tester {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e) {
            IMeasuringUnit dmu = new Ev(new Sensor(SensorType.Displacement, "QAAS", "mm", 0.0005), new Sensor(SensorType.Displacement, "AAAS", "m", 1));
            Action Measure = new Action(() => {
                foreach ( var sensor in dmu.Sensors.Select((v, i) => new { Value = v, Index = i }) ) {
                    System.Diagnostics.Debug.WriteLine("{0}:{1},{2}{3}", sensor.Index, sensor.Value.Name, sensor.Value.Resolution, sensor.Value.UnitName);
                }
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

    }
}
