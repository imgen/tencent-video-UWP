using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    class OriginalPage:CharOperation,SonPartialPage
    {
        #region const
        struct Area
        {
            public bool expand;
            public Canvas can;
            public Button title;
            public double height;
            public List<ItemModE> items;
            public List<ImageContext> context;
        }
        static string[] title_o = { "原创精选", "Original selection" };
        static string[] title_d1 = { "周一", "Monday" };
        static string[] title_d2 = { "周二", "Tuesday" };
        static string[] title_d3 = { "周三", "Wednesday" };
        static string[] title_d4 = { "周四", "Thursday" };
        static string[] title_d5 = { "周五", "Friday" };
        static string[] title_d6 = { "周六", "Saturday" };
        static string[] title_d7 = { "周日", "Sunday" };
        static string[][] title_all = {title_d1,title_d2,title_d3,title_d4,title_d5,title_d6,title_d7,title_o};
        static Area[] area ;
        static char[] Key_or = "原创精选".ToCharArray();
        static char[] Key_week = "一周更新时间表".ToCharArray();
        ScrollViewer sv;
        StackPanel sp;
        ItemSize its;
        #endregion

        static ImageContext AnlayzeItem(ref char[] c_buff, int s, int e)
        {
            ImageContext ic = new ImageContext();
            s = FindCharArray(ref c_buff, ref Key_href, s);
            char[] tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
            string aa;
            if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                aa = new string(tt);
            else aa = "http:" + new string(tt);
            ic.href = aa;
            s = FindCharArray(ref c_buff, ref Key_title, s);
            tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
            ic.title = new string(tt);
            s = FindCharArray(ref c_buff, ref Key_src, s);
            tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
            if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                aa = new string(tt);
            else aa = "http:" + new string(tt);
            ic.src = aa;
            return ic;
        }
        public void Create(Canvas p,Thickness m)
        {
            if(sv!=null)
            {
                sv.Visibility = Visibility.Visible;
                ReSize(m);
                return;
            }
            sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            p.Children.Add(sv);
            sp = new StackPanel();
            sv.Content = sp;
            area = new Area[8];
            for(int i=0;i<8;i++)
            {
                Canvas can = new Canvas();
                area[i].can = can;
                Button b = new Button();
                b.Foreground = Component.nav_brush;
                b.Background = Component.trans_brush;
                b.Content = title_all[i][Component.language];
                b.DataContext = i;
                b.Click += (o, e) => {
                    int index = (int)(o as Button).DataContext;
                    if(area[index].expand)
                    {
                        area[index].expand = false;
                        area[index].can.Height = 30;
                        int len = area[index].items.Count;
                        for (int c = 0; c < len; c++)
                            area[index].items[c].can.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        area[index].expand = true;
                        area[index].can.Height = area[index].height;
                        int len = area[index].items.Count;
                        for (int c = 0; c < len; c++)
                            area[index].items[c].can.Visibility = Visibility.Visible;
                    }
                };
                can.Children.Add(b);
                area[i].title = b;
                area[i].items = new List<ItemModE>();
                area[i].context = new List<ImageContext>();
                sp.Children.Add(can);
            }
            ReSize(m);
        }
        public void UpdatePage(string data)
        {
            for (int i = 0; i < 8; i++)
                area[i].context.Clear();
            char[] buff = data.ToCharArray();
            buff = DeleteChar(ref buff, '\\');
            int s = FindCharArray(ref buff,ref Key_or,0);
            s = FindCharArray(ref buff,ref Key_h2,s);
            int e = FindCharArray(ref buff,ref Key_c_over,s);
            while (s > 0)
            {
                s = FindCharArray(ref buff, ref Key_list_item, s, e);
                if (s < 0)
                    break;
                area[7].context.Add(AnlayzeItem(ref buff,s,e));
            }
            s = e;
            s = FindCharArray(ref buff,ref Key_week,s);
            s = FindCharArray(ref buff, ref Key_weekline_title, s);
            e = FindCharArray(ref buff, ref Key_weekline_title, s);
            int end = FindCharArray(ref buff,ref Key_c_over, e);
            for(int i=0;i<7;i++)
            {
                while(s>0)
                {
                    s = FindCharArray(ref buff, ref Key_list_item, s, e);
                    if (s < 0)
                        break;
                    area[i].context.Add(AnlayzeItem(ref buff, s, e));
                }
                s = e;
                e = FindCharArray(ref buff,ref Key_weekline_title,e);
                if (e < 0)
                    e = end;
            }
            for (int i = 0; i < 8; i++)
            {
                UpdateArea(ref area[i]);
                ResizeArea(ref area[i]);
            }
        }
        void ResizeArea(ref Area ar)
        {
            double sw = its.sw;
            double sh = its.sh;
            double dx = 0;
            double dy = 30;
            int l = ar.context.Count;
            for (int i = 0; i < l; i++)
            {
                if (dx + sw > its.w)
                {
                    dx = 0;
                    dy += sh;
                }
                ItemModE im = ar.items[i];
                im.can.Margin = new Thickness(dx, dy, 0, 0);
                im.img.Width = its.iw;
                im.img.Height = its.ih;
                im.title.Width = its.iw;
                im.title.Margin = new Thickness(0, its.oy_t, 0, 0);
                im.button.Width = its.iw;
                im.button.Height = sh;
                dx += sw;
            }
            dy += sh;
            ar.can.Width = its.w;
            if (ar.expand)
                ar.can.Height = dy;
            else ar.can.Height = 30;
            ar.height = dy;
        }
        void UpdateArea(ref Area ar)
        {
            List<ImageContext> lic = ar.context;
            int len = lic.Count;
            List<ItemModE> lim = ar.items;
            int c =lim.Count;
            ItemModE im;
            for (int i=0;i< len;i++)
            {
                if(i>=c)
                {
                    im = Component.CreateItemMod();
                    im.button.Click += (o,e)=>{
                        object ob = (o as Button).DataContext;
                        if (ob != null)
                        {
                            VideoPage.SetAdress(o as string);
                            PageManageEx.CreateNewPage(PageTag.videopage);
                        }
                    };
                    ar.can.Children.Add(im.can);
                    lim.Add(im);
                }
                im = lim[i];
                ImageContext ic = lic[i];
                if (ic.src == null)
                    break;
                (im.img.Source as BitmapImage).UriSource = new Uri(ic.src);
                im.button.DataContext = ic.href;
                if (ic.title == null)
                    im.title.Text = "";
                else im.title.Text = ic.title;
                if (!ar.expand)
                    im.can.Visibility = Visibility.Collapsed;
            }
            for (int i = c-1; i >= len; i--)
            {
                im = lim[i];
                lim.RemoveAt(i);
                //ReCycleItemMod(im.index);
            }
        }
        public void ReSize(Thickness m)
        {
            double w = m.Right - m.Left;
            double h = m.Bottom - m.Top;
            //CalculItemSize(ref its,w);
            for(int i=0;i<8;i++)
                ResizeArea(ref area[i]);
            sv.Width = w;
            sv.Height = h;
            sp.Width = w;
        }
        public void Hide()
        {
            sv.Visibility = Visibility.Collapsed;
        }
        public void Dispose()
        {
            sv.Visibility = Visibility.Collapsed;
            for(int i=0;i<8;i++)
            {
                List<ItemModE> lim = area[i].items;
                Canvas can = area[i].can;
                int max = can.Children.Count-1;
                int len = lim.Count;
                for(int c=len-1;c>=0;c--)
                {
                    can.Children.RemoveAt(max);
                    max--;
                    int index = lim[c].index;
                    lim.RemoveAt(c);
                    //ReCycleItemMod(index);
                }
            }
        }
    }
}
