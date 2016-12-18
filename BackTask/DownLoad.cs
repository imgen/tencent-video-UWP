using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;

namespace BackTask
{
    public sealed class DownLoad : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var _deferral = taskInstance.GetDeferral();
            //
            // TODO: Insert code to start one or more asynchronous methods using the
            //       await keyword, for example:
            //
            // await ExampleMethodAsync();
            //
            //var details = (BackgroundTransferCompletionGroupTriggerDetails)taskInstance.TriggerDetails;
           // Start();
            _deferral.Complete();
        }
        List<Mission> lm;
        DownMisson dm;
        int index;
        public void Start()
        {
            index = 0;
            dm = new DownMisson();
            FileManage.LoadMisson(LoadMisson);
        }
        void LoadMisson(bool ok)
        {
            if (ok)
            {
                lm = FileManage.lm;
                DispatchMission();
            }
            else
            {
                //process over
            }
        }
        BackgroundDownloader BD;
        async void DispatchMission()
        {
            if (BD == null)
                BD = new BackgroundDownloader();
            StorageFile ss= await FileManage.CreateFile(lm[0],"0");
            DownloadOperation dp= BD.CreateDownload(new Uri(lm[0].href+"1"+lm[0].vkey), ss);
            await dp.StartAsync();
        }
        void DownCompelete(Mission m)
        {
            lm[index] = m;
            index++;
            DispatchMission();
        }
    }
    public sealed class Analysis : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var _deferral = taskInstance.GetDeferral();
            //
            // TODO: Insert code to start one or more asynchronous methods using the
            //       await keyword, for example:
            //
            // await ExampleMethodAsync();
            //

            Start();
            _deferral.Complete();
        }
        List<Mission> lm;
        DownMisson dm;
        int index;
        public void Start()
        {
            index = 0;
            dm = new DownMisson();
            FileManage.LoadMisson(LoadMisson);
        }
        void LoadMisson(bool ok)
        {
            if (ok)
            {
                lm = FileManage.lm;
                DispatchMission();
            }
            else
            {
                //process over
            }
        }
        void DispatchMission()
        {
            int c = lm.Count;
            if (index + 1 == c)
            {
                return;//process over
            }
            for (int i = index; i < c; i++)
            {
                if (lm[i].downindex < 3)
                {
                    dm.StartMission(FileManage.sharp, lm[i], DownCompelete);
                    index = i;
                    break;
                }
            }
        }
        void DownCompelete(Mission m)
        {
            lm[index] = m;
            index++;
            DispatchMission();
        }
    }
    class DownMisson
    {
        BackgroundDownloader BD;
        DownloadOperation down;
        public StorageFolder rsf { set; get; }//root
        StorageFolder sf;
        Action<Mission> callback;
        VideoAddress va;
        VideoInfo vi;
        Mission miss;
        int part;
        public void StartMission(int sharp, Mission m,Action<Mission> back)
        {
            callback = back;
            miss = m;
            part = m.downindex;
            if (BD == null)
                BD = new BackgroundDownloader();
            if (va == null)
                va = new VideoAddress();
            va.SetVid(m.vid,sharp,SetVideoInfo);
        }
        void SetVideoInfo()
        {
            vi = new VideoInfo();
            va.GetAddress(ref vi);
            CreateDownload();
        }
        async void CreateDownload()
        {
            label0:;
            string str;
            if (vi.type == 1)
                str = vi.href;
            else str = vi.href + (part + 1).ToString() + vi.vkey;
            StorageFile ss = await sf.CreateFileAsync(part.ToString(), CreationCollisionOption.ReplaceExisting);
            down = BD.CreateDownload(new Uri(str), ss);
            try
            {
                await down.StartAsync();
                miss.downindex = part;
            }catch (Exception ex)
            {

            }
            part++;
            if (part < miss.max)
                goto label0;
            callback(miss);
        }
    }
    struct Mission
    {
        public int downindex;
        public int max;
        public int time;
        public string vid;
        public string name;
        public string href;
        public string vkey;
    }
    class FileManage
    {
        static StorageFolder rsf;
        public static List<Mission> lm;
        static Action<bool> callback;
        public static int sharp=720;
        static string GetToken()
        {
            byte[] buff = LoadFile("vs");
            if(buff==null)
              return null;
            return ReadToken(buff);
        }
        static string ReadToken(byte[] buff)
        {
            unsafe
            {
                fixed (byte* bp = &buff[48])
                {
                    int* ip = (int*)bp;
                    sharp = *ip;
                }
                fixed (byte*bp=&buff[128])
                {
                    int* ip = (int*)bp;
                    int l = *ip;
                    if (l == 0)
                        return null;
                    ip++;
                    return new string(CopyChar((char*)ip, l));
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
        static byte[] LoadFile(string name)
        {
            IsolatedStorageFile temp = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream fs;
            if (temp.FileExists(name))
            {
                fs = temp.OpenFile(name, FileMode.Open);
                byte[] b = new byte[fs.Length];
                fs.Read(b, 0, b.Length);
                //b= AES_Decrypt(b,cypher);
                fs.Dispose();
                temp.Dispose();
                return b;
            }
            temp.Dispose();
            return null;
        }
        public static void SaveFile(byte[] buff, string name)
        {
            IsolatedStorageFile temp = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream fs;
            if (temp.FileExists(name))
                fs = temp.OpenFile(name, FileMode.OpenOrCreate);
            else
                fs = temp.CreateFile(name);
            //buff = AES_Encrypt(buff, cypher);
            fs.Write(buff, 0, buff.Length);
            fs.Dispose();
            temp.Dispose();
        }
        public static void DeleteFile(string name)
        {
            IsolatedStorageFile temp = IsolatedStorageFile.GetUserStoreForApplication();
            if (temp.FileExists(name))
                temp.DeleteFile(name);
        }
        static async void GetRootFolder(Action<bool> act)
        {
            string token = GetToken();
            if (token != null)
                rsf = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
            else
            {
                rsf = KnownFolders.VideosLibrary;
                rsf = await rsf.CreateFolderAsync("TencentVideo", CreationCollisionOption.OpenIfExists);
            }
            act(true);
        }
        static void ReadMisson(ref byte[] buff)
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
                    char* cp;
                    for (int i = 0; i < l; i++)
                    {
                        m.downindex = *ip;
                        ip++;
                        m.max = *ip;
                        ip++;
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
                        ip = (int*)cp;

                        c = *ip;
                        ip++;
                        cp = (char*)ip;
                        m.href = new string(CopyChar(cp, c));
                        cp += c;
                        ip = (int*)cp;

                        c = *ip;
                        ip++;
                        cp = (char*)ip;
                        m.vkey = new string(CopyChar(cp, c));
                        cp += c;
                        ip = (int*)cp;

                        lm.Add(m);
                    }
                }
            }
        }
        static byte[] WriteMisson()
        {

            int c = lm.Count;
            int len = c * 20 + 4;
            for (int i = 0; i < c; i++)
            {
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
                    char* cp;
                    for (int i = 0; i < c; i++)
                    {
                        *ip = lm[i].downindex;
                        ip++;
                        *ip = lm[i].max;
                        ip++;
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
                        ip = (int*)cp;
                    }
                }
            }
            return buff;
        }
        public static void LoadMisson(Action<bool> act)
        {
            callback = act;
            byte[] buff = LoadFile("buff");
            if (buff == null)
            {
                act(false);
                return;
            }
            ReadMisson(ref buff);
            GetRootFolder(act);
        }
        public static void SaveMisson()
        {
            byte[] buff;
            if (lm != null)
                if (lm.Count > 0)
                {
                    buff = WriteMisson();
                    SaveFile(buff,"buff");
                }
                else
                {
                    DeleteFile("buff");
                }
        }
        public async static Task<StorageFile> CreateFile(Mission m,string name)
        {
            StorageFolder sf = await rsf.CreateFolderAsync(m.vid,CreationCollisionOption.OpenIfExists);
            return await sf.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
        }

    }
    class NetClass
    {
        static HttpClient hc;
        public static void Initial()
        {
            hc = new HttpClient();
            hc.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36"));
        }
        public static  void TaskGet(string url, Action<string> t)
        {
            if (hc == null)
                Initial();
            byte[] buff = { };
            try
            {
                //buff = await hc.GetByteArrayAsync(new Uri(url));
                Task<string> b= hc.GetStringAsync(new Uri(url));
                t(b.Result);
#if !DEBUG
                t(Encoding.UTF8.GetString(buff));
#endif
            }
            catch (Exception ex)
            {
                
            }
#if DEBUG
            t(Encoding.UTF8.GetString(buff));
#endif
        }
        public static async void TaskGet(string url, Action<string> t, string refer)
        {
            if (hc == null)
                Initial();
            byte[] buff = { };
            try
            {
                hc.DefaultRequestHeaders.Referrer= new Uri(refer);
                buff = await hc.GetByteArrayAsync(new Uri(url));
#if !DEBUG
                t(Encoding.UTF8.GetString(buff));
#endif
            }
            catch (Exception ex)
            {
                
            }
#if DEBUG
            t(Encoding.UTF8.GetString(buff));
#endif
        }
        public static async Task<string> Post(string url, string content, string refer)
        {
            if (hc == null)
                Initial();
            hc.DefaultRequestHeaders.Referrer= new Uri(refer);
            Task<HttpResponseMessage> m =  hc.PostAsync(new Uri(url), new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded"));//"application/x-www-form-urlencoded"
            return await m.Result.Content.ReadAsStringAsync();
        }
        public static async void TaskPost(string url, Action<string> t, string content)
        {
            if (hc == null)
                Initial();
            try
            {
                var cc = await hc.PostAsync(new Uri(url), new StringContent(content,
                   Encoding.UTF8, "application/x-www-form-urlencoded"));
                t(await cc.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                
            }
        }
    }
    struct VideoInfo
    {
        public int type;
        public int alltime;
        public int part;
        public int site;
        public string vid;
        public string tilte;
        public string href;
        public string vkey;
        public string[] sharp;
        public string[] cmd5;
    }
    class VideoAddress
    {
        #region static
        static char[] Key_urlA = "\"url\"".ToCharArray();
        static char[] Key_less = "<".ToCharArray();
        static char[] Key_key = "<key>".ToCharArray();
        static char[] Key_cmd5 = "\"cmd5\"".ToCharArray();
        static char[] Key_dispatch = "dispatch".ToCharArray();
        static char[] Key_http = "http".ToCharArray();
        static char[] Key_fc = "\"fc\"".ToCharArray();
        static char[] Key_fn = "\"fn\"".ToCharArray();
        static char[] Key_fvkey = "\"fvkey\"".ToCharArray();
        static char[] Key_idA = "\"id\"".ToCharArray();
        static char[] Key_td = "\"td\"".ToCharArray();
        static char[] Key_ti = "\"ti\"".ToCharArray();
        static char[] Key_name = "\"name\"".ToCharArray();
       
        static int FindCharArray(ref char[] source, char c1, int index)
        {
            for (int i = index; i < source.Length; i++)
            {
                if (source[i] == c1)
                    return i;
            }
            return -1;
        }
        static int FindCharArray(ref char[] source, ref char[] content, int index)
        {
            for (int i = index; i < source.Length; i++)
            {
                if (content[0] == source[i])
                {
                    int t = i;
                    t++;
                    if (t >= source.Length)
                        return -1;
                    for (int c = 1; c < content.Length; c++)
                    {
                        if (content[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= source.Length)
                            return -1;
                    }
                    return t;
                }
                label1:;
            }
            return -1;
        }
        static char[] FindCharArrayA(ref char[] source, char sc, char ec, ref int index)
        {
            if (index < 0)
                index = 0;
            char temp = sc;
            bool o = false;
            int s = 0;
            for (int i = index; i < source.Length; i++)
            {
                if (temp == source[i])
                {
                    if (o)
                    {
                        int l = i - s;
                        char[] cc = new char[l];
                        for (int c = 0; c < l; c++)
                        { cc[c] = source[s]; s++; }
                        index = i;
                        return cc;
                    }
                    else { o = true; s = i; s++; temp = ec; }
                }
            }
            return null;
        }
        static char[] FindCharArrayA(ref char[] source, ref char[] sc, ref char[] ec, ref int index)
        {
            if (index < 0)
                index = 0;
            char[] temp = sc;
            bool o = false;
            int s = 0;
            for (int i = index; i < source.Length; i++)
            {
                if (temp[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < temp.Length; c++)
                    {
                        if (temp[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= source.Length)
                            return null;
                    }
                    if (o)
                    {
                        int l = t - s - 1;
                        temp = new char[l];
                        for (int c = 0; c < l; c++)
                        { temp[c] = source[s]; s++; }
                        index = t;
                        return temp;
                    }
                    else { o = true; s = t; i = t; temp = ec; }
                }
                label1:;
            }
            return null;
        }
        static char[] DeleteChar(ref char[] source, char c)
        {
            int len = source.Length;
            char[] temp = new char[len];
            int s = 0;
            for (int i = 0; i < len; i++)
            {
                if (source[i] != c)
                {
                    temp[s] = source[i];
                    s++;
                }
            }
            char[] temp2 = new char[s];
            for (int i = 0; i < s; i++)
                temp2[i] = temp[i];
            return temp2;
        }
        static int CharToInt(ref char[] source)
        {
            int c;
            if (source.Length < 10)
                c = source.Length;
            else c = 10;
            int r = 0;
            for (int i = 0; i < c; i++)
            {
                int t = source[i];
                if (t > 57 || t < 48)
                    return r;
                t -= 48;
                r *= 10;
                r += t;
            }
            return r;
        }
        static char[] CopyCharArry(ref char[] source, int index, int count)
        {
            char[] temp = new char[count];
            for (int i = 0; i < count; i++)
            {
                temp[i] = source[index];
                index++;
            }
            return temp;
        }

        static async void GetVideoInfo(string p, string vid, Action<VideoInfoA> callback)
        {
            string s = await NetClass.Post("http://vv.video.qq.com/getinfo", p,
                "http://imgcache.qq.com/tencentvideo_v1/playerv3/TencentPlayer.swf?max_age=86400&v=20161114");
            char[] c = s.ToCharArray();
            c = DeleteChar(ref c, '\\');
            //Debug.WriteLine(new string(c));
            VideoInfoA vi = new VideoInfoA();
            vi.vid = vid;
            vi.http = GetHttp(ref c);
            GetVI(ref c, ref vi);
            if (vi.fregment > 0)
            {
                string[] md5 = new string[vi.fregment];
                GetCmd5(ref c, ref md5);
                vi.cmd5 = md5;
            }
            callback(vi);
        }
        static string[] GetHttp(ref char[] source)
        {
            string[] ss = new string[5];
            int c = 0;
            int i = 0;
            label0:;
            c = FindCharArray(ref source, ref Key_urlA, c);
            if (c < 0)
            {
                string[] temp = new string[i];
                for (int p = 0; p < i; p++)
                    temp[p] = ss[p];
                return temp;
            }
            char[] tt = FindCharArrayA(ref source, '\"', '\"', ref c);
            int t = FindCharArray(ref tt, '/', 0);
            t = (int)tt[t + 2];
            if (t > 57 || t < 48)
            {
                if (FindCharArray(ref tt, ref Key_dispatch, 0) > -1) ;
                {
                    string str;
                    if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                        str = new string(tt);
                    else str = "http:" + new string(tt);
                    ss[i] = ss[0];
                    ss[0] = str;
                }
                goto label0;
            }
            if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                ss[i] = new string(tt);
            else ss[i] = "http:" + new string(tt);
            i++;
            goto label0;
        }
        static void GetCmd5(ref char[] source, ref string[] str)
        {
            int c = str.Length;
            int s = 0;
            for (int i = 0; i < c; i++)
            {
                s = FindCharArray(ref source, ref Key_cmd5, s);
                if (s < 0)
                    return;
                s++;
                str[i] = new string(FindCharArrayA(ref source, '\"', '\"', ref s));
            }
        }
        static void GetVI(ref char[] source, ref VideoInfoA vi)
        {
            int t = FindCharArray(ref source, ref Key_td, 0);
            char[] tt = FindCharArrayA(ref source, '\"', '\"', ref t);
            vi.alltime = CharToInt(ref tt);
            t = FindCharArray(ref source, ref Key_ti, t);
            tt = FindCharArrayA(ref source, '\"', '\"', ref t);
            vi.title = new string(tt);
            t = FindCharArray(ref source, ref Key_fc, 0);
            tt = FindCharArrayA(ref source, ':', ',', ref t);
            vi.fregment = CharToInt(ref tt);
            int s = 0;
            int a = FindCharArray(ref source, ref Key_fn, 0);
            char[] fn = FindCharArrayA(ref source, '\"', '.', ref a);
            vi.fn = new string(fn);
            a++;
            if (source[a] != 'p')
                vi.type = 1;
            else vi.type = 0;

            SharpItem[] temp = new SharpItem[6];
            int c = s = 0;
            for (int i = 0; i < 6; i++)
            {
                tt = FindCharArrayA(ref source, '(', ')', ref s);
                if (tt == null)
                {
                    SharpItem[] r = new SharpItem[c];
                    for (int d = 0; d < c; d++)
                        r[d] = temp[d];
                    vi.vi = r;
                    return;
                }
                SharpItem VI = new SharpItem();
                VI.sharp = new string(tt);
                VI.sharpA = CharToInt(ref tt);
                s = FindCharArray(ref source, ref Key_idA, s);
                tt = FindCharArrayA(ref source, ':', ',', ref s);
                if (tt != null)
                {
                    if (tt.Length > 2)
                    {
                        s = FindCharArray(ref source, ref Key_name, s);
                        VI.fmt = new string(FindCharArrayA(ref source, '\"', '\"', ref s));
                        string str = new string(CopyCharArry(ref tt, tt.Length - 3, 3));
                        VI.fid = "10" + str;
                        VI.pid = ".p" + str + ".";
                        temp[c] = VI;
                    }
                    else
                    {
                        s = FindCharArray(ref source, ref Key_name, s);
                        VI.fmt = new string(FindCharArrayA(ref source, '\"', '\"', ref s));
                        a = FindCharArray(ref source, ref Key_fvkey, a);
                        VI.vkey = new string(FindCharArrayA(ref source, '\"', '\"', ref a));
                        temp[c] = VI;
                    }
                    c++;
                }
            }
        }
        static async void GetVideoKey(string vid, string fn, SharpItem vi, Action<string, int> callback, int part)
        {
            string str = "platform=11&otype=xml&vids=" + vid + "&format=" + vi.fid + "&filename=" + fn + vi.pid;
            str += part.ToString() + ".mp4";
            str = await NetClass.Post("http://vv.video.qq.com/getkey", str,
                "http://imgcache.qq.com/tencentvideo_v1/playerv3/TencentPlayer.swf?max_age=86400&v=20161114");
            char[] tc = str.ToCharArray();
            int ccc = 0;
            tc = FindCharArrayA(ref tc, ref Key_key, ref Key_less, ref ccc);
            if (tc != null)
            {
                str = new string(tc);
                callback(str, part);
            }
            else callback(null, part);
        }
        static async void GetVideoFregment(string vid, string fn, SharpItem vi, Action<int> callback)
        {
            string str = "platform=11&otype=xml&vids=" + vid + "&format=" + vi.fid + "&filename=" + fn + vi.pid;
            int c = 2;
            l:;
            string cc = str + c.ToString() + ".mp4";
            cc = await NetClass.Post("http://vv.video.qq.com/getkey", cc,
                "http://imgcache.qq.com/tencentvideo_v1/playerv3/TencentPlayer.swf?max_age=86400&v=20160819");
            char[] tc = cc.ToCharArray();
            //Debug.WriteLine(new string(DeleteChar(ref tc, '/')));
            if (FindCharArray(ref tc, ref Key_key, 0) > 0)
            { c++; goto l; }
            c--;
            callback(c);
        }
        #endregion

        struct SharpItem
        {
            public int sharpA;
            public string sharp;
            public string fid;
            public string pid;
            public string fmt;
            public string vkey;
            public string[] vkeys;
        }
        struct VideoInfoA
        {
            public int type;
            public int alltime;
            public int fregment;
            public string vid;
            public string fn;
            public string title;
            public SharpItem[] vi;
            public string[] http;
            public string[] cmd5;
        }
        string vid;
        VideoInfoA via;
        int defsharp;
        int sharpindex;
        int siteindex;
        Action done;
        string parament;
        public void SetVid(string v, int sharp, Action callback)//sharp
        {
            vid = v;
            defsharp = sharp;
            done = callback;
            siteindex = 0;
            string str;
            switch (sharp)
            {
                case 270:
                    str = "sd";
                    break;
                case 360:
                    str = "hd";
                    break;
                default:
                    str = "shd";
                    break;
            }
            parament = "otype=json&defn=" + str + "&vids=" + vid;
            GetVideoInfo(parament, vid, SetVideoInfo);
        }
        void SetVideoInfo(VideoInfoA a)
        {
            if (a.http.Length == 0)
                return;
            via = a;
            SharpItem[] vi = via.vi;
            ChangeSharp(defsharp, done);
        }
        public void ChangeSharpA(int index, Action callback)
        {
            done = callback;
            if (index < 0)
                index = 0;
            if (index >= via.vi.Length)
                index = via.vi.Length - 1;
            sharpindex = index;
            if (via.vi[sharpindex].vkey == null)
                GetVideoKey(via.vid, via.fn, via.vi[sharpindex], (vk, p) =>
                {
                    via.vi[sharpindex].vkey = vk;
                    if (done != null)
                        done();
                }, 1);
            else done();
        }
        public void ChangeSharp(int sharp, Action callback)
        {
            defsharp = sharp;
            done = callback;
            SharpItem[] vi = via.vi;
            int c = vi.Length;
            int s = 270;
            for (int i = 0; i < c; i++)
            {
                int t = vi[i].sharpA;
                if (t == defsharp)
                {
                    sharpindex = i;
                    break;
                }
                else if (t > s & t < defsharp)
                {
                    s = t;
                    sharpindex = i;
                }
            }
            if (via.vi[sharpindex].vkey == null)
                GetVideoKey(via.vid, via.fn, via.vi[sharpindex], (vk, p) =>
                {
                    via.vi[sharpindex].vkey = vk;
                    if (done != null)
                        done();
                }, 1);
            else done();
        }
        public void GetAddress(ref VideoInfo vic)
        {
            vic.part = via.fregment;
            vic.alltime = via.alltime;
            SharpItem vi = via.vi[sharpindex];
            vic.type = via.type;
            if (via.type == 1)
            {
                vic.href = via.http[siteindex] + via.fn + ".mp4?type=mp4&fmt=auto&vkey=" + vi.vkey
                         + "";// + vi.fmt;
            }
            else
            {
                vic.href = via.http[siteindex] + via.fn + vi.pid;
                vic.vkey = ".mp4?sdtfrom=v1001&type=mp4&fmt=auto&vkey=" + vi.vkey;//+ vi.fmt;//href + part +vkey
            }
            int c = via.vi.Length;
            vic.sharp = new string[c];
            for (int i = 0; i < c; i++)
                vic.sharp[i] = via.vi[i].sharp;
            vic.site = via.http.Length;
            vic.tilte = via.title;
            vic.vid = vid;
            vic.cmd5 = via.cmd5;
        }
        public void ChangeSite(int index, ref VideoInfo vic)
        {
            siteindex = index;
            SharpItem vi = via.vi[sharpindex];
            vic.type = via.type;
            if (via.type == 1)
            {
                vic.href = via.http[index] + via.fn + ".mp4?vkey=" + vi.vkey
                         + "&type=mp4&fmt=" + vi.fmt;
            }
            else
            {
                vic.href = via.http[index] + via.fn + vi.pid;
                vic.vkey = ".mp4?vkey=" + vi.vkey + "&type=mp4&fmt=" + vi.fmt;//href + part +vkey
            }
        }
        public async Task<string> GetPartVkey(int part)
        {
            SharpItem vi = via.vi[sharpindex];
            string str = "platform=11&otype=xml&vids=" + vid + "&format=" + vi.fid + "&filename=" + via.fn + vi.pid;
            str += part.ToString() + ".mp4";
            str = await NetClass.Post("http://vv.video.qq.com/getkey", str,
                "http://imgcache.qq.com/tencentvideo_v1/playerv3/TencentPlayer.swf?max_age=86400&v=20161114");
            //Debug.WriteLine(str);
            char[] tc = str.ToCharArray();
            int ccc = 0;
            tc = FindCharArrayA(ref tc, ref Key_key, ref Key_less, ref ccc);
            if (tc != null)
                return new string(tc);
            else return null;
        }
    }
}
