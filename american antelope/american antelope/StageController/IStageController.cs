using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS.Common.StageController {
    [Flags()]
    public enum StageStates : uint {
        Stopped = 0x0,
        Running = 0x1,
        DetectedLimit = 0x10,
        DetectedReturnError = 0x20,
    }

    public interface IStageController : IUnit {
        int AxisCount { get; }
        void Move(int[] axes, int[] travels);
        void Move(params int[] travels);
        void MoveTo(int[] axes, int[] positions);
        void MoveTo(params int[] positions);
        void ReturnToOrigin();
        void ReturnToOrigin(params int[] axes);
        void Stop();
        void Stop(params int[] axes);
        /// <summary>
        /// <paramref name="state"/>になるか、<paramref name="waitTime"/>[ms]経過するまで待機する。
        /// </summary>
        /// <param name="state">このパラメータで指定した状態になるまで待つ。</param>
        /// <param name="waitTime"><see cref="Wait(StageStates, int)"/>メソッドの最大待ち時間[ms]。0より小さい場合、<paramref name="state"/>の状態になるまで待機する。</param>
        /// <exception cref="TimeoutException">待ち時間を超えた場合に送出される。</exception>
        void Wait(StageStates state = StageStates.Stopped, int waitTime = -1);
    }
}
