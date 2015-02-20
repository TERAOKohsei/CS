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
using CS.Common.StageController;

namespace tester {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            try {
                //ICommunication c = new SerialPort("COM3");
                //c.Open();
                //c.WriteLine("X:1");
                //var data = c.ReadLine();
                //System.Diagnostics.Debug.WriteLine(String.Format("aaa{0}aaa", data));
                //System.Threading.Thread.Sleep(100);
                //c.WriteLine("MGO:A1000");
                //data = c.ReadLine();
                //System.Diagnostics.Debug.WriteLine(String.Format("aaa{0}aaa", data));
                //c.Close();
                IStageController sc = new CsController(CsControllerType.QtAdm2, "COM3");
                sc.Connect();
                sc.ReturnToOrigin();
                //System.Threading.Thread.Sleep(1000);
                //sc.Stop();
                sc.Wait();
                System.Diagnostics.Debug.WriteLine(((int)sc.GetState(0)).ToString());
                System.Threading.Thread.Sleep(1000);
                sc.Disconnect();
                sc.Dispose();
            } catch ( Exception exc ) {
                // MessageBox.Show(exc.Message);
            }
        }
    }
}
