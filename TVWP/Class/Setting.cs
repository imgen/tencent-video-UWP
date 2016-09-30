using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace TVWP.Class
{
    class Setting
    {
        static Canvas can;
        static TextBlock declare;
        static string auth = "因为时间关系（准备去弄cry engine）,主页，多语言支持，下载缓存功能已砍"+
            "，设置和存储功能也没做，实在不擅长UI，源码地址https://github.com/huqiang0204/TVUWP，可以自己去改，" +
            "另外还有一款unity开发的游戏，飞行射击测试版（名字这么齪->微软不让改，只能下架改名上架）." +
            "2d碰撞算法，顶点贴图，cg着色，多线程交互委托，内存池，等等，手机能稳定50-60帧，场景载入速度（你看不到30ms内），启动速度不能改（这锅是unity引擎限制）"+
            "我QQ群89644028 求工作，求收留huqiang1990@outlook.com\r\n"+
            "搜索功能有明显Bug，腾讯给的搜索结果包涵了其它网站链接，过滤没有足够的时间去写，如果是打开其它网站的链接，解析失败会造成程序崩溃（闪退）";
        public static void Create(Canvas parent,Thickness margin)
        {
            can = new Canvas();
            can.Margin = margin;
            double w = margin.Right - margin.Left;
            double h = margin.Bottom - margin.Top;
            can.Width = w;
            can.Height = h;
            parent.Children.Add(can);
            can.Background = new SolidColorBrush(Colors.White);
            declare = new TextBlock();
            declare.TextWrapping = TextWrapping.Wrap;
            declare.Width = w;
            declare.Text = auth;
            declare.FontSize = 18;
            can.Children.Add(declare);
        }
        public static void Resize(Thickness margin)
        {
            double w = margin.Right - margin.Left;
            double h = margin.Bottom - margin.Top;
            can.Width = w;
            can.Height = h;
            declare.Width = w;
        }
        public static void Dispose()
        {
            can.Visibility = Visibility.Collapsed;
            declare.Visibility = Visibility.Collapsed;
            GC.SuppressFinalize(declare);
            GC.SuppressFinalize(can);
            GC.Collect();
        }
    }
}
