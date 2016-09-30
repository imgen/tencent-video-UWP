using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    class SearchResult:Data
    {
        struct SearchFilter
        {
            public string content;
            public string sort;//0=related 1=new 2=hot
            public string date;//pubfilter 0=all 1=day 2=week,3=month 4=year
            public string time;//duration 0=all 1=less10 2=10-30 3=30-60 4=than60
            //public string scope;//tabid 0=all 1=moive 2=tv 3=mi 4=animation 6=history 7=other
        }
        #region main
        static int top;
        static SearchContainer SC = new SearchContainer();
        static double w,h;
        public static void Create(Canvas parent,Thickness margin)
        {
            if (SC.sv != null)
            {
                SC.sv.Visibility = Visibility.Visible;
#if desktop
                Resize(margin);
#endif
            }
            else
            {
                w = margin.Right - margin.Left;
                h = margin.Bottom - margin.Top;
                SC = new SearchContainer();
                ScrollViewer sv = new ScrollViewer();
                SC.sv = sv;
                sv.Width = w;
                sv.Height = h;
                sv.Margin = margin;
                parent.Children.Insert(0,sv);
                Canvas can = new Canvas();
                SC.can = can;
                sv.Content = can;
                SC.items = new List<SearchItem>();
            }
        }
        public static void Hide()
        {
            SC.sv.Visibility = Visibility.Collapsed;
        }
        public static void Show()
        {
            SC.sv.Visibility = Visibility.Visible;
        }
        public static void Resize(Thickness margin)
        {
            w = margin.Right - margin.Left;
            h = margin.Bottom - margin.Top;
            SC.sv.Width = w;
            SC.sv.Height = h;
            SC.sv.Margin = margin;
            margin.Left = 10;
            margin.Top = 10;
            Thickness tk = new Thickness(10,280,0,0);
            List<SearchItem> temp = SC.items;
            for (int i=0;i<top;i++)
            {
                temp[i].img.Margin = margin;
                temp[i].title.Margin = tk;
                margin.Left += 300;
                tk.Left += 300;
                if(margin.Left+300>w)
                {
                    margin.Left = 10;
                    margin.Top += 300;
                    tk.Left = 10;
                    tk.Top += 300;
                }
            }
        }
        static void I_released(Object sender, PointerRoutedEventArgs e)
        {
            ImageContext ic=(ImageContext)(sender as Image).DataContext;
            VideoView.LoadViewPageData(ic.href);
            Main.CreatePage(1,PageTag.view);
        }
        static async void Search(SearchFilter sf)
        {
            string str = "http://v.qq.com/x/search/?q="
                + sf.content + sf.date + sf.time + sf.sort;
            str = Uri.EscapeUriString(str);
            string cc = await WebClass.GetResults(str);
            Reset();
            Search_Analyze(cc);
        }
        static void Search_Analyze(string data)
        {
            main_buff = data.ToCharArray();
            main_buff = DeleteChar(ref main_buff, '\\');
            //Debug.WriteLine(new string(main_buff));
            int s = 0,e=0;
            e = FindCharArray(ref Key_result_item, 0);
            double x = screenX;
            Thickness tk = new Thickness(10, 10, 0, 0);
            ImageContext ic = new ImageContext();
            do
            {
                label0:;
                s = FindCharArray(ref Key_result_item, e);
                if (s < 0)
                    break;
                e = FindCharArray(ref Key_result_item, s+50);
                if (e < 0)
                    e = main_buff.Length;
                s++;
                if (main_buff[s] == 'v')
                {
                    if (Analyze_v(s, e, ref ic))
                        AddItem(ic, tk);
                    else goto label0;
                }
                else//'h'
                {
                    Analyze_h(s, ref ic);
                    AddItem(ic, tk);
                }
                tk.Left += 300;
                if (tk.Left + 300 > w)
                {
                    tk.Left = 10;
                    tk.Top += 300;
                }
            } while (e < main_buff.Length);
            SC.can.Height = tk.Top + 300;
        }
        static bool Analyze_v(int s,int e ,ref ImageContext ic)
        {
            int f = s;
            s = FindCharArray(ref main_buff, ref Key_titleA, s, e);
            ic.title = new string(FindCharArrayA(ref main_buff, '\'', '\'', s, e));
            s = FindCharArray(ref Key_src, s);
            ic.src = new string(FindCharArrayA(ref main_buff, '\"', '\"', ref s));
            s = FindCharArray(ref Key_desc_text, s);
            ic.detail = new string(FindCharArrayA(ref main_buff, '>', '<', ref s));

            int t = FindCharArray(ref main_buff, ref Key_playlist, s, e);
            if (t < 0)
                t = FindCharArray(ref main_buff, ref Key_figures_list, s, e);
            s = FindCharArray(ref Key_href, t);
            char[] tt = FindCharArrayA(ref main_buff, '\"', '\"', ref s);
            int c = FindCharArray(ref tt, ref Key_vidA, 0);
            if (c < 0)
                return false;
            ic.href = new string(CopyCharArry(ref tt,0,c-5));
            ic.vid = new string(CopyCharArry(ref tt, c, tt.Length - c));
            return true;
        }
        static void Analyze_h(int s,ref ImageContext ic)
        {
            s = FindCharArray(ref Key_href, s);
            char[] tt = FindCharArrayA(ref main_buff, '\"', '\"', ref s);
            int c = tt.Length - 1;
            ic.vid = new string(FallFindCharArray(ref tt, '/', '.', ref c));
            ic.href = new string(tt);

            s = FindCharArray(ref Key_src, s);
            ic.src = new string(FindCharArrayA(ref main_buff, '\"', '\"', ref s));

            s = FindCharArray(ref Key_alt, s);
            ic.title = new string(FindCharArrayA(ref main_buff, '\"', '\"', ref s));
           // Debug.WriteLine(ic.title);
            ic.detail = "";
        }
        static void AddItem(ImageContext ic,Thickness tk)
        {
            List<SearchItem> temp = SC.items;
            BitmapImage bi = new BitmapImage(new Uri(ic.src));
            if (top < SC.items.Count)
            {
                temp[top].title.Text = ic.title;
                //SC.items[i].detail.Text = "";
                temp[top].img.Margin = tk;
                temp[top].img.Source = bi;
                temp[top].img.DataContext = ic;
                tk.Top += 270;
                temp[top].title.Margin = tk;
                temp[top].img.Visibility = Visibility.Visible;
                temp[top].title.Visibility = Visibility.Visible;
            }
            else
            {
                SearchItem si = new SearchItem();
                Image img =
                si.img = new Image();
                img.Width = 260;
                img.Height = 260;
                img.Source = bi;
                img.DataContext = ic;
                img.Margin = tk;
                img.PointerReleased += I_released;
                img.Source = bi;
                SC.can.Children.Add(img);
                tk.Top += 260;
                TextBlock tb = si.title = new TextBlock();
                tb.Foreground = font_brush;
                tb.Margin = tk;
                tb.Text = ic.title;
                tb.Width = 290;
                tb.TextWrapping = TextWrapping.Wrap;
                SC.can.Children.Add(tb);
                SC.items.Add(si);
            }
            top++;
        }
        static void Reset()
        {
            if (SC.items.Count == 0)
                return;
            top = 0;
            int c = SC.items.Count;
            c--;
            if(c>20)
            {
                for (int e = c; e > 20; e--)
                {
                    SearchItem cc = SC.items[e];
                    SC.items.RemoveAt(e);
                    GC.SuppressFinalize(cc.title);
                    //GC.SuppressFinalize(cc.detail);
                    GC.SuppressFinalize(cc.img);
                }
                GC.Collect();
            }
            for (int e = 0; e < SC.items.Count; e++)
            {
                SC.items[e].img.Visibility = Visibility.Collapsed;
                SC.items[e].title.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region bar
        struct SearchBar
        {
            public Canvas can;
            public TextBox textbox;
            public SymbolIcon home;
            public SymbolIcon setting;
            public SymbolIcon filter;
            public SymbolIcon search;
            public ComboBox date;
            public ComboBox time;
            public ComboBox scope;
        }
        static SearchFilter Search_SF = new SearchFilter();
        static SearchBar Search_SB = new SearchBar();
        static void Filter_Create()
        {
            if (Search_SB.date != null)
            {
                if (Search_SB.date.Visibility == Visibility.Visible)
                {
                    Search_SB.date.Visibility = Visibility.Collapsed;
                    Search_SB.time.Visibility = Visibility.Collapsed;
                    Search_SB.scope.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Search_SB.date.Visibility = Visibility.Visible;
                    Search_SB.time.Visibility = Visibility.Visible;
                    Search_SB.scope.Visibility = Visibility.Visible;
                }
                return;
            }
            Canvas can = Search_SB.can;//new Canvas();
            SolidColorBrush scb = new SolidColorBrush(Colors.Pink);
            ComboBox cb = new ComboBox();
            cb.Background = scb;
            cb.ItemsSource = SearchDate;
            Thickness margin = new Thickness(60, 30, 0, 0);
            cb.Margin = margin;
            cb.SelectedIndex = 0;
            cb.SelectionChanged += (sender, e) => {
                Search_SF.date = "&pubfilter=" + (sender as ComboBox).SelectedIndex.ToString();
            };
            Search_SB.date = cb;
            can.Children.Add(cb);
            cb = new ComboBox();
            cb.Background = scb;
            margin.Top += 30;
            cb.ItemsSource = SearchTime;
            cb.Margin = margin;
            cb.SelectedIndex = 0;
            cb.SelectionChanged += (sender, e) => {
                Search_SF.time = "&duration=" + (sender as ComboBox).SelectedIndex.ToString();
            };
            Search_SB.time = cb;
            can.Children.Add(cb);
            cb = new ComboBox();
            cb.Background = scb;
            margin.Top += 30;
            cb.ItemsSource = SearchScope;
            cb.Margin = margin;
            cb.SelectedIndex = 0;
            cb.SelectionChanged += (sender, e) => {
                Search_SF.sort = "&tabid=" + (sender as ComboBox).SelectedIndex.ToString();
            };
            Search_SB.scope = cb;
            can.Children.Add(cb);
        }
        public static void Create_Bar(Canvas parent, Thickness margin)
        {
            if (Search_SB.can != null)
            {
                Search_SB.can.Visibility = Visibility.Visible;
                Bar_Resize(margin);
                return;
            }
            Search_SB = new SearchBar();
            double w = margin.Right - margin.Left;
            Canvas can = new Canvas();
            parent.Children.Add(can);
            can.Margin = margin;
            Search_SB.can = can;
            Thickness tk = new Thickness();
            SymbolIcon si = new SymbolIcon();
            tk.Top = 5;
            si.Margin = tk;
            si.Symbol = Symbol.Setting;
            si.PointerReleased += (s, e) => { Main.CreatePage(1, PageTag.setting); };
            can.Children.Add(si);
            Search_SB.setting = si;

            si = new SymbolIcon();
            tk.Left = 30;
            si.Margin = tk;
            si.Symbol = Symbol.Home;
            si.PointerReleased += (s, e) => { Main.CreatePage(0,PageTag.nav); };
            can.Children.Add(si);
            Search_SB.home = si;

            si = new SymbolIcon();
            si.Symbol = Symbol.Filter;
            tk.Left = 60;
            si.Margin = tk;
            si.PointerReleased += (s, e) => { Filter_Create(); };
            can.Children.Add(si);
            Search_SB.filter = si;

            si = new SymbolIcon();
            si.Symbol = Symbol.Find;
            tk.Left = w - 30;
            tk.Right = w;
            si.Margin = tk;
            si.PointerReleased += (s, e) => { Find(); };
            can.Children.Add(si);
            Search_SB.search = si;

            TextBox tb = new TextBox();
            tb.KeyDown += s_keydown;
            Search_SB.textbox = tb;
            tb.Width = w-120;
            tb.Height = 30;
            tk.Left = 90;
            tk.Top = 0;
            tb.Margin = tk;
            can.Children.Add(tb);
        }
        public static void Bar_Resize(Thickness margin)
        {
            double w = margin.Right - margin.Left;
            Search_SB.textbox.Width = w - 120;
            margin.Left = w - 30;
            margin.Top = 0;
            Search_SB.search.Margin=margin;
        }
        public static void Bar_Hide()
        {
            Search_SB.can.Visibility = Visibility.Collapsed;
        }
        public static void Bar_Show()
        {
            Search_SB.can.Visibility = Visibility.Visible;
        }
        static void s_keydown(Object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                Find();
        }
        static void Find()
        {
            if (Search_SB.textbox.Text != null & Search_SB.textbox.Text != "")
            {
                Search_SF.content = Search_SB.textbox.Text;
                Search(Search_SF);
                Main.CreatePage(0,PageTag.search);
            }
        }
        #endregion
    }
}
