using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    class NavPage:Data
    {

#region vraiable
        struct itemmod
        {
            public Canvas can;
            public Image img;
            public Button title;
            public TextBlock content;
        }
        struct dataitem
        {
            public string text;
            public string tag;
        }
        struct datamod
        {
            public string label;
            public List<dataitem> items;
        }
        struct filteritem
        {
            public TextBlock tip;
            public ComboBox select;
        }
        struct FilterBar
        {
            public Canvas can;
            public List<filteritem> items;
        }
        static ScrollViewer sv;
        static Canvas canvas;
        static itemmod[] buff;
        static List<datamod> data;
        static FilterBar fb;
        static bool create, load,nav_change;
        static char[] c_buff;
        static string filter="";
        static int nav_index,order_index,start;
        static Thickness mar;
#endregion

#region main
        public static void Create(Canvas parent,Thickness margin)
        {
            mar = margin;
            if(sv!=null)
            {
                sv.Margin = margin;
                sv.Visibility = Visibility.Visible;
                tool.can.Visibility = Visibility.Visible;
                Resize(margin);
                return;
            }
            nav_change = true;
            SetNav(0);
            double w = margin.Right - margin.Left;
            margin.Bottom -= 30;
            double h = margin.Bottom - margin.Top;
            sv = new ScrollViewer();
            sv.Margin = margin;
            sv.Width = w;
            sv.Height = h;
            parent.Children.Insert(0,sv);
            canvas = new Canvas();
            sv.Content = canvas;
            buff = new itemmod[20];
            SolidColorBrush brush = new SolidColorBrush(Colors.Red);
            SolidColorBrush bru = new SolidColorBrush(Color.FromArgb(0,0,0,0));
            Thickness tk = new Thickness();
            for (int i=0;i<20;i++)
            {
                if (tk.Left + 200 > w)
                {
                    tk.Left = 0;
                    tk.Top += 400;
                }
                Canvas c = new Canvas();
                c.Width = 200;
                c.Height = 400;
                c.Margin = tk;
                canvas.Children.Add(c);
                buff[i].can = c;

                Image img = new Image();
                img.Width = 190;
                img.Height = 300;
                img.PointerPressed += (o,e)=>{ Jump((o as Image).DataContext as string); };
                c.Children.Add(img);
                buff[i].img = img;

                Button b = new Button();
                b.Width = 180;
                b.Height = 30;
                b.Background = bru;
                b.Foreground = brush;
                b.Margin = new Thickness(0,295,180,320);
                b.Click += (o, e) => { Jump((o as Button).DataContext as string); };
                c.Children.Add(b);
                buff[i].title = b;

                TextBlock tb = new TextBlock();
                tb.Width = 190;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Foreground = font_brush;
                tb.Margin = new Thickness(0,325,200,0);
                c.Children.Add(tb);
                buff[i].content = tb;

                tk.Left += 200;
            }
            tk.Top += 420;
            canvas.Width = w;
            canvas.Height = tk.Top;
            data = new List<datamod>();
            create = true;
            if (load)
                Analyze();
            load = false;
            margin.Top = margin.Bottom;
            margin.Bottom += 30;
            CreateToolBar(parent,margin);

            fb = new FilterBar();
            Canvas t = new Canvas();
            parent.Children.Add(t);
            t.Background = new SolidColorBrush(Colors.Pink);
            fb.can = t;
            t.Visibility = Visibility.Collapsed;
            fb.items = new List<filteritem>();
        }
        public static void Resize(Thickness margin)
        {
            mar = margin;
            double w = margin.Right - margin.Left;
            margin.Bottom -= 30;
            double h = margin.Bottom - margin.Top;
            sv.Width = w;
            sv.Height = h;
            sv.Margin = margin;
            Thickness tk = new Thickness();
            for(int i=0;i<20;i++)
            {
                if (tk.Left + 200 > w)
                {
                    tk.Top += 400;
                    tk.Left = 0;
                }
                buff[i].can.Margin = tk;
                tk.Left += 200;
            }
            tk.Top += 420;
            canvas.Height = tk.Top;
            canvas.Width = w;
            margin.Top = margin.Bottom;
            margin.Bottom += 30;
            tool.can.Margin = margin;
        }
        public static void Hide()
        {
            sv.Visibility = Visibility.Collapsed;
            tool.can.Visibility = Visibility.Collapsed;
            if (lb != null)
                lb.Visibility = Visibility.Collapsed;
            if (fb.can != null)
                fb.can.Visibility = Visibility.Collapsed;
        }
        static void Jump(string address)
        {
            VideoView.LoadViewPageData(address);
            Main.CreatePage(1,PageTag.view);
        }
        static void SetNav(int index)
        {
            nav_index = index;
            if (index >= 0)
                SetAddress(GetAdress());
        }
        static void Analyze()
        {
            bool ok;
            if (nav_index < 12)
            {
                ok = GetVideoInfo();
                if (ok)
                    GetFilter(ref c_buff);
            }
            else
            {
                if (nav_index == 12)
                   ok= GetVideoInfoA(ref c_buff, ref Key_mod_item, "http://v.qq.vom/x/");
                else if (nav_index == 13)
                   ok= GetVideoInfoA(ref c_buff, ref Key_li,"");
                else
                   ok= GetVideoInfoA(ref c_buff, ref Key_li,"");
            }
            if (ok)
                Main.NotifyStop();
            else
            {
                Main.Notify("加载姿势不正确，请换姿势姿势(刷新或更换过滤选项)", Colors.Red);
                Timer.Delegate(()=> { Main.NotifyStop();Timer.Stop(); },3000);
            }
        }
        static string GetAdress()
        {
            string str=nav_data[nav_index].href;
            if(nav_index<12)
            {
                if (start > 0)
                    str += order_value[order_index] + "&offset=" + (start * 20).ToString()+filter;
                else str += order_value[order_index] + filter;
                return str;
            }
            else if (nav_index == 12)
            {
                if (filter != "")
                    return str += filter + "_" + (start + 1).ToString() + ".html";
                else return str += "9_-1_1_0_" + (start+1).ToString() + ".html";
            }
            else if (nav_index == 13)
            {
                if (filter != "")
                    return str += filter + "_" + start.ToString() + ".html";
                else return str += "0/1_" + start.ToString() + ".html";
            }
            else
            {
                if(filter!="")
                return str += filter + "_" + start.ToString() + "_20.html";
                else return str += "1_" + start.ToString() + "_20.html";
            }
        }
        async static void SetAddress(string url)
        {
            Main.Notify("正在载入。。。。+1S", Colors.Green);
            string str = await WebClass.GetResults(url);
            c_buff = str.ToCharArray();
            c_buff = DeleteChar(ref c_buff,'\\');
            //Debug.WriteLine(new string(c_buff));
            if (create)
                Analyze();
            else load = true;
        }
        
        static void GetFilter(ref char[] t)
        {
            for(int i=0;i<data.Count;i++)
                data[i].items.Clear();
            data.Clear();
            int s = FindCharArray(ref t,ref Key_mod_filter_list,0);
            s = FindCharArray(ref t,ref Key_item_toggle,s);
            int e = s;
            string str;
            if(s>0)
            for(int i=0;i<8;i++)
            {
                e = FindCharArray(ref t, ref Key_div_e, s);
                datamod fm = new datamod();
                fm.items = new List<dataitem>();
                dataitem di = new dataitem();
                int c = s;
                c = FindCharArray(ref t,ref Key_label,c,e);
                fm.label = new string(FindCharArrayA(ref t,'>','<',ref c));
                for(int d=0;d<20;d++)
                {
                    c = FindCharArray(ref t, ref Key_boss,c,e);
                    if (c < 0)
                        break;
                    str = new string(FindCharArrayA(ref t,'\"','\"',ref c));
                    c = FindCharArray(ref t,ref Key_index,c,e);
                    str +="="+ new string(FindCharArrayA(ref t, '\"', '\"', ref c));
                    di.tag = str;
                    di.text = new string(FindCharArrayA(ref t, '>', '<', ref c));
                    fm.items.Add(di);
                }
                data.Add(fm);
                    s = FindCharArray(ref t, ref Key_item_toggle, e);
                    if (s < 0)
                        break;
                }
        }
        static void SetFilter()
        {
            int i;
            filteritem fi = new filteritem();
            for (i=0;i<data.Count;i++)
            {
                List<filteritem> t = fb.items;
                if(i>=fb.items.Count)
                {
                    TextBlock tb = new TextBlock();
                    tb.Foreground = font_brush;
                    tb.Text = data[i].label;
                    fi.tip = tb;
                    fb.can.Children.Add(tb);
                    ComboBox cb = new ComboBox();
                    fi.select = cb;
                    cb.SelectionChanged += SelectChange;
                    for(int c=0;c<data[i].items.Count;c++)
                       cb.Items.Add(data[i].items[c].text);
                    fb.can.Children.Add(cb);
                    fb.items.Add(fi);
                }
                else
                {
                    TextBlock tb = fb.items[i].tip;
                    tb.Visibility = Visibility.Visible;
                    tb.Text = data[i].label;
                    ComboBox cb = fb.items[i].select;
                    cb.Visibility = Visibility.Visible;
                    cb.Items.Clear();
                    for (int c = 0; c < data[i].items.Count; c++)
                        cb.Items.Add(data[i].items[c].text);
                }
            }
            for (int d = i; d < fb.items.Count; d++)
            {
                fb.items[d].tip.Visibility = Visibility.Collapsed;
                fb.items[d].select.Visibility = Visibility.Collapsed;
            }
        }
        static void ResizeFilter(Thickness margin)
        {
            Thickness tk = new Thickness();
            int c;
            if (nav_index < 12)
                c = data.Count;
            else if (nav_index < 14)
                c = 2;
            else c = 1;
            for(int i=0;i<c;i++)
            {
                fb.items[i].tip.Margin = tk;
                tk.Left = 70;
                fb.items[i].select.Margin = tk;
                tk.Left = 0;
                tk.Top += 30;
            }
            double h = tk.Top;
            fb.can.Width = 140;
            fb.can.Height = h;
            margin.Top = margin.Bottom - h;
            fb.can.Margin = margin;
        }
        static bool GetVideoInfo()
        {
            int s = FindCharArray(ref c_buff,ref Key_figures_list,0);
            if (s < 0)
                return false;
            int i=0,e;
            s = FindCharArray(ref c_buff, ref Key_list_item_hover, s);
            if(s>0)
                for (i = 0; i < 20; i++)
                {
                    e = FindCharArray(ref c_buff, ref Key_list_item_hover, s);
                    if (e < 0)
                    {
                        e = c_buff.Length;
                        GetVideoItem(s, e, i);
                        buff[i].can.Visibility = Visibility.Visible;
                        i++;
                        break;
                    }
                    GetVideoItem(s, e, i);
                    s = e;
                }
            while(i<20)
            {
                buff[i].can.Visibility = Visibility.Collapsed;
                i++;
            }
            return true;
        }
        static void GetVideoItem(int s,int e,int i)
        {
            s = FindCharArray(ref c_buff, ref Key_href, s);
            string str = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            buff[i].img.DataContext = str;
            buff[i].title.DataContext = str;
            s = FindCharArray(ref c_buff, ref Key_lazyload, s);
            str = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            BitmapImage bi = new BitmapImage(new Uri(str));
            buff[i].img.Source = bi;
            s = FindCharArray(ref c_buff, ref Key_alt, s);
            buff[i].title.Content = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            int d = FindCharArray(ref c_buff, ref Key_figure_desc, s, e);
            if (d > 0)
            {
                char[] tt = FindCharArrayA(ref c_buff, '>', '<', ref d);
                tt = DeleteChar(ref tt, (char)9);
                str = new string(tt);// + Environment.NewLine;
                s = d;
            }
            else str = "";
            d = FindCharArray(ref c_buff, ref Key_info_inner, s, e);
            if (d > 0)
                str = str + "播放数:" + new string(FindCharArrayA(ref c_buff, '>', '<', ref d));
            buff[i].content.Text = str;
        }
        static bool GetVideoInfoA(ref char[] t,ref char[] key,string bhref)
        {
            int s = FindCharArray(ref t, ref Key_mod_video_list, 0);
            if (s < 0)
                return false;
            s = FindCharArray(ref t, ref key,s);
            int i=0;
            if(s>0)
                for(i=0;i<20;i++)
                {
                    s = FindCharArray(ref t, ref Key_href, s);
                    string href =bhref+new string( FindCharArrayA(ref t,'\"','\"',ref s));
                    buff[i].img.DataContext = href;
                    buff[i].title.DataContext = href;
                    s = FindCharArray(ref t,ref Key_title,s);
                    buff[i].title.Content= new string(FindCharArrayA(ref t, '\"', '\"', ref s));
                    s = FindCharArray(ref t,ref Key_src,s);
                    string src = new string(FindCharArrayA(ref t, '\"', '\"', ref s));
                    BitmapImage bi = new BitmapImage(new Uri(src));
                    buff[i].img.Source = bi;
                    buff[i].content.Text = "";
                    buff[i].can.Visibility = Visibility.Collapsed;
                    s = FindCharArray(ref t, ref key, s);
                    if (s < 0)
                    {
                        i++;
                        break;
                    }
                }
            while (i < 20)
            {
                buff[i].can.Visibility = Visibility.Collapsed;
                i++;
            }
            return true;
        }
        static void SelectChange(object o, SelectionChangedEventArgs e)
        {
            filter = "";
            if(nav_index<12)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    int c = fb.items[i].select.SelectedIndex;
                    if (c > 0)
                        filter += "&" + data[i].items[c].tag;
                }
            }
            else
            {
                if(nav_index==12)
                {
                    int c = fb.items[0].select.SelectedIndex;
                    if (c < 0)
                        c = 0;
                    int d = fb.items[1].select.SelectedIndex;
                    if (d < 0)
                        d = 0;
                    filter = filter12[c].href+filter12_o[d].href+"_0_";
                }
                else if(nav_index==13)
                {
                    int c = fb.items[0].select.SelectedIndex;
                    if (c < 0)
                        c = 0;
                    int d = fb.items[1].select.SelectedIndex;
                    if (d < 0)
                        d = 0;
                    filter = filter13[c].href +"/"+ filter13_o[d].href;
                }
                else
                {
                    int d = fb.items[0].select.SelectedIndex;
                    if (d < 0)
                        d = 0;
                    filter = filter13_o[d].href;
                }
            }
        }
        static void SetFilter12()
        {
            SetFilterA(new Nav_Data[][] { filter12,filter12_o},new string[][] {type,sort});
        }
        static void SetFilter13()
        {
            SetFilterA(new Nav_Data[][] { filter13, filter12_o }, new string[][] { type, sort });
        }
        static void SetFilter14()
        {
            SetFilterA(new Nav_Data[][] { filter12_o }, new string[][] { sort });
        }
        static void SetFilterA(Nav_Data[][] data,string[][] tilte)
        {
            int i;
            filteritem fi = new filteritem();
            for (i = 0; i < data.Length; i++)
            {
                Nav_Data[] n = data[i];
                List<filteritem> t = fb.items;
                if (i >= fb.items.Count)
                {
                    TextBlock tb = new TextBlock();
                    tb.Foreground = font_brush;
                    tb.Text = tilte[i][language];
                    fi.tip = tb;
                    fb.can.Children.Add(tb);
                    ComboBox cb = new ComboBox();
                    fi.select = cb;
                    cb.SelectionChanged += SelectChange;
                    for (int c = 0; c < n.Length; c++)
                        cb.Items.Add(n[c].text[language]);
                    fb.can.Children.Add(cb);
                    fb.items.Add(fi);
                }
                else
                {
                    TextBlock tb = fb.items[i].tip;
                    tb.Visibility = Visibility.Visible;
                    tb.Text = tilte[i][language];
                    ComboBox cb = fb.items[i].select;
                    cb.Visibility = Visibility.Visible;
                    cb.Items.Clear();
                    for (int c = 0; c < n.Length; c++)
                        cb.Items.Add(n[c].text[language]);
                }
            }
            for (int d = i; d < fb.items.Count; d++)
            {
                fb.items[d].tip.Visibility = Visibility.Collapsed;
                fb.items[d].select.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region tool bar
        struct ToolBar
        {
            public Canvas can;
            public SymbolIcon nav;
            public SymbolIcon filter;
            public ComboBox sort;
            public SymbolIcon left;
            public SymbolIcon right;
            public SymbolIcon refresh;
            public TextBox textbox;
        }
        static ListBox lb;
        static void CreateNavList(Canvas parent,Thickness margin)
        {
            double w = margin.Right - margin.Left;
            double h = margin.Bottom - margin.Top;
            if (lb != null)
            {
                lb.Visibility = Visibility.Visible;
                lb.Height = h;
                lb.Margin = margin;
                return;
            }
            lb = new ListBox();
            lb.Margin = margin;
            lb.Width = w;
            lb.Height = h;
            lb.SelectionChanged +=(o,e)=> {
                start = 0;
                tool.textbox.Text = "0";
                SetNav(lb.SelectedIndex); };
            lb.Background = new SolidColorBrush(Colors.Pink);
            parent.Children.Add(lb);
            for (int i = 0; i < nav_data.Length; i++)
                lb.Items.Add(nav_data[i].text[language]);
        }
        static void HideList()
        {
            lb.Visibility = Visibility.Collapsed;
        }
        static void ShowList()
        {
            lb.Visibility = Visibility.Visible;
        }
        static void ResizeList(Thickness margin)
        {
            lb.Height = margin.Bottom - margin.Top;
        }

        static ToolBar tool;
        static void CreateToolBar(Canvas parent,Thickness margin)
        {
            if(tool.can!=null)
            {
                tool.can.Visibility = Visibility.Visible;
                tool.can.Margin = margin;
                return;
            }
            Canvas can = new Canvas();
            parent.Children.Add(can);
            can.Margin = margin;
            tool.can = can;

            Thickness tk = new Thickness(0,5,0,0);
            SymbolIcon si = new SymbolIcon();
            si.Margin = tk;
            si.Symbol = Symbol.List;
            si.PointerPressed += Tool_Nav_Press;
            can.Children.Add(si);
            tool.nav = si;

            si = new SymbolIcon();
            tk.Left += 30;
            si.Margin = tk;
            si.Symbol = Symbol.Filter;
            si.PointerPressed += Tool_Filter_Press;
            can.Children.Add(si);
            tool.filter = si;

            tk.Left += 30;
            tk.Top = 0;
            ComboBox c = new ComboBox();
            c.Margin = tk;
            c.Items.Add(order_4[language]);
            c.Items.Add(order_5[language]);
            c.Items.Add(order_6[language]);
            c.SelectedIndex = order_index;
            c.SelectionChanged += (o, e) => { order_index = tool.sort.SelectedIndex; };
            can.Children.Add(c);
            tool.sort = c;

            si = new SymbolIcon();
            tk.Top = 5;
            tk.Left += 80;
            si.Margin = tk;
            si.Symbol = Symbol.Back;
            si.PointerPressed += Tool_Left_Press;
            can.Children.Add(si);
            tool.left = si;

            TextBox tb = new TextBox();
            tk.Top = 0;
            tk.Left += 30;
            tb.Width = 30;
            tb.KeyDown += Tool_KeyDown;
            tb.Margin = tk;
            tb.Text = "0";
            can.Children.Add(tb);
            tool.textbox = tb;

            si = new SymbolIcon();
            tk.Top = 5;
            tk.Left += 70;
            si.Margin = tk;
            si.Symbol = Symbol.Forward;
            si.PointerPressed += Tool_Right_Press;
            can.Children.Add(si);
            tool.right = si;

            si = new SymbolIcon();
            tk.Left += 30;
            si.Margin = tk;
            si.Symbol = Symbol.Refresh;
            si.PointerPressed += Tool_Refresh_Press;
            can.Children.Add(si);
            tool.refresh = si;
        }
        static void Tool_Nav_Press(object o,object e)
        {
            filter = "";
            start = 0;
            nav_change = true;
            Thickness tk = mar;
            tk.Right = 80;
            tk.Bottom -= 30;
            if(lb==null)
            CreateNavList(sv.Parent as Canvas,tk);
            else if(lb.Visibility==Visibility.Visible)
            {
                lb.Visibility = Visibility.Collapsed;
                tk.Right = mar.Right;
                sv.Margin = tk;
                return;
            }
            else  lb.Visibility = Visibility.Visible;
            tk.Left = 90;
            tk.Right = mar.Right+90;
            sv.Margin = tk;
        }
        static void Tool_Filter_Press(object o, object e)
        {
            if(fb.can.Visibility==Visibility.Visible)
            {
                fb.can.Visibility = Visibility.Collapsed;
                return;
            }
            fb.can.Visibility = Visibility.Visible;
            if(nav_change)
            {
                if (nav_index < 12)
                    SetFilter();
                else if (nav_index == 12)
                    SetFilter12();
                else if (nav_index == 13)
                    SetFilter13();
                else SetFilter14();
                nav_change = false;
            }
            Thickness tk = mar;
            tk.Left = 80;
            tk.Bottom -= 30;
            ResizeFilter(tk);
        }
        static void Tool_Left_Press(object o, object e)
        {
            if(start>0)
            {
                start--;
                SetAddress(GetAdress());
                tool.textbox.Text = start.ToString();
            }
        }
        static void Tool_Right_Press(object o, object e)
        {
            start++;
            SetAddress(GetAdress());
            tool.textbox.Text = start.ToString();
        }
        static void Tool_Refresh_Press(object o, object e)
        {
            SetAddress(GetAdress());
        }
        static void Tool_KeyDown(object o, KeyRoutedEventArgs e)
        {
            if(tool.textbox.Text.Length>=3)
            {
                e.Handled = true;
                return;
            }
            if(e.Key==VirtualKey.Enter)
            {
                start = Convert.ToInt32(tool.textbox.Text);
                SetAddress(GetAdress());
                return;
            }
            int c = (int)e.Key;
            if(c<48||c>57)
                e.Handled = true;
        }
        #endregion
    }

}
