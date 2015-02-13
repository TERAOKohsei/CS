using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common {
    /// <summary>
    /// 装置としての機能を提供する。
    /// </summary>
    public interface IUnit : IDisposable {
        /// <summary>
        /// 装置との接続が確立しているかを取得する。
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 装置と接続する。
        /// </summary>
        void Connect();
        /// <summary>
        /// 装置との接続を解除する。
        /// </summary>
        void Disconnect();
    }
}
