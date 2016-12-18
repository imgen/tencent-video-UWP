using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
        public static void ChangeDeletgate(Action act)
        {
            run = act;
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

        static ScrollViewer sv;
        static Panel panel;
        static double speed;
        static int st;
        static bool hor;
        public static void Scorll(double s,ScrollViewer v,Panel p,bool h)
        {
            st = DateTime.Now.Millisecond;
            sv = v;
            panel = p;
            speed = s;
            hor= h;
            Delegate(Scorlling,16);
        }
        static void Scorlling()
        {
            if(sv.Visibility==Visibility.Collapsed)
            {
                Stop();
                return;
            }
            int t = DateTime.Now.Millisecond;
            int a = t - st;
            st = t;
            if (a < 0)
                a += 1000;
            speed *= 0.93f;
            if (speed > -0.008f & speed < 0.008f)
                Stop();
            double x = speed * a;
            if (hor)
            {
                x += panel.Margin.Left;
                double ox= sv.Width - panel.Width;
                if (x > 0)
                {
                    x = 0;
                    Stop();
                }
                else
                if (x < ox)
                {
                    x = ox;
                    Stop();
                }
                double top = panel.Margin.Top;
                panel.Margin = new Thickness(x, top, 0, 0);
            }
            else
            {
                x += panel.Margin.Top;
                double oy = sv.Height - panel.Height;
                if(x>0)
                {
                    x = 0;
                    Stop();
                }else
                if (x < oy)
                {
                    x = oy;
                    Stop();
                }
                double left = panel.Margin.Left;
                panel.Margin = new Thickness(left, x, 0, 0);
            }
        }
    }
}
