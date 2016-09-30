using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.Graphics.Display;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace TVWP.Class
{
    class Player:Data
    {
        struct PlayStateBar
        {
            public Canvas can;
            public Border background;
            public ProgressBar progress;
            public SymbolIcon flag;
            public SymbolIcon play;
            public SymbolIcon volume;
            public Slider slider;
            public ComboBox sharp;
            public ComboBox site;
        }
        #region variable
        struct StateInfo
        {
            public Canvas parent;
            public int play_index;
            public int down_index;
            public bool download;
            public int level;
            public int site;
            public double volume;
            public string vid;
        }
        static VideoInfoA VIA;
        static StateInfo current;
        static MediaElement me;
        static List<MediaElement> container;
        static PlayStateBar PSB = new PlayStateBar();
        static DisplayRequest display;
        #endregion

        #region UI
        static void CreateBar(Canvas parent, Thickness margin)
        {
            if (PSB.can != null)
                return;
            Canvas can = new Canvas();
            PSB.can = can;
            //can.Background = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
            parent.Children.Add(can);
            can.Height = 60;
            can.Margin = margin;

            Border bor = new Border();
            bor.Background = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
            bor.Height = 90;
            bor.Width = screenX;
            PSB.background = bor;
            //can.Background = new SolidColorBrush(Colors.White);

            can.Children.Add(bor);

            ProgressBar pb = new ProgressBar();
            pb.Width = screenX;
            pb.Background = new SolidColorBrush(Color.FromArgb(64, 255, 0, 0));
            pb.Foreground = new SolidColorBrush(Color.FromArgb(128, 0, 250, 18));
            pb.PointerReleased += ProgressChange;
            pb.Height = 5;
            pb.Margin = new Thickness(0, 20, 0, 0);
            can.Children.Add(pb);
            PSB.progress = pb;

            SymbolIcon flag = new SymbolIcon();
            flag.Symbol = Symbol.Flag;
            flag.Foreground = new SolidColorBrush(Colors.OrangeRed);
            can.Children.Add(flag);
            PSB.flag= flag;

            SymbolIcon si = new SymbolIcon();
            si.Symbol = Symbol.Pause;
            si.PointerPressed += Pause;
            Thickness tk = new Thickness();
            tk.Top = 30;
            tk.Left = 10;
            tk.Top = 40;
            si.Margin = tk;
            can.Children.Add(si);
            PSB.play = si;

            si = new SymbolIcon();
            si.Symbol = Symbol.Volume;
            tk.Left += 30;
            si.Margin = tk;
            PSB.volume = si;
            si.PointerReleased += volume_released;
            can.Children.Add(si);

            Slider s = new Slider();
            s.Margin = tk;
            //s.Orientation = Orientation.Vertical;
            s.Visibility = Visibility.Collapsed;
            s.LostFocus += slider_lost;
            s.ValueChanged += slider_change;
            s.Width = 100;
            //s.Height = 100;
            PSB.slider = s;
            can.Children.Add(s);

            TextBlock tb = new TextBlock();
            tb.FontSize = 18;
            tb.Text = "请勿调戏进度条";
            tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            tk.Left += 120;
            tb.Margin = tk;
            can.Children.Add(tb);

            tk.Left += 140;
            ComboBox cb = new ComboBox();
            cb.Margin = tk;
            cb.SelectionChanged +=(o,e)=>{ current.level = (o as ComboBox).SelectedIndex; };
            PSB.sharp = cb;
            can.Children.Add(cb);

            tk.Left += 80;
            cb = new ComboBox();
            cb.Margin = tk;
            cb.SelectionChanged += (o, e) => { current.site = (o as ComboBox).SelectedIndex; };
            PSB.site = cb;
            can.Children.Add(cb);
        }
        public static void Create(Canvas parent, Thickness margin)
        {
            current.parent = parent;
            if (me != null)
            {
                me.Visibility = Visibility.Visible;
                Resize();
                Timer.Delegate(Refresh, 300);
                display.RequestActive();
                return;
            }
            me = new MediaElement();
            double w = margin.Right - margin.Left;
            double h = margin.Bottom - margin.Top;
            me.Width = w;
            me.Height = h;
            me.Stretch = Stretch.Uniform;
            me.MediaEnded += PlayEnd;
            parent.Children.Insert(2,me);
            margin.Top = margin.Bottom - 70;
            CreateBar(parent, margin);
            current.volume = 100;
            Timer.Delegate(Refresh,300);
            container = new List<MediaElement>();
            container.Add(me);
            display = new DisplayRequest();
            display.RequestActive();
#if phone
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
#endif
        }
        static void Pause(object sender, RoutedEventArgs e)
        {
            if (me.CurrentState == MediaElementState.Playing)
            {
                PSB.play.Symbol = Symbol.Play;
                me.Pause();
            }
            else
            {
                PSB.play.Symbol = Symbol.Pause;
                me.Play();
            }
        }
        static void Jump(Object sender, RangeBaseValueChangedEventArgs e)
        {
            double tl = e.NewValue;
            tl *= VIA.alltime;
            tl /= 100;
            int s = (int)tl;
            int h = s / 3600;
            int t = s % 3600;
            int m = t / 60;
            s = t % 60;
            //player.Position = new TimeSpan(0,h,m,s);
        }
        public static void Pressed()
        {
            if (PSB.can == null)
                return;
            if (OffsetX == 0 & OffsetY == 0)
                if (Y > 90 & Y < screenY - 60)
                    PSB.can.Visibility = PSB.can.Visibility != Visibility.Visible ?
                        Visibility.Visible : Visibility.Collapsed;
        }
        public static void Resize()
        {
            me.Width = screenX;
            me.Height = screenY;
            PSB.progress.Width = screenX;
            Thickness tk = new Thickness(0, screenY - 70, screenX, screenY);
            PSB.can.Margin = tk;
            PSB.background.Margin = tk;
        }
        static void ProgressChange(object sender, PointerRoutedEventArgs e)
        {
            X = Window.Current.CoreWindow.PointerPosition.X;
            PSB.flag.Margin = new Thickness(X, 0, 0, 0);
            double tl = X / screenX;
            tl *= VIA.alltime;
            int s = (int)tl;
            int split = (VIA.fregment - 1) * 300;
            int i;
            if(tl<split)
            {
                i = s / 300;
                s %= 300;
            }
            else
            {
                i = VIA.fregment - 1;
                s -= split;
            }
            int m = s / 60;
            s %= 60;
            TimeSpan ts = new TimeSpan(0,m,s);
            PlayChange(i,ts);
        }
        static void Refresh()
        {
            if(current.download)
            PSB.progress.Value = DownProgress()*100;
            TimeSpan ts = me.Position;
            double s = ts.Minutes * 60 + ts.Seconds;
            s += current.play_index * 300;
            s /= VIA.alltime;
            s *= screenX;
            PSB.flag.Margin = new Thickness(s, 0, 0, 0);
            double b = me.BufferingProgress;
            if (b < 1)
            {
                b *= 100;
                int c = (int)b;
                Main.Notify( "正在缓冲" + c.ToString() + "%",Colors.Green,Color.FromArgb(0,0,0,0));
            }
            else Main.NotifyStop();
        }
        static void slider_lost(object sender, RoutedEventArgs e)
        {
            PSB.slider.Visibility = Visibility.Collapsed;
        }
        static void slider_change(Object sender, RangeBaseValueChangedEventArgs e)
        {
            current.volume = e.NewValue;
            me.Volume = e.NewValue / 100;
            Main.Notify("音量:" + e.NewValue.ToString(), Colors.Blue, Color.FromArgb(0, 0, 0, 0));
        }
        static void volume_released(Object sender, PointerRoutedEventArgs e)
        {
            PSB.slider.Visibility = Visibility.Visible;
            PSB.slider.Margin = new Thickness(60, 30, 0, 0);
            PSB.slider.Value = current.volume;
        }
        static void PlayEnd(object o,RoutedEventArgs e)
        {
            container[current.play_index].Visibility = Visibility.Collapsed;
            current.play_index++;
            if (current.play_index < VIA.fregment)
                PlayChange();
            else
            {
                string vid = VideoView.GetNextVid(current.vid);
                if(vid!=null)
                    Setvid(vid);
            }
        }
        public static void Dispose()
        {
            Timer.Stop();
            display.RequestRelease();
            for (int i=0;i<container.Count;i++)
            {
                container[i].Stop();
                container[i].Visibility = Visibility.Collapsed;
            }
            PSB.can.Visibility = Visibility.Collapsed;
            Main.NotifyStop();
#if phone
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
#endif
        }
        #endregion

        #region control
        public static void Setvid(string vid)
        {
            current.vid = vid;
            current.play_index = 0;
            current.down_index = 0;
            current.download = false;
            WebClass.GetVideoInfo(vid,SetVideInfo);
        }
        static string[] site = new string[] { "站点", "site" };
        static void SetVideInfo(VideoInfoA via)
        {
            PSB.sharp.Items.Clear();
            VIA = via;
            VideoInfo[] vi = via.vi;
            for (int i = 0; i < vi.Length; i++)
                PSB.sharp.Items.Add(via.vi[i].sharp);
            PSB.site.Items.Clear();
            string[] http = VIA.http;
            for (int i = 0; i < http.Length; i++)
                PSB.site.Items.Add(site[language] +i.ToString());
            char[] temp = vi[0].sharp.ToCharArray();
            int s = CharToInt(ref temp);
            int c = 0;
            for (int i=1;i<vi.Length;i++)
            {
                temp = vi[i].sharp.ToCharArray();
                int t = CharToInt(ref temp);
                if(t>s)
                {s = t;c = i;}
            }
            PSB.sharp.SelectedIndex= current.level = c;
            WebClass.GetVideoKey(via.vid, VIA.vi[c],(v,vk)=>{ VIA.vi[current.level].vkey = vk; PlayStart(); },1);
            WebClass.GetVideoFregment(via.vid, via.vi[c], (e)=>{ VIA.fregment = e; });
        }
        static void PlayStart()
        {
            int level = current.level;
            VideoInfo vi = VIA.vi[level];
            string str = VIA.http[current.site] + vi.fn + "1.mp4?vkey=" + vi.vkey
                    + "&type=mp4&fmt=" + vi.fmt;
            me.Source = new Uri(str);
            me.Volume = current.volume;
            me.Play();
            current.download = true;
        }
        static void PlayChange()
        {
            me = container[current.play_index];
            me.Width = screenX;
            me.Height = screenY;
            me.Stretch = Stretch.Uniform;
            me.Margin = new Thickness(0, 0, screenX, screenY);
            me.Volume = current.volume / 100;
            me.Visibility = Visibility.Visible;
            me.Play();
        }
        static void PlayChange(int index, TimeSpan position)
        {
            int c =index- container.Count;
            if(current.down_index!=index)
            {
                container[current.down_index].Stop();
                container[current.down_index].Visibility = Visibility.Collapsed;
            }
            for (int i = container.Count; i <= index; i++)
            {
                MediaElement m = new MediaElement();
                current.parent.Children.Insert(2, m);
                container[current.down_index] = m;
                VideoInfo vi = VIA.vi[current.level];
                string str = VIA.http[current.site] + vi.fn + (i + 1).ToString() + ".mp4?vkey=" + vi.vkey
                        + "&type=mp4&fmt=" + vi.fmt;
                m.MediaEnded += PlayEnd;
                m.Source = new Uri(str);
                container.Add(m);
            }
            current.down_index = index;
            me = container[index];
            current.play_index = index;
            me.Width = screenX;
            me.Height = screenY;
            me.Stretch = Stretch.Uniform;
            me.Margin = new Thickness(0, 0, screenX, screenY);
            me.Volume = current.volume / 100;
            me.Position = position;
            me.Visibility = Visibility.Visible;
            me.Play();
        }
        static void DownCurrent()
        {
            MediaElement m;
            if (current.down_index<container.Count)
                m = container[current.down_index];
            else
            {
                m = new MediaElement();
                container.Add(m);
                int level = current.level;
                VideoInfo vi = VIA.vi[level];
                int c = current.down_index + 1;
                string str = VIA.http[current.site] + vi.fn + c.ToString() + ".mp4?vkey=" + vi.vkey
                        + "&type=mp4&fmt=" + vi.fmt;
                m.MediaEnded += PlayEnd;
                m.Source = new Uri(str);
            }
            m.Width = 100;
            m.Height = 100;
            m.Margin = new Thickness(-100, -100, 0, 0);
            current.parent.Children.Insert(2, m);
            m.Play();
            m.Pause();
            m.Position = new TimeSpan();
        }
        static double DownProgress()//return progress
        {
            int i = current.down_index;
            if (i >= container.Count)
                return 0;
            MediaElement m = container[i];
            double p = m.DownloadProgress;
            if (p == 1)
            {
                current.down_index++;
                if (current.down_index < VIA.fregment)
                    DownCurrent();
                else { current.download = false; return 1; }
            }
            if (i == VIA.fregment - 1)
            {
                double t = i * 300;
                p *= VIA.alltime - t;
                p += t;
            }
            else
            {
                p += i;
                p *= 300;
            }
            if (VIA.alltime != 0)
                p /= VIA.alltime;
            return p;
        }
        #endregion
    }
}