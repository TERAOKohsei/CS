using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common.Communications {
    /// <summary>
    /// 通信機能を提供する。
    /// </summary>
    public interface ICommunication : IDisposable {
        /// <summary>
        /// 通信回線が開いているかどうかを取得する。
        /// </summary>
        Boolean IsOpen { get; }
        /// <summary>
        /// <see cref="CS.Common.Communications.ICommunication.ReadLine()"/>メソッドと<see cref="CS.Common.Communications.ICommunication.WriteLine(System.String)"/>メソッド、
        /// <see cref="CS.Common.Communications.ICommunication.WriteLine(System.String, System.Object[])"/>メソッドの呼び出しの末尾を解釈する際に使用する値を
        /// 取得または設定する。
        /// </summary>
        String NewLine { get; set; }
        void Open();
        Int32 Read();
        String ReadLine();
        void Write(String value);
        void Write(String format, params Object[] args);
        void WriteLine(String value = "");
        void WriteLine(String format, params Object[] args);
    }
}
