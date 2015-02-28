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
            IMeasuringUnit dmu = new Ev();
            Action Measure = new Action(() => {
                dmu.Measure();
                dmu.GetValues();
            });

            buttonStart.IsEnabled = false;
            Task.Run(() => {
                System.Threading.Thread.Sleep(10000);
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
