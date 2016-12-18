using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TVWP.Class
{
    class Login:Component
    {
        static Canvas parent;
        static WebView wv;
        static TextBlock tip;
        public static void Create(Canvas p,Thickness m)
        {
            parent = p;
            tip = CreateTextBlockNext();
            tip.Text = "实在没办法，如果您有什么高见可以教我(去腾讯拿!!!我这样的菜鸟不会收吧)。没有vip，无法测试";
            tip.TextWrapping = TextWrapping.Wrap;
            double w = m.Right - m.Left;
            tip.Width = w;
            tip.Height = 40;
            tip.Foreground = warning_brush;
            tip.Margin = m;
            parent.Children.Add(tip);

            wv = new WebView();
            wv.Width = w;
            m.Top += 40;
            wv.Height = m.Bottom - m.Top;
            wv.Margin = m;
            wv.NavigationCompleted += NavOver;
            parent.Children.Add(wv);
            wv.Navigate(new Uri("http://v.qq.com/u/history/"));
            wv.NewWindowRequested += (o, e) => { e.Handled = true; };
        }
        static void NavOver(WebView w,WebViewNavigationCompletedEventArgs e)
        {
            WebClass.SetCookie();
        }
        public static void Dispose()
        {
            parent.Children.Remove(wv);
            parent.Children.Remove(tip);
            GC.SuppressFinalize(wv);
            RecycleTextBlock(tip);
            wv = null;
        }
        public static void ReSize(Thickness m)
        {
            double w = m.Right - m.Left;
            tip.Width = w;
            wv.Width = w;
            m.Top += 40;
            wv.Height = m.Bottom - m.Top;
            wv.Margin = m;
        }
    }
}
