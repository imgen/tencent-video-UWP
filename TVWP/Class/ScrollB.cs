using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace TVWP.Class
{
    class Scroll_UC : Component, IDisposable
    {
        struct Picture
        {
            public Image img;
            public BitmapImage map;
        }
        struct Position
        {
            public int ss;
            public double sio;
            public double start;
            public double offset;
        }
        struct Size
        {
            public int index;
            public double width;
            public double height;
        }
        struct UIPoint
        {
            public double width;
            public double height;
            public double ox;
            public double oy;
            public int layout;
            public string content;
        }
        struct Item
        {
            public bool reg;
            public int index;
            public TextBlock[] ltb;
            public Picture[] lp;
            public Ellipse ell;
            public ImageBrush brush;
            public BitmapImage map;
            public TextBlock title;
            public TextBlock time;
            public Border bor;
        }
        ScrollViewer sv;
        Canvas content;
        Canvas parent;
        Thickness margin;
        double width;
        double height;
        bool min, max;
        const double os_max = 2000000;
        const double os_sta = 200000;
        Position pos;
        List<Size> ls;
        Size si;
        List<UIPoint[]> lup;
        
        public Action<object> itemclick;
        public bool LoadOver { get; set; }
        public Action Getmore;
        public Scroll_UC()
        {
            sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            sv.ViewChanged += (o, e) => { SlidingY(); };
            content = new Canvas();
            sv.Content = content;
            content.Height = os_max;
            //pos.offset = os_max;
            lci = new List<UpCommentInfo>();
            ls = new List<Size>();
            lup = new List<UIPoint[]>();
            i_buff = new Item[10];
        }
      
        public List<UpCommentInfo> lci { get; set; }
        public void Refresh()
        {
            int i;
            for (i = ls.Count; i < lci.Count; i++)
            {
                si.index = -1;
                ls.Add(si);
                CalculSize(i);
            }
            if (lci.Count > 0)
            {
                content.Visibility = Visibility.Visible;
                Sliding(0);
            }
            else content.Visibility = Visibility.Collapsed;
        }
        public void SetParent(Canvas p)
        {
            p.Children.Add(sv);
            parent = p;
        }
        public void ReSize(Thickness m)
        {
            if (margin == m)
                return;
            margin = m;
            width = m.Right - m.Left;
            height = m.Bottom - m.Top;
            sv.Width = width;
            sv.Height = height;
            sv.Margin = m;
            content.Width = width;
        }
        public void ReSet()
        {
            pos.start = 0;
            //pos.offset = os_sta;
            pos.ss = 0;
            pos.sio = 0;
            lci.Clear();
            ls.Clear();
            lup.Clear();
            LoadOver = false;
            content.Height = os_max;
            sv.ChangeView(0,0,1);
            for(int i=0;i<10;i++)
            {
                if(i_buff[i].reg)
                {
                    RecycleArea(i);
                }
            }
        }
        public void Dispose()
        {
            parent.Children.Remove(sv);
            content.Children.Clear();
            for (int i = 0; i < 10; i++)
            {
                if (i_buff[i].reg)
                {
                    DisposeArea(i);
                }
            }
            sv.Content = null;
            GC.SuppressFinalize(content);
            GC.SuppressFinalize(sv);
            GC.SuppressFinalize(this);
        }
        void CalculSize(int s)
        {
            double h = CalcualArea(s);
            si.width = width;
            si.height = h;
            ls[s] = si;
        }

        public void ShowBorder()
        {
            sv.BorderBrush = bor_brush;
            sv.BorderThickness = new Thickness(1, 1, 1, 1);
        }
        public void Hide()
        {
            sv.Visibility = Visibility.Collapsed;
        }
        public void Show()
        {
            sv.Visibility = Visibility.Visible;
        }

        static Item[] i_buff;//10
        static int RegArea(int index)
        {
            for(int i=0;i<10;i++)
            {
                if(!i_buff[i].reg)
                {
                    i_buff[i].reg = true;
                    i_buff[i].index = index;
                    return i;
                }
            }
            return -1;
        }

        void UpdateArea(double dx,double dy, UpCommentInfo ci, UIPoint[] up,  Item cc)
        {
            cc.bor.Margin = new Thickness(0, dy, 0, 0);
            cc.ell.Margin = new Thickness(0, dy + 5, 0, 0);
            cc.title.Margin = new Thickness(54, dy, 0, 0);
            cc.time.Margin = new Thickness(54, dy + 24, 0, 0);
            if (ci.url != null & ci.url != "")
                cc.map.UriSource = new Uri(ci.url);
            string str = ci.nick + "  uid:" + ci.u_id;// + "  mid:" + buff_com[i].uidB;
            cc.title.Text = str;
            cc.time.Text = ci.time + ci.region;
            dy += 54;
            List<UpContent> lu = ci.detail_s;
            int c = lu.Count;
            int ts=0;
            int ms = 0;
            TextBlock[] t = cc.ltb;
            Picture[] p = cc.lp;
            for(int i=0;i<c;i++)
            {
                if(lu[i].type=='i')//image
                {
                    p[ms].map.UriSource = new Uri(lu[i].content);
                    double ox = dx + up[i].ox;
                    double oy = dy + up[i].oy;
                    p[ms].img.Margin = new Thickness(ox,oy,0,0);
                    ms++;
                }
                else//text
                {
                    t[ts].Text = up[i].content;
                    t[ts].Width = width;
                    t[ts].Height = up[i].height;
                    double ox = dx + up[i].ox;
                    double oy = dy + up[i].oy;
                    t[ts].Margin = new Thickness(ox, oy, 0, 0);
                    ts++;
                }
            }
        }
        double CalcualArea(int s)
        {
            List<UpContent> lu = lci[s].detail_s;
            if (s >= lup.Count)
                lup.Add(AreaLayout(lu, s));
            else if (ls[s].width == width)
                return ls[s].height;
            UIPoint[] up = lup[s];
            //0=all,1=left,2=right,3=piece
            int c = lu.Count;
            double ox = 0;
            double oy = 0;
            int count = (int)(width / 15.6f);
            int count1 = count - 9;
            int count2 = count - 7;
            for (int i = 0; i < c; i++)
            {
                if (lu[i].type == 'i')
                {
                    switch (up[i].layout)
                    {
                        case 0:
                            up[i].ox = ox;
                            up[i].oy = oy;
                            ox += 140;
                            if (ox >= width)
                            {
                                ox = 0;
                                oy += 144;
                            }
                            break;
                        case 1:
                            if (lu[i].width > lu[i].height)
                            {
                                up[i].ox = 5;
                                up[i].oy = oy - 18;
                            }
                            else
                            {
                                up[i].ox = -15;
                                up[i].oy = oy;
                            }
                            break;
                        case 2:
                            up[i].ox = width - 140;
                            up[i].oy = oy;
                            ox = 0;
                            break;
                        case 3:
                            up[i].ox = ox;
                            up[i].oy = oy;
                            ox += 140;
                            if (ox+140 >= width)
                            {
                                ox = 0;
                                oy += 140;
                            }
                            break;
                    }
                }
                else
                {
                    switch (up[i].layout)
                    {
                        case 0:
                            if(i>0)
                            {
                                if (up[i-1].layout == 3)
                                    oy += 140;
                            }
                            up[i].oy = oy;
                            int row;
                            char[] t = CharOperation.CharWarp(lu[i].text, count, out row);
                            up[i].content = new string(t);
                            up[i].width = width;
                            int h = row * 20;
                            up[i].height = h;
                            oy += h;
                            break;
                        default:
                            int a;
                            if (up[i].layout == 1)
                                a = i + 1;
                            else a = i - 1;
                            if (lu[a].width > lu[a].height)
                            {
                                t = CharOperation.CharWarp(lu[i].text, count1, 5, count, out row);
                                if (up[i].layout == 2)
                                    t = CharOperation.CharInsertSpace(ref t, 5, 36);
                            }
                            else
                            {
                                t = CharOperation.CharWarp(lu[i].text, count2, 8, count, out row);
                                if (up[i].layout == 2)
                                    t = CharOperation.CharInsertSpace(ref t, 8, 28);
                            }
                            up[i].oy = oy;
                            up[i].content = new string(t);
                            up[i].width = width;
                            row++;
                            h = row * 20;
                            up[i].height = h;
                            if (up[i].layout == 1)
                            {
                                if (lu[a].width > lu[a].height)
                                {
                                    up[a].oy = oy - 18;
                                    up[a].ox = width - 150;
                                }
                                else
                                {
                                    up[a].oy = oy + 4;
                                    up[a].ox = width - 130;
                                }
                                i++;
                            }
                            if (h < 144)
                                oy += 144;
                            else oy += h;
                            break;
                    }
                }
            }
            if(c>0)
            if (up[c - 1].layout == 3)
                oy += 140;
            return oy+84;
        }
        UIPoint[] AreaLayout(List<UpContent> lu, int s)
        {
            int c = lu.Count;
            UIPoint[] up = new UIPoint[c];
            int t = 0,o=0;
            for(int i=0;i<c;i++)
            {
                if(lu[i].type=='i')
                {
                    if(t==0)
                    {
                        up[i].layout = 1;
                        t = 1;
                    }else if(t==1)
                    {
                        up[i].layout = 2;
                        t = 2;
                    }else if(t==2)
                    {
                        if(o=='i')
                        {
                            up[i].layout = 3;
                            up[i -1].layout = 3;
                            up[i - 2].layout = 0;
                            t = 3;
                        }else
                        {
                            up[i].layout = 1;
                            t = 1;
                        }
                    }
                    else up[i].layout = 3;
                    o = 'i';
                }
                else
                {
                    if(i+1<c)
                    {
                        if(t==0)
                        {
                            if (lu[i + 1].type == 'i')
                            {
                                up[i].layout = 1;
                                t = 1;
                            }
                            else
                            {
                                up[i].layout = 0;
                                t = 0;
                            }
                        }else
                        if(t==1)
                        {
                            if (o == 'i')
                            { up[i].layout = 2;t = 2; }
                            else
                            {
                                up[i].layout = 0;
                                up[i - 1].layout = 0;
                                t = 0;
                            }
                        }else if(t==2)
                        {
                            if(o=='i')
                            {
                                up[i].layout = 1;
                                t = 1;
                            }else
                            {
                                up[i].layout = 0;
                                t = 0;
                            }
                        }
                        else
                        {
                            up[i].layout = 0;
                            t = 0;
                        }
                    }
                    else
                    {
                        if (t == 1)
                        {
                            up[i].layout = 2;
                            t = 2;
                        }
                        else
                        {
                            up[i].layout = 0;
                            t = 0;
                        }
                    }
                    o = 't';
                }
            }
            return up;
        }
        void CreateArea(int index,List<UpContent> lu)
        {
            Item cb = i_buff[index];

            if(cb.ell==null)
            {
                cb.ell = new Ellipse();//head
                cb.ell.Width = 36;
                cb.ell.Height = 36;
                cb.brush = new ImageBrush();
                cb.map = new BitmapImage();
                cb.ell.Fill = cb.brush;
                cb.brush.ImageSource = cb.map;
                content.Children.Add(cb.ell);

                cb.time = CreateTextBlockNext();
                cb.time.Foreground = font_brush;
                cb.title = CreateTextBlockNext();
                cb.title.Foreground = title_brush;
                cb.bor = CreateBorderNext();
                cb.bor.BorderBrush = filter_brush;
                cb.bor.BorderThickness = new Thickness(0, 1, 0, 0);
                cb.bor.Width = width;
                cb.bor.Height = 2;
                content.Children.Add(cb.bor);
                content.Children.Add(cb.time);
                content.Children.Add(cb.title);
            }
            else
            {
                cb.ell.Visibility = Visibility.Visible;
                cb.time.Visibility = Visibility.Visible;
                cb.title.Visibility = Visibility.Visible;
                cb.bor.Visibility = Visibility.Visible;
            }
            
            int c = lu.Count;
            int t = 0;
            for (int i = 0; i < c; i++)
            {
                if (lu[i].type == 't')
                    t++;
            }
            int m = c - t;
            TextBlock[] temp = new TextBlock[t];
            for (int i = 0; i < t; i++)
            {
                temp[i] = CreateTextBlockNext();
                temp[i].Width = width;
                temp[i].Foreground = font_brush;
                content.Children.Add(temp[i]);
            }
            cb.ltb = temp;
            Picture[] p = new Picture[m];
            for (int i = 0; i < m; i++)
            {
                p[i].img = CreateImageNext();
                p[i].map = p[i].img.Source as BitmapImage;
                p[i].img.Width = 140;
                p[i].img.Height = 140;
                content.Children.Add(p[i].img);
            }
            cb.lp = p;
            i_buff[index] = cb;
        }
        void RecycleArea(int index)
        {
            i_buff[index].reg = false;
            Item cb = i_buff[index];
            cb.ell.Visibility = Visibility.Collapsed;
            cb.time.Visibility = Visibility.Collapsed;
            cb.title.Visibility = Visibility.Collapsed;
            cb.bor.Visibility = Visibility.Collapsed;
            TextBlock[] t = cb.ltb;
            int c = t.Length;
            for (int i = 0; i < c; i++)
            {
                content.Children.Remove(t[i]);
                RecycleTextBlock(t[i]);
            }
            Picture[] p = cb.lp;
            c = p.Length;
            for (int i = 0; i < c; i++)
            {
                content.Children.Remove(p[i].img);
                RecycleImage(p[i].img);
            }
        }
        void DisposeArea(int index)
        {
            i_buff[index].reg = false;
            Item cb = i_buff[index];
            GC.SuppressFinalize(cb.ell);
            GC.SuppressFinalize(cb.map);
            GC.SuppressFinalize(cb.brush);
            RecycleBorder(cb.bor);
            RecycleTextBlock(cb.time);
            RecycleTextBlock(cb.title);
            TextBlock[] t = cb.ltb;
            int c = t.Length;
            for (int i = 0; i < c; i++)
            {
                content.Children.Remove(t[i]);
                RecycleTextBlock(t[i]);
            }
            Picture[] p = cb.lp;
            c = p.Length;
            for (int i = 0; i < c; i++)
            {
                content.Children.Remove(p[i].img);
                RecycleImage(p[i].img);
            }
        }
        void SlidingY()
        {
            double oy = pos.offset - sv.VerticalOffset;
            pos.offset = sv.VerticalOffset;
            if (oy != 0 & lci.Count > 0)
            {
                Sliding(oy);
            }
        }
        void Sliding(double oy)
        {
            //if (min & oy >= 0)
            //{
            //    pos.start = pos.offset;
            //    pos.sio = 0;
            //    return;
            //}
            //else min = false;
            //if (max & oy < 0)
            //{
            //    pos.start = pos.offset + pos.sio;
            //    return;
            //}
            //else max = false;
            pos.sio += oy;
            GetPosition();
            Update();
        }
        void GetPosition()
        {
            int s = pos.ss;
            if (s >= ls.Count - 2)
                return;
            if(ls[s].width!=width)
            {
                double o = CalcualArea(s);
                si.width = width;
                si.height = o;
                ls[s] = si;
            }
            double ih = ls[s].height;
            ih += pos.start+pos.sio;
            if (ih < -30)
            {
                if(pos.ss<lci.Count-2)
                {
                    RecycleArea(ls[s].index);
                    si = ls[s];
                    si.index = -1;
                    ls[s] = si;
                    pos.start += ls[s].height;
                    pos.sio += ls[s].height;
                    pos.ss++;
                }
            }
            else if(pos.start+pos.sio>-30)
            {
                if (pos.ss > 0)
                {
                    pos.ss--;
                    s--;
                    int r = RegArea(s);
                    Size z = ls[s];
                    z.index = r;
                    z.height = CalcualArea(s);
                    z.width = width;
                    ls[s] = z;
                    pos.start -= z.height;
                    pos.sio -= z.height;
                    CreateArea(r, lci[s].detail_s);
                    UpdateArea(0, pos.start, lci[s], lup[s], i_buff[r]);
                }
            }
        }
        void Update()
        {
            int s=pos.ss;
            double dy = pos.offset+pos.sio;
            int c = lci.Count;
            double top = pos.offset + height+30;
            while(s<c)
            {
                int o = ls[s].index;
                if (o < 0)
                {
                    int r = RegArea(s);
                    Size z = ls[s];
                    z.index = r;
                    z.height = CalcualArea(s);
                    z.width = width;
                    ls[s] = z;
                    CreateArea(r, lci[s].detail_s);
                    UpdateArea(0, dy, lci[s], lup[s], i_buff[r]);
                    dy += z.height;
                }
                else
                {
                    dy += ls[s].height;
                    if(dy<pos.offset-30)
                    {
                        RecycleArea(ls[s].index);
                        si = ls[s];
                        si.index = -1;
                        ls[s] = si;
                        pos.start += ls[s].height;
                        pos.sio += ls[s].height;
                        pos.ss++;
                    }
                }
                s++;
                if(dy>top)
                {
                    break;
                }
                if(s==c)
                {
                    if (dy < height)
                        content.Height = height - 5;
                    else content.Height = dy;
                }
            }
            if(s>=c-4)
            {
                if (!LoadOver)
                    if (Getmore != null)
                        Getmore();
            }
            while(s<c)
            {
                if (ls[s].index > 0)
                {
                    RecycleArea(ls[s].index);
                    si = ls[s];
                    si.index = -1;
                    ls[s] = si;
                }
                else break;
                s++;
            }
        }
    }
}
