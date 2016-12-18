using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    class HomePage_m
    {
        #region main img area
        static Cla_data nav_es = new Cla_data { count = 5, title = new string[] { "独家精选", "Exclusive selection" }, page = PageAddress.ExclusiveSeletion };
        static Cla_data nav_e2 = new Cla_data { count = 5, title = new string[] { "", "" }, page = PageAddress.ExclusiveSeletion };
        static Cla_data nav_hl = new Cla_data { count = 5, title = new string[] { "热闻一览", "Hot List" }, page = PageAddress.OriginalSelection };
        static Cla_data nav_pv = new Cla_data { count = 5, title = new string[] { "热门综艺", "Popular Variety" }, page = PageAddress.PopularVariety };
        static Cla_data nav_fy = new Cla_data { count = 3, title = new string[] { "", "" }, page = PageAddress.None };
        static Cla_data nav_f2 = new Cla_data { count = 3, title = new string[] { "", "" }, page = PageAddress.None };
        static Cla_data nav_st = new Cla_data { count = 5, title = new string[] { "同步剧场", "Synchronous theater" }, page = PageAddress.SynchronousTheater };
        static Cla_data nav_mb = new Cla_data { count = 5, title = new string[] { "电影大片", "Movie blockbuster" }, page = PageAddress.MovieBlockbuster };
        static Cla_data nav_vi = new Cla_data { count = 5, title = new string[] { "VIP", "VIP" }, page = PageAddress.None };
        static Cla_data nav_nb = new Cla_data { count = 5, title = new string[] { "NBA专区", "NBA area" }, page = PageAddress.None };
        static Cla_data nav_eh = new Cla_data { count = 5, title = new string[] { "娱乐热点", "Entertainment hot spots" }, page = PageAddress.Entertainment };
        static Cla_data nav_as = new Cla_data { count = 5, title = new string[] { "动漫精选", "Animation selection" }, page = PageAddress.Animation };
        static Cla_data nav_ch = new Cla_data { count = 5, title = new string[] { "少儿", "Children" }, page = PageAddress.Children };
        static Cla_data nav_at = new Cla_data { count = 5, title = new string[] { "美剧·英剧", "American TV series" }, page = PageAddress.AmericanTV };
        static Cla_data nav_nd = new Cla_data { count = 5, title = new string[] { "网络剧", "Network drama" }, page = PageAddress.Documentary };
        static Cla_data nav_im = new Cla_data { count = 5, title = new string[] { "网络电影", "Internet Movie" }, page = PageAddress.Microfilm };
        static Cla_data nav_os = new Cla_data { count = 5, title = new string[] { "原创精选", "Original selection" }, page = PageAddress.OriginalSelection };
        static Cla_data nav_fu = new Cla_data { count = 5, title = new string[] { "最强笑点", "The strongest laugh point" }, page = PageAddress.laughPoint };
        static Cla_data nav_ec = new Cla_data { count = 5, title = new string[] { "独家演唱会", "Exclusive concert" }, page = PageAddress.None };
        static Cla_data nav_mv = new Cla_data { count = 5, title = new string[] { "MV八音盒", "MV music box" }, page = PageAddress.None };
        static Cla_data nav_si = new Cla_data { count = 5, title = new string[] { "体育资讯", "Sports information" }, page = PageAddress.None };
        static Cla_data nav_fh = new Cla_data { count = 5, title = new string[] { "时尚热度榜", "Fashion heat list" }, page = PageAddress.Fashion };
        static Cla_data nav_li = new Cla_data { count = 5, title = new string[] { "生活资讯", "Living Information" }, page = PageAddress.Living };
        static Cla_data nav_ca = new Cla_data { count = 5, title = new string[] { "汽车资讯", "Automotive Information" }, page = PageAddress.Automotive };
        static Cla_data nav_do = new Cla_data { count = 5, title = new string[] { "纪录片", "Documentary" }, page = PageAddress.Documentary };
        static Cla_data nav_fc = new Cla_data { count = 5, title = new string[] { "财经课堂", "Finance Classroom" }, page = PageAddress.Finance };
        static Cla_data nav_ba = new Cla_data { count = 5, title = new string[] { "母婴常识", "Mother and child knowledge" }, page = PageAddress.MotherAndChild };
        static Cla_data nav_tc = new Cla_data { count = 5, title = new string[] { "腾讯拍客", "Tencent shoot off" }, page = PageAddress.None };
        static Cla_data nav_ed = new Cla_data { count = 5, title = new string[] { "教育", "Education" }, page = PageAddress.Technology };
        static Cla_data nav_ga = new Cla_data { count = 5, title = new string[] { "精品游戏", "Fine game" }, page = PageAddress.Game };
        static Cla_data nav_ss = new Cla_data { count = 5, title = new string[] { "段子页卡", "Section Subcard" }, page = PageAddress.RealEstate };
        static Cla_data nav_cc = new Cla_data { count = 5, title = new string[] { "粤语频道", "Cantonese Channel" }, page = PageAddress.None };
        static Cla_data nav_ho = new Cla_data { count = 5, title = new string[] { "房产资讯", "Real estate information" }, page = PageAddress.RealEstate };
        static Cla_data nav_sk = new Cla_data { count = 5, title = new string[] { "韩国综艺", "South Korea Variety" }, page = PageAddress.None };

        static Cla_data[] nav_all = {nav_es,nav_e2, nav_hl,nav_pv, nav_fy,nav_f2,nav_st, nav_mb,nav_vi,nav_nb, nav_eh,nav_as,
        nav_ch,nav_at,nav_nd,nav_im,nav_os,nav_fu,nav_ec,nav_mv,nav_si,nav_fh,nav_li,nav_ca,nav_do,nav_fc,
        nav_ba,nav_tc,nav_ed,nav_ga,nav_ss, nav_cc,nav_ho,nav_sk};
        #endregion

        #region main
        static string data1;
        static bool task1, load1;
        static Scroll_ex sv;
        static Thickness margin;
        public static void Create(Canvas p, Thickness m)
        {
            if (sv != null)
            {
                sv.Show();
                ReSize(m);
                return;
            }
            task1 = load1 = false;
            WebClass.TaskGet("http://m.v.qq.com/index.html", (o) => {
                data1 = o;
                if (task1)
                {
                    ParseData.AnalyzeData(o.ToCharArray(),ref sv.area);
                    sv.Refresh();
                }
                else load1 = true;
            });
            margin = m;
            sv = new Scroll_ex();
#if desktop
            sv.maxcolumn = 5;
#else
            sv.maxcolumn = 4;
            sv.Lock = MainEx.LockPivot;
            sv.UnLock = MainEx.UnLockPivot;
#endif
            sv.UsingCustomTitle = true;
            sv.ForceUpdateOnce = true;
            sv.area = new Area_m[nav_all.Length];
            sv.ItemClick = (o) => {
                if (o != null)
                {
                    VideoPage.SetAdress(o as string);
                    PageManageEx.CreateNewPage(PageTag.videopage);
                }
            };
            sv.PageClick = (o) => {
                PageAddress t = (PageAddress)o;
                if (t != PageAddress.None)
                {
                    PartialNav.LoadPage(t);
                    PageManageEx.CreateNewPage(PageTag.partial);
                }
            };
            for (int i = 0; i < nav_all.Length; i++)
            {
                int c = nav_all[i].count;
                sv.area[i].page = nav_all[i].page;
                sv.area[i].title = nav_all[i].title;
                sv.area[i].count = c;
                sv.area[i].data = new ItemData_m[c];
            }
            sv.ReSize(m);
            sv.SetParent(p);
            task1 = true;
            if (load1)
            {
                ParseData.AnalyzeData(data1.ToCharArray(), ref sv.area);
                sv.Refresh();
                load1 = false;
            }
        }
        public static void Hide()
        {
            if(sv!=null)
            sv.Hide();
        }
        public static void Show()
        {
            sv.Show();
        }
        public static void Dispose()
        {
            sv.Dispose();
            sv = null;
        }
        public static bool Back()
        {
            if (sv.Back())
                return true;
            return false;
        }

        public static void ReSize(Thickness m)
        {
            if (m == margin)
                return;
            if (sv == null)
                return;
            margin = m;
            sv.ReSize(m);
            sv.Refresh();
        }
#endregion
    }
}