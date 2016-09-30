using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    class VideoView:Data
    {
        struct VideoInfoView
        {
            public Canvas can;
            public ScrollViewer sv;
            public Image img;
            public Button title;
            public TextBlock detail;
            public CheckBox download;
            public int down_index;
        }
        struct Episode
        {
            public double width;
            public Canvas can;
            public List<Button> items;
        }
        #region view
        static VideoInfoView View_VI = new VideoInfoView();
        static VideoInfoA VIA;
        static Episode episode = new Episode();
        static List<string> episode_vid;
        public static void View_Create(Canvas parent, Thickness margin)
        {
            if (View_VI.can != null)
            {
                View_VI.sv.Visibility = Visibility.Visible;
                Episode_Reset();
                View_Resize(margin);
                return;
            }
            double w = margin.Right - margin.Left;
            double h = margin.Bottom - margin.Top;
            ScrollViewer sv = new ScrollViewer();
            View_VI.sv = sv;
            sv.Width = w;
            sv.Height = h;
            sv.Margin = margin;
            parent.Children.Add(sv);

            Canvas can = new Canvas();
            sv.Content = can;
            View_VI.can = can;
            Image img = new Image();
            View_VI.img = img;
#if desktop
            img.Width = 300;
            img.Height = 300;
            //img.Margin = new Thickness(150, 0, 0, 0);
#else
            img.Width = w-20;
            img.Height = w-20;
#endif
            img.PointerReleased += TV_press;
            can.Children.Add(img);

            CheckBox cb = new CheckBox();
            cb.FontSize = 18;
            cb.Content = "垃圾程序，我要自己下载";
            cb.Margin = new Thickness(10, w, 0, 0);
            View_VI.download = cb;
            can.Children.Add(cb);

            Button b = new Button();
            View_VI.title = b;
#if desktop
            b.Margin = new Thickness(10, 300, 0, 0);
#else
            b.Margin = new Thickness(10, w+40, 0, 0);
#endif
            b.FontSize = 18;
            b.Click += TV_EpisodeClick;
            b.Background = new SolidColorBrush();
            b.Foreground = new SolidColorBrush(Colors.Pink);
            can.Children.Add(b);
            

            TextBlock tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Width = w - 20;
            View_VI.detail = tb;
#if desktop
            tb.Margin = new Thickness(10, 330, 0, 0);
#else
            tb.Margin = new Thickness(10, w+80, 0, 0);
#endif
            tb.Foreground = font_brush;
            can.Children.Add(tb);

            Canvas t = new Canvas();
            can.Children.Add(t);
            t.Margin = new Thickness(0, 480, w, h);
            episode.can = t;
            episode.items = new List<Button>();
            episode_vid = new List<string>();
            episode.width = w;
        }
        public static void View_Hide()
        {
            View_VI.sv.Visibility = Visibility.Collapsed;
        }
        public static void View_Show()
        {
            View_VI.sv.Visibility = Visibility.Visible;
        }
        public static void View_Resize(Thickness margin)
        {
            double w = margin.Right - margin.Left;
            double h = margin.Bottom - margin.Top;
            Thickness tk = new Thickness(0,0,w,w);
            View_VI.sv.Margin = margin;
            View_VI.sv.Width = w;
            View_VI.sv.Height = h;
            w -= 20;
            View_VI.img.Width = w;
            View_VI.img.Height = w;
            tk.Top = tk.Bottom;
            tk.Bottom += 40;
            View_VI.download.Margin = tk;
            tk.Top = tk.Bottom;
            tk.Bottom += 40;
            View_VI.title.Margin = tk;
            tk.Left = 0;
            tk.Top = tk.Bottom;
            tk.Bottom =0;
            View_VI.detail.Width = w;
            View_VI.detail.Margin = tk;
            double ht = View_VI.detail.Text.Length * 324 / w;
            tk.Top += ht;
            if (episode_vid.Count>0)
            {
                episode.can.Margin = tk;
                Thickness t = new Thickness();
                for(int i=0;i<episode_vid.Count;i++)
                {
                    if(t.Left +40>w)
                    {
                        t.Left = 0;
                        t.Top += 40;
                    }
                    episode.items[i].Margin = t;
                    t.Left += 40;
                }
                episode.can.Width = w;
                episode.can.Height = t.Top+50;
                tk.Top += t.Top + 50;
            }
            episode.width = w;
            View_VI.can.Width = w;
            View_VI.can.Height = tk.Top;
        }
        public static void Episode_Reset()
        {
            ep_top = 0;
            int len = episode.items.Count;
            for (int c = 0; c < len; c++)
                episode.items[c].Visibility = Visibility.Collapsed;
        }
        static void TV_press(Object sender, PointerRoutedEventArgs e)
        {
            if (DateTime.Now.Ticks - timenow > presstime)
                return;
            ImageContext ic = (ImageContext)(sender as Image).DataContext;
            if (View_VI.download.IsChecked==true)
                CreateLinkFile(ic.vid);
            else
            {
                Player.Setvid(ic.vid);
                Main.CreatePage(2, PageTag.player);
            }
        }
        static void CreateLinkFile(string vid)
        {
            Main.Notify("正在解析地址,请等待几秒", Colors.Green, Colors.White);
            WebClass.GetVideoInfo(vid,SetVideoInfo);
        }
        static void SetVideoInfo(VideoInfoA via)
        {
            VIA = via;
            VideoInfo[] vi = via.vi;
            char[] temp = vi[0].sharp.ToCharArray();
            int s = CharToInt(ref temp);
            int c = 0;
            for (int i = 1; i < vi.Length; i++)
            {
                temp = vi[i].sharp.ToCharArray();
                int t = CharToInt(ref temp);
                if (t > s)
                { s = t; c = i; }
            }
            View_VI.down_index = c;
            WebClass.GetVideoKey(via.vid, via.vi[c], (v, vk) => { VIA.vi[View_VI.down_index].vkey = vk; }, 1);
            WebClass.GetVideoFregment(via.vid, via.vi[c], GetFregment);
        }
        static void GetFregment(int count)
        {
            Main.Notify("正在存储地址", Colors.Red, Colors.White);
            VideoInfo vi = VIA.vi[View_VI.down_index];
            string str = "";
            for(int i=1;i<=count;i++)
            {
               str +=  VIA.http[0] + vi.fn + i.ToString() + ".mp4?vkey=" + vi.vkey
                        + "&type=mp4&fmt=" + vi.fmt+"\r\n";
            }
            FileManage.SaveFile(str,"address.txt");
            DataPackage dp = new DataPackage();
            dp.SetText(str);
            Clipboard.SetContent(dp);
            Main.Notify("已复制到剪切版,链接有效期为3小时，过期请重新生成,保存路径\r\n"+
                ApplicationData.Current.LocalFolder.Path+"\address.txt\r\n打开迅雷即可下载",Colors.Red,Colors.White);
            Timer.Delegate(()=> { Main.NotifyStop();Timer.Stop(); },5000);
        }
        static void TV_EpisodeClick(object sender, RoutedEventArgs e)
        {
            string vid = (sender as Button).DataContext as string;
            if (vid == null)
                return;
            if (View_VI.download.IsChecked == true)
                CreateLinkFile(vid);
            else
            {
                Player.Setvid(vid);
                Main.CreatePage(2, PageTag.player);
            }
        }
        #endregion

        #region tv list mod
        static VideoContainer TV_VC = new VideoContainer();
        public static void TV_Create(Canvas parent, Thickness margin)
        {
            if (TV_VC.sv != null)
            {
                TV_VC.sv.Visibility = Visibility.Visible;
                TV_Reset();
#if desktop
                TV_Resize(margin);
#endif
                return;
            }
            double w = margin.Right - margin.Left;
            double h = margin.Bottom - margin.Top;
            TV_VC.parent = new StackPanel();
            ScrollViewer sv = new ScrollViewer();
            sv.Margin = margin;
            sv.Width = w;
            sv.Height = h;
            TV_VC.sv = sv;
            sv.Content = TV_VC.parent;
            parent.Children.Add(sv);
            TV_VC.LVI = new List<VideoItem>();
        }
        public static void TV_Hide()
        {
            TV_VC.sv.Visibility = Visibility.Collapsed;
        }
        public static void TV_Show()
        {
            TV_VC.sv.Visibility = Visibility.Visible;
        }
        public static void TV_Resize(Thickness margin)
        {
            double w = margin.Right - margin.Left;
            double h = margin.Bottom - margin.Top;
            TV_VC.sv.Margin = margin;
            TV_VC.sv.Width = w;
            TV_VC.sv.Height = h;
        }
        public static void TV_Reset()
        {
            tv_top = 0;
            int len = TV_VC.LVI.Count;
            for (int c = 0; c < len; c++)
            {
                TV_VC.LVI[c].img.Visibility = Visibility.Collapsed;
                TV_VC.LVI[c].textblock.Visibility = Visibility.Collapsed;
            }
        }
        
#endregion

#region comment mod
        static CommentContainer Com_CC = new CommentContainer();
        static bool Com_load,Com_over;
        public static void Com_Create(Canvas parent, Thickness margin)
        {
            if (Com_CC.stack != null)
            {
                Com_CC.sv.Visibility = Visibility.Visible;
                Com_Reset();
                return;
            }
            double w = margin.Right - margin.Left;
            double h = margin.Bottom - margin.Top;
            Com_CC.stack = new StackPanel();
            ScrollViewer sv = new ScrollViewer();
            Com_CC.sv = sv;
            sv.Content = Com_CC.stack;
            sv.Width = w;
            sv.Height = h;
            parent.Children.Add(sv);
            sv.Margin = margin;
            sv.ViewChanged += CommentViewChange;
            Com_CC.son = new List<CommentConponent>();
        }
        public static void Com_Hide()
        {
            Com_CC.sv.Visibility = Visibility.Collapsed;
        }
        public static void Com_Show()
        {
            Com_CC.sv.Visibility = Visibility.Visible;
        }
        public static void Com_Reset()
        {
            if (Com_CC.son == null)
                return;
            int c = Com_CC.son.Count;
            c--;
            if(c>50)
            {
                for (int e = c; e > 50; e--)
                {
                    CommentConponent cc = Com_CC.son[e];
                    Com_CC.son.RemoveAt(e);
                    Com_CC.stack.Children.RemoveAt(e);
                    GC.SuppressFinalize(cc.title);
                    GC.SuppressFinalize(cc.time);
                    GC.SuppressFinalize(cc.content);
                    GC.SuppressFinalize(cc.head);
                    GC.SuppressFinalize(cc.can);
                }
                GC.Collect();
            }
            for (int e = 0; e < Com_CC.son.Count; e++)
                Com_CC.son[e].can.Visibility = Visibility.Collapsed;
        }
        public static void Com_Resize(Thickness margin)
        {
            double w = margin.Right - margin.Left;
            double h = margin.Bottom - margin.Top;
            Com_CC.sv.Width = w;
            Com_CC.sv.Height = h;
            Com_CC.sv.Margin = margin;
        }
        static void CommentViewChange(object sender, ScrollViewerViewChangedEventArgs e)
        {
            double d = Com_CC.sv.Height + Com_CC.sv.VerticalOffset;
            d += 100;
            if (d > Com_CC.sv.ExtentHeight)
            {
                if (Com_load)
                    return;
                if(!Com_over)
                {
                    Com_load = true;
                    GetCommentMore();
                }
            }
        }
#endregion

#region add
        static int tv_top=0,ep_top;
        public static void Hide()
        {
            if(TV_VC.sv!=null)
            TV_VC.sv.Visibility = Visibility.Collapsed;
            View_VI.sv.Visibility = Visibility.Collapsed;
            if(Com_CC.sv!=null)
            Com_CC.sv.Visibility = Visibility.Collapsed;
        }
        public static void Show()
        {
            if (TV_VC.sv != null)
                TV_VC.sv.Visibility = Visibility.Visible;
            View_VI.sv.Visibility = Visibility.Visible;
            if (Com_CC.sv != null)
                Com_CC.sv.Visibility = Visibility.Visible;
        }
        public static void Reset()
        {
            tv_top = 0;
            ep_top = 0;
            int len = TV_VC.LVI.Count;
            for (int c = 0; c < len; c++)
            {
                TV_VC.LVI[c].img.Visibility = Visibility.Collapsed;
                TV_VC.LVI[c].textblock.Visibility = Visibility.Collapsed;
            }
            Com_Reset();
            len = episode.items.Count;
            for (int c = 0; c < len; c++)
                episode.items[c].Visibility = Visibility.Collapsed;
        }
        public static string GetNextVid(string vid)
        {
            if (episode_vid.Count < 2)
                return null;
            for(int i=0;i<episode_vid.Count-1;i++)
                if(vid==episode_vid[i])
                { i++;return episode_vid[i]; }
            return null;
        }
        static void SetView(ImageContext ic)
        {
            if(ic.src!=null)
            View_VI.img.Source = new BitmapImage(new Uri(ic.src));
            View_VI.img.DataContext = ic;
            View_VI.title.Content = ic.title;
            View_VI.title.DataContext = ic.vid;
            if(ic.detail!=null)
            View_VI.detail.Text = ic.detail;
        }
        static void AddTV(ImageContext context)
        {
            if (tv_top < TV_VC.LVI.Count)
            {
                Image img = TV_VC.LVI[tv_top].img;
                img.Visibility = Visibility.Visible;
                img.DataContext = context;
                BitmapImage bi = new BitmapImage();
                bi.UriSource = new Uri(context.src);
                img.Source = bi;
                TextBlock tb = TV_VC.LVI[tv_top].textblock;
                tb.Visibility = Visibility.Visible;
                tb.Text = context.title;
            }
            else
            {
                Image img = new Image();
                TV_VC.parent.Children.Add(img);
                img.Width = 280;
                img.Height = 280;
                img.DataContext = context;
                BitmapImage bi = new BitmapImage(new Uri(context.src));
                img.Source = bi;
                TextBlock tb = new TextBlock();
                TV_VC.parent.Children.Add(tb);
                tb.Text = context.title;
                tb.Foreground = font_brush;
                VideoItem vi = new VideoItem();
                vi.img = img;
                vi.textblock = tb;
                TV_VC.LVI.Add(vi);
            }
            tv_top++;
        }
        static void AddEpisode(string vid)
        {
            if (ep_top < episode.items.Count)
            {
                Button b = episode.items[ep_top];
                b.Visibility = Visibility.Visible;
                ep_top++;
                b.Content = ep_top.ToString();
                b.DataContext = vid;
            }
            else
            {
                Button b = new Button();
                episode.can.Children.Add(b);
                b.Visibility = Visibility.Visible;
                ep_top++;
                b.Content = ep_top.ToString();
                b.DataContext = vid;
                b.Click += TV_EpisodeClick;
                episode.items.Add(b);
            }
        }
        public static async void LoadViewPageData(string url)
        {
            label1:;
            char[] d = url.ToCharArray();
            int c = FindCharArray(ref d,ref Key_vqq,0);
            if(c>0)
            {
                char[] t = new char[d.Length+2];
                for (int i = 0; i < 15; i++)
                    t[i] = d[i];
                t[15] = '/';
                t[16] = 'x';
                c = 15;
                for(int i=17;i<t.Length;i++)
                {
                    t[i] = d[c];
                    c++;
                }
                url = new string(t);
            }
            string cc = await WebClass.GetResults(url);
            if (cc.Length < 4096)
            {
                main_buff = cc.ToCharArray();
                main_buff = DeleteChar(ref main_buff, '\\');
                int s = FindCharArray(ref Key_refresh, 0);
                s = FindCharArray(ref Key_url, s);
                char[] tt;
                if (s < 0)
                {
                    s = FindCharArray(ref Key_href, 0);
                    tt = FindCharArrayA(ref main_buff, '\"', '\"', ref s);
                }
                else
                {
                    s = FindCharArray(ref main_buff, 'h', s);
                    int e = FindCharArray(ref main_buff, '\'', '\"', s);
                    e -= s;
                    tt = CopyCharArry(s, e);
                }
                if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                    url = new string(tt);
                else url = "http:" + new string(tt);
               goto label1;
            }
            ViewAnalyzeM(cc);
        }
        static string cur_vid;
        static void ViewAnalyzeM(string data)
        {
            main_buff = data.ToCharArray();
            main_buff = DeleteChar(ref main_buff, '\\');
            int s = 0;
            string text;
            int t = FindCharArray(ref Key_videoinfo, s);
            string vid;
            ImageContext ic = new ImageContext();
            if (t < 0)
            {
                s = FindCharArray(ref Key_coverinfo, s);
                s = FindCharArray(ref Key_id, s);
                ic.vid = vid = new string(FindCharArrayA(ref main_buff, '\'', '\'', ref s));
                SetView(ic);
                goto label0;
            }
            else
            {
                s = FindCharArray(ref Key_interactionCount, s);
                s += 4;
                text = "播放数:" + new string(FindCharArrayA(ref main_buff, '\"', '\"', ref s));
                s = FindCharArray(ref Key_datePublished, s);
                s += 4;
                text += "  发布时间：" + new string(FindCharArrayA(ref main_buff, '\"', '\"', ref s));
                ic.detail = text;
                s = t;
                s = FindCharArray(ref Key_titleA, s);
                s++;
                ic.title = new string(FindCharArrayA(ref main_buff, '\"', '\"', ref s));
                s = FindCharArray(ref Key_duration, s);
                s++;
                text = new string(FindCharArrayA(ref main_buff, '\"', '\"', ref s));
                ic.time = Convert.ToInt32(text);
                s = FindCharArray(ref Key_vid, s);
                s++;
                vid = new string(FindCharArrayA(ref main_buff, '\"', '\"', ref s));
                ic.vid = vid;

            }
            s = FindCharArray(ref Key_player_figure, 0);
            if (s > 0)
            {
                s = FindCharArray(ref Key_src, s);
                char[] tt = FindCharArrayA(ref main_buff, '\"', '\"', ref s);
                ic.src = "http:" + new string(tt);
            }
            SetView(ic);
            cur_vid = vid;
            //Debug.WriteLine(data);
            int c = ViewPlayListM();
#if desktop
            ViewAnalListM(c);
            LoadComment(vid);
#else
            FindSrc(c);
#endif
            label0:;
        }
#if phone
        static void FindSrc(int start)
        {
            int s = 0;
            if (start < 1)
                s = FindCharArray(ref Key_mod_playlist, s);
            else s = start;
            s = FindCharArray(ref Key_list_item, s);
            int i = 0;
            while (s > 0)
            {
                s = FindCharArray(ref Key_curvid, s);
                if (s < 0)
                    break;
                string vid = new string(FindCharArrayA(ref main_buff, '\'', '\'', ref s));
                string title = FindString(ref Key_title, ref Key_quote, ref s);
                if (title == null)
                    break;
                string src = "http://" + new string(FindCharArrayA(ref main_buff, ref Key_slash, ref Key_quote, ref s));
                if (vid == cur_vid)
                {
                    View_VI.img.Source = new BitmapImage(new Uri(src));
                    return;
                }
                i++;
                s = FindCharArray(ref Key_list_item, s);
            }
        }
#endif
        static int ViewPlayListM()
        {
            episode_vid.Clear();
            int s = 0;
            s = FindCharArray(ref Key_description, s);
            s = FindCharArray(ref Key_content, s);
            string str = new string(FindCharArrayA(ref main_buff, '\"', '\"', ref s));
            View_VI.detail.Text = str;
            s = FindCharArray(ref Key_img, s);
            s = FindCharArray(ref Key_content, s);
            str = new string(FindCharArrayA(ref main_buff, '\"', '\"', ref s));
            s = FindCharArray(ref Key_mod_episode, s);
            int e = FindCharArray(ref Key_mod_playlist, s);
            if (s < 0)
                return e;
            View_VI.img.Source = new BitmapImage(new Uri(str));
            do
            {
                s = FindCharArray(ref Key_curvid, s);
                if (s > e || s < 0)
                    break;
                str = new string(FindCharArrayA(ref main_buff, '\'', '\'', ref s));
                episode_vid.Add(str);
                AddEpisode(str);
            } while (s < e);
            View_Resize(View_VI.sv.Margin);
            return e;
        }
        static void ViewAnalListM(int start)
        {
            int s = 0;
            if (start < 1)
                s = FindCharArray(ref Key_mod_playlist, s);
            else s = start;
            s = FindCharArray(ref Key_list_item, s);
            int i = 0;
            ImageContext ic = new ImageContext();
            while (s > 0)
            {
                s = FindCharArray(ref Key_curvid, s);
                if (s < 0)
                    break;
                ic.vid = new string(FindCharArrayA(ref main_buff, '\'', '\'', ref s));
                s = FindCharArray(ref Key_tl, s);
                char[] cc = FindCharArrayA(ref main_buff, '\"', '\"', ref s);
                ic.time = CharToInt(ref cc);
                ic.title = FindString(ref Key_title, ref Key_quote, ref s);
                if (ic.title == null)
                    break;
                ic.src = "http://" + new string(FindCharArrayA(ref main_buff, ref Key_slash, ref Key_quote, ref s));
                AddTV(ic);
                if (ic.vid == cur_vid)
                    View_VI.img.Source = new BitmapImage(new Uri(ic.src));
                i++;
                s = FindCharArray(ref Key_list_item, s);
            }
        }
#endregion

#region comment load
        static List<CommentInfo> buff_com = new List<CommentInfo>();
        static string cid;
        public static async void LoadComment(string vid)
        {
            Com_load = true;
            Com_over = false;
            string cc = "http://ncgi.video.qq.com/fcgi-bin/video_comment_id?otype=json&op=3&vid=" + vid;
            cid = await WebClass.GetResults(cc,
                "http://v.qq.com/x/cover/k8m3u2xb0nk9eee/t0327opjakl.html");
            //Debug.WriteLine(cur_cid);
            char[] temp = cid.ToCharArray();
            int s = FindCharArray(ref temp, ref Key_comment_id, 0);
            char[] t = FindCharArrayA(ref temp, '\"', '\"', ref s);
            cid = new string(t);
            GetComment("0");
        }
        static async void GetComment(string sid)
        {
            string url = "http://coral.qq.com/article/" + cid + "/comment?otype=xml&commentid=" + sid;
            string cc = await WebClass.GetResults(url,
                "http://v.qq.com/txyp/coralComment_yp_1.0.htm");
            AnalyComment(cc);
        }
        static void AnalyComment(string str)
        {
            //Debug.WriteLine(str);
            char[] temp = str.ToCharArray();
            int s = 0, ss = buff_com.Count;
            CommentInfo ci = new CommentInfo();
            int i;
            for (i = 0; i < 50; i++)
            {
                s = FindCharArray(ref temp, ref Key_js_id, s);
                if (s < 0)
                    break;
                ci.uid = new string(FindCharArrayA(ref temp, '\"', '\"', ref s));
                s = FindCharArray(ref temp, ref Key_js_timeD, s);
                char[] tt = FindCharArrayA(ref temp, '\"', '\"', ref s);
                ci.time = GetString16A(ref tt);
                s = FindCharArray(ref temp, ref Key_js_content, s);
                tt = FindCharArrayA(ref temp, '\"', '\"', ref s);
                ci.content = GetString16(ref tt);
                s = FindCharArray(ref temp, ref Key_js_userid, s);
                ci.uidA = new string(FindCharArrayA(ref temp, '\"', '\"', ref s));
                //s = FindString(ref temp, ref Key_js_useridA, s);
                //ci.uidB = new string(FindCharArrayA(ref temp, ref Key_equal, ref Key_and, ref s));
                s = FindCharArray(ref temp, ref Key_js_nick, s);
                tt = FindCharArrayA(ref temp, '\"', '\"', ref s);
                ci.username = GetString16(ref tt);
                s = FindCharArray(ref temp, ref Key_js_head, s);
                tt = FindCharArrayA(ref temp, '\"', '\"', ref s);
                ci.url = new string(DeleteChar(ref tt, '\\'));
                s = FindCharArray(ref temp, ref Key_js_vip, s);
                ci.vip = new string(FindCharArrayA(ref temp, '\"', '\"', ref s));
                s = FindCharArray(ref temp, ref Key_js_region, s);
                tt = FindCharArrayA(ref temp, '\"', '\"', ref s);
                ci.region = GetString16(ref tt);
                buff_com.Add(ci);
            }
            if (i > 0)
                CommentLoad(ss, i);
            else Com_over = true;
        }
        static void GetCommentMore()
        {
            int s = buff_com.Count - 1;
            string sid = buff_com[s].uid;
            GetComment(sid);
        }
        static void CommentLoad(int s, int count)
        {
            List<CommentConponent> temp = Com_CC.son;
            int len = temp.Count;
            int all = buff_com.Count;
            int i;
            for (i = s; i < len & i < all; i++)
            {
                temp[i].can.Visibility = Visibility.Visible;
                BitmapImage bi = new BitmapImage();
                bi.UriSource = new Uri(buff_com[i].url);
                temp[i].head.Source = bi;
                temp[i].title.Text = buff_com[i].username;
                temp[i].time.Text = buff_com[i].time + buff_com[i].region;
                temp[i].content.Text = buff_com[i].content;
            }
            CommentConponent CC = new CommentConponent();
            Thickness tk = new Thickness();
            while (i < all)
            {
                Canvas can = new Canvas();
                Com_CC.stack.Children.Add(can);
                CC.can = can;
                Image img = new Image();
                CC.head = img;
                img.Width = 48;
                img.Height = 48;
                BitmapImage bi = new BitmapImage();
                if (buff_com[i].url.Length > 10)
                    bi.UriSource = new Uri(buff_com[i].url);
                else bi.UriSource = new Uri("ms-appx:///Pic/User.png");
                img.Source = bi;
                can.Children.Add(img);
                TextBlock tb = new TextBlock();
                CC.title = tb;
                can.Children.Add(tb);
                tk.Top = 0;
                tk.Left = 54;
                tb.Margin = tk;
                tb.Foreground = font_brush;
                string str = buff_com[i].username + "  uid:" + buff_com[i].uidA;// + "  mid:" + buff_com[i].uidB;
                tb.Text = str;
                tb = new TextBlock();
                CC.time = tb;
                can.Children.Add(tb);
                tk.Top = 24;
                tb.Margin = tk;
                tb.Foreground = font_brush;
                tb.Text = buff_com[i].time + buff_com[i].region;
                tb = new TextBlock();
                CC.content = tb;
                can.Children.Add(tb);
                tk.Left = 0;
                tk.Top = 54;
                tb.Margin = tk;
                tb.Width = 380;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Foreground = font_brush;
                tb.Text = buff_com[i].content;
                Com_CC.son.Add(CC);
                int c = buff_com[i].content.Length;
                c /= 21;
                c++;
                c *= 21;
                c += 70;
                can.Height = c;
                i++;
            }
            Com_load = false;
        }
#endregion
    }
}
