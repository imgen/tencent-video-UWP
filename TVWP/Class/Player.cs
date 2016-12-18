using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace TVWP.Class
{
    class Player:Component,Navigation
    {
        static string[] volume = { "音量:","Volume:"};
        struct PlayStateBar
        {
            public Canvas canA;
            public Canvas canB;
            public Canvas bor;
            public ProgressBar progress;
            public SymbolIcon flag;
            public SymbolIcon play;
            public SymbolIcon volume;
            public Slider slider;
            public ComboBox sharp;
            public ComboBox site;
            public TextBlock title;
        }
        #region variable
        struct StateInfo
        {
            public Canvas parent;
            public int play_part;
            public int down_index;
            public bool play;
            public bool download;
            //public int level;
            //public int site;
            public double volume;
            public string vid;
        }
        static VideoInfo vic;
        static StateInfo current;
        static MediaElement me;
        static MediaElement[] buff;
        static PlayStateBar PSB = new PlayStateBar();
        static DisplayRequest display;
        static double screenW, screenH;
        static VideoAddress va;
        #endregion

        #region UI
        static long time;
        static Point sx;
        static double ss;
        static void CreateBar(Canvas p)
        {
            if (PSB.canA != null)
                return;
            Canvas can = new Canvas();
            PSB.canA = can;
            p.Children.Add(can);
            can = new Canvas();
            p.Children.Add(can);
            PSB.canB = can;
            can.Background = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
            can.Width = screenW;
            can.Height = 70;
            can.Margin = new Thickness(0, screenH - 70, 0, 0);

            ProgressBar pb = new ProgressBar();
            pb.PointerReleased += ProgressChange;
            pb.Background = new SolidColorBrush(Color.FromArgb(64, 255, 0, 0));
            pb.Foreground = new SolidColorBrush(Color.FromArgb(128, 0, 250, 18));
            pb.Width = screenW;
            pb.Height = 5;
            pb.Margin = new Thickness(0, 20, 0, 0);
            can.Children.Add(pb);
            PSB.progress = pb;

            SymbolIcon flag = new SymbolIcon();
            flag.Symbol = Symbol.Flag;
            flag.Foreground = new SolidColorBrush(Colors.OrangeRed);
            can.Children.Add(flag);
            PSB.flag = flag;

            SymbolIcon si = new SymbolIcon();
            si.Symbol = Symbol.Pause;
            si.PointerPressed += Pause;
            PSB.play = si;
            can.Children.Add(si);
            Thickness tk = new Thickness();
            tk.Top = 30;
            tk.Left = 10;
            tk.Top = 40;
            si.Margin = tk;

            si = new SymbolIcon();
            si.Symbol = Symbol.Volume;
            si.PointerReleased += volume_released;
            PSB.volume = si;
            can.Children.Add(si);
            tk.Left += 30;
            si.Margin = tk;

            Slider s = new Slider();
            s.Visibility = Visibility.Collapsed;
            s.ValueChanged += slider_change;
            s.Width = 100;
            PSB.slider = s;
            can.Children.Add(s);
            s.Margin = tk;

            ComboBox cb = new ComboBox();
            cb.SelectionChanged += (o, e) => { ChangeSharp((o as ComboBox).SelectedIndex); };
            PSB.sharp = cb;
            can.Children.Add(cb);
            tk.Left += 140;
            cb.Margin = tk;

            cb = new ComboBox();
            cb.SelectionChanged += (o, e) => { ChangSite((o as ComboBox).SelectedIndex); };
            PSB.site = cb;
            can.Children.Add(cb);
            tk.Left += 80;
            cb.Margin = tk;

            TextBlock tb = new TextBlock();
            tb.Foreground = title_brush;
            tk.Left += 80;
            tb.Margin = tk;
            can.Children.Add(tb);
            PSB.title = tb;

            can = new Canvas();
            p.Children.Add(can);
            can.Background = trans_brush;
            PSB.bor = can;
            can.Width = screenW;
            can.Height = screenH;
            can.PointerPressed += (o, e) => {
                time = DateTime.Now.Ticks;
                PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                sx = pp.Position;
                ss = PSB.flag.Margin.Left;
            };
            can.PointerMoved += (o,e) => {
                PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                if(pp.IsInContact)
                {
                    double ox = pp.Position.X - sx.X;
                    Thickness t = PSB.flag.Margin;
                    double x = ss + ox;
                    if (x < 0)
                        x = 0;
                    if (x > screenX)
                        x = screenX;
                    t.Left = x;
                    PSB.flag.Margin = t;
                    x /= screenX;
                    x *= vic.alltime;
                    int h = (int)x / 3600;
                    int se = (int)x % 3600;
                    int min = se / 60;
                    se %= 60;
                    string st = h.ToString() + ":" + min.ToString() + ":" + se.ToString();
                    Main.Notify(st,font_brush,trans_brush,1000);
                }
            };
            can.PointerReleased += (o, e) => {
                if(DateTime.Now.Ticks-time<presstime)
                {
                    if (PSB.canB.Visibility == Visibility.Visible)
                    {
                        PSB.canB.Visibility = Visibility.Collapsed;
                        PSB.bor.Width = screenW;
                        PSB.bor.Height = screenH;
                    }
                    else
                    {
                        PSB.canB.Visibility = Visibility.Visible;
                        PSB.bor.Width = screenW;
                        PSB.bor.Height = screenH - 70;
                    }
                }
                else
                {
                    PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                    double ox = pp.Position.X - sx.X;
                    X = ss + ox;
                    if (X < 0)
                        X = 0;
                    if (X > screenX)
                        X = screenX;
                    Jump();
                }
            };
        }
        static void Create(Canvas parent)
        {
            current.parent = parent;
            if (me != null)
            {
                me.Visibility = Visibility.Visible;
                PSB.canA.Visibility = Visibility.Visible;
                PSB.bor.Visibility = Visibility.Visible;
                Resize();
                Timer.Delegate(Refresh, 300);
                display.RequestActive();
#if phone
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
#endif
                return;
            }
            screenW = Window.Current.Bounds.Width;
            screenH = Window.Current.Bounds.Height;
#if phone
            if(screenH>screenW)
            {
                double t = screenH;
                screenH = screenW;
                screenW = t;
            }
#endif
            CreateBar(parent);
            me = new MediaElement();
            PSB.canA.Children.Add(me);
            me.Stretch = Stretch.Uniform;
            me.MediaEnded += PlayEnd;
            current.volume = 100;
            Timer.Delegate(Refresh, 300);
            buff = new MediaElement[48];//(48*5)/60=4h
            buff[0] = me;
            display = new DisplayRequest();
            display.RequestActive();
#if phone
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
#endif
            Resize();
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
      
        public static void Pressed()
        {
            if (PSB.canB == null)
                return;
            if (OffsetX == 0 & OffsetY == 0)
                if (Y > 90 & Y < screenH - 60)
                    PSB.canB.Visibility = PSB.canB.Visibility != Visibility.Visible ?
                        Visibility.Visible : Visibility.Collapsed;
        }
        public static void Resize()
        {
            screenW = Window.Current.Bounds.Width;
            screenH = Window.Current.Bounds.Height;
#if phone
            if (screenH > screenW)
            {
                double t = screenH;
                screenH = screenW;
                screenW = t;
            }
#endif
            me.Width = screenW;
            me.Height = screenH;
            PSB.progress.Width = screenW;
            Thickness tk = new Thickness(0, screenH - 70, screenW, screenH);
            PSB.canB.Margin = tk;
            PSB.canB.Width = screenW;
            PSB.bor.Width = screenW;
            if (PSB.canB.Visibility == Visibility.Visible)
                PSB.bor.Height = screenH - 70;
            else PSB.bor.Height = screenH;
        }
        static void ProgressChange(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
            X = pp.Position.X;
            Jump();
        }
        static void Jump()
        {
            PSB.flag.Margin = new Thickness(X, 0, 0, 0);
            double tl = X / screenW;
            tl *= vic.alltime;
            int s = (int)tl;
            int p = s / 300;
            if (p >= vic.part)
                p--;
            s -= p * 300;
            int h = s / 3600;
            int t = s % 3600;
            int m = t / 60;
            s = t % 60;
            if (p != current.play_part)
            {
                current.play_part = p;
                me.Visibility = Visibility.Collapsed;
                me.Stop();
                buff[current.down_index].Visibility = Visibility.Collapsed;
                current.down_index = current.play_part;
                PlayChange();
            }
            me.Position = new TimeSpan(0, h, m, s);
        }
        
        static void slider_change(Object sender, RangeBaseValueChangedEventArgs e)
        {
            current.volume = e.NewValue;
            me.Volume = e.NewValue / 100;
            Main.Notify(volume[language] + e.NewValue.ToString(), title_brush, trans_brush, 0);
        }
        static void volume_released(Object sender, PointerRoutedEventArgs e)
        {
            PSB.slider.Visibility = Visibility.Visible;
            PSB.slider.Margin = new Thickness(60, 30, 0, 0);
            PSB.slider.Value = current.volume;
        }
        static void PlayEnd(object o, RoutedEventArgs e)
        {
            buff[current.play_part].Visibility = Visibility.Collapsed;
            current.play_part++;
            if (current.play_part < vic.part)
                PlayChange();
            else
            {
                string vid = VideoPage.GetNextVid(current.vid);
                if (vid != null)
                    Setvid(vid);
            }
        }
        static void Dispose()
        {
            Timer.Stop();
            display.RequestRelease();
            for (int i = 0; i < 48; i++)
            {
                if(buff[i]!=null)
                {
                    buff[i].Stop();
                    buff[i].Visibility = Visibility.Collapsed;
                }
            }
            PSB.canB.Visibility = Visibility.Collapsed;
            PSB.canA.Visibility = Visibility.Collapsed;
            PSB.bor.Visibility = Visibility.Collapsed;
            Main.NotifyStop();
#if phone
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
#endif
        }
        #endregion

        #region control
        public static void Setvid(string vid)
        {
            current.vid = vid;
            current.play_part = 0;
            current.down_index = 0;
            current.download = false;
            current.play = false;
            if (va == null)
                va = new VideoAddress();
            va.SetVid(vid,Setting.sharp,PlayStart);
        }
        static string[] site = new string[] { "站点", "site" };
        static void PlayStart(string vid)
        {
            vic = new VideoInfo();
            va.GetAddress(ref vic);
            string str;
            if (vic.type == 1)
                str = vic.href;
            else str = vic.href + "1" + vic.vkey;
            me.Source = new Uri(str);
            me.Volume = current.volume;
            me.Play();
            current.play = true;
            current.download = true;
            ComboBox cb = PSB.sharp;
            cb.Items.Clear();
            int c = vic.sharp.Length;
            for (int i = 0; i < c; i++)
                cb.Items.Add(vic.sharp[i]);
            cb = PSB.site;
            cb.Items.Clear();
            c = vic.site;
            for (int i = 0; i < c; i++)
                cb.Items.Add(site[language]+i.ToString());
            PSB.title.Text = vic.tilte;
        }
        static void PlayChange()
        {
            if (buff[current.play_part] == null)
            {
                MediaElement m = new MediaElement();
                m.MediaEnded += PlayEnd;
                buff[current.play_part] = m;
                current.parent.Children.Insert(2, m);
            }
            me = buff[current.play_part];
            me.Width = screenW;
            me.Height = screenH;
            me.Stretch = Stretch.Uniform;
            string str;
            if (vic.type == 1)
                str = vic.href;
            else 
                str = vic.href + (current.play_part + 1).ToString() + vic.vkey + "&guid=" + vic.cmd5[current.play_part];
            me.Source = new Uri(str);
            me.Margin = new Thickness(0, 0, screenW, screenH);
            me.Volume = current.volume / 100;
            me.Visibility = Visibility.Visible;
            me.Play();
        }
        static void Refresh()
        {
            if (!current.play)
                return;
            PSB.progress.Value = DownProgress() * 100;
            TimeSpan ts = me.Position;
            double s = ts.Minutes * 60 + ts.Seconds;
            s += current.play_part * 300;
            s /= vic.alltime;
            s *= screenW;
            PSB.flag.Margin = new Thickness(s, 0, 0, 0);
            double b = me.BufferingProgress;
            if (b < 1)
            {
                b *= 100;
                int c = (int)b;
                Main.Notify("正在缓冲" + c.ToString() + "%", nav_brush, trans_brush, 30);
            }
            else Main.NotifyStop();
        }
        static void DownCurrent()
        {
            MediaElement m;
            if(buff[current.down_index]==null)
            {
                m = new MediaElement();
                m.MediaEnded += PlayEnd;
                buff[current.down_index] = m;
                current.parent.Children.Insert(2, m);
            }
            m = buff[current.down_index];
            m.Visibility = Visibility.Visible;
            m.Width = 100;
            m.Height = 100;
            string str;
            if (vic.type == 1)
                str = vic.href;
            else str = vic.href + (current.down_index + 1).ToString() + vic.vkey;
            m.Source = new Uri(str);
            m.Margin = new Thickness(-100, -100, 0, 0);
            m.Play();
            m.Pause();
        }
        static double DownProgress()//return progress
        {
            int i = current.down_index;
            if (i >= vic.part)
                return 1;
            MediaElement m = buff[i];
            double p = m.DownloadProgress;
            if (p == 1)
            {
                current.down_index++;
                if (current.down_index < vic.part)
                    DownCurrent();
                else { current.download = false; return 1; }
            }
            if (i == vic.part - 1)
            {
                double t = i * 300;
                p *= vic.alltime - t;
                p += t;
            }
            else
            {
                p += i;
                p *= 300;
            }
            if (vic.alltime != 0)
                p /= vic.alltime;
            return p;
        }
        static void ChangeSharp(int index)
        {
            if (index < 0)
                return;
            va.ChangeSharpA(index,(s)=> {
                va.GetAddress(ref vic);
                string str;
                if (vic.type == 1)
                    str = vic.href;
                else
                {
                    str = vic.href + (current.play_part+1).ToString() + vic.vkey;
                    if(current.down_index<vic.part)
                    buff[current.down_index].Source = new Uri(vic.href + current.down_index.ToString() + vic.vkey);
                }
                me.Source = new Uri(str);
            });
        }
        static void ChangSite(int index)
        {
            if (index < 0)
                return;
            va.ChangeSite(index ,ref vic);
            string str;
            if (vic.type == 1)
                str = vic.href;
            else {
                str = vic.href + (current.play_part + 1).ToString() + vic.vkey;
                buff[current.play_part].Source = new Uri(vic.href+current.down_index.ToString()+vic.vkey);
            }
            me.Source = new Uri(str);
        }
        public void Create(Canvas p,Thickness m)
        {
            Create(p);
        }
        public void Hide()
        {

        }
        public void Show()
        {

        }
        public bool Back()
        {
            Dispose();
            return false;
        }
        public void ReSize(Thickness m)
        {
            Resize();
        }
        #endregion
    }
    class PlayerEx:Component, Navigation
    {
        static string[] volume = { "音量:", "Volume:" };
        static MediaElement me;
        static DisplayRequest display;
        static double screenH,screenW;
        struct PlayStateBar
        {
            public Canvas canA;
            public Canvas canB;
            public Canvas bor;
            public ProgressBar progress;
            public SymbolIcon flag;
            public SymbolIcon play;
            public SymbolIcon volume;
            public Slider slider;
            public TextBlock title;
        }
        static PlayStateBar PSB;
        static Canvas parent;
        static Mission miss;
        static StorageFolder sf;
        static StorageFile ss;
        static IRandomAccessStream iras;
        static int part;
        static long time;
        static Point sx;
        static double sst;
        static int playindex;
        public static void Create(Canvas p)
        {
            if (me != null)
            {
                me.Visibility = Visibility.Visible;
                PSB.canA.Visibility = Visibility.Visible;
                PSB.bor.Visibility = Visibility.Visible;
                Resize();
                //Timer.Delegate(Refresh, 300);
                display.RequestActive();
#if phone
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
#endif
                return;
            }
            parent = p;
            me = new MediaElement();
            me.Width = screenW;
            me.Height = screenH;
            me.Stretch = Stretch.Uniform;
            me.MediaEnded += PlayEnd;
            p.Children.Add(me);
            PSB = new PlayStateBar();
            CreateBar(p);
            screenW = Window.Current.Bounds.Width;
            screenH = Window.Current.Bounds.Height;
#if phone
            if (screenH > screenW)
            {
                double t = screenH;
                screenH = screenW;
                screenW = t;
            }
#endif
            display = new DisplayRequest();
            display.RequestActive();
#if phone
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
#endif
            Resize();

        }
        static void CreateBar(Canvas p)
        {
            if (PSB.canA != null)
                return;
            Canvas can = new Canvas();
            PSB.canA = can;
            p.Children.Add(can);
            can = new Canvas();
            p.Children.Add(can);
            PSB.canB = can;
            can.Background = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
            can.Width = screenW;
            can.Height = 70;
            can.Margin = new Thickness(0, screenH - 70, 0, 0);

            ProgressBar pb = new ProgressBar();
            pb.PointerReleased += ProgressChange;
            pb.Background = new SolidColorBrush(Color.FromArgb(64, 255, 0, 0));
            pb.Foreground = new SolidColorBrush(Color.FromArgb(128, 0, 250, 18));
            pb.Width = screenW;
            pb.Height = 5;
            pb.Margin = new Thickness(0, 20, 0, 0);
            can.Children.Add(pb);
            PSB.progress = pb;

            SymbolIcon flag = new SymbolIcon();
            flag.Symbol = Symbol.Flag;
            flag.Foreground = new SolidColorBrush(Colors.OrangeRed);
            can.Children.Add(flag);
            PSB.flag = flag;

            SymbolIcon si = new SymbolIcon();
            si.Symbol = Symbol.Pause;
            si.PointerPressed += Pause;
            PSB.play = si;
            can.Children.Add(si);
            Thickness tk = new Thickness();
            tk.Top = 30;
            tk.Left = 10;
            tk.Top = 40;
            si.Margin = tk;

            si = new SymbolIcon();
            si.Symbol = Symbol.Volume;
            si.PointerReleased += volume_released;
            PSB.volume = si;
            can.Children.Add(si);
            tk.Left += 30;
            si.Margin = tk;

            Slider s = new Slider();
            s.Visibility = Visibility.Collapsed;
            s.ValueChanged += slider_change;
            s.Width = 100;
            PSB.slider = s;
            can.Children.Add(s);
            s.Margin = tk;

            TextBlock tb = new TextBlock();
            tb.Foreground = Component.title_brush;
            tk.Left += 80;
            tb.Margin = tk;
            can.Children.Add(tb);
            PSB.title = tb;

            can = new Canvas();
            p.Children.Add(can);
            can.Background = Component.trans_brush;
            PSB.bor = can;
            can.Width = screenW;
            can.Height = screenH;
            can.PointerPressed += (o, e) => {
                time = DateTime.Now.Ticks;
                PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                sx = pp.Position;
                sst = PSB.flag.Margin.Left;
            };
            can.PointerMoved += (o, e) => {
                PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                if (pp.IsInContact)
                {
                    double ox = pp.Position.X - sx.X;
                    Thickness t = PSB.flag.Margin;
                    t.Left = sst + ox;
                    if (t.Left < 0)
                        t.Left = 0;
                    if (t.Left > screenX)
                        t.Left = screenX;
                    PSB.flag.Margin = t;
                }
            };
            can.PointerReleased += (o, e) => {
                if (DateTime.Now.Ticks - time < presstime)
                {
                    if (PSB.canB.Visibility == Visibility.Visible)
                    {
                        PSB.canB.Visibility = Visibility.Collapsed;
                        PSB.bor.Width = screenW;
                        PSB.bor.Height = screenH;
                    }
                    else
                    {
                        PSB.canB.Visibility = Visibility.Visible;
                        PSB.bor.Width = screenW;
                        PSB.bor.Height = screenH - 70;
                    }
                }
                else
                {
                    PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                    double ox = pp.Position.X - sx.X;
                    X = sst + ox;
                    if (X < 0)
                        X = 0;
                    if (X > screenX)
                        X = screenX;
                    Jump();
                }
            };
        }
        public static void Resize()
        {
            screenW = Window.Current.Bounds.Width;
            screenH = Window.Current.Bounds.Height;
#if phone
            if (screenH > screenW)
            {
                double t = screenH;
                screenH = screenW;
                screenW = t;
            }
#endif
            me.Width = screenW;
            me.Height = screenH;
            PSB.progress.Width = screenW;
            Thickness tk = new Thickness(0, screenH - 70, screenW, screenH);
            PSB.canB.Margin = tk;
            PSB.canB.Width = screenW;
            PSB.bor.Width = screenW;
            if (PSB.canB.Visibility == Visibility.Visible)
                PSB.bor.Height = screenH - 70;
            else PSB.bor.Height = screenH;
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
        static void ProgressChange(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
            double X = pp.Position.X;
        }
        static void Jump()
        {
            PSB.flag.Margin = new Thickness(X, 0, 0, 0);
            double tl = X / screenW;
            tl *= miss.time;
            int s = (int)tl;
            int p = s / 300;
            if (p >= miss.max)
                p--;
            s -= p * 300;
            int h = s / 3600;
            int t = s % 3600;
            int m = t / 60;
            s = t % 60;
            if (p != part)
            {
                part = p;
                PlayNextPart();
            }
            me.Position = new TimeSpan(0, h, m, s);
        }
        static void volume_released(Object sender, PointerRoutedEventArgs e)
        {
            PSB.slider.Visibility = Visibility.Visible;
            PSB.slider.Margin = new Thickness(60, 30, 0, 0);
            //PSB.slider.Value = current.volume;
        }
        static void slider_change(Object sender, RangeBaseValueChangedEventArgs e)
        {
            //current.volume = e.NewValue;
            me.Volume = e.NewValue / 100;
            Main.Notify(volume[Component.language]+ e.NewValue.ToString(), Component.title_brush, Component.trans_brush, 0);
        }
        public async static void PlayEx(int index, StorageFolder s)
        {
            playindex = index;
            miss = DownLoad.Getmission(index);
            part = 0;
            sf = s;
            ss = await s.CreateFileAsync("0", CreationCollisionOption.OpenIfExists);
            IRandomAccessStream i = await ss.OpenAsync(FileAccessMode.Read);
            part = 0;
            me.SetSource(i, ss.FileType);
            me.Play();
            if (iras != null)
                iras.Dispose();
            iras = i;
        }
        async static void PlayNextPart()
        {
            miss = DownLoad.Getmission(playindex);
            if(miss.progress[part]==1)
            {
                ss = await sf.CreateFileAsync(part.ToString(), CreationCollisionOption.OpenIfExists);
                IRandomAccessStream i = await ss.OpenAsync(FileAccessMode.Read);
                part = 0;
                me.SetSource(i, ss.FileType);
                me.Play();
                if (iras != null)
                    iras.Dispose();
                iras = i;
            }
        }
        static void Dispose()
        {
            if (iras != null)
                iras.Dispose();
            me.Stop();
            me.Visibility = Visibility.Collapsed;
            PSB.canA.Visibility = Visibility.Collapsed;
            PSB.canB.Visibility = Visibility.Collapsed;
            PSB.bor.Visibility = Visibility.Collapsed;
            display.RequestRelease();
#if phone
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
#endif
        }
        static void PlayEnd(object o, RoutedEventArgs e)
        {
            part++;
            PlayNextPart();
        }
        public void Create(Canvas p, Thickness m)
        {
            Create(p);
        }
        public void Hide()
        {

        }
        public void Show()
        {

        }
        public bool Back()
        {
            Dispose();
            return false;
        }
        public void ReSize(Thickness m)
        {
            Resize();
        }
    }
}