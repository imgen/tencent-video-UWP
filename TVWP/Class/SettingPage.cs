using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace TVWP.Class
{
    class Abo : Component
    {
        static Canvas parent;
        static TextBlock tb;
        public static void Create(Canvas p, Thickness m)
        {
            if (tb != null)
            {
                tb.Visibility = Visibility.Visible;
                tb.Width = m.Right - m.Left;
                return;
            }
            parent = p;
            tb = CreateTextBlockNext();
            p.Children.Add(tb);
            tb.Foreground = font_brush;
            tb.TextWrapping = TextWrapping.Wrap;
            //tb.TextAlignment = TextAlignment.Center;
            tb.Margin = m;
            tb.Width = m.Right - m.Left;
            tb.Height = m.Bottom - m.Top;
            tb.Text = "作者述：\r\n" + "本软件为第三方，功能有限（个人技术不够吧）.\r\n" +
                "1.无法解开ckey（防盗链密钥），因此缓存容易被封ip资源，缓存模块快写完才发现。" +
                "然后尝试每5分钟缓存一个分段，可以避开封禁，但是uwp后台不允许创建任务。前台创建任务后，"
                + "后台会同时下载多个分段，ip会立马被封（只能播放当前下载视频的前5分钟）。使用迅雷下载也一样\r\n" +
                "2.评论功能没有找到解决方案。\r\n" +
                "3.从form编程模式转到xaml，xaml网页式布局降低了开发难度（我并不喜欢这样，代码自由度太低，资源重复利用率低）。"
                + "xaml不支持多线程很是纠结(需要的不是async await，而是像from中每个线程都有自己的ui和消息队列）\r\n" +
                "4.没有使用任何外部组件（小巧），因此解析数据时程序容易崩溃（只收录了调试时遇到的状况）\r\n" +
                "感谢大家的支持，功能残缺实在惭愧，多次考虑是否下架的，能改的尽量改吧。"+
                "https://github.com/huqiang0204/准备停更了，源码在此，有兴趣就拿去改吧";

        }
        public static void ReSize(Thickness m)
        {
            tb.Margin = m;
            tb.Width = m.Right - m.Left;
            tb.Height = m.Bottom - m.Top;
        }
        public static void Dispose()
        {
            parent.Children.Remove(tb);
            RecycleTextBlock(tb);
            tb = null;
        }
    }
    class Setting
    {
        public const int cur_version = 4;
        enum ColorBrush : int
        {
            Background, Border, Font, Filter, Navigation, Title, All
        }
        struct Setup
        {
            public Color[] colors;//10*4= 40 byte
            public List<History> history;//100 buff
        }
        public static Color bk_color { get; set; }
        public static Color font_color { get; set; }
        public static Color title_color { get; set; }
        public static Color filter_color { get; set; }
        public static Color nav_color { get; set; }
        public static Color bor_color { get; set; }
        public static string token { get; set; }
        public static Color[] colors { get; set; }
        public static int sharp { get; set; }
        public static int buffsharp { get; set; }
        public static int language { get; set; }
        public static int startindex { get; set; }
        public static int version { get; set; }
        public static int buffinterval { get; set; }
        public static int start_app_buff { get; set; }
        static Setup setup;
        public static void AddHistory(History his)
        {
            if (setup.history == null)
                setup.history = new List<History>();
            setup.history.Insert(0, his);
            if (setup.history.Count > 100)
                setup.history.RemoveAt(100);
        }
        static void DeploySetup()
        {
            bk_color = setup.colors[(int)ColorBrush.Background];
            bor_color = setup.colors[(int)ColorBrush.Border];
            font_color = setup.colors[(int)ColorBrush.Font];
            title_color = setup.colors[(int)ColorBrush.Title];
            filter_color = setup.colors[(int)ColorBrush.Filter];
            nav_color = setup.colors[(int)ColorBrush.Navigation];
        }
        static void CollectSetup()
        {
            setup.colors[(int)ColorBrush.Background] = bk_color;
            setup.colors[(int)ColorBrush.Border] = bor_color;
            setup.colors[(int)ColorBrush.Font] = font_color;
            setup.colors[(int)ColorBrush.Title] = title_color;
            setup.colors[(int)ColorBrush.Filter] = filter_color;
            setup.colors[(int)ColorBrush.Navigation] = nav_color;
        }
        public static void DefaltSet()
        {
            setup.colors = new Color[(int)ColorBrush.All];
            bk_color = Color.FromArgb(255, 64, 64, 64);
            bor_color = Colors.Azure;
            font_color = Colors.White;
            title_color = Colors.LightBlue;
            filter_color = Colors.LightPink;
            nav_color = Colors.LightGreen;
            sharp = 720;
            buffsharp = 720;
            startindex = 1;
            if (KnownFolders.VideosLibrary.DisplayName != "视频")
                language = 1;
        }
        public static void LoadDispose()
        {
            setup = new Setup();
            byte[] buff = FileManage.LoadFile("vs");
            if (buff == null)
                DefaltSet();
            else
            {
                ReadDispose(buff);
                DeploySetup();
            }
        }
        public static void SaveDispose()
        {
            CollectSetup();
            byte[] buff;
            WriteHistory(out buff);
            WriteDispose(ref buff);
            FileManage.SaveFile(buff,"vs");
        }
        static void ReadDispose(byte[] buff)
        {
            int a = (int)ColorBrush.All;
            Color[] co = new Color[a];
            int index = 0;
            for (int i = 0; i < a; i++)
            {
                co[i].A = buff[index];
                index++;
                co[i].R = buff[index];
                index++;
                co[i].G = buff[index];
                index++;
                co[i].B = buff[index];
                index++;
            }
            setup.colors = co;
            unsafe
            {
                fixed (byte* pb = &buff[40])
                {
                    int* ip = (int*)pb;
                    language = *ip;
                    ip++;
                    sharp = *ip;
                    ip++;
                    buffsharp = *ip;
                    ip++;
                    startindex = *ip;
                    ip++;
                    version = *ip;
                    ip++;
                    buffinterval = *ip;
                    ip++;
                    start_app_buff = *ip;
                    ip = (int*)pb;
                    ip += 22;
                    int l = *ip;
                    if (l > 0)
                    {
                        ip++;
                        token = new string(CopyChar((char*)ip, l));
                    }
                }
            }
            if (buff.Length > 256)
                ReadHistory(ref buff);
        }
        static void ReadHistory(ref byte[] buff)
        {
            unsafe
            {
                fixed (byte* pb = &buff[252])
                {
                    int* ip = (int*)pb;
                    int c = *ip;
                    if (c == 0)
                        return;
                    setup.history = new List<History>();
                    History his = new History();
                    ip++;
                    char* cp = (char*)ip;
                    for (int i = 0; i < c; i++)
                    {
                        int l = *ip;
                        cp += 2;
                        his.name = new string(CopyChar(cp, l));
                        cp += l;
                        ip = (int*)cp;

                        l = *ip;
                        cp += 2;
                        his.vid = new string(CopyChar(cp, l));
                        cp += l;
                        ip = (int*)cp;

                        l = *ip;
                        cp += 2;
                        his.href = new string(CopyChar(cp, l));
                        cp += l;
                        ip = (int*)cp;
                        setup.history.Add(his);
                    }
                }
            }
        }
        unsafe static char[] CopyChar(char* cp, int count)
        {
            char[] c = new char[count];
            for (int i = 0; i < count; i++)
            {
                c[i] = *cp;
                cp++;
            }
            return c;
        }
        static void WriteDispose(ref byte[] buff)
        {
            Color[] co = setup.colors;
            int index = 0;
            int a = setup.colors.Length;
            for (int i = 0; i < a; i++)
            {
                buff[index] = co[i].A;
                index++;
                buff[index] = co[i].R;
                index++;
                buff[index] = co[i].G;
                index++;
                buff[index] = co[i].B;
                index++;
            }
            unsafe
            {
                fixed (byte* pb = &buff[40])
                {
                    int* ip = (int*)pb;
                    *ip = language;
                    ip++;
                    *ip = sharp;
                    ip++;
                    *ip = buffsharp;
                    ip++;
                    *ip = startindex;
                    ip++;
                    *ip = version;
                    ip++;
                    *ip = buffinterval;
                    ip++;
                    *ip = start_app_buff;
                    if (token != null)
                    {
                        ip = (int*)pb;
                        ip += 22;
                        int l = token.Length;
                        *ip = l;
                        ip++;
                        char* cp = (char*)ip;
                        char[] t = token.ToCharArray();
                        for (int i = 0; i < l; i++)
                        {
                            *cp = t[i];
                            cp++;
                        }
                    }
                }
            }
        }
        static void WriteHistory(out byte[] buff)
        {
            if (setup.history == null)
            {
                buff = new byte[252]; return;
            }
            int c = setup.history.Count;
            if (c == 0)
            { buff = new byte[252]; return; }
            int len = c * 16 + 256;
            for (int i = 0; i < c; i++)
            {
                len += setup.history[i].name.Length;
                len += setup.history[i].vid.Length;
                len += setup.history[i].href.Length;
            }
            buff = new byte[len];
            unsafe
            {
                fixed (byte* pb = &buff[252])
                {
                    int* ip = (int*)pb;
                    *ip = c;
                    ip++;
                    char* cp = (char*)ip;
                    for (int i = 0; i < c; i++)
                    {
                        History his = setup.history[i];
                        cp += 4;
                        *ip = his.time;
                        int l = his.name.Length;
                        ip++;
                        *ip = l;
                        char[] cr = his.name.ToCharArray();
                        for (int t = 0; t < l; t++)
                        {
                            *cp = cr[t];
                            cp++;
                        }

                        l = his.vid.Length;
                        ip = (int*)cp;
                        *ip = l;
                        cr = his.vid.ToCharArray();
                        cp += 2;
                        for (int t = 0; t < l; t++)
                        {
                            *cp = cr[t];
                            cp++;
                        }

                        l = his.href.Length;
                        ip = (int*)cp;
                        *ip = l;
                        cr = his.href.ToCharArray();
                        cp += 2;
                        for (int t = 0; t < l; t++)
                        {
                            *cp = cr[t];
                            cp++;
                        }
                        ip = (int*)cp;
                    }
                }
            }
        }
        static StorageFolder rsf;//root
        public static async void GetRootFolder(Action<StorageFolder> act)
        {
            if(rsf!=null)
                if (act != null)
                    act(rsf);
            if (Setting.token != null)
                rsf = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(Setting.token);
            else
            {
                rsf = KnownFolders.VideosLibrary;
                rsf = await rsf.CreateFolderAsync("TencentVideo", CreationCollisionOption.OpenIfExists);
            }
            if(act!=null)
              act(rsf);
        }
    }
    class ColorManage:Component
    {
        Canvas parent;
        Canvas can;
        ColorTemplate ct;
        struct option
        {
            public TextBlock declare;
            public Button color;
        }
        static string[] bk_c = {"背景","background" };
        static string[] font_c = {"字体","font" };
        static string[] title_c = {"标题","title" };
        static string[] nav_c = { "导航","navgation"};
        static string[] filter_c = {"筛选","filter" };
        static string[] bor_c = { "边框", "Border" };
        static string[][] all_c = {bk_c, font_c,title_c,nav_c,filter_c, bor_c };
        option[] op;
        int index;
        Thickness margin;
        public void Create(Canvas p,Thickness m)
        {
            if(can!=null)
            {
                can.Visibility = Visibility.Visible;
                ReSize(m);
                return;
            }
            can = new Canvas();
            ct = new ColorTemplate();
            ct.Seletced = SetColor;
            p.Children.Add(can);
            parent = p;
            int c = all_c.Length;
            op = new option[c];
            double y = 10;
            ItemClick ic = new ItemClick();
            ic.click = (o) => {
                index = (int)o;
#if desktop
                ct.Show(parent, new Thickness(128, 0, 0, 0));
#else
                    Thickness tk = new Thickness();
                    tk.Left = 0;
                    tk.Top += 30+index*30;
                    ct.Show(parent, tk);
#endif
            };
            double dx = m.Left+10;
            double dxr = dx + 80;
            for (int i = 0; i < c; i++)
            {
                TextBlock tb = CreateTextBlockNext();
                tb.Margin = new Thickness(dx, y, 0, 0);
                op[i].declare = tb;
                tb.Foreground = font_brush;
                tb.Text= all_c[i][language];
                Button but = CreateButtonNext();
                op[i].color = but;
                but.Width = 20;
                but.Height = 20;
                ic.tag = i;
                but.DataContext = ic;
                but.Margin = new Thickness(dxr,y,0,0);
                y += 30;
                can.Children.Add(tb);
                can.Children.Add(but);
            }
            can.Height = y;
            ReSize(m);
            op[0].color.Background = bk_brush;
            op[1].color.Background = font_brush;
            op[2].color.Background = title_brush;
            op[3].color.Background = nav_brush;
            op[4].color.Background = filter_brush;
            op[5].color.Background = bor_brush;
        }
        void SetColor(Color c)
        {
            switch(index)
            {
                case 0:
                    bk_brush.Color = c;
                    Setting.bk_color = c;
                    break;
                case 1:
                    font_brush.Color = c;
                    Setting.font_color = c;
                    break;
                case 2:
                    title_brush.Color=c;
                    Setting.title_color = c;
                    break;
                case 3:
                    nav_brush.Color=c;
                    Setting.nav_color = c;
                    break;
                case 4:
                    filter_brush .Color=c;
                    Setting.filter_color = c;
                    break;
                case 5:
                    bor_brush.Color = c;
                    Setting.bor_color = c;
                    break;
            }
        }
        public void ReSize(Thickness m)
        {
            if (margin == m)
                return;
            margin = m;
            double w = m.Right - m.Left;
            double h = margin.Bottom - margin.Top;
            can.Width = w;
            if (can.Height < h)
                can.Height = h;
        }
        public void ChangeLan()
        {
            int c = op.Length;
            for (int i = 0; i < c; i++)
                op[i].declare.Text = all_c[i][language];
            if (ct != null)
                ct.ChangeLan();
        }
        public void Dispose()
        {
            ct.Dispose();
            can.Children.Clear();
            int c = op.Length;
            for(int i=0;i<c;i++)
            {
                RecycleButton(op[i].color);
                RecycleTextBlock(op[i].declare);
            }
            parent.Children.Remove(can);
            GC.SuppressFinalize(can);
            GC.SuppressFinalize(this);
        }
    }
    class ColorTemplate
    {
        Canvas can;
        Image img;
        Rectangle rta;
        BitmapImage bi,biA;
        InMemoryRandomAccessStream memoryStream;
        DataWriter datawriter;
        InMemoryRandomAccessStream memoryStreamA;
        DataWriter datawriterA;
        Slider SA, SR, SG, SB;
        TextBlock TA, TR, TG, TB;
        Border bor;
        byte CA, CR, CG, CB;
        Button ok;
        Button cancel;
        public Action<Color> Seletced;
        static string[] cancel_s = {"取消","Cancel" };
        static string[] ok_s = { "确定","OK"};
        public void Show(Canvas p,Thickness m)
        {
            if(can!=null)
            {
                can.Visibility = Visibility.Visible;
                can.Margin = m;
                return;
            }
            can = new Canvas();
            can.Background = new SolidColorBrush(Colors.White);
            can.Margin = m;
            can.Width = 310;
            can.Height = 420;
            p.Children.Add(can);
            CreateMod(can);
            CreateModA(can);
            SA = new Slider();
            SA.Foreground = new SolidColorBrush(Colors.Black);
            SA.Height = 20;
            SA.Width = 256;
            SA.Maximum = 255;
            SA.Value = 255;
            SA.ValueChanged += (o, e) => {
                CA = (byte)SA.Value;
                TA.Text = "A:" + CA.ToString();
                bor.Background = new SolidColorBrush(Color.FromArgb(CA,CR,CG,CB)); };
            SR = new Slider();
            SR.Foreground = new SolidColorBrush(Colors.Red);
            SR.Height = 20;
            SR.Width = 256;
            SR.Maximum = 255;
            SR.Value = 255;
            SR.ValueChanged += (o, e) => {
                CR = (byte)SR.Value;
                TR.Text = "R:" + CR.ToString();
                bor.Background = new SolidColorBrush(Color.FromArgb(CA, CR, CG, CB));
            };
            SG = new Slider();
            SG.Foreground = new SolidColorBrush(Color.FromArgb(255,0,255,0));
            SG.Height = 20;
            SG.Width = 256;
            SG.Maximum = 255;
            SG.ValueChanged += (o, e) => {
                CG = (byte)SG.Value;
                TG.Text = "G:" + CG.ToString();
                bor.Background = new SolidColorBrush(Color.FromArgb(CA, CR, CG, CB));
            };
            SB = new Slider();
            SB.Foreground = new SolidColorBrush(Colors.Blue);
            SB.Height = 20;
            SB.Width = 256;
            SB.Maximum = 255;
            SB.ValueChanged += (o, e) => {
                CB = (byte)SB.Value;
                TB.Text = "B:" + CB.ToString();
                bor.Background = new SolidColorBrush(Color.FromArgb(CA, CR, CG, CB));
            };
            can.Children.Add(SA);
            can.Children.Add(SR);
            can.Children.Add(SG);
            can.Children.Add(SB);
            TA = new TextBlock();
            TA.Text = "A:255";
            TR = new TextBlock();
            TR.Text = "R:255";
            TR.Foreground = new SolidColorBrush(Color.FromArgb(255,255,0,0));
            TG = new TextBlock();
            TG.Text = "G:0";
            TG.Foreground = new SolidColorBrush(Color.FromArgb(255,0,255,0));
            TB = new TextBlock();
            TB.Text = "B:0";
            TB.Foreground = new SolidColorBrush(Color.FromArgb(255,0,0,255));
            can.Children.Add(TA);
            can.Children.Add(TR);
            can.Children.Add(TG);
            can.Children.Add(TB);
            bor = new Border();
            bor.Width = 48;
            bor.Height = 48;
            bor.Background= new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            can.Children.Add(bor);
            CA = 255;
            CR = 255;
            CG = 0;
            CB = 0;
            cancel = new Button();
            cancel.Content = cancel_s[Component.language];
            cancel.Click += (o, e) => {
                can.Visibility = Visibility.Collapsed;
            };
            can.Children.Add(cancel);
            ok = new Button();
            ok.Content = ok_s[Component.language];
            ok.Click+=(o,e)=> {
                if (Seletced != null)
                    Seletced(Color.FromArgb(CA,CR,CG,CB));
                can.Visibility = Visibility.Collapsed;
            };
            can.Children.Add(ok);
            Order();
        }
        async void CreateMod(Canvas p)
        {
            bi = new BitmapImage();
            memoryStream = new InMemoryRandomAccessStream();
            datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            CreateBitmap();
            HSVTemplate(0, ref map);
            datawriter.WriteBytes(map);
            await datawriter.StoreAsync();
            bi.SetSource(memoryStream);
            img = new Image();
            img.Source = bi;
            img.Width = 256;
            img.Height = 256;
            img.PointerReleased += (o, e) => {
                PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                pp = e.GetCurrentPoint(o as UIElement);
                int x = (int)pp.RawPosition.X;
                int y =255-(int)pp.RawPosition.Y;
                int index = y * 768 + x*3+54;
                CB = map[index];
                index++;
                CG = map[index];
                index++;
                CR = map[index];
                SR.Value = CR;
                SG.Value = CG;
                SB.Value = CB;
            };
            p.Children.Add(img);
            datawriter.Dispose();
        }
        async void CreateModA(Canvas p)
        {
            biA = new BitmapImage();
            memoryStreamA = new InMemoryRandomAccessStream();
            datawriterA = new DataWriter(memoryStreamA.GetOutputStreamAt(0));
            CreateHTemp();
            HTemplate(ref temp);
            datawriterA.WriteBytes(temp);
            await datawriterA.StoreAsync();
            biA.SetSource(memoryStreamA);
            rta = new Rectangle();
            ImageBrush ib = new ImageBrush();
            rta.Fill = ib;
            rta.Width = 20;
            rta.Height = 256;
            rta.PointerReleased += (o, e)=>{
                PointerPoint pp= PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                pp= e.GetCurrentPoint(o as UIElement);
                float y = (float)pp.RawPosition.Y;
                y = (256 - y)* 1.40625f;
                ChangeTemplate(y);
            };
            rta.PointerMoved += (o, e) => {
                PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                if(pp.IsInContact)
                {
                    pp = e.GetCurrentPoint(o as UIElement);
                    float y = (float)pp.RawPosition.Y;
                    y = (256 - y) * 1.40625f;
                    ChangeTemplate(y);
                }
            };
            ib.ImageSource = biA;
            p.Children.Add(rta);
            datawriterA.Dispose();
        }
        static byte[] maphead = new byte[] { 0x42, 0x4D, 0x36, 0, 3, 0, 0, 0, 0, 0, 0x36, 0, 0, 0, 0x28, 0,
            0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0x18, 0, 0, 0,
            0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0 };//+256*256*3 RGBA
        byte[] map;
        void CreateBitmap()
        {
            map = new byte[196662];
            for (int i = 0; i < 54; i++)
                map[i] = maphead[i];
        }
        static void HSVTemplate(float H, ref byte[] buff)
        {
            //54-196662
            // 1/255=0.0039216f
            int index = 54;
            float S = 0;
            float V = 0;
            float R, G, B;
            H /= 60;
            int i = (int)H;
            float f = H - i;
            for (int d = 0; d < 256; d++)
            {
                for (int t = 0; t < 256; t++)
                {
                    if (S == 0)
                    { R = G = B = V; goto label1; }
                    float a = V * (1 - S);
                    float b = V * (1 - S * f);
                    float c = V * (1 - S * (1 - f));
                    switch (i)
                    {
                        case 0: R = V; G = c; B = a; break;
                        case 1: R = b; G = V; B = a; break;
                        case 2: R = a; G = V; B = c; break;
                        case 3: R = a; G = b; B = V; break;
                        case 4: R = c; G = a; B = V; break;
                        default: R = V; G = a; B = b; break;//5
                    }
                    label1:;
                    S += 0.0039216f;
                    buff[index] = (byte)(B * 255);
                    index++;
                    buff[index] = (byte)(G * 255);
                    index++;
                    buff[index] = (byte)(R * 255);
                    index++;
                }
                S = 0;
                V += 0.0039216f;
            }
        }
        async void ChangeTemplate(float H)
        {
            HSVTemplate(H, ref map);
            memoryStream.Seek(0);
            datawriter = new DataWriter(memoryStream.GetOutputStreamAt(0));
            datawriter.WriteBytes(map);
            await datawriter.StoreAsync();
            bi.SetSource(memoryStream);
            await datawriter.FlushAsync();
        }
        static byte[] Hhead = new byte[] { 0x42, 0x4D, 0xD8, 5, 0, 0, 0, 0, 0, 0, 0x36, 0, 0, 0, 0x28, 0,
            0, 0, 1, 0, 0, 0, 0x68, 1, 0, 0, 1, 0, 0x18, 0, 0, 0,
            0, 0, 0xa2, 5, 0, 0, 0x12, 0xb, 0, 0, 0x12, 0xb, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0 };//+1*360*4 RGBA 5D8
        byte[] temp;
        static void HTemplate(ref byte[] buff)
        {
            //54-196662
            // 1/255=0.0039216f
            float H = 0;
            int index = 54;
            float S = 1;
            float V = 1;
            float R, G, B;
            float a = 0;
            for (int t = 0; t < 360; t++)
            {
                H = t;
                H /= 60;
                int i = (int)H;
                float f = H - i;
                float b = 1 - S * f;
                float c = 1 - S * (1 - f);
                switch (i)
                {
                    case 0: R = V; G = c; B = a; break;
                    case 1: R = b; G = V; B = a; break;
                    case 2: R = a; G = V; B = c; break;
                    case 3: R = a; G = b; B = V; break;
                    case 4: R = c; G = a; B = V; break;
                    default: R = V; G = a; B = b; break;//5
                }
                buff[index] = (byte)(B * 255);
                index++;
                buff[index] = (byte)(G * 255);
                index++;
                buff[index] = (byte)(R * 255);
                index+=2;
            }
        }
        void CreateHTemp()
        {
            temp = new byte[1496];
            for (int i = 0; i < 54; i++)
                temp[i] = Hhead[i];
        }
        public void Dispose()
        {
            if (can == null)
            {
                GC.SuppressFinalize(this);
                return;
            }
            can.Children.Clear();
            GC.SuppressFinalize(can);
            GC.SuppressFinalize(img);
            GC.SuppressFinalize(rta);
            GC.SuppressFinalize(SA);
            GC.SuppressFinalize(SR);
            GC.SuppressFinalize(SG);
            GC.SuppressFinalize(SB);
            GC.SuppressFinalize(TA);
            GC.SuppressFinalize(TR);
            GC.SuppressFinalize(TG);
            GC.SuppressFinalize(TB);
            GC.SuppressFinalize(cancel);
            GC.SuppressFinalize(ok);
            GC.SuppressFinalize(bor);
            memoryStream.Dispose();
            memoryStreamA.Dispose();
            GC.SuppressFinalize(this);
        }
        void Order()
        {
            rta.Margin = new Thickness(266,0,0,0);
            SA.Margin = new Thickness(0,266,0,0);
            SR.Margin = new Thickness(0, 286, 0, 0);
            SG.Margin = new Thickness(0, 306, 0, 0);
            SB.Margin = new Thickness(0, 326, 0, 0);
            TA.Margin = new Thickness(266,266,0,0);
            TR.Margin = new Thickness(266,286,0,0);
            TG.Margin = new Thickness(266,306,0,0);
            TB.Margin = new Thickness(266,326,0,0);
            bor.Margin = new Thickness(100,356,0,0);
            cancel.Margin = new Thickness(188,356,0,0);
            ok.Margin = new Thickness(256,356,0,0);
        }
        public void ChangeLan()
        {
            if (ok == null)
                return;
            ok.Content = ok_s[Component.language];
            cancel.Content = cancel_s[Component.language];
        }
    }
    class SettingPage : Component
    {
        static Canvas parent;
        static ColorManage cm;
        static ToggleSwitch ts;
        static TextBlock sharp_t, start_t, bst, ls;
        static ComboBox sharp_c, start_c, bsc;
        static Button restore;
        static Thickness margin;
        //static DownloadPage dp;
        static string[] str_s = { "清晰度", "Sharp" };
        static string[] str_ss = { "720", "360", "270" };
        static string[] str_st = { "启动页面", "Start Page" };
        static string[] str_r = { "还原设置", "Restore" };
        static string[] str_bs = { "缓存清晰度", "Buff sharp" };
        public static void Create(Canvas p, Thickness m)
        {
            if (cm != null)
            {
                cm.ReSize(m);
                ReSize(m);
                return;
            }
            parent = p;
            cm = new ColorManage();
            cm.Create(p, m);
            ts = new ToggleSwitch();
            if (language == 1)
                ts.IsOn = true;
            ts.Toggled += (o, e) => {
                language = ts.IsOn ? 1 : 0;
                Setting.language = language;
                MainEx.LanguageChange();
                cm.ChangeLan();
                sharp_t.Text = str_s[language];
                start_t.Text = str_st[language];
                bst.Text = str_bs[language];
                start_c.Items.Clear();
                int c = MainEx.s_all.Length;
                for (int i = 0; i < c; i++)
                    start_c.Items.Add(MainEx.s_all[i][language]);
                start_c.SelectedIndex = Setting.startindex;
                restore.Content = str_r[language];
                ChangLanguage();
                //dp.ChangLanguage();
            };
            p.Children.Add(ts);
            ls = CreateTextBlockNext();
            ls.Foreground = font_brush;
            ls.Text = "English";
            p.Children.Add(ls);

            sharp_t = CreateTextBlockNext();
            sharp_t.Foreground = font_brush;
            sharp_t.Text = str_s[language];
            p.Children.Add(sharp_t);
            start_t = CreateTextBlockNext();
            start_t.Foreground = font_brush;
            start_t.Text = str_st[language];
            p.Children.Add(start_t);
            bst = CreateTextBlockNext();
            bst.Foreground = font_brush;
            bst.Text = str_bs[language];
            p.Children.Add(bst);

            sharp_c = new ComboBox();
            for (int i = 0; i < 3; i++)
                sharp_c.Items.Add(str_ss[i]);
            switch (Setting.sharp)
            {
                case 270:
                    sharp_c.SelectedIndex = 2;
                    break;
                case 360:
                    sharp_c.SelectedIndex = 1;
                    break;
                default:
                    sharp_c.SelectedIndex = 0;
                    break;
            }

            sharp_c.SelectionChanged += (o, e) => {
                switch (sharp_c.SelectedIndex)
                {
                    case 1:
                        Setting.sharp = 360;
                        break;
                    case 2:
                        Setting.sharp = 270;
                        break;
                    default:
                        Setting.sharp = 720;
                        break;
                }
            };
            p.Children.Add(sharp_c);

            bsc = new ComboBox();
            for (int i = 0; i < 3; i++)
                bsc.Items.Add(str_ss[i]);
            switch (Setting.buffsharp)
            {
                case 270:
                    bsc.SelectedIndex = 2;
                    break;
                case 360:
                    bsc.SelectedIndex = 1;
                    break;
                default:
                    bsc.SelectedIndex = 0;
                    break;
            }
            bsc.SelectionChanged += (o, e) => {
                switch (bsc.SelectedIndex)
                {
                    case 1:
                        Setting.buffsharp = 360;
                        break;
                    case 2:
                        Setting.buffsharp = 270;
                        break;
                    default:
                        Setting.buffsharp = 720;
                        break;
                }
            };
            p.Children.Add(bsc);

            start_c = new ComboBox();
            int l = MainEx.s_all.Length;
            for (int i = 0; i < l; i++)
                start_c.Items.Add(MainEx.s_all[i][language]);
            start_c.SelectedIndex = Setting.startindex;
            start_c.SelectionChanged += (o, e) => {
                if (start_c.SelectedIndex > -1)
                    Setting.startindex = start_c.SelectedIndex;
            };
            p.Children.Add(start_c);

            CreateDown(p);
            restore = new Button();
            restore.Click += (o, e) => { Restore(); };
            restore.Content = str_r[language];
            restore.Foreground = font_brush;
            p.Children.Add(restore);
            ReSize(m);
            //dp= new DownloadPage();
            //dp.Create(p,new Thickness(0,305,m.Right,m.Bottom));
        }
        public static void ReSize(Thickness m)
        {
            if (margin == m)
                return;
            margin = m;
            double w = m.Right - m.Left;
            double h = m.Bottom - m.Top;
            double dx = m.Left;
            double dxr = dx + 80;
            ls.Margin = new Thickness(dx, 186, 0, 0);
            ts.Margin = new Thickness(dxr, 180, 0, 0);
            sharp_t.Margin = new Thickness(dx, 210, 0, 0);
            sharp_c.Margin = new Thickness(dxr, 210, 0, 0);
            bst.Margin = new Thickness(dx, 240, 0, 0);
            bsc.Margin = new Thickness(dxr, 240, 0, 0);
            start_t.Margin = new Thickness(dx, 270, 0, 0);
            start_c.Margin = new Thickness(dxr, 270, 0, 0);
            m.Top = 300;
            ReSizeDown(m);
            restore.Margin = new Thickness(dx, 390, 0, 0);
        }
        public static void Dispose()
        {
            cm.Dispose();
            parent.Children.Remove(sharp_t);
            parent.Children.Remove(start_t);
            parent.Children.Remove(bst);
            parent.Children.Remove(sharp_c);
            parent.Children.Remove(start_c);
            parent.Children.Remove(bsc);
            parent.Children.Remove(restore);
            parent.Children.Remove(ts);
            parent.Children.Remove(ls);

            RecycleTextBlock(sharp_t);
            RecycleTextBlock(start_t);
            RecycleTextBlock(bst);

            GC.SuppressFinalize(ls);
            GC.SuppressFinalize(ts);
            GC.SuppressFinalize(restore);
            GC.SuppressFinalize(sharp_c);
            GC.SuppressFinalize(bsc);
            GC.SuppressFinalize(start_c);
            cm = null;
            margin.Right = 0;
            DisposeDown();
            //dp.Dispose();
        }
        static void Restore()
        {
            Setting.DefaltSet();
            bk_brush.Color = Setting.bk_color;
            bor_brush.Color = Setting.bor_color;
            title_brush.Color = Setting.title_color;
            filter_brush.Color = Setting.filter_color;
            nav_brush.Color = Setting.nav_color;
            language = Setting.language;
            sharp_c.SelectedIndex = 0;
            start_c.SelectedIndex = Setting.startindex;
            if (language == 1)
                ts.IsOn = true;
            else ts.IsOn = false;
        }

        static TextBlock direct,buff_t,time_t;
        static ToggleSwitch startbuff;
        static Button choice;
        static ComboBox time_c;
        static string[] str_c = { "缓存目录", "Buffer Directory" };
        static string[] str_sb = { "程序启动继续缓存", "Program starts to continue caching" };
        static string[] str_sd = { "240", "60", "120", "180", "300" };
        static string[] str_bt = { "缓存间隔","Buff interval"};
        static StorageFolder rsf;//root
        static async void GetFolder(Action<string> back)
        {
            FolderPicker fp = new FolderPicker();
            fp.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            fp.FileTypeFilter.Add(".mp4");
            rsf = await fp.PickSingleFolderAsync();
            if (rsf == null)
                return;
            Setting.token = StorageApplicationPermissions.FutureAccessList.Add(rsf);
            back(rsf.Path);
        }
        public static void CreateDown(Canvas p)
        {
            Setting.GetRootFolder((r) => rsf = r);
            direct = CreateNewTextBlock();
            direct.Foreground = font_brush;
            buff_t = CreateNewTextBlock();
            buff_t.Foreground = font_brush;
            buff_t.Text = str_sb[language];
            time_t = CreateNewTextBlock();
            time_t.Foreground = font_brush;
            time_t.Text = str_bt[language];

            choice = CreateButtonNext();
            choice.Foreground = font_brush;
            choice.Content = str_c[language];
            choice.Width = 80;
            choice.Click += (o, e) => { GetFolder((s) => { direct.Text = s; }); };
            direct.Text = rsf.Path;
            direct.Foreground = font_brush;

            p.Children.Add(choice);
            p.Children.Add(direct);
            p.Children.Add(buff_t);
            p.Children.Add(time_t);
            startbuff = new ToggleSwitch();
            if (Setting.start_app_buff == 1)
                ts.IsOn = true;
            startbuff.Toggled += (o, e) =>
            {
                Setting.start_app_buff = startbuff.IsOn ? 1 : 0;
            };
            parent.Children.Add(startbuff);
            time_c = new ComboBox();
            for (int i = 0; i < 5; i++)
                time_c.Items.Add(str_sd[i]);
            time_c.SelectedIndex = Setting.buffinterval;
            time_c.SelectionChanged += (o, e) => { Setting.buffinterval = time_c.SelectedIndex; };
            parent.Children.Add(time_c);
        }
        public static void ChangLanguage()
        {
            choice.Content = str_c[language];
            buff_t.Text = str_sb[language];
            time_t.Text = str_bt[language];
        }
        public static void DisposeDown()
        {
            parent.Children.Remove(direct);
            parent.Children.Remove(choice);
            parent.Children.Remove(buff_t);
            parent.Children.Remove(time_t);
            RecycleTextBlock(direct);
            RecycleTextBlock(buff_t);
            RecycleTextBlock(time_t);
            RecycleButton(choice);
            parent.Children.Remove(startbuff);
            GC.SuppressFinalize(startbuff);
            parent.Children.Remove(time_c);
            GC.SuppressFinalize(time_c);
        }
        public static void ReSizeDown(Thickness m)
        {
            if (choice == null)
                return;
            choice.Margin = m;
            m.Left += 80;
            m.Top += 8;
            direct.Margin = m;
            m.Top += 22;
            time_c.Margin = m;
            m.Left -= 80;
            m.Top += 8;
            time_t.Margin = m;
            m.Top += 30;
            buff_t.Margin = m;
            m.Top -= 8;
            m.Left += 120;
            startbuff.Margin = m;
        }
    }
}