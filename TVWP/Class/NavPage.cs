using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVWP.Control;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    class NavPage:CharOperation
    {
        struct Nav_DataA
        {
            public string[] title;
            public string href;
            public Nav_DataA(string[] t, string h) { title = t; href = h; }
        }
        #region sort filter
        static string[] order_4 = new string[] { "热播", "Hot" };
        static string[] order_5 = new string[] { "最新", "new" };
        static string[] order_6 = new string[] { "评分", "Score" };
        static string[] order_value = new string[] { "?sort=4", "?sort=5", "?sort=6" };
        #endregion

        #region class rs
        static string[] movie = new string[] { "电影", "Movie" };
        static string[] tv = new string[] { "电视剧", "TV" };
        static string[] variety = new string[] { "综艺", "Variety" };
        static string[] animation = new string[] { "动漫", "Animation" };
        static string[] children = new string[] { "少儿", "Children" };
        static string[] mv = new string[] { "MV", "MV" };
        static string[] docoment = new string[] { "纪录片", "Docoment" };
        static string[] news = new string[] { "新闻", "News" };
        static string[] entertainment = new string[] { "娱乐", "Entertainment" };
        static string[] sports = new string[] { "体育", "Sports" };
        static string[] games = new string[] { "游戏", "Games" };
        static string[] fun = new string[] { "搞笑", "Fun" };
        static string[] cla = new string[] { "课程", "Class" };
        static string[] fashion = new string[] { "时尚", "Fashion" };
        static string[] other = new string[] { "其它", "Other" };

        public static string[] type = new string[] { "分类", "Type" };
        public static string[] sort = new string[] { "排序", "Sort" };
        #endregion

        #region href rs
        static string movie_href = "http://v.qq.com/x/movielist/";// /?sort=4 &offset=20|40.....
        static string tv_href = "http://v.qq.com/x/teleplaylist/";
        static string variety_href = "http://v.qq.com/x/varietylist/";
        static string animation_href = "http://v.qq.com/x/cartoonlist/";
        static string children_href = "http://v.qq.com/x/childrenlist/";
        static string mv_href = "http://v.qq.com/x/musiclist/";
        static string docoment_href = "http://v.qq.com/x/documentarylist/";
        static string news_href = "http://v.qq.com/x/newslist/";
        static string entertainment_href = "http://v.qq.com/x/entlist/";
        static string sports_href = "http://v.qq.com/x/sportlist/";
        static string games_href = "http://v.qq.com/x/gamelist/";
        static string fun_href = "http://v.qq.com/x/funnylist/";

        static string fashion_href = "http://v.qq.com/fashion/list/1/";// type/type/type_offset
        static string other_href = "http://v.qq.com/worldcuplist/21_-1_-1_-1_-1_-1_";//type_page_count

        static Nav_DataA[] nav_data = new Nav_DataA[] {new Nav_DataA(movie,movie_href),new Nav_DataA(tv,tv_href),
        new Nav_DataA(variety,variety_href),new Nav_DataA(animation,animation_href),new Nav_DataA(children,children_href),
        new Nav_DataA(mv,mv_href),new Nav_DataA(docoment,docoment_href),new Nav_DataA(news,news_href),
        new Nav_DataA(entertainment,entertainment_href),new Nav_DataA(sports,sports_href),new Nav_DataA(games,games_href),
        new Nav_DataA(fun,fun_href),new Nav_DataA(fashion,fashion_href),new Nav_DataA(other,other_href)};
        #endregion

        #region filter12 rs
        static string[] f12_t_l = new string[] { "全部", "All" };
        static string[] f12_t_l_0 = new string[] { "历史", "History" };
        static string[] f12_t_l_1 = new string[] { "经济", "Economics" };
        static string[] f12_t_l_2 = new string[] { "生活", "living" };
        static string[] f12_t_l_3 = new string[] { "百科", "encyclopedia" };
        static string[] f12_t_l_4 = new string[] { "互联网", "Internet" };
        static string[] f12_t_l_5 = new string[] { "晃眼", "glare" };
        static string[] f12_t_l_6 = new string[] { "下午茶", "tea" };
        const string f12_t_v = "9_-1_";
        const string f12_t_v_0 = "1_0_";
        const string f12_t_v_1 = "1_1_";
        const string f12_t_v_2 = "1_2_";
        const string f12_t_v_3 = "1_3_";
        const string f12_t_v_4 = "1_4_";
        const string f12_t_v_5 = "1_5_";
        const string f12_t_v_6 = "1_6_";
        static string[] f12_o_l = new string[] { "更新", "Update" };
        static string[] f12_o_l_0 = new string[] { "热度", "Hot" };
        static string[] f12_o_l_1 = new string[] { "评分", "Score" };
        const string f12_o_v = "0";
        const string f12_o_v_0 = "1";
        const string f12_o_v_1 = "2";
        static Nav_DataA[] filter12 = new Nav_DataA[] {new Nav_DataA(f12_t_l,f12_t_v) ,new Nav_DataA(f12_t_l_0,f12_t_v_0),
        new Nav_DataA(f12_t_l_1,f12_t_v_1),new Nav_DataA(f12_t_l_2,f12_t_v_2),new Nav_DataA(f12_t_l_3,f12_t_v_3),
        new Nav_DataA(f12_t_l_4,f12_t_v_4),new Nav_DataA(f12_t_l_5,f12_t_v_5),new Nav_DataA(f12_t_l_6,f12_t_v_6)};
        static Nav_DataA[] filter12_o = new Nav_DataA[] {new Nav_DataA(f12_o_l,f12_o_v),new Nav_DataA(f12_o_l_0,f12_o_v_0),
        new Nav_DataA(f12_o_l_1,f12_o_v_1)};
        #endregion

        #region filter13 rs
        static string[] f13_t_l_1 = new string[] { "新闻", "News" };
        static string[] f13_t_l_2 = new string[] { "访谈", "Interview" };
        static string[] f13_t_l_3 = new string[] { "大片", "Large" };
        static string[] f13_t_l_4 = new string[] { "街拍", "Street beat" };
        static string[] f13_t_l_5 = new string[] { "奢侈品", "Luxury" };
        const string f13_t_v_1 = "221";
        const string f13_t_v_2 = "222";
        const string f13_t_v_3 = "224";
        const string f13_t_v_4 = "225";
        const string f13_t_v_5 = "227";
        static Nav_DataA[] filter13 = new Nav_DataA[] { new Nav_DataA(f12_t_l,f12_o_v),new Nav_DataA(f13_t_l_1,f13_t_v_1),
        new Nav_DataA(f13_t_l_2,f13_t_v_2),new Nav_DataA(f13_t_l_3,f13_t_v_3),new Nav_DataA(f13_t_l_4,f13_t_v_4),
        new Nav_DataA(f13_t_l_5,f13_t_v_5)};
        static Nav_DataA[] filter13_o = new Nav_DataA[] { new Nav_DataA(f12_o_l, f12_o_v_0), new Nav_DataA(f12_o_l_0, f12_o_v_1) };
        #endregion

        #region rs
        const int itemcount = 20;
        
        static int nav_index, start, order_index;
        static string filter = "";
        static List<ItemDataB> lid;
        static char[] c_buff;
        static bool create, load;
        static Canvas parent;
        static Thickness margin;
        #endregion

        static ScrollViewB svb;
        public static void Create(Canvas p, Thickness m)//parent,margin
        {
            margin = m;
            if (svb != null)
            {
                nb.can.Visibility = Visibility.Visible;
                nb.show_filter = false;
                SettingChanged();
                ReSize(m);
                return;
            }
            load = false;
            create = false;
            SetNav(0);
            parent = p;
            svb = new ScrollViewB();
            svb.itemclick = (o) =>
            {
                if(o!=null)
                {
                    VideoPage.SetAdress(o as string);
                    PageManageEx.CreateNewPage(PageTag.videopage);
                }
            };
            lid=svb.data;
            svb.SetParent(p);
            svb.ReSize(m);
#if desktop
            svb.ShowBorder();
#endif
            buff_fid = new FilterItemData[8];
            create = true;
            if (load)
                Analyze();
            CreateNavBar();
            ReSize(m);
        }
        static void NavReSize(Thickness m)
        {
            if (margin == m)
                return;
            double w = m.Right - m.Left;
            double h = m.Bottom - m.Top;
            if (h < 0)
                return;
            svb.ReSize(m);
            svb.Refresh();
        }
        public static void Hide()
        {
            //nb.can.Visibility = Visibility.Collapsed;
            //if (fp != null)
            //    fp.Visibility = Visibility.Collapsed;
        }
        public static void Show()
        {
            nb.can.Visibility = Visibility.Visible;
        }
        static void Analyze()
        {
            lid.Clear();
            bool ok = false;
            if (nav_index < 12)
            {
                ok = ParseData.Analyze_Nav(ref c_buff,lid,itemcount);
                svb.Refresh();
                if (ok)
                    ParseData.GetFilterData(ref c_buff,ref buff_fid);
            }
            else
            {
                if (nav_index == 12)
                    ok = ParseData.Analyze_NavA(ref c_buff, ref Key_li, "",itemcount,lid);
                else
                    ok = ParseData.Analyze_NavA(ref c_buff, ref Key_li, "",itemcount,lid);
                svb.Refresh();
            }
            if (!ok)
            {
                Main.Notify("加载姿势不正确，请换姿势姿势(刷新或更换过滤选项)",Component.warning_brush);
            }
        }
        static void SettingChanged()
        {
            if (lan != Component.language)
                LangChanged();
        }
        static int lan;
        static void LangChanged()
        {
            for (int i = 0; i < 5; i++)
                nb.button[i].Text = nav_content[i][Component.language];
            ListBox lb = nb.order.GetListBox;
            lb.Items.Clear();
            lb.Items.Add(order_4[Component.language]);
            lb.Items.Add(order_5[Component.language]);
            lb.Items.Add(order_6[Component.language]);
            lb = nb.nav;
            lb.Items.Clear();
            for (int i = 0; i < nav_data.Length; i++)
                lb.Items.Add(nav_data[i].title[Component.language]);
            lan = Component.language;
        }
        #region set data
        static void SetNav(int index)
        {
            nav_index = index;
            filter = "";
            start = 0;
            if (index >= 0)
                WebClass.TaskGet(GetAdress(), AnalyzeEx);
        }
        static string GetAdress()
        {
            string str = nav_data[nav_index].href;
            if (nav_index < 12)
            {
                if (start > 0)
                    str += order_value[order_index] + "&offset=" + (start * 20).ToString() + filter;
                else str += order_value[order_index] + filter;
                return str;
            }
            else if (nav_index == 12)
            {
                if (filter != "")
                    return str += filter + "_" + start.ToString() + ".html";
                else return str += "0/1_" + start.ToString() + ".html";
            }
            else
            {
                if (filter != "")
                    return str += filter + "_" + start.ToString() + "_20.html";
                else return str += "1_" + start.ToString() + "_20.html";
            }
        }
        static void AnalyzeEx(string data)
        {
            char[] buff = data.ToCharArray();
            c_buff = DeleteChar(ref buff, '\\');
            //Debug.WriteLine(new string(c_buff));
            if (create)
                Analyze();
            else load = true;
        }
#endregion

#region filter mod
        struct FilterItemMod
        {
            public ListBoxH lbh;
            public ListBox lb;
            public TextBlock tilte;
        }
        static FilterItemMod[] buff_fim;
        static FilterItemData[] buff_fid;
#endregion

#region filter
        static void CreateFilter(int i)
        {
            TextBlock tb = new TextBlock();
            tb.Foreground = Component.font_brush;
            buff_fim[i].tilte = tb;
            fp.Children.Add(tb);
            ListBoxH lbh = new ListBoxH();
            buff_fim[i].lbh = lbh;
            buff_fim[i].lb = lbh.GetListBox;
            buff_fim[i].lb.SelectionChanged += (o, e) => { SetFilter(); };
            buff_fim[i].lb.Foreground = Component.filter_brush;
            fp.Children.Add(buff_fim[i].lbh);
        }
        static void GetFilterData()
        {
            int s = FindCharArray(ref c_buff, ref Key_mod_filter_list, 0);
            s = FindCharArray(ref c_buff, ref Key_item_toggle, s);
            int e = s;
            string str;
            List<ItemContext> ls;
            ItemContext ic = new ItemContext();
            int i = 0;
            if (s > 0)
                for (i = 0; i < 8; i++)
                {
                    e = FindCharArray(ref c_buff, ref Key_div_e, s);
                    if (buff_fid[i].data == null)
                        buff_fid[i].data = new List<ItemContext>();
                    ls = buff_fid[i].data;
                    ls.Clear();
                    int c = s;
                    c = FindCharArray(ref c_buff, ref Key_label, c, e);
                    buff_fid[i].tilte = new string(FindCharArrayA(ref c_buff, '>', '<', ref c));
                    for (int d = 0; d < 20; d++)
                    {
                        c = FindCharArray(ref c_buff, ref Key_boss, c, e);
                        if (c < 0)
                            break;
                        str = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref c));
                        c = FindCharArray(ref c_buff, ref Key_index, c, e);
                        str += "=" + new string(FindCharArrayA(ref c_buff, '\"', '\"', ref c));
                        ic.tag = str;
                        ic.text = new string(FindCharArrayA(ref c_buff, '>', '<', ref c));
                        ls.Add(ic);
                    }
                    s = FindCharArray(ref c_buff, ref Key_item_toggle, e);
                    if (s < 0)
                        break;
                }
            for (int c = i; c < 8; c++)
                buff_fid[c].tilte = "";
        }
        static void UpdateFilter()
        {
            int i = 0;
            for (i = 0; i < 8; i++)
            {
                if (buff_fid[i].tilte != "")
                {
                    if (buff_fim[i].tilte == null)
                    {
                        CreateFilter(i);
                        goto label1;
                    }
                    buff_fim[i].tilte.Visibility = Visibility.Visible;
                    buff_fim[i].lb.Visibility = Visibility.Visible;
                    buff_fim[i].lb.Items.Clear();
                    label1:;
                    List<ItemContext> li = buff_fid[i].data;
                    int l = li.Count;
                    for (int c = 0; c < l; c++)
                        buff_fim[i].lb.Items.Add(li[c].text);
                    buff_fim[i].tilte.Text = buff_fid[i].tilte;
                }
                else break;
            }
            for (int c = i; c < 8; c++)
                if (buff_fim[c].lb != null)
                {
                    buff_fim[c].tilte.Visibility = Visibility.Collapsed;
                    buff_fim[c].lb.Visibility = Visibility.Collapsed;
                }
        }
        static void SetFilter()
        {
            filter = "";
            if (nav_index < 12)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (buff_fid[i].tilte != "" & buff_fim[i].lb != null)
                    {
                        int c = buff_fim[i].lb.SelectedIndex;
                        if (c > 0)
                            filter += "&" + buff_fid[i].data[c].tag;
                    }
                    else break;
                }
            }
        }
        static double ReszieFilter(double w)
        {
            int i = 0;
            double dy = 0;
            for (i = 0; i < 8; i++)
            {
                if (buff_fim[i].tilte != null && buff_fim[i].tilte.Visibility == Visibility.Visible)
                {
                    buff_fim[i].tilte.Margin = new Thickness(0, dy + 10, 0, 0);
                    buff_fim[i].lb.Margin = new Thickness(60, dy, 0, 0);
                    buff_fim[i].lb.Width = w - 70;
                }
                else break;
                dy += 40;
            }
#if phone
            fp.Width = w;
            fp.Height = dy;
#endif
            return dy;
        }
        public static void ReSize(Thickness m)
        {
            margin = m;
            nb.can.Margin = m;
            m.Top += 90;
            double w = m.Right - m.Left;
            double dy = 0;
            if (nb.show_filter)
            {
                dy = ReszieFilter(w);
                fp.Margin = m;
#if desktop
                m.Top += dy;
#endif
            }
            else if (nb.show_nav)
            {
#if desktop
                if (nb.show_nav)
                {
                    m.Left += 90;
                    nb.nav.Height = m.Bottom - m.Top;
                }
#else
                if (nb.show_nav)
                    nb.nav.Height = m.Bottom - m.Top;
#endif
            }
            NavReSize(m);
        }
#endregion

#region app bar
        struct NavBar
        {
            public Canvas can;
            public ListBoxH order;
            public ListBox nav;
            public TextBox pagecount;
            public bool show_filter;
            public bool show_nav;
            public bool filter_change;
            public TextBlock[] button;
        }
        static Canvas fp;//filter parent
        static NavBar nb;
        static string[] nav_bar = new string[] { "导航栏", "Navgation" };
        static string[] nav_back = new string[] { "上一页", "Back" };
        static string[] nav_next = new string[] { "下一页", "Next" };
        static string[] nav_fil = new string[] { "筛选", "Filter" };
        static string[] nav_refresh = new string[] { "刷新", "Refresh" };
        static string[][] nav_content = { nav_bar, nav_back, nav_next, nav_fil, nav_refresh };
        readonly static double[] but_dx = { 0, 60, 180, 240, 300 };
        readonly static PointerEventHandler[] reh = {nav_c_click,back_click,forward_click,filter_click,
            (o,e)=>{WebClass.TaskGet(GetAdress(), AnalyzeEx);} };
        static void CreateNavBar()
        {
            if (nb.can != null)
            {
                nb.can.Visibility = Visibility.Visible;
                return;
            }
            nb = new NavBar();
            fp = new Canvas();
#if phone
            fp.Background = Component.bk_brush;
#endif
            Canvas can = new Canvas();
            nb.can = can;
            parent.Children.Add(can);
            parent.Children.Add(fp);
            nb.order = new ListBoxH();
            nb.order.Margin = new Thickness(0, 40, 0, 0);
            nb.order.GetListBox.Foreground = Component.filter_brush;
            ListBox lb = nb.order.GetListBox;
            lb.SelectionChanged += (o, e) => {
                order_index = nb.order.GetListBox.SelectedIndex;
                WebClass.TaskGet(GetAdress(), AnalyzeEx);
            };
            lb.Items.Add(order_4[Component.language]);
            lb.Items.Add(order_5[Component.language]);
            lb.Items.Add(order_6[Component.language]);

            nb.button = new TextBlock[5];
            TextBlock b;
            for (int i = 0; i < 5; i++)
            {
                b = new TextBlock();
                nb.button[i] = b;
                b.Margin = new Thickness(but_dx[i], 8, 0, 0);
                b.Text = nav_content[i][Component.language];
                b.Foreground = Component.nav_brush;
                b.PointerPressed += reh[i];
                can.Children.Add(b);
            }
            nb.pagecount = new TextBox();
            nb.pagecount.Margin = new Thickness(110, 0, 0, 0);
            nb.pagecount.Text = "0";
            nb.pagecount.KeyDown += Tool_KeyDown;
            nb.nav = new ListBox();
            nb.nav.Margin = new Thickness(0, 90, 0, 0);
            nb.nav.Visibility = Visibility.Collapsed;
            nb.nav.Background = Component.bk_brush;
            nb.nav.Foreground = Component.nav_brush;
            nb.pagecount.Foreground = Component.font_brush;
            nb.nav.SelectionChanged += (o, e) => {
                nb.filter_change = true;
                SetNav(nb.nav.SelectedIndex);
                nb.pagecount.Text = "0";
#if phone
                nb.nav.Visibility = Visibility.Collapsed;
#endif
            };
            can.Children.Add(nb.order);
            can.Children.Add(nb.pagecount);
            can.Children.Add(nb.nav);
            buff_fim = new FilterItemMod[8];
            nb.filter_change = true;
        }
