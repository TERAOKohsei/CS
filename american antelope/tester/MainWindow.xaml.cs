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
                using ( IStageController sc = new CsController(CsControllerType.QtAdm2, "COM3") ) {
                    sc.Connect();
                    sc.ReturnToOrigin();
                    sc.Wait();
                    System.Diagnostics.Debug.WriteLine(((int)sc.GetState(0)).ToString());
                    System.Threading.Thread.Sleep(1000);
                    sc.Dispose();
                }
            } catch ( OperationCanceledException ) {
                // do nothing.
            } catch ( Exception exc ) {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
