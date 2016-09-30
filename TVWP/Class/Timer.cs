using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.UI.Xaml;
namespace TVWP.Class
{
    class Timer
    {
        static Action run { get; set; }
        static DispatcherTimer dt;
        public static void Inital()
        {
            dt = new DispatcherTimer();
            dt.Tick += Tick;
            dt.Stop();
        }
        public static void Delegate(Action act,int time)
        {
            if (run != null)
                return;
            dt.Interval = new TimeSpan(0,0,0,0,time);
            run = act;
            dt.Start();
        }
        static void Tick(object sender, object e)
        {
            if(run!=null)
            {
                run();
            }
        }
        public static void Stop()
        {
            run = null;
            dt.Stop();
        }
    }
}
