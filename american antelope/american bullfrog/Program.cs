using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;

namespace CS.Applications.AmericanBullfrog {
    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            using ( var mutex = new Mutex(false, Application.ProductName) ) {
                if ( mutex.WaitOne(0, false) ) {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FormMain());
                }

                GC.KeepAlive(mutex);
            }
        }
    }
}
