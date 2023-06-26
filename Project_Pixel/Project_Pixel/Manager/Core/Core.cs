using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Threading
{
    /// <summary>
    /// 해당 클래스는 상속 받아 사용해야합니다.
    /// </summary>
    public class Core
    {
        private bool isRunning;             // 수행 유무 체크입니다.
        private Thread updateThread;        // 업데이트 전용 쓰레드입니다.
        private Stopwatch stopWatch;        // 시간 체크
        private const int updateInterval = 16; // 업데이트 간격 (16ms = 60fps)

        /// <summary>
        /// 유니티의 Start() 함수입니다.
        /// 해당 함수를 오버라이드 하여 사용하면 됩니다.
        /// 유니티의 Start() 함수 처럼 사용하면 됩니다.
        /// </summary>
        public virtual void Start()
        {
            isRunning = true;
            stopWatch = Stopwatch.StartNew();

            updateThread = new Thread(UpdateLoop);
            updateThread.Start();
        }

        /// <summary>
        /// 유니티의 Update() 함수입니다.
        /// 해당 함수를 오버라이드 하여 사용하면 됩니다.
        /// 유니티의 Update() 함수 처럼 사용하면 됩니다.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// 업데이트 루프문입니다.
        /// </summary>
        private void UpdateLoop()
        {
            while (isRunning)
            {
                long elapsed = stopWatch.ElapsedMilliseconds;
                if (elapsed >= updateInterval)
                {
                    Update();
                    stopWatch.Restart();
                }
            }
        }

        /// <summary>
        /// 해당 쓰레드를 종료 시킵니다.
        /// 업데이트가 정지됩니다.
        /// </summary>
        public void Stop()
        {
            isRunning = false;
            updateThread.Join();
        }
    }
}
