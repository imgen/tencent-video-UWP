using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.Globalization;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    enum PageTag : int
    {
        none,nav,search,setting,view,player
    }
    class Main:Data
    {
        struct CurrentState
        {
            public int index;
            public PageTag tag;
            public PageTag main;
            //public Action inital;
            public Action<PageTag> clear;
        }

        #region const
        
        #endregion

        #region main
        static Action back { get; set; }
        static Action pressed { get; set; }
        static Action moved { get; set; }
        static Action released { get; set; }
        static Canvas main_canv { get; set; }
        static Canvas canA,canB;
        static TextBlock notify;
        //static Border bk_ground;
        static CurrentState current=new CurrentState();

        public static void Initial(Canvas can)
        {
            main_canv = can;
            WebClass.Initial();
            Window.Current.CoreWindow.PointerPressed += Pointer_Pressed;
            Window.Current.CoreWindow.PointerMoved += Pointer_Moved;
            Window.Current.CoreWindow.PointerReleased += Pointer_Released;
            Window.Current.SizeChanged += SizeChange;
#if desktop
            Window.Current.CoreWindow.KeyDown += Core_KeyDown;
#endif
            Timer.Inital();
            screenX = Window.Current.Bounds.Width;
            screenY = Window.Current.Bounds.Height;
            //bk_brush = new SolidColorBrush(Colors.LightBlue);
            font_brush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            canA = new Canvas();
            canA.Width = screenX;
            canA.Height = screenY;
            canA.Background = bk_brush;
            main_canv.Children.Add(canA);
            //canA.Margin = new Thickness(0,30,screenX,screenY);

            canB = new Canvas();
            canB.Width = 200;
            canB.Height = 200;
            canB.Background = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
            canB.Visibility = Visibility.Collapsed;
            main_canv.Children.Add(canB);
            notify = new TextBlock();
            notify.Width = 200;
            notify.TextWrapping = TextWrapping.Wrap;
            canB.Children.Add(notify);
            CreatePage(0,PageTag.nav);
        }
        static void Pointer_Pressed(object sender, object e)
        {
            timenow = DateTime.Now.Ticks;
            if (pressed != null)
            {
                X = Window.Current.CoreWindow.PointerPosition.X;
                Y = Window.Current.CoreWindow.PointerPosition.Y;
                OffsetX = 0; OffsetY = 0;
                pressed();
            }
        }
        static void Pointer_Moved(object sender, object e)
        {
            if (moved != null)
            {
                uint id = Window.Current.CoreWindow.PointerCursor.Id;
                if (PointerPoint.GetCurrentPoint(id).IsInContact)
                {
                    double tx = Window.Current.CoreWindow.PointerPosition.X;
                    double ty = Window.Current.CoreWindow.PointerPosition.Y;
                    OffsetX = tx - X;
                    OffsetY = ty - Y;
                    X = tx; Y = ty;
                    pressed();
                }
            }
        }
        static void Pointer_Released(object sender, object e)
        {
            if (released != null)
            {
                X = Window.Current.CoreWindow.PointerPosition.X;
                Y = Window.Current.CoreWindow.PointerPosition.Y;
                released();
            }
        }
        static void SizeChange(object sender, WindowSizeChangedEventArgs e)
        {
            screenX = e.Size.Width;
            canA.Width = screenX;
            screenY = e.Size.Height;
            canA.Height = screenY;
            PageResize[current.index]();
        }
        static void Core_KeyDown(object sender, object e)
        {
            if ((e as KeyEventArgs).VirtualKey == Windows.System.VirtualKey.Escape)
                Back();
        }
        public static void Back()
        {
            if (current.index > 0)
            {
                if (current.index == 1)
                    CreatePage(0, current.main);
                else CreatePage(1,PageTag.view);
            }
            else Application.Current.Exit();
        }
        #endregion

        #region page manage
        static Action<PageTag>[] PageManage = new Action<PageTag>[] { Create0,Create1,Create2};
        static Action[] PageResize = new Action[] {Resize0,Resize1,Resize2 };
        public static void CreatePage(int level, PageTag tag)
        {
            current.index = level;
            if (tag == current.tag)
                return;
            if (current.clear != null)
                current.clear(tag);
            PageManage[level](tag);
            current.tag = tag;
        }
        static void Create0(PageTag tag)
        {
            current.main = tag;
            current.clear = Clear0;
            if (current.tag == PageTag.setting)
                return;
            Thickness tk = new Thickness(0,0,screenX,30);
            SearchResult.Create_Bar(canA, tk);
            tk.Top = 40;
            tk.Bottom = screenY;
            tk.Right = screenX;
            if (tag == PageTag.nav)
                NavPage.Create(canA, tk);
            else SearchResult.Create(canA,tk);
        }
        static void Clear0(PageTag tag)
        {
            if (tag == PageTag.setting)
                return;
            if(tag==PageTag.view)
            SearchResult.Bar_Hide();
            if (current.tag == PageTag.nav)
                NavPage.Hide();
            else SearchResult.Hide();
        }
        static void Resize0()
        {
            Thickness tk = new Thickness(0, 0, screenX, 30);
            SearchResult.Bar_Resize(tk);
            tk.Top = 40;
            tk.Right = screenX;
            tk.Bottom = screenY;
            if (current.tag == PageTag.nav)
                NavPage.Resize(tk);
            else SearchResult.Resize(tk);
        }
        static void Create1(PageTag tag)
        {
            current.clear = Clear1;
            if (tag == PageTag.setting)
            {
                Setting.Create(main_canv, new Thickness(0, 0, screenX, screenY));
                return;
            }
#if desktop
            if(current.tag==PageTag.player)
                VideoView.Show();
            else
            {
                VideoView.TV_Create(canA, new Thickness(0, 0, 300, screenY));
                VideoView.View_Create(canA, new Thickness(300, 0, 900, screenY));
                VideoView.Com_Create(canA, new Thickness(900, 0, 1300, screenY));
            }
#else
            if (current.tag == PageTag.player)
            { VideoView.Show(); VideoView.View_Resize(new Thickness(0, 0, screenX, screenY)); }
            else
                VideoView.View_Create(main_canv, new Thickness(0, 0, screenX, screenY));
#endif
        }
        static void Clear1(PageTag tag)
        {
            if(current.tag==PageTag.setting)
            {
                Setting.Dispose();
                return;
            }
            if (tag != PageTag.player)
                VideoView.Com_Reset();
            VideoView.Hide();
        }
        static void Resize1()
        {
            if(current.tag==PageTag.setting)
            {
                Setting.Resize(new Thickness(0,0,screenX,screenY));
                return;
            }
#if desktop
            VideoView.TV_Resize(new Thickness(0, 0, 300, screenY));
            VideoView.View_Resize(new Thickness(300, 0, 900, screenY));
            VideoView.Com_Resize(new Thickness(900, 0, 1300, screenY));
#else
            VideoView.View_Resize(new Thickness(0, 0, screenX, screenY));
#endif
        }
        static void Create2(PageTag tag)
        {
            canA.Background = new SolidColorBrush(Colors.Black);
            Player.Create(canA,new Thickness(0,0,screenX,screenY));
            pressed = Player.Pressed;
            current.clear = Clear2;
        }
        static void Clear2(PageTag tag)
        {
            Player.Dispose();
            canA.Background = new SolidColorBrush(Colors.White);
            pressed = null;
        }
        static void Resize2()
        {
            Player.Resize();
        }

#endregion

        public static void Notify(string msg)
        {
            canB.Visibility = Visibility.Visible;
            canB.Background = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128));
            notify.Text = msg;
            notify.Foreground = font_brush;
            canB.Margin = new Thickness(screenX / 2 - 100,screenY/2-50,0,0);
        }
        public static void Notify(string msg,Color font)
        {
            Notify(msg,font, Color.FromArgb(128, 128, 128, 128));
        }
        public static void Notify(string msg, Color font,Color bk)
        {
            canB.Visibility = Visibility.Visible;
            canB.Background = new SolidColorBrush(bk);
            notify.Text = msg;
            notify.Foreground = new SolidColorBrush(font);
            canB.Margin = new Thickness(screenX / 2 - 100, screenY / 2 - 50, 0, 0);
        }
        public static void NotifyStop()
        {
            canB.Visibility = Visibility.Collapsed;
        }
    }
}
