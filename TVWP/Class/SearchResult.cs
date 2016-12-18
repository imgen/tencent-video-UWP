using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    class SearchResult_m
    {
        static string address = "http://m.v.qq.com/search.html?act=0&keyWord=";// + content;
        static TextBox tb;
        static SymbolIcon ok;
        static Scroll_S sv;
        static int index;
        static int part;
        static bool loading;
        public static void Create(Canvas p,Thickness m)
        {
            if(tb!=null)
            {
                
                ReSize(m);
                return;
            }
            double w = m.Right - m.Left;
            tb = new TextBox();
            tb.KeyDown += s_keydown;
            tb.Width = w - 30;
            p.Children.Add(tb);

            ok = new SymbolIcon();
            ok.Symbol = Symbol.Find;
            Thickness tk = new Thickness();
            tk.Left = w - 30;
            tk.Right = w;
            ok.Margin = tk;
            ok.PointerReleased += (s, e) => { Find(); };
            p.Children.Add(ok);

            
            m.Top += 40;
            m.Right -= 4;
            m.Bottom -= 10;
            sv = new Scroll_S();
            sv.SetParent(p);
            sv.ReSize(m);
            sv.Getmore = () => { loading = true; index += 15;part++; Find(); };
            sv.itemclick = (o) => {
                VideoPage.SetAdress(o as string);
                PageManageEx.CreateNewPage(PageTag.videopage);
            };
#if desktop
            sv.ShowBorder();
#endif
        }
        static void s_keydown(Object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                index = 0;
                part = 1;
                if (!loading)
                {
                    loading = true;
                    sv.LoadOver = false;
                    sv.data.Clear();
                    Find();
                }
            }
        }
        static void Find()
        {
            string url = "http://node.video.qq.com/x/cgi/msearch?contextValue=last_end=%3D"+
                index.ToString()+"%26areaId%3D101&keyWord=";
            url += Uri.EscapeUriString(tb.Text);
            url += "&contextType=2&callback=jsonp"+part.ToString();
            string str = address + tb.Text;
            str = Uri.EscapeUriString(str);
            NetClass.TaskGet(url, Analyze,str);
        }
        static void Analyze(string data)
        {
            int c= ParseData.Search_ex(data.ToCharArray(), sv.data);
            if (c < 14)
                sv.LoadOver = true;
            sv.Refresh();
            loading = false;
        }
        public static void ReSize(Thickness m)
        {
            double w = m.Right - m.Left;
            tb.Width = w - 30;
            Thickness tk = new Thickness();
            tk.Left = w - 30;
            tk.Right = w;
            ok.Margin = tk;
            m.Top += 40;
            m.Right -= 4;
            m.Bottom -= 10;
            sv.ReSize(m);
            sv.Refresh();
        }
        public static void Hide()
        {
            
        }
        public static void Show()
        {

        }
    }
}
