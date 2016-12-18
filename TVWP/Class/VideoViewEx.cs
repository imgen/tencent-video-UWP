using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TVWP.Control;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    class VideoPage : CharOperation, Navigation
    {
        #region
        struct Cover_info
        {
            public string title;
            public string s_title;
            public string cid;
            public string pic;
        }
        static string[] t_rel = { "相关视频", "Related video" };
        static string[] t_vid = { "视频信息", "Viewd info" };
        static string[] t_com = { "评论", "Comment" };
        static string[][] t_all = { t_rel, t_vid, t_com };

        static string[] c_hot = { "热评", "Hot_Com" };
        static string[] c_up = { "点评", "Up_Com" };
        static string[] c_com = { "评论", "Comment" };
        static string[][] c_all = { c_com,c_hot,c_up };

        static char[] Key_m = "m.".ToCharArray();
        static string type;
        static string site;
        static Cover_info ci;
        static VideoPanel vp;
        static ScrollViewA sva;
        static ScrollViewC svc;
        static ScrollViewC hvc;
        static Scroll_UC suc;
        static string cur_vid;
        static Thickness margin;
        struct Header
        {
            public Canvas can;
            public Button[] nav;
        }
        static Header head;
        static Header c_head;
        static Canvas parent;
        static void CreateHead(Canvas p, Thickness m)//pc
        {
            if (head.can != null)
            {
                head.can.Visibility = Visibility.Visible;
                return;
            }
            Canvas can = new Canvas();
            head.can = can;
            p.Children.Add(can);
            head.nav = new Button[3];
            for (int i = 0; i < 3; i++)
            {
                Button b =Component.CreateButtonNext();
                head.nav[i] = b;
                b.Content = t_all[i][Setting.language];
                b.Foreground = Component.nav_brush;
                b.Background = Component.trans_brush;
                b.FontSize = 18;
                can.Children.Add(b);
            }
        }
        static void CreateBar(Canvas p,Thickness m)
        {
            if (c_head.can != null)
            {
                c_head.can.Visibility = Visibility.Visible;
                return;
            }
            Canvas can = new Canvas();
            c_head.can = can;
            p.Children.Add(can);
            c_head.nav = new Button[3];
            for (int i = 0; i < 3; i++)
            {
                Button b = Component.CreateButtonNext();
                b.BorderBrush = Component.filter_brush;
                c_head.nav[i] = b;
                b.Content = c_all[i][Setting.language];
                b.Foreground = Component.nav_brush;
                b.Background = Component.trans_brush;
                b.FontSize = 18;
                can.Children.Add(b);
            }
            ItemClick ic = new ItemClick();
            ic.tag = 0;
            ic.click = (o) =>
            {
                hvc.Hide();
                suc.Hide();
                svc.Show();
                c_head.nav[0].Background = Component.tag_brush_b;
                c_head.nav[1].Background = null;
                c_head.nav[2].Background = null;
            };
            c_head.nav[0].DataContext = ic;

            ic = new ItemClick();
            ic.tag = 0;
            ic.click = (o) =>
            {
                svc.Hide();
                suc.Hide();
                hvc.Show();
                c_head.nav[1].Background = Component.tag_brush_b;
                c_head.nav[0].Background = null;
                c_head.nav[2].Background = null;
            };
            c_head.nav[1].DataContext = ic;

            ic = new ItemClick();
            ic.tag = 0;
            ic.click = (o) =>
            {
                hvc.Hide();
                svc.Hide();
                suc.Show();
                c_head.nav[2].Background = Component.tag_brush_b;
                c_head.nav[1].Background = null;
                c_head.nav[0].Background = null;
            };
            c_head.nav[2].DataContext = ic;
        }
        #endregion


        static PivotPage pp;
        static int index;
        static void CreatePage(Canvas p,Thickness m)
        {
            if(vp!=null)
            {
#if desktop
                head.can.Visibility = Visibility.Visible;
#else
                pp.pivot.Visibility = Visibility.Visible;
#endif
                Resize(m);
                return;
            }
            parent = p;
            vp = new VideoPanel();
            sva = new ScrollViewA();
            sva.itemclick = (o) => {
                string vid = o as string;
                PlayControl(vid);
            };
            svc = new ScrollViewC();
            hvc = new ScrollViewC();
            suc = new Scroll_UC();
#if desktop
            CreateHead(p,m);
            CreateBar(head.can,m);
            vp.ShowBorder();
            vp.SetParent(head.can);
            sva.ShowBorder();
            sva.SetParent(head.can);
            svc.ShowBorder();
            svc.SetParent(head.can);
            hvc.ShowBorder();
            hvc.SetParent(head.can);
            suc.ShowBorder();
            suc.SetParent(head.can);
#else
            Component.CreatePivot(ref pp, ref t_all);
            Component.ResizePivot(ref pp, m);
            p.Children.Add(pp.pivot);
            vp.SetParent(pp.son[1]);
            sva.SetParent(pp.son[0]);
            svc.SetParent(pp.son[2]);
            hvc.SetParent(pp.son[2]);
            suc.SetParent(pp.son[2]);
            CreateBar(pp.son[2],m);
            index = 1;
            pp.pivot.SelectedIndex = 1;
            pp.pivot.SelectionChanged += (o, e) => {
                pp.head[index].Background = Component.trans_brush;
                index = pp.pivot.SelectedIndex;
                pp.head[index].Background = Component.tag_brush_b;
            };
#endif
            Resize(m);
            svc.Getmore = () => {
                int c = svc.lci.Count - 1;
                GetComment(svc.lci[c].m_id);
            };
            hvc.Getmore = () => {
                int c = hvc.lci.Count - 1;
                GetComment(hvc.lci[c].m_id);
            };
            suc.Getmore = () => {
                int c = suc.lci.Count - 1;
                GetUpComment(suc.lci[c].m_id);
            };
            hvc.Hide();
            suc.Hide();
        }
        static void  Resize(Thickness m)
        {
            if (margin == m)
                return;
            margin = m;
#if desktop
            double w = m.Right - m.Left;
            double h = m.Bottom - m.Top;
            double dx = w * 0.3333333f;
            for (int i = 0; i < 3; i++)
            {
                head.nav[i].Margin = new Thickness(i * dx, 0, 0, 0);
                head.nav[i].Width = dx;
            }
            head.can.Width = w;
            m.Top += 40;
            m.Right = dx - 10;
            sva.ReSize(m);
            sva.Refresh();
            m.Left = dx;
            m.Right += dx;
            vp.ReSize(m);
            vp.Refresh();
            m.Left += dx;
            m.Right += dx;
            m.Bottom -= 40;
            svc.ReSize(m);
            svc.Refresh();
            hvc.ReSize(m);
            hvc.Refresh();
            suc.ReSize(m);
            suc.Refresh();
            w = m.Right - m.Left;
            m.Top = m.Bottom;
            m.Bottom += 40;
            c_head.can.Margin = m;
            dx = w / 3;
            for (int i = 0; i < 3; i++)
            {
                c_head.nav[i].Margin = new Thickness(i * dx, 0, 0, 0);
                c_head.nav[i].Width = dx;
            }
#else
            Component.ResizePivot(ref pp, m);
            m.Bottom -= 90;
            vp.ReSize(m);
            sva.ReSize(m);
            svc.ReSize(m);
            svc.Refresh();
            hvc.ReSize(m);
            svc.Refresh();
            suc.ReSize(m);
            suc.Refresh();
            m.Top = m.Bottom;
            m.Bottom += 40;
            c_head.can.Margin = m;
            double w = m.Right - m.Left;
            double dx = w / 3;
            for (int i = 0; i < 3; i++)
            {
                c_head.nav[i].Margin = new Thickness(i * dx, 0, 0, 0);
                c_head.nav[i].Width = dx;
            }
#endif
        }
        public static void SetAdress(string url)
        {
            char[] c_buff = url.ToCharArray();
            int t = FindCharArray(ref c_buff, ref Key_m, 0);
            if(t>-1)//mobile
            {
                type = "m";//mobile
                site = url;
                NetClass.TaskGet(site,AnalyzeM,site);
                return;
            }
            else//desktop
            {
                t = FindCharArray(ref c_buff, ref Key_x, 0);
                if (t < 0)
                {
                    int c = FindCharArray(ref c_buff, ref Key_qq, 0);
                    c--;
                    type = new string(FindCharArrayA(ref c_buff, '/', '/', ref c));
                    if (type == "live")
                    { site = url; goto label0; }
                    int l = c_buff.Length - 1;
                    int i;
                    for (i = l; i > -1; i--)
                    {
                        if (c_buff[i] == '/')
                            break;
                    }
                    string tag = new string(FindCharArrayA(ref c_buff, '/', '.', ref i));
                    site = "http://v.qq.com/x/" + type + "/" + tag + ".html";
                }
                else
                {
                    site = url;
                    t -= 2;
                    type = new string(FindCharArrayA(ref c_buff, '/', '/', ref t));
                }
            }
            label0:;
            NetClass.TaskGet(site,Analyze,site);
        }
        static void Analyze(string data)
        {
            if (data.Length < 4096)
            {
                char[] c_buff = data.ToCharArray();
                c_buff = DeleteChar(ref c_buff, '\\');
                int o = FindCharArray(ref c_buff, ref Key_refresh, 0);
                if (o < 0)
                {
                    o = FindCharArray(ref c_buff, ref Key_split, 0);
                    if (o < 0)
                        return;
                    o += 6;
                    site += new string(FindCharArrayA(ref c_buff, '\'', '\'', ref o));
                    NetClass.TaskGet(site, Analyze, site);
                    return;
                }
                o = FindCharArray(ref c_buff, ref Key_url, o);
                char[] tt;
                if (o < 0)
                {
                    o = FindCharArray(ref c_buff, ref Key_href, 0);
                    tt = FindCharArrayA(ref c_buff, '\"', '\"', ref o);
                }
                else
                {
                    o = FindCharArray(ref c_buff, 'h', o);
                    int e = FindCharArray(ref c_buff, '\'', '\"', o);
                    e -= o;
                    tt = CopyCharArry(ref c_buff, o, e);
                }
                if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                    SetAdress(new string(tt));
                else SetAdress("http:" + new string(tt));
                return;
            }
            vp.data.Clear();
            sva.data.Clear();
            char[] c = data.ToCharArray();
            c = DeleteChar(ref c,'\\');
            if (type == "cover")
                GetEp_infoA(ref c,vp.data);
            int s = GetCoverInfo(ref c);
            if (s < 0)
                s = 0;
            vp.vi = GetVideoInfo(ref c, s);
            GetListInfo(ref c, sva.data, s);
            if (type=="cover")
            {
                GetCoverList(ref c,sva.data,s,vp.data.Count);
            }
            else
            {
                GetPageList(ref c,sva.data);
            }
            s = FindCharArray(ref c, ref Key_player_figure, 0);
            if (s > 0)
            {
                s = FindCharArray(ref c, ref Key_src, s);
                char[] tt = FindCharArrayA(ref c, '\"', '\"', ref s);
                vp.vi.src = "http:" + new string(tt);
            }
            cur_vid = vp.vi.vid;
            UpdatePage();
        }
        static void AnalyzeM(string data)
        {
            vp.data.Clear();
            sva.data.Clear();
            char[] c = data.ToCharArray();
            c = DeleteChar(ref c, '\\');
            ImageContext ic = ParseData.Des_PlayPage(c, vp.data, sva.data);
            cur_vid = ic.vid;
            vp.vi = ic;
            UpdatePage();
        }
        static void UpdatePage()
        {
            svc.ReSet();
            hvc.ReSet();
            suc.ReSet();
            vp.Refresh();
            sva.Refresh();
            svc.Refresh();
            suc.Refresh();
            LoadComment(cur_vid);
        }
        static int GetEp_infoA(ref char[] c_buff, List<EP_Info> lep)
        {
            int s = 0;
            s = FindCharArray(ref c_buff, ref Key_mod_episode, s);
            if (s < 0)
                return 0;
            int e = FindCharArray(ref c_buff, ref Key_mod_playlist, s);
            EP_Info ep = new EP_Info();
            do
            {
                s = FindCharArray(ref c_buff, ref Key_curvid, s);
                if (s > e || s < 0)
                    break;
                ep.vid = new string(FindCharArrayA(ref c_buff, '\'', '\'', ref s));
                s = FindCharArray(ref c_buff, ref Key_titleA, s);
                ep.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                lep.Add(ep);
            } while (s < e);
            return e;
        }
        static int GetCoverInfo(ref char[] c_buff)
        {
            int s = FindCharArray(ref c_buff, ref Key_coverinfo, 0);
            if (s < 0)
            {
                ci.cid = null;
                return -1;
            }
            int t = FindCharArray(ref c_buff, ref Key_titleA, s);
            ci.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
            t++;
            ci.s_title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
            t = FindCharArray(ref c_buff, ref Key_id, t);
            ci.cid = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
            t = FindCharArray(ref c_buff, ref Key_pic, t);
            char[] c = FindCharArrayA(ref c_buff, '\"', '\"', ref t);
            if (c[0] != 'h')
                ci.pic = "http:" + new string(c);
            else ci.pic = new string(c);
            return t;
        }
        static ImageContext GetVideoInfo(ref char[] c_buff ,int s)
        {
            ImageContext ic = new ImageContext();
            s = FindCharArray(ref c_buff, ref Key_interactionCount, s);
            s += 4;
            string text = "播放数:" + new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            s = FindCharArray(ref c_buff, ref Key_datePublished, s);
            s += 4;
            text += "  发布时间：" + new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            ic.detail = text;
            int d = FindCharArray(ref c_buff, ref Key_description, 0);
            if (d > 0)
            {
                d = FindCharArray(ref c_buff, ref Key_content, d);
                d++;
                ic.detail += "\r\n" + new string(FindCharArrayA(ref c_buff, '\"', '\"', ref d));
            }
            s = FindCharArray(ref c_buff, ref Key_img, s);
            d = FindCharArray(ref c_buff, ref Key_content, s);
            ic.src += new string(FindCharArrayA(ref c_buff, '\"', '\"', ref d));
            s = FindCharArray(ref c_buff, ref Key_videoinfo, s);
            if (s < 0)
                return ic;
            s = FindCharArray(ref c_buff, ref Key_titleA, s);
            ic.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            s++;
            s = FindCharArray(ref c_buff, ref Key_duration, s);
            s++;
            text = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            ic.time = Convert.ToInt32(text);
            s = FindCharArray(ref c_buff, ref Key_vidB, s);
            s++;
            ic.vid = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            return ic;
        }
        static void GetListInfo(ref char[] c_buff,List<ItemDataA> data,int s)
        {
            int t = FindCharArray(ref c_buff,ref Key_listinfo,s);
            if (t < 0)
                return;
            int e = FindCharArray(ref c_buff,ref Key_listinfoE,t);
            t = FindCharArray(ref c_buff,ref Key_data,t);
            t = FindCharArray(ref c_buff,ref Key_vidB,t,e);
            ItemDataA ei = new ItemDataA();
            while(t>0)
            {
                ei.detail = new string(FindCharArrayA(ref c_buff,'\"','\"',ref t));
                t = FindCharArray(ref c_buff,ref Key_duration,t);
                t++;
                ei.href= new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                t = FindCharArray(ref c_buff,ref Key_titleA,t);
                t++;
                ei.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                t = FindCharArray(ref c_buff,ref Key_preview,t);
                char[] c = FindCharArrayA(ref c_buff, '\"', '\"', ref t);
                if (c[0] != 'h')
                    ei.src = "http:" + new string(c);
                else ei.src = new string(c);
                t = FindCharArray(ref c_buff, ref Key_vidB, t, e);
                data.Add(ei);
            }
        }
        static void GetCoverList(ref char[] c_buff, List<ItemDataA> lic, int start, int ec)
        {
            int e = FindCharArray(ref c_buff, ref Key_listinfoE, start);
            if (e < 0)
                e = c_buff.Length;
            int s = start;
            s = FindCharArray(ref c_buff, ref Key_data, s);
            s = FindCharArray(ref c_buff, ref Key_vidB, s, e);
            int index = ec;
            int count = 0;
            ItemDataA ic = new ItemDataA();
            while (s > 0)
            {
                ic.detail = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                s = FindCharArray(ref c_buff, ref Key_titleA, s);
                s++;
                ic.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                s = FindCharArray(ref c_buff, ref Key_preview, s);
                if (s < 0)
                    break;
                char[] t = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                if (FindCharArray(ref t, ref Key_http, 0) > -1)
                    ic.src = new string(t);
                else ic.src = "http:" + new string(t);
                if (count >= index)
                    lic.Add(ic);
                s = FindCharArray(ref c_buff, ref Key_vidB, s, e);
                count++;
            }
        }
        static void GetPageList(ref char[] c_buff, List<ItemDataA> lic)
        {
            int s = FindCharArray(ref c_buff, ref Key_mod_playlist, 0);
            if (s < 0)
                return;
            ItemDataA ic = new ItemDataA();
            s = FindCharArray(ref c_buff, ref Key_list_itemA, s);
            int e = FindCharArray(ref c_buff, ref Key_ul_e, s);
            int c;
            while (s > 0)
            {
                c = FindCharArray(ref c_buff, ref Key_href, s);
                char[] href = FindCharArrayA(ref c_buff, '\"', '\"', ref c);
                int a = FindCharArray(ref href, ref Key_vidA, 0);
                if (a > 0)
                    ic.detail = new string(CopyCharArry(ref href, a, href.Length - a));
                else goto label0;
                ic.href = "http://v.qq.com" + new string(href);
                c = FindCharArray(ref c_buff, ref Key_src, c);
                char[] t = FindCharArrayA(ref c_buff, '\"', '\"', ref c);
                if (FindCharArray(ref t, ref Key_http, 0) > -1)
                    ic.src = new string(t);
                else ic.src = "http:" + new string(t);
                c = FindCharArray(ref c_buff, ref Key_alt, c);
                ic.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref c));
                lic.Add(ic);
                label0:;
                s = FindCharArray(ref c_buff, ref Key_list_itemA, c);
                if (s > e)
                    break;
            }
        }

        static bool loadC,loadH,loadU;
        static string cid;
        static string pid;
        static void LoadComment(string vid)
        {
            if (vid == null)
                return;
            char[] c = site.ToCharArray();
            int t = FallFindCharArray(ref c,'/',c.Length-1);
            t--;
            pid = new string(FindCharArrayA(ref c, '/', '.', ref t)) + "?";
            loadC = false;
            string cc = "http://ncgi.video.qq.com/fcgi-bin/video_comment_id?otype=json&op=3&vid=" + vid;
            WebClass.TaskGet(cc, TaskCid, "http://v.qq.com/x/cover/k8m3u2xb0nk9eee/t0327opjakl.html");
        }
        static void TaskCid(string data)
        {
            char[] temp = data.ToCharArray();
            int s = FindCharArray(ref temp, ref Key_comment_id, 0);
            char[] t = FindCharArrayA(ref temp, '\"', '\"', ref s);
            cid = new string(t);
            GetComment("0");
            GetHotComment("0");
            GetUpComment(null);
        }
        static void GetComment(string sid)
        {
            if (loadC)
                return;
            loadC = true;
            string url = "http://coral.qq.com/article/" + cid + "/comment?otype=xml&reqnum=20&commentid=" + sid;
            WebClass.TaskGet(url, AnalyComment, "http://v.qq.com/txyp/coralComment_yp_1.0.htm");
        }
        static void AnalyComment(string str)
        {
            char[] c_buff = str.ToCharArray();
            int i = (ParseData.AnalyComment(ref c_buff, svc.lci));
            if (i > 0)
                svc.Refresh();
            else svc.LoadOver = true;
            loadC = false;
        }
        static void GetHotComment(string sid)
        {
            if (loadH)
                return;
            loadH = true;
            string url = "https://coral.qq.com/article/" + cid + "/hotcomment?reqnum=20&commentid=" + sid;
            WebClass.TaskGet(url, AnalyHotComment, "http://v.qq.com/txyp/coralComment_yp_1.0.htm");
        }
        static void AnalyHotComment(string str)
        {
            char[] c_buff = str.ToCharArray();
            int i = (ParseData.AnalyComment(ref c_buff, hvc.lci));
            if (i > 0)
                hvc.Refresh();
            else hvc.LoadOver = true;
            loadH = false;
        }
        static void GetUpComment(string sid)
        {
            if (loadU)
                return;
            loadU = true;
            string url = "https://video.coral.qq.com/filmreviewr/c/upcomment/"+pid+ "reqnum=10";
            if (sid != null)
                url += "&commentid=" + sid;
            WebClass.TaskGet(url, AnalyUpComment, "http://v.qq.com/txyp/coralComment_yp_1.0.htm");
        }
        static void AnalyUpComment(string str)
        {
            char[] c_buff = str.ToCharArray();
            int i = (ParseData.AnalyUpComment(ref c_buff, suc.lci));
            if (i > 0)
                suc.Refresh();
            else suc.LoadOver = true;
            loadU = false;
        }

        public bool Back()
        {
            Hide();
            return false;
        }
        public void Create(Canvas p, Thickness m)
        {
            CreatePage(p,m);
        }
        public void Hide()
        {
#if desktop
            head.can.Visibility = Visibility.Collapsed;
#else
            pp.pivot.Visibility = Visibility.Collapsed;
#endif
        }
        public void ReSize(Thickness m)
        {
            Resize(m);
        }
        public void Show()
        {
#if desktop
            head.can.Visibility = Visibility.Visible;
#else
            pp.pivot.Visibility = Visibility.Visible;
#endif
        }

        public static void PlayControl(string vid)
        {
            int index = vp.operation;
            switch (index)
            {
                case 1:
                    DownLoad.AddVid(vid);
                    Main.Notify("已添加至缓存列表", Component.nav_brush, Component.font_brush, 300);
                    break;
                default:
                    Player.Setvid(vid);
                    PageManageEx.CreateNewPage(PageTag.player);
                    break;
            }
        }
        //static VideoAddress va;
        //static void CreateLinkFile(string vid)
        //{
        //    Main.Notify("正在解析地址,请等待几秒", Component.nav_brush, Component.font_brush, 0);
        //    if (va == null)
        //        va = new VideoAddress();
        //    va.SetVid(vid, 720, (s) => {
        //        VideoInfo vi = new VideoInfo();
        //        va.GetAddress(ref vi);
        //        string str = "";
        //        if (vi.type == 1)
        //            str = vi.href;
        //        else
        //        {
        //            int count = vi.part;
        //            for (int i = 1; i <= count; i++)
        //                str += vi.href + i.ToString() + vi.vkey + "\r\n";
        //        }
        //        DataPackage dp = new DataPackage();
        //        dp.SetText(str);
        //        Clipboard.SetContent(dp);
        //        Main.Notify("已复制到剪切版,链接有效期为3小时，过期请重新生成,打开迅雷即可下载", Component.warning_brush, Component.font_brush, 5000);
        //        Timer.Delegate(() => { Main.NotifyStop(); }, 5000);
        //    });
        //}
        public static string GetNextVid(string vid)
        {
            int c = vp.data.Count - 1;
            if (c > 0)
                for (int i = 0; i < c; i++)
                    if (vid == vp.data[i].vid)
                        return (vp.data[c + 1].vid);
            return null;
        }
    }

    class VideoPanel:Component,IDisposable
    {
        static string[] o_play = { "播放", "Play" };
        static string[] o_downA = { "缓存", "Buff" };
        struct VideoInfoView
        {
            public Canvas can;
            public ScrollViewer sv;
            public Image img;
            public BitmapImage map;
            public Button title;
            public TextBlock detail;
            public ListBoxH option;
            public ListViewG gv;
        }
        VideoInfoView viv;
        public VideoPanel()
        {
            data = new List<EP_Info>();
            ScrollViewer sv = new ScrollViewer();
            viv.sv = sv;
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            viv.img = new Image();
            viv.map = new BitmapImage();
            viv.img.Source = viv.map;
            viv.img.PointerReleased += (o, e) => {
                object t = viv.img.DataContext;
                if (t != null)
                    VideoPage.PlayControl(t as string);
            };
            viv.title = new Button();
            viv.title.Click += (o, e) => {
                object t = viv.title.DataContext;
                if (t != null)
                    VideoPage.PlayControl(t as string);
            };
            viv.title.Background = trans_brush;
            viv.title.Foreground = title_brush;
            viv.detail = CreateTextBlockNext();
            viv.detail.TextWrapping = TextWrapping.Wrap;
            viv.detail.Foreground = font_brush;
            viv.gv = new ListViewG();
            viv.gv.Visibility = Visibility.Collapsed;
            viv.gv.DataContext = 170;
            viv.gv.GetListBox.SelectionChanged += (o, e) =>
            {
                int index = (o as ListView).SelectedIndex;
                if (index > -1)
                    VideoPage.PlayControl(data[index].vid);
            };
            ListBoxH lbh = new ListBoxH();
            ListBox lb = lbh.GetListBox;
            lb.Items.Add(o_play[Setting.language]);
            lb.Items.Add(o_downA[Setting.language]);
            lb.Foreground = font_brush;
            viv.option = lbh;
            Canvas can = new Canvas();
            viv.can = can;
            viv.sv.Content = can;
            can.Children.Add(viv.img);
            can.Children.Add(viv.title);
            can.Children.Add(viv.option);
            can.Children.Add(viv.detail);
            can.Children.Add(viv.gv);

        }
        Canvas parent;
        Thickness margin;
        public int operation { get { return viv.option.GetListBox.SelectedIndex; }
            set { viv.option.GetListBox.SelectedIndex = operation; } }
        public void SetParent(Canvas p)
        {
            parent = p;
            p.Children.Add(viv.sv);
        }
        public List<EP_Info> data;
        public ImageContext vi;
        public void Dispose()
        {
            parent.Children.Remove(viv.sv);
            viv.sv.Content = null;
            viv.can.Children.Clear();
            RecycleTextBlock(viv.detail);
            GC.SuppressFinalize(viv.img);
            GC.SuppressFinalize(viv.map);
            GC.SuppressFinalize(viv.title);
            GC.SuppressFinalize(viv.option);
            GC.SuppressFinalize(viv.gv);
            GC.SuppressFinalize(data);
            GC.SuppressFinalize(this);
        }
        double width,height;
        double oy;
        public void ReSize(Thickness m)
        {
            if (margin == m)
                return;
            margin = m;
            width = m.Right - m.Left;
            height = m.Bottom - m.Top;
            ScrollViewer sv = viv.sv;
            sv.Margin = m;
            sv.Width = width;
            sv.Height = height;
            double iw;
            if (width < 280)
                iw = width;
            else iw = 280;
            double ih = iw * 1.3f;
            viv.img.Width = iw;
            viv.img.Height = ih;
            viv.img.Margin = new Thickness((width - iw) * 0.5f, -50, 0, 0);
            iw -= 50;
            viv.title.Margin = new Thickness(10, iw, 0, 0);
            ih = iw + 40;
            viv.option.Margin = new Thickness(10, ih, 0, 0);
            ih += 50;
            double dw = width - 10;
            viv.detail.Width = dw;
            viv.detail.Margin = new Thickness(5, ih, 0, 0);
            oy = ih;
            viv.gv.Width = dw;
            viv.can.Width = width;
            if (ih < height)
                viv.can.Height = height;
            else viv.can.Height = ih;
        }
        public void Refresh()
        {
            if (vi.vid == null)
            {
                viv.img.Visibility = Visibility.Collapsed;
                viv.title.Visibility = Visibility.Collapsed;
                viv.detail.Text = "视频不见了";
                return;
            }
            viv.img.Visibility = Visibility.Visible;
            viv.title.Visibility = Visibility.Visible;
            if (vi.src != null & vi.src != "")
                viv.map.UriSource = new Uri(vi.src);
            viv.img.DataContext = vi.vid;
            viv.title.Content = vi.title;
            viv.title.DataContext = vi.vid;
            if (vi.detail != null)
                viv.detail.Text = vi.detail;
            else viv.detail.Text = "";
            int c = data.Count;
            double dw = width - 10;
            double ht = viv.detail.Text.Length * 330 / dw;
            dw = oy + ht;
            viv.gv.Margin = new Thickness(5, dw, 0, 0);
            if (c > 0)
            {
                viv.gv.Visibility = Visibility.Visible;
                ListView lv = viv.gv.GetListBox;
                lv.Items.Clear();
                for (int i = 0; i < c; i++)
                    lv.Items.Add(data[i].title);
                double h = 200;
                if (dw + h < height)
                    h = height - dw - 5;
                viv.gv.Height = h;
                dw += h;
            }
            else viv.gv.Visibility = Visibility.Collapsed;
            if (dw < height)
                viv.can.Height = height;
            else viv.can.Height = dw;
        }
        public void Hide()
        {
            viv.sv.Visibility = Visibility.Collapsed;
        }
        public void Show()
        {
            viv.sv.Visibility = Visibility.Visible;
        }
        public void ShowBorder()
        {
            viv.sv.BorderBrush = bor_brush;
            viv.sv.BorderThickness = new Thickness(1, 1, 1, 1);
        }
    }
}