#endregion

#region event
        static void nav_c_click(object o, object e)
        {
            if (nb.show_nav)
            {
                nb.show_nav = false;
                nb.nav.Visibility = Visibility.Collapsed;
                ReSize(margin);
            }
            else
            {
                nb.show_nav = true;
                nb.show_filter = false;
                fp.Visibility = Visibility.Collapsed;
                nb.nav.Visibility = Visibility.Visible;
                ListBox lb = nb.nav;
                if (lb.Items.Count < 1)
                {
                    for (int i = 0; i < nav_data.Length; i++)
                        lb.Items.Add(nav_data[i].title[Component.language]);
                }
                ReSize(margin);
            }
        }
        static void back_click(object o, object e)
        {
            if (start > 0)
            {
                start--;
                nb.pagecount.Text = start.ToString();
                WebClass.TaskGet(GetAdress(), AnalyzeEx);
            }
        }
        static void forward_click(object o, object e)
        {
            start++;
            nb.pagecount.Text = start.ToString();
            WebClass.TaskGet(GetAdress(), AnalyzeEx);
        }
        static void filter_click(object o, object e)
        {
            if (nav_index < 6)
            {
                if (nb.show_filter)
                {
                    nb.show_filter = false;
                    fp.Visibility = Visibility.Collapsed;
                    ReSize(margin);
                }
                else
                {
                    nb.show_filter = true;
                    nb.show_nav = false;
                    nb.nav.Visibility = Visibility.Collapsed;
                    fp.Visibility = Visibility.Visible;
                    if (nb.filter_change)
                        UpdateFilter();
                    ReSize(margin);
                }
            }
        }
        static void Tool_KeyDown(object o, KeyRoutedEventArgs e)
        {
            if (nb.pagecount.Text.Length >= 3)
            {
                e.Handled = true;
                return;
            }
            if (e.Key == VirtualKey.Enter)
            {
                start = Convert.ToInt32(nb.pagecount.Text);
                WebClass.TaskGet(GetAdress(), AnalyzeEx);
                return;
            }
            int c = (int)e.Key;
            if (c < 48 || c > 57)
                e.Handled = true;
        }
#endregion
    }
}
