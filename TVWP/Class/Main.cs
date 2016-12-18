using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Globalization;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    enum PageTag : int
    {
        none,main,partial,page_m, nav,search,videopage,player,playerEx,all
    }
    class Main:Component
    {

        #region main
        static Navigation Nav { get; set; }
        static Canvas main_canv { get; set; }

        static Canvas canA;
        //static Border bk_ground;
        public static void Initial(Canvas can)
        {
            main_canv = can;
            Window.Current.SizeChanged += SizeChange;
#if desktop
            Window.Current.CoreWindow.KeyDown += Core_KeyDown;
            Window.Current.CoreWindow.PointerReleased+= (o,e) => {
                if (e.CurrentPoint.Properties.PointerUpdateKind == PointerUpdateKind.XButton2Released)
                    Back(); 
            };
#endif
            Timer.Inital();
            screenX = Window.Current.Bounds.Width;
            screenY = Window.Current.Bounds.Height;
#if phone
            if (screenX > screenY)
                screenX -= 45;
            else screenY -= 23;
#endif
            canA = new Canvas();
            canA.Width = screenX;
            canA.Height = screenY;
            canA.Background = bk_brush;
            main_canv.Children.Add(canA);
            Timer.Delegate(LoadPart0,1);
            PageManageEx.Initial(canA);
            PageManageEx.CreateNewPage(PageTag.main);
            Setting.GetRootFolder(null);
        }
        static void LoadPart0()
        {
            CreateNotify(main_canv);
            //WebClass.SetCookie();
            DownLoad.GetVideoList();
            //Login.Initial(canA);
            //Timer.ChangeDeletgate(LoadItemBuff);
            Timer.Stop();
            //DownLoadEx.InitialA();
            if(Setting.version!=Setting.cur_version)
            {
                Setting.version = Setting.cur_version;
                Tip();
            }
            Buffcomponent();
        }
        static async void Tip()
        {
            MessageDialog md = new MessageDialog("本软件为第三方\r\n"+
                "本次更新: "+
                "https://github.com/huqiang0204/准备停更了，源码在此，有兴趣就拿去改吧");
            md.Title = "Tips";
            await md.ShowAsync();
        }
        static void SizeChange(object sender, WindowSizeChangedEventArgs e)
        {
            screenX = e.Size.Width;
            screenY = e.Size.Height;
            canA.Width = screenX;
            canA.Height = screenY;
#if phone
            if (screenX > screenY)
                screenX -= 45;
            else screenY -= 25;
#endif
            PageManageEx.ReSize(new Thickness(0,0,screenX,screenY));
        }
        static void Core_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.VirtualKey == Windows.System.VirtualKey.Escape)
                Back();
        }
        public static void Back()
        {
            if (PageManageEx.Back())
                return;
            else
            {
                Setting.SaveDispose();
                DownloadPage.SaveMission(()=> {
                    Application.Current.Exit();
                });
            }
        }
        #endregion

        #region part
        struct Message
        {
            public Canvas can;
            public TextBlock notify;
            public DispatcherTimer dt;
        }
        static Message msg;
        static void CreateNotify(Canvas p)
        {
            Canvas can = new Canvas();
            p.Children.Add(can);
            can.Width = 200;
            can.Height = 200;
            can.Visibility = Visibility.Collapsed;
            can.PointerPressed += (o, e) => { msg.can.Visibility = Visibility.Collapsed; };
            msg.can = can;
            TextBlock tb = new TextBlock();
            can.Children.Add(tb);
            tb.Width = 200;
            tb.TextAlignment = TextAlignment.Center;
            tb.TextWrapping = TextWrapping.Wrap;
            msg.notify = tb;
            DispatcherTimer dt = new DispatcherTimer();
            msg.dt = dt;
        }
        static void Tick(object sender, object e)
        {
            msg.can.Visibility = Visibility.Collapsed;
            msg.dt.Stop();
        }
        public static void Notify(string str)
        {
            Notify(str, font_brush,bk_brush, 2000);
        }
        public static void Notify(string str,int time)
        {
            Notify(str, font_brush, bk_brush, time);
        }
        public static void Notify(string msg,Brush font)
        {
            Notify(msg,font, half_t_brush,2000);
        }
        public static void Notify(string str, Brush font,Brush bk,int time)
        {
            msg.can.Visibility = Visibility.Visible;
            msg.can.Background = bk;
            msg.notify.Text = str;
            msg.notify.Foreground = font;
#if desktop
            msg.can.Margin = new Thickness(screenX / 2 - 100,screenY/2-50,0,0);
#else
            msg.notify.Width = screenX;
            msg.can.Width = screenX;
            msg.can.Margin = new Thickness(0, 0, 0, 0);
#endif
            if(time>0)
            {
                msg.dt.Interval = new TimeSpan(0, 0, 0, 0, time);
                msg.dt.Start();
            }
        }
        public static void NotifyStop()
        {
            msg.can.Visibility = Visibility.Collapsed;
        }
#endregion
    }

    class PageManageEx
    {
        static Navigation[] nav_buff;
        static Navigation[] ln;
        static Navigation current;
        static int point;
        static Canvas parent;
        static void CreateNewPageA(PageTag tag)
        {
            switch (tag)
            {
                case PageTag.main:
                    nav_buff[(int)PageTag.main] = new MainEx();
                    break;
                case PageTag.partial:
                    nav_buff[(int)PageTag.partial] = new PartialNav();
                    break;
                case PageTag.page_m:
                    nav_buff[(int)PageTag.page_m] = new PageNav_m();
                    break;
                case PageTag.nav:

                    break;
                case PageTag.search:

                    break;
                case PageTag.videopage:
                    nav_buff[(int)PageTag.videopage] = new VideoPage();
                    break;
                case PageTag.player:
                    nav_buff[(int)PageTag.player] = new Player();
                    break;
                case PageTag.playerEx:
                    nav_buff[(int)PageTag.playerEx] = new PlayerEx();
                    break;
            }
        }

        public static void Initial(Canvas p)
        {
            nav_buff = new Navigation[(int)PageTag.all];
            ln = new Navigation[8];
            parent = p;
            point = -1;
        }
        public static void CreateNewPage(PageTag tag)
        {
            point++;
            if (current != null)
                current.Hide();
            int index = (int)tag;
            if (nav_buff[index] == null)
                CreateNewPageA(tag);
            current = nav_buff[index];
            ln[point] = current;
            current.Create(parent, new Thickness(0, 0, Component.screenX, Component.screenY));
        }
        public static bool Back()
        {
            if (current.Back())
                return true;
            else
            {
                point--;
                if(point<0)
                    return false;
                current = ln[point];
                current.Show();
                return true;
            }
        }
        public static void ReSize(Thickness m)
        {
            current.ReSize(m);
        }
    }

    class MainEx : Component,Navigation
    {
        #region main
        static string[] s_main = { "首页", "Main" };
        static string[] s_nav = { "分类页", "Class" };
        static string[] s_search = { "搜索", "Search" };
        static string[] s_page = { "频道", "Channel" };
        public static string[][] s_all = { s_main, s_nav, s_search, s_page };
        #endregion

        #region part
        static PivotPage pp;
        static Canvas parent;
        static int index;
        static void ChangePage()
        {
            switch (pp.index)
            {
                case 0:
                    HomePage_m.Hide();
                    break;
                case 1:
                    NavPage.Hide();
                    break;
                case 2:
                    SearchResult_m.Hide();
                    break;
                case 3:
                    //PartialPage.hi;
                    break;
            }
            pp.index = pp.pivot.SelectedIndex;
            switch (pp.index)
            {
                case 0:
                    HomePage_m.Create(pp.son[0], pp.item_margin);
                    break;
                case 1:
                    NavPage.Create(pp.son[1], pp.item_margin);
                    break;
                case 2:
                    SearchResult_m.Create(pp.son[2], pp.item_margin);
                    break;
                case 3:
                    PartialPage.Create(pp.son[3], pp.item_margin);
                    break;
            }
        }
        static void MainPageResize(Thickness tk)
        {
            pp.pivot.Margin = tk;
            ResizePivot(ref pp, tk);
            pp.pivot.Width = tk.Right-tk.Left;
            tk.Right -= tk.Left;
            tk.Top = 0;
            tk.Left = 0;
            tk.Bottom -= 40;
            pp.item_margin = tk;
            switch (pp.index)
            {
                case 0:
                    HomePage_m.ReSize(tk);
                    break;
                case 1:
                    NavPage.ReSize(tk);
                    break;
                case 2:
                    SearchResult_m.ReSize(tk);
                    break;
                case 3:
                    PartialPage.Resize(tk);
                    break;
                case 4:
                    SettingPage.ReSize(tk);
                    break;
            }
        }
        static void CreateMainPage(Canvas p,Thickness tk)
        {
            if (pp.pivot != null)
            {
                pp.pivot.Visibility = Visibility.Visible;
                MainPageResize(tk);
                return;
            }
            CreatePivot(ref pp, ref s_all);
            ResizePivot(ref pp, tk);
            p.Children.Add(pp.pivot);
            
            pp.pivot.Margin = tk;
            pp.pivot.Width = tk.Right-tk.Left;
            tk.Right -= tk.Left;
            tk.Top = 0;
            tk.Left = 0;
            tk.Bottom -= 40;
            pp.item_margin = tk;
            index = 1;
            pp.pivot.SelectionChanged += (o, e) => {
                pp.head[index].Background = trans_brush;
                index = pp.pivot.SelectedIndex;
                pp.head[index].Background = tag_brush_b;
            };
            pp.pivot.SelectionChanged += (o, e) => { ChangePage(); };
            pp.pivot.SelectedIndex = Setting.startindex;
        }
        public static void LockPivot()
        {
            pp.pivot.IsLocked = true;
        }
        public static void UnLockPivot()
        {
            pp.pivot.IsLocked = false;
        }

        public static void LanguageChange()
        {
            int c = pp.head.Length;
            for (int i = 0; i < c; i++)
                pp.head[i].Content = s_all[i][language];
        }
        public void Create(Canvas p, Thickness m)
        {
            parent = p;
            if (screenX > screenY)
                m.Left += 40;
            else m.Bottom -= 40;
            switch (current)
            {
                case 0:
                    CreateMainPage(p,m);
                    break;
                case 1:
                    SettingPage.Create(parent, m);
                    break;
                case 2:
                    Abo.Create(parent, m);
                    break;
                case 3:
                    DownloadPage.Create(parent, m);
                    break;
            }
            CreateBar();
        }
        public bool Back()
        {
            if (pp.index == 0)
                if (HomePage_m.Back())
                    return true;
            return false;
        }
        public void Hide()
        {
            switch (current)
            {
                case 0:
                pp.pivot.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    SettingPage.Dispose();
                    break;
                case 2:
                    Abo.Dispose();
                    break;
                case 3:
                    DownloadPage.Dispose();
                    break;
            }
            HideBar();
        }
        public void Show()
        {
            Thickness m = new Thickness(0, 0, screenX, screenY);
            if (screenX > screenY)
                m.Left += 40;
            else m.Bottom -= 40;
            switch (current)
            {
                case 0:
                    pp.pivot.Visibility = Visibility.Visible;
                    break;
                case 1:
                    SettingPage.Create(parent, m);
                    break;
                case 2:
                    Abo.Create(parent, m);
                    break;
                case 3:
                    DownloadPage.Create(parent, m);
                    break;
            }
            ShowBar();
        }
        public void ReSize(Thickness m)
        {
            BarResize();
            if (screenX > screenY)
                m.Left += 40;
            else m.Bottom -= 40;
            MainPageResize(m);
        }
        #endregion

        #region bar
        struct MenuBar
        {
            public Border bk;
            public AppBarButton home;
            public AppBarButton buff;
            public AppBarButton setting;
            public AppBarButton about;
        }
        static MenuBar menu;
        static void CreateBar()
        {
            if(menu.bk!=null)
            {
                BarResize();
                ShowBar();
                return;
            }
            current = 0;
            menu.bk = new Border();
            menu.bk.Background = bk_brush;
            AppBarButton si = new AppBarButton();
            si.Icon =new SymbolIcon(Symbol.Home);
            si.Click += (o, e) => { ChangeCurrent(0); };
            si.Foreground = filter_brush;
            si.Width = 36;
            si.Height = 36;
            menu.home = si;
            si = new AppBarButton();
            si.Icon = new SymbolIcon(Symbol.Setting);
            si.Click += (o, e) => { ChangeCurrent(1); };
            si.Foreground = filter_brush;
            si.Width = 36;
            si.Height = 36;
            menu.buff = si;
            si = new AppBarButton();
            si.Icon =new SymbolIcon( Symbol.Comment);
            si.Click += (o, e) => { ChangeCurrent(2); };
            si.Foreground = filter_brush;
            si.Width = 36;
            si.Height = 36;
            menu.setting = si;
            si = new AppBarButton();
            si.Icon = new SymbolIcon(Symbol.Download);
            si.Click += (o, e) => { ChangeCurrent(3); };
            si.Foreground = filter_brush;
            si.Width = 36;
            si.Height = 36;
            menu.about = si;
            parent.Children.Add(menu.bk);
            parent.Children.Add(menu.home);
            parent.Children.Add(menu.buff);
            parent.Children.Add(menu.setting);
            parent.Children.Add(menu.about);
            BarResize();
        }
        static void BarResize()
        {
            if(screenX>screenY)
            {
                menu.bk.Height = screenY;
                menu.bk.Width = 36;
                menu.bk.Margin= menu.home.Margin = new Thickness(0,0,0,0);
                menu.buff.Margin = new Thickness(0,40,0,0);
                menu.setting.Margin = new Thickness(0,80,0,0);
                menu.about.Margin = new Thickness(0, 120, 0, 0);
            }
            else
            {
                menu.bk.Width = screenX;
                menu.bk.Height = 36;
                double dy = screenY - 36;
                menu.bk.Margin = new Thickness(0, dy, 0, 0);
                dy -= 4;
                menu.home.Margin = new Thickness(0,dy,0,0);
                menu.buff.Margin = new Thickness(40, dy, 0, 0);
                menu.setting.Margin = new Thickness(80, dy, 0, 0);
                menu.about.Margin = new Thickness(120, dy, 0, 0);
            }
            Thickness m = new Thickness(0,0,screenX,screenY);
            if (screenX > screenY)
                m.Left += 40;
            else m.Bottom -= 40;
            switch (current)
            {
                case 1:
                    SettingPage.ReSize( m);
                    break;
                case 2:
                    Abo.ReSize(m);
                    break;
                case 3:
                    DownloadPage.ReSize(m);
                    break;
            }
        }
        static void HideBar()
        {
            menu.bk.Visibility = Visibility.Collapsed;
            menu.home.Visibility = Visibility.Collapsed;
            menu.buff.Visibility = Visibility.Collapsed;
            menu.setting.Visibility = Visibility.Collapsed;
            menu.about.Visibility = Visibility.Collapsed;
        }
        static void ShowBar()
        {
            menu.bk.Visibility = Visibility.Visible;
            menu.home.Visibility = Visibility.Visible;
            menu.buff.Visibility = Visibility.Visible;
            menu.setting.Visibility = Visibility.Visible;
            menu.about.Visibility = Visibility.Visible;
        }
        static int current;
        static void ChangeCurrent(int index)
        {
            if (current == index)
                return;
            switch (current)
            {
                case 0:
                    pp.pivot.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    SettingPage.Dispose();
                    break;
                case 2:
                    Abo.Dispose();
                    break;
                case 3:
                    DownloadPage.Dispose();
                    break;
            }
            Thickness m = new Thickness(0,0,screenX,screenY);
            if (screenX > screenY)
                m.Left += 40;
            else m.Bottom -= 40;
            switch (index)
            {
                case 0:
                    pp.pivot.Visibility = Visibility.Visible;
                    break;
                case 1:
                    SettingPage.Create(parent,m);
                    break;
                case 2:
                    Abo.Create(parent,m);
                    break;
                case 3:
                    DownloadPage.Create(parent,m);
                    break;
            }
            current = index;
        }
        #endregion
    }
}
