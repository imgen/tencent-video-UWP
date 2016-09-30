using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace TVWP.Class
{
    class WebClass:Data
    {
        #region main
        static HttpClient hc;

        public static void Initial()
        {
            hc = new HttpClient();
            hc.DefaultRequestHeaders.UserAgent.Add(new Windows.Web.Http.Headers.HttpProductInfoHeaderValue(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36"));
        }
        public static async Task<string> GetResultsA(string url)
        {
            HttpResponseMessage hr = await hc.GetAsync(new Uri(url));
            hr.EnsureSuccessStatusCode();
            return await hr.Content.ReadAsStringAsync();
        }
        public static async Task<string> GetResults(string url)
        {
            IBuffer ib= await hc.GetBufferAsync(new Uri(url));
            var dr = DataReader.FromBuffer(ib);
            byte[] buff=new byte[ib.Length];
            dr.ReadBytes(buff);
            return Encoding.UTF8.GetString(buff);
        }
        public static async Task<string> GetResults(string url,string refer)
        {
            //url += "&otype=json";
            hc.DefaultRequestHeaders.Referer = new Uri(refer);
            IBuffer ib = await hc.GetBufferAsync(new Uri(url));
            var dr = DataReader.FromBuffer(ib);
            byte[] buff = new byte[ib.Length];
            dr.ReadBytes(buff);
            return Encoding.UTF8.GetString(buff);
        }
        public static async Task<string> Post(string url,string content)
        {
            var cc=  await hc.PostAsync(new Uri( url), new HttpStringContent(content, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));//"application/x-www-form-urlencoded"
            return await  cc.Content.ReadAsStringAsync();
        }
        public static async Task<string> Post(string url, string content,string refer)
        {
            hc.DefaultRequestHeaders.Referer = new Uri(refer);
            var cc = await hc.PostAsync(new Uri(url), new HttpStringContent(content, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));//"application/x-www-form-urlencoded"
            return await cc.Content.ReadAsStringAsync();
        }
        #endregion

        #region videoinfo
        public static async void GetVideoInfo(string vid, Action<VideoInfoA> callback)
        {
            string s = await Post("http://vv.video.qq.com/getinfo",
                "otype=json&format=shd&vids=" + vid);
            char[] c = s.ToCharArray();
            c = DeleteChar(ref c, '\\');
            VideoInfoA vi = new VideoInfoA();
            vi.vid = vid;
            vi.http = GetHttp(ref c);
            GetVI(ref c, ref vi);
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
                goto label0;
            if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                ss[i] = new string(tt);
            else ss[i] = "http:" + new string(tt);
            i++;
            goto label0;
        }
        static void GetVI(ref char[] source, ref VideoInfoA vi)
        {
            int t = FindCharArray(ref source, ref Key_td, 0);
            char[] tt = FindCharArrayA(ref source, '\"', '\"', ref t);
            vi.alltime = CharToInt(ref tt);
            int part = 0;
            int s = FindCharArray(ref source, ref Key_cmd5, 0);
            while (s > 0)
            {
                part++;
                s = FindCharArray(ref source, ref Key_cmd5, s);
            }
            vi.fregment = part;
            VideoInfo[] temp = new VideoInfo[6];
            int c= s = 0;
            for (int i = 0; i < 6; i++)
            {
                tt = FindCharArrayA(ref source, '(', ')', ref s);
                if (tt == null)
                {
                    VideoInfo[] r = new VideoInfo[c];
                    for (int d = 0; d < c; d++)
                        r[d] = temp[d];
                    vi.vi = r;
                }
                VideoInfo VI = new VideoInfo();
                VI.sharp = new string(tt);
                s = FindCharArray(ref source, ref Key_idA, s);
                tt = FindCharArrayA(ref source, ':', ',', ref s);
                if (tt != null & tt.Length > 2)
                {
                    s = FindCharArray(ref source, ref Key_name, s);
                    VI.fmt = new string(FindCharArrayA(ref source, '\"', '\"', ref s));
                    string str = new string(CopyCharArry(ref tt, tt.Length - 3, 3));
                    VI.fid = "10" + str;
                    VI.id = str;
                    VI.fn = vi.vid + ".p" + VI.id + ".";
                    temp[c] = VI;
                    c++;
                }
            }
        }
        public static void GetVideoKey(string vid, VideoInfo vi, Action<string,string> callback)
        {
            GetVideoKey(vid, vi, callback, 1);
        }
        public static async void GetVideoKey(string vid, VideoInfo vi, Action<string,string> callback, int part)
        {
            string str = "platform=11&otype=xml&vids=" + vid + "&format=" + vi.fid + "&filename=" + vi.fn;
            str += part.ToString() + ".mp4";
            str = await Post("http://vv.video.qq.com/getkey", str,
                "http://imgcache.qq.com/tencentvideo_v1/playerv3/TencentPlayer.swf?max_age=86400&v=20160819");
            char[] tc = str.ToCharArray();
            int ccc = 0;
            str = new string(FindCharArrayA(ref tc, ref Key_key, ref Key_less, ref ccc));
            callback(vid, str);
        }
        public static async void GetVideoFregment(string vid,VideoInfo vi,Action<int> callback)
        {
            string str = "platform=11&otype=xml&vids=" + vid + "&format=" + vi.fid + "&filename=" + vi.fn;
            int c = 2;
            l:;
            string cc=str + c.ToString() + ".mp4";
            cc = await Post("http://vv.video.qq.com/getkey", cc,
                "http://imgcache.qq.com/tencentvideo_v1/playerv3/TencentPlayer.swf?max_age=86400&v=20160819");
            char[] tc = cc.ToCharArray();
            if(FindCharArray(ref tc,ref Key_key,0)>0)
            { c++;goto l; }
            c--;
            callback(c);
        }
        #endregion

        #region translation
        public static async void Translation(string content,int tag,Action<string,int> callback)
        {
            string str = "http://api.microsofttranslator.com/V2/Ajax.svc/Translate?appId=A4D660A48A6A97CCA791C34935E4C02BBB1BEC1C&from=zh-cn&to=en&text="
                + Uri.EscapeDataString(content);
            str = await GetResults(str);
            callback(str,tag);
        }
        #endregion
    }
}