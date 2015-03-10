using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using CS.Common;

namespace CS.CommonRc.StageControllers {
    [Flags()]
    public enum StageStates : uint {
        Stopped = 0x1,
        Running = 0x2,
        DetectedLimit = 0x10,
        DetectedReturnError = 0x100,
        DetectedEmergencyError = 0x200,
        UnknownState = 0xffff,
    }

    public interface IStageController : IUnit, IXmlSerializable {
        int AxisCount { get; }
        int[] Positions { get; }
        StageStates[] States { get; }
        int GetPosition(int axis);
        StageStates GetState(int axis);
        void Move(int[] axes, int[] travels);
        void Move(params int[] travels);
        void MoveTo(int[] axes, int[] positions);
        void MoveTo(params int[] positions);
        void ReturnToOrigin();
        void ReturnToOrigin(params int[] axes);
        void Stop();
        void Stop(params int[] axes);
        /// <summary>
        /// <paramref namePropertyValue="state"/>になるか、<paramref namePropertyValue="waitTime"/>[ms]経過するまで待機する。
        /// </summary>
        /// <param namePropertyValue="state">このパラメータで指定した状態になるまで待つ。</param>
        /// <param namePropertyValue="waitTime"><see cref="Wait(StageStates, int)"/>メソッドの最大待ち時間[ms]。0より小さい場合、<paramref namePropertyValue="state"/>の状態になるまで待機する。</param>
        /// <exception cref="TimeoutException">待ち時間を超えた場合に送出される。</exception>
        void Wait(StageStates state = StageStates.Stopped, int waitTime = -1);
    }
}
