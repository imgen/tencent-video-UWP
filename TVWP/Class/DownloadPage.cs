using System;
using System.Collections.Generic;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace TVWP.Class
{
    struct Mission
    {
        public bool downloading;
        public byte status;
        public byte month;
        public byte day;
        public byte hour;
        public byte min;
        public byte max;
        public byte done;
        public byte[] progress;
        public int time;
        public string vid;
        public string name;
    }
    class DownLoad
    {
        struct Down
        {
            public int index;
            public string vid;
            public VideoInfo vic;
            public VideoAddress va;
            public DownloadOperation dp;
            public StorageFolder sf;
            public int delay;
            public int part;
            public int max;
            public int done;
            public byte[] progress;
        }
        static DispatcherTimer dt;
        static BackgroundDownloader BD;
        static bool downloading;
        static char[] error403 = "(403)".ToCharArray();
        //static int downindex;
        static Down[] current;
        static StorageFolder rsf;
        protected static Action<string> Show;
        public static void AddVid(string vid)
        {
            if (lm == null)
                lm = new List<Mission>();
            else
            {
                int c = lm.Count;
                for (int i = 0; i < c; i++)
                    if (vid == lm[i].vid)
                        return;
            }
            Mission m = new Mission();
            m.vid = vid;
            lm.Add(m);
            if(!downloading)
            {
                StartDownload();
            }
        }
        static int GetDownMission()
        {
            if (lm == null)
                return -1;
            int c = lm.Count;
            for(int i=0;i<c;i++)
            {
                if (!lm[i].downloading)
                {
                    if (lm[i].status == 0)
                        return i;
                    if (lm[i].status == 1)
                    {
                        byte t = (byte)DateTime.Now.Month;
                        if (lm[i].month != t)
                            return i;
                        t = (byte)DateTime.Now.Day;
                        if (lm[i].day != t)
                            return i;
                        t = (byte)DateTime.Now.Hour;
                        if (t - lm[i].month > 6)
                            return i;
                    }
                }
            }
            return -1;
        }
        public static void StartDownload()
        {
            downloading = true;
            if(current==null)
            {
                current = new Down[3];
                for(int i=0;i<3;i++)
                {
                    current[i].index = -1;
                    current[i].va = new VideoAddress();
                }
            }
            if (dt == null)
            {
                dt = new DispatcherTimer();
                dt.Tick += Surveillance;
                dt.Interval = new TimeSpan(0, 0, 2);
                BD = new BackgroundDownloader();
            }
            dt.Start();
        }
        static void Surveillance(object o,object e)
        {
            if (rsf == null)
            {
                Setting.GetRootFolder((r)=> {
                rsf = r;
                GetMission(); });
                return; }
            int c = 0;
            for(int i=0;i<3;i++)
            {
                if(current[i].index<0)
                {
                    int t = GetDownMission();
                    if(t>-1)
                    {
                        string vid = lm[t].vid;
                        Mission m = lm[t];
                        m.downloading=true;
                        lm[t] = m;
                        current[i].index = t;
                        current[i].vid = vid;
                        current[i].va.SetVid(vid,Setting.buffsharp,GetAddress);
                    }else c++;
                }else
                {
                    if(current[i].delay<0)
                    {
                        if(current[i].dp!=null)
                        {
                            BackgroundDownloadProgress bkd = current[i].dp.Progress;
                            double s = bkd.TotalBytesToReceive;
                            double t = bkd.BytesReceived;
                            if (s == t)
                                current[i].va.SetVid(current[i].vic.vid, Setting.buffsharp, GetAddress);
                        }
                    }
                }
                current[i].delay--;
            }
            if (Show != null)
                GetDetail();
            if (c >= 3)
            { dt.Stop(); downloading = false; }
        }
        static void GetAddress(string vid)
        {
            for(int i=0;i<3;i++)
            {
                if(vid==current[i].vid)
                {
                    current[i].va.GetAddress(ref current[i].vic);
                    current[i].sf = null;
                    int c = current[i].index;
                    //current[i].part = lm[c].downindex;
                    Mission m = lm[c];
                    if(m.name==null)
                    {
                        m.max = (byte)current[i].vic.part;
                        m.name = current[i].vic.tilte;
                        m.progress = new byte[m.max];
                        m.time = current[i].vic.alltime;
                    }
                    current[i].progress = m.progress;
                    current[i].max = m.max;
                    lm[c] = m;
                    StartMissionNextPart(i);
                }
            }
        }
        async static void StartMissionNextPart(int index)
        {
            ls:;
            byte[] b = current[index].progress;
            int e = b.Length;
            int s = current[index].part;
            for (int i = s; i < e; i++)
                if (b[i] == 0)
                { current[index].part = i; goto ss; }
            current[index].index = -1;
            return;
            ss:;
            if (current[index].sf == null)
                current[index].sf = await rsf.CreateFolderAsync(current[index].vid, CreationCollisionOption.OpenIfExists);
            int part = current[index].part;
            StorageFile ss = await current[index].sf.CreateFileAsync(part.ToString(), CreationCollisionOption.ReplaceExisting);  
            VideoInfo vic = current[index].vic;
            current[index].part++;
            string str;
            if (vic.type == 1)
                str = vic.href;
            else str = vic.href + (part+1).ToString() + vic.vkey + "&guid=" + vic.cmd5[part];
            current[index].dp = BD.CreateDownload(new Uri(str), ss);
            switch (Setting.buffinterval)
            {
                case 0:
                    current[index].delay = 120;
                    break;
                case 1:
                    current[index].delay = 30;
                    break;
                case 2:
                    current[index].delay = 60;
                    break;
                case 3:
                    current[index].delay = 90;
                    break;
                default:
                    current[index].delay = 150;
                    break;
            }
            try
            {
                await current[index].dp.StartAsync();
                int c = current[index].index;
                current[index].progress[part] = 1;
                part++;
                current[index].done++;
                Mission m = lm[c];
                m.done = (byte)current[index].done;
                m.progress = current[index].progress;
                if (part>= current[index].max)
                {
                    current[index].index = -1;
                }
                if (current[index].done >= current[index].max)
                {
                    m.status = 2;
                    m.done = 1;
                    current[index].index = -1;
                }
                m.month = (byte)DateTime.Now.Month;
                m.day = (byte)DateTime.Now.Day;
                m.hour = (byte)DateTime.Now.Hour;
                m.min = (byte)DateTime.Now.Minute;
                lm[c] = m;
            }
            catch (Exception ex)
            {
                char[] temp = ex.Message.ToCharArray();
                if (CharOperation.FindCharArray(ref temp, ref error403, 0) > 0)
                    Main.Notify("错误403: " + current[index].vic.tilte + "分段" + part.ToString()
                        + " 已被<<腾讯大大>>禁止访问", Component.warning_brush);
                else Main.Notify(ex.Message,Component.warning_brush);
                int c = current[index].index;
                Mission m = lm[c];
                current[index].part++;
                part++;
                m.month = (byte)DateTime.Now.Month;
                m.day = (byte)DateTime.Now.Day;
                m.hour = (byte)DateTime.Now.Hour;
                m.min = (byte)DateTime.Now.Minute;
                m.status = 1;
                lm[c] = m;
                if (part >= current[index].max)
                    current[index].index = -1;
                else goto ls;
            }
        }
        static void GetDetail()
        {
            string str = "";
            for(int i=0;i<3;i++)
            {
                if (current[i].index > -1)
                {
                    if(current[i].dp!=null)
                    {
                        BackgroundDownloadProgress bkd = current[i].dp.Progress;
                        double s = bkd.TotalBytesToReceive;
                        double t = bkd.BytesReceived;
                        if (s == t)
                        {
                            int o = current[i].delay;
                            o *= 2;
                            int d = current[i].part+1;
                            str += current[i].vic.tilte + " 在"+o.ToString() + "秒之后下载第" + d.ToString() + "个分段\r\n";
                        }
                        else
                        {
                            int d = current[i].part;
                            double r = t/s;
                            str += "正在缓存 " + current[i].vic.tilte + " 第" + d.ToString() + "个分段" + (r * 100).ToString() + "%\r\n";
                        }
                    }
                }
            }
            Show(str);
        }
        #region
       
        public static void GetVideoList()
        {
            if (rsf == null)
               Setting.GetRootFolder((r) => { rsf = r; GetMission(); });
            else
                GetMission();
            if(Setting.start_app_buff==1)
               StartDownload();
        }
        static async void GetMission()
        {
            StorageFile ss = await rsf.CreateFileAsync("info", CreationCollisionOption.OpenIfExists);
            if (ss != null)
            {
                try
                {
                    IRandomAccessStream i = await ss.OpenAsync(FileAccessMode.ReadWrite);
                    if (i.Size == 0)
                    {
                        i.Dispose();
                        return;
                    }
                    DataReader dr = new DataReader(i);
                    await dr.LoadAsync((uint)i.Size);
                    byte[] buff = new byte[i.Size];
                    dr.ReadBytes(buff);
                    dr.Dispose();
                    i.Dispose();
                    ReadMission(ref buff);
                }
                catch (Exception ex)
                {
                    Main.Notify(ex.Message);
                }
            }
        }
        public async static void SaveMission(Action callback)
        {
            byte[] buff;
            if (lm != null)
                if (lm.Count > 0)
                {
                    buff = WriteMission();
                    StorageFile ss = await rsf.CreateFileAsync("info", CreationCollisionOption.OpenIfExists);
                    if (ss != null)
                    {
                        IRandomAccessStream i = await ss.OpenAsync(FileAccessMode.ReadWrite);
                        i.Size =(ulong) buff.Length;
                        i.Seek(0);
                        DataWriter dw = new DataWriter(i);
                        dw.WriteBytes(buff);
                        await dw.StoreAsync();
                        dw.Dispose();
                        i.Dispose();
                    }
                }
                else
                {
                    StorageFile ss = await rsf.CreateFileAsync("info", CreationCollisionOption.OpenIfExists);
                    if(ss!=null)
                    await ss.DeleteAsync();
                }
            if (callback != null)
                callback();
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
        unsafe static byte[] DecodeProgress(byte* bp,int count)
        {
            byte[] b = new byte[count];
            int c = count / 8;
            int r = count % 8;
            if (r > 0)
                c++;
            int s = 0;
            for(int i=0;i<c;i++)
            {
                byte o = *bp;
                for(int t=0;t<8;t++)
                {
                    if ((o & 1) == 1)
                        b[s] = 1;
                    o >>= 1;
                    s++;
                    if (s >= count)
                        return b;
                }
                bp++;
            }
            return b;
        }
        unsafe static void EncodeProgress(byte*bp, byte[] data)
        {
            int l = data.Length;
            int c = l / 8;
            int r = l % 8;
            if (r > 0)
                c++;
            int s = 0;
            for(int i=0;i<c;i++)
            {
                byte o = 0;
                byte d = 1;
                for(int t=0;t<8;t++)
                {
                    if (data[s] == 1)
                        o |= d;
                    d <<= 1;
                    s++;
                    if(s>=l)
                    {
                        *bp = o;
                        return;
                    }
                }
                *bp = o;
                bp++;
            }
        }
        protected static List<Mission> lm;
        static byte[] WriteMission()
        {
            int c = lm.Count;
            int len = c * 20 + 4;//c*(7 byte+1byte +12byte)+4byte (count)
            for (int i = 0; i < c; i++)
            {
                int t = lm[i].max;
                int o = t / 8;
                int r = t % 8;
                if (r > 0)
                    o++;
                len += o;
                len += lm[i].vid.Length * 2;//char=byte*2
                len += lm[i].name.Length * 2;
            }
            byte[] buff = new byte[len];
            unsafe
            {
                fixed (byte* bp = &buff[0])
                {
                    int* ip = (int*)bp;
                    *ip = c;
                    ip++;
                    byte* op=(byte*)ip;
                    char* cp;
                    for (int i = 0; i < c; i++)
                    {
                        *op = lm[i].status;
                        op++;
                        *op = lm[i].month;
                        op++;
                        *op = lm[i].day;
                        op++;
                        *op = lm[i].hour;
                        op++;
                        *op = lm[i].min;
                        op++;
                        *op = lm[i].max;
                        op++;
                        *op = lm[i].done;
                        op++;
                        int m = lm[i].max;
                        int o = m / 8;
                        int r = m % 8;
                        if (r > 0)
                            o++;
                        *op = (byte)o;
                        op++;
                        EncodeProgress(op,lm[i].progress);
                        op += o;
                        ip = (int*)op;

                        *ip = lm[i].time;
                        ip++;
                        int l = lm[i].vid.Length;
                        *ip = l;
                        ip++;
                        cp = (char*)ip;
                        char[] cr = lm[i].vid.ToCharArray();
                        for (int t = 0; t < l; t++)
                        {
                            *cp = cr[t];
                            cp++;
                        }
                        ip = (int*)cp;

                        l = lm[i].name.Length;
                        *ip = l;
                        ip++;
                        cp = (char*)ip;
                        cr = lm[i].name.ToCharArray();
                        for (int t = 0; t < l; t++)
                        {
                            *cp = cr[t];
                            cp++;
                        }
                        op = (byte*)cp;
                    }
                }
            }
            return buff;
        }
        static void ReadMission(ref byte[] buff)
        {
            lm = new List<Mission>();
            Mission m = new Mission();
            unsafe
            {
                fixed (byte* bp = &buff[0])
                {
                    int* ip = (int*)bp;
                    int l = *ip;
                    ip++;
                    byte* op = (byte*)ip;
                    char* cp;
                    for (int i = 0; i < l; i++)
                    {
                        m.status = *op;
                        op++;
                        m.month = *op;
                        op++;
                        m.day = *op;
                        op++;
                        m.hour = *op;
                        op++;
                        m.min = *op;
                        op++;
                        m.max = *op;
                        op++;
                        m.done = *op;
                        op++;
                        byte o= *op;
                        op++;
                        m.progress= DecodeProgress(op,m.max);
                        op += o;
                        ip = (int*)op;

                        m.time = *ip;
                        ip++;
                        int c = *ip;
                        ip++;
                        cp = (char*)ip;
                        m.vid = new string(CopyChar(cp, c));
                        cp += c;
                        ip = (int*)cp;

                        c = *ip;
                        ip++;
                        cp = (char*)ip;
                        m.name = new string(CopyChar(cp, c));
                        cp += c;
                        op = (byte*)cp;
                        lm.Add(m);
                    }
                }
            }
        }
        public static void Pause()
        {
            if (dt != null)
                dt.Stop();
        }
        public static Mission Getmission(int index)
        {
            return lm[index];
        }
        #endregion
    }
    class DownloadPage:DownLoad
    {
        static DownView dv;
        static TextBlock detail;
        static Thickness margin;
        static StorageFolder rsf;//root
        static Canvas parent;
        static Button black;
        static Border bor;
        static DisplayRequest display;
        public static void Create(Canvas p, Thickness m)
        {
            Setting.GetRootFolder((r) => rsf = r);
            parent = p;
            black = new Button();
            p.Children.Add(black);
            black.Content = "进入全黑，双击点亮，打开节电模式，关闭金刚键";
            black.Click += (o, e) => {
                if (display == null)
                    display = new DisplayRequest();
                display.RequestActive();
                bor.Visibility = Visibility.Visible;
#if phone
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
#endif
            };
            black.Margin = m;
            m.Top += 30;
            detail = Component.CreateTextBlockNext();
            detail.Foreground = Component.font_brush;
            detail.TextWrapping = TextWrapping.Wrap;
            detail.Margin = m;
            detail.Width = m.Right - m.Left;
          
            detail.Height = 70;
            p.Children.Add(detail);

            dv = new DownView();
            dv.Play = Play;
            dv.Delete = Delete;
            dv.SetParent(p);
            m.Top += 70;

            dv.Resize(m);
            dv.data = lm;
            dv.Refresh();
            dv.ShowBorder();
            Show = (s) => { detail.Text = s; dv.Refresh(); };
           
            CreatBlack();
        }
        static long time;
        static int count;
        static void CreatBlack()
        {
            bor = new Border();
            bor.Width= Window.Current.Bounds.Width;
            bor.Height = Window.Current.Bounds.Height;
            bor.Background = new SolidColorBrush(Colors.Black);
            bor.PointerPressed += (o, e) =>
              {
                  long t = DateTime.Now.Ticks;
                  if (t - time > Component.presstime)
                  {
                      count = 0;
                  }
                  time = t;
              };
            bor.PointerReleased += (o, e) =>
            {
                long t = DateTime.Now.Ticks;
                if (t - time < Component.presstime)
                {
                    time = t;
                    count++;
                    if(count>=2)
                    {
                        bor.Visibility = Visibility.Collapsed;
                        count = 0;
                        display.RequestRelease();
#if phone
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
#endif
                    }
                }
                else count = 0;
            };
            bor.Visibility = Visibility.Collapsed;
            parent.Children.Add(bor);
        }
        public static void Dispose()
        {
            Show = null;
            parent.Children.Remove(detail);
            parent.Children.Remove(black);
            GC.SuppressFinalize(black);
            Component.RecycleTextBlock(detail);
            dv.Dispose();
        }
        static async void Play(int index)
        {
            StorageFolder ss = await rsf.CreateFolderAsync(lm[index].vid, CreationCollisionOption.OpenIfExists);
            PageManageEx.CreateNewPage(PageTag.playerEx);
            PlayerEx.PlayEx(index, ss);
        }
        static async void Delete(int index)
        {
            Mission m = lm[index];
            lm.RemoveAt(index);
            StorageFolder ss = await rsf.CreateFolderAsync(m.vid, CreationCollisionOption.OpenIfExists);
            await ss.DeleteAsync(StorageDeleteOption.PermanentDelete);//default == recyle bin
            dv.Refresh();
        }
        public static void ReSize(Thickness m)
        {
            if (m == margin)
                return;
            margin = m;
            double w = m.Right - m.Left;
            if (w < 500)
                m.Top += 140;
            else m.Top += 70;
            if(dv!=null)
            {
                dv.Resize(m);
                dv.Refresh();
            }
        }
    }
}
