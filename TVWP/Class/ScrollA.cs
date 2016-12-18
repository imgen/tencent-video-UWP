using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace TVWP.Class
{
    class ScrollViewA:Component, IDisposable
    {
        struct Item
        {
            public Image img;
            public BitmapImage map;
            public TextBlock title;
            public Button button;
        }
        struct Position
        {
            public int column;
            public int maxcount;
        }
        struct ItemSizeA
        {
            public double svw;
            public double svh;
            public double cw;
            public double ch;
            public double iw;
            public double ih;
            public double oy_t;
        }
        ScrollViewer sv;
        Canvas content;
        Canvas parent;
        Thickness margin;
        ItemSizeA isa;
        Position pos;
        List<Item> li;
        public ScrollViewA()
        {
            sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            content = new Canvas();
            sv.Content = content;
            sv.ViewChanged += Sliding;
            li = new List<Item>();
            data = new List<ItemDataA>();
        }
        void Sliding(object o, ScrollViewerViewChangedEventArgs e)
        {
            OrderItem();
        }
        public Action<object> itemclick;
        void OrderItem()
        {
            double os = sv.VerticalOffset;
            int index = (int)((os - isa.ch * 0.3f) / isa.ch);
            double dy = index * isa.ch;
            index *= pos.column;
            Item ima;
            int max = li.Count;
            if (max == 0)
                return;
            int s = index % max;
            double dx = 0;
            if (index + max<= data.Count+ pos.column)
                for (int i = 0; i < max; i++)
                {
                    ima = li[s];
                    double ox = dx + 5;
                    ima.img.Margin = new Thickness(ox, dy, 0, 0);
                    ima.title.Margin = new Thickness(ox,dy+isa.oy_t,0,0);
                    ima.button.Margin = new Thickness(ox,dy,0,0);
                    ima.button.DataContext = data[index].href;
                    if (data[index].src != null)
                        if (ima.map.UriSource==null|| ima.map.UriSource.OriginalString != data[index].src)
                            ima.map.UriSource = new Uri(data[index].src);
                    ima.title.Text = data[index].title;
                    ItemClick ic = new ItemClick();
                    ic.click = itemclick;
                    ic.tag = data[index].detail;
                    ima.button.DataContext = ic;
                    index++;
                    if (index >= data.Count)
                        break;
                    s++;
                    if (s >= max)
                        s = 0;
                    dx += isa.cw;
                    if (dx + isa.iw > isa.svw)
                    {
                        dx = 0;
                        dy += isa.ch;
                    }
                }
        }
        public List<ItemDataA> data { set; get; }
        public void Refresh()
        {
            int max = data.Count;
            int row = max / pos.column;
            int r = max % pos.column;
            if (r > 0)
                row++;
            double h = row * isa.ch;
            if (h < isa.svh)
                content.Height = isa.svh;
            else content.Height = h;
            if (max > pos.maxcount)
                max = pos.maxcount;
            int c = li.Count;
            int i;
            Item imb;
            for (i = 0; i < max; i++)
            {
                if (i >= c)
                    imb = CreateNewItem();
                else
                    imb = li[i];
                ResizeItem(ref imb);
            }
            for (i = c - 1; i >= max; i--)
            {
                imb = li[i];
                content.Children.Remove(imb.img);
                content.Children.Remove(imb.title);
                content.Children.Remove(imb.button);
                RecycleImage(imb.img);
                RecycleTextBlock(imb.title);
                RecycleButton(imb.button);
                li.RemoveAt(i);
            }
            if (data.Count > 0)
                OrderItem();
        }
        public void SetParent(Canvas p)
        {
            parent = p;
            p.Children.Add(sv);
        }
        public void ReSize(Thickness m)
        {
            if (margin == m)
                return;
            margin = m;
            sv.Margin = m;
            double w = m.Right - m.Left;
            double h = m.Bottom - m.Top;
            isa.svw= sv.Width = w;
            isa.svh= sv.Height = h;
            int c =(int) (w / minX);
            if (c < 1)
                c = 1;
            w /= c;
            isa.cw = w;
            isa.iw = w - 10;
            isa.ih = isa.iw*0.5625f;
            isa.ch = isa.ih+40;
            isa.oy_t = isa.ih;
            int r = (int)(h / isa.ch);
            pos.maxcount = (r+2)*c;
            pos.column = c;
        }
        Item CreateNewItem()
        {
            Item i = new Item();
            i.img = Component.CreateImageNext();
            i.map = i.img.Source as BitmapImage;
            i.title = Component.CreateNewTextBlock();
            i.title.Foreground = Component.title_brush;
            i.button = Component.CreateButtonNext();
            i.button.Background = Component.trans_brush;
            i.button.BorderBrush = Component.filter_brush;
            i.button.BorderThickness = new Thickness(1,1,1,1);
            content.Children.Add(i.img);
            content.Children.Add(i.title);
            content.Children.Add(i.button);
            li.Add(i);
            return i;
        }
        void ResizeItem(ref Item i)
        {
            i.img.Width = isa.iw;
            i.img.Height = isa.ih;
            i.title.Width = isa.iw;
            i.button.Width = isa.iw;
            i.button.Height = isa.ch - 8;
        }
        public void Dispose()
        {
            parent.Children.Remove(sv);
            content.Children.Clear();
            int c = li.Count;
            for (int i = c - 1; i > 0; i--)
            {
                Item ida = li[i];
                Component.RecycleImage(ida.img);
                Component.RecycleTextBlock(ida.title);
                Component.RecycleButton(ida.button);
            }
            li.Clear();
            sv.Content = null;
            GC.SuppressFinalize(content);
            GC.SuppressFinalize(sv);
            GC.SuppressFinalize(li);
            GC.SuppressFinalize(this);
        }
        public void ShowBorder()
        {
            sv.BorderBrush = Component.bor_brush;
            sv.BorderThickness = new Thickness(1, 1, 1, 1);
        }
    }

    class ScrollViewB:Component, IDisposable
    {
        struct Item
        {
            public Border bor;
            public Image img;
            public BitmapImage map;
            public TextBlock title;
            public TextBlock detail;
            public Button button;
            public Image mark;
            public BitmapImage m_map;
        }
        struct Position
        {
            public int column;
            public int row;
            public int maxcount;
        }
        struct ItemSizeB
        {
            public double svw;
            public double svh;
            public double cw;
            public double ch;
            public double iw;
            public double ih;
            public double ox_i;
            public double oy_t;
            public double oy_d;
        }
        ScrollViewer sv;
        Canvas content;
        Canvas parent;
        Thickness margin;
        ItemSizeB isa;
        Position pos;
        List<Item> li;
        public Action<object> itemclick;
        public ScrollViewB()
        {
            sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            content = new Canvas();
            sv.Content = content;
            sv.ViewChanged += Sliding;
            data = new List<ItemDataB>();
            li = new List<Item>();
        }
        void Sliding(object o, ScrollViewerViewChangedEventArgs e)
        {
            OrderItem();
        }
        Item CreateNewItem()
        {
            Item i = new Item();
            i.img = Component.CreateImageNext();
            i.map = i.img.Source as BitmapImage;
            i.mark = Component.CreateImageNext();
            i.m_map = i.mark.Source as BitmapImage;
            i.bor = Component.CreateBorderNext();
            i.bor.Height = 54;
            i.bor.Background = Component.tag_brush_b;
            i.title = Component.CreateTextBlockNext();
            i.title.Foreground = Component.title_brush;
            i.title.FontSize = 18;
            i.detail = Component.CreateTextBlockNext();
            i.detail.Foreground = Component.font_brush;
            i.detail.FontSize = 13;
            i.detail.Height = 54;
            i.button = Component.CreateButtonNext();
            i.button.BorderThickness = new Thickness(1,1,1,1);
            i.button.BorderBrush = Component.filter_brush;
            li.Add(i);
            content.Children.Add(i.img);
            content.Children.Add(i.bor);
            content.Children.Add(i.mark);
            content.Children.Add(i.title);
            content.Children.Add(i.detail);
            content.Children.Add(i.button);
            return i;
        }
        void ResizeItem(ref Item im)
        {
            im.img.Width = isa.iw;
            im.img.Height = isa.ih;
            im.title.Width = isa.iw;
            im.detail.Width = isa.iw;
            im.button.Width = isa.iw+4;
            im.button.Height = isa.ch-5;
            im.bor.Width = isa.iw;
        }
        void OrderItem()
        {
            double os = sv.VerticalOffset;
            int row = (int)((os - isa.ch * 0.3f) / isa.ch);
            int index = row * pos.column;
            double dy = row * isa.ch;
            double dx = 0;
            Item imb;
            int max = li.Count;
            if (max == 0)
                return;
            int s = index % max;
            if (index + max <= data.Count + pos.column)
                for (int i = 0; i < max; i++)
                {
                    imb = li[s];
                    if (dx + isa.cw > isa.svw)
                    {
                        dx = 0;
                        dy += isa.ch;
                    }
                    double ox = dx + isa.ox_i;
                    imb.img.Margin = new Thickness(ox, dy+2, 0, 0);
                    imb.title.Margin = new Thickness(ox, isa.oy_t+dy, 0, 0);
                    imb.button.Margin = new Thickness(ox, dy, 0, 0);
                    if(imb.map.UriSource==null || imb.map.UriSource.OriginalString!= data[index].src)
                       imb.map.UriSource = new Uri(data[index].src);
                    if (data[index].mark != null)
                    {
                        imb.mark.Visibility = Visibility.Visible;
                        if (imb.m_map.UriSource == null || imb.m_map.UriSource.OriginalString != data[index].mark)
                            imb.m_map.UriSource = new Uri(data[index].mark);
                        imb.mark.Margin= new Thickness(ox, dy+2, 0, 0);
                    }
                    else imb.mark.Visibility = Visibility.Collapsed;
                    if(data[index].detail!=null)
                    {
                        imb.detail.Visibility = Visibility.Visible;
                        imb.bor.Visibility = Visibility.Visible;
                        imb.detail.Text = data[index].detail;
                        Thickness tk= new Thickness(ox, isa.oy_d + dy, 0, 0);
                        imb.detail.Margin = tk;
                        imb.bor.Margin = tk;
                    }
                    else
                    {
                        imb.detail.Visibility = Visibility.Collapsed;
                        imb.bor.Visibility = Visibility.Collapsed;
                    }
                    dx += isa.cw;
                    imb.img.DataContext = index;
                    imb.title.Text = data[index].title;
                    ItemClick ic = new ItemClick();
                    ic.click = itemclick;
                    ic.tag = data[index].href;
                    imb.button.DataContext = ic;
                    index++;
                    if (index >= data.Count)
                        break;
                    s++;
                    if (s >= max)
                        s = 0;
                }
        }
        public List<ItemDataB> data { set; get; }
        public void Refresh()
        {
            int max = data.Count;
            int r = max % pos.column;
            int  row =max/ pos.column;
            if (r > 0)
                row++;
            double h = row * isa.ch;
            if(h<isa.svh)
                content.Height = isa.svh;
            else content.Height = h;
            if (max > pos.maxcount)
                max = pos.maxcount;
            int c = li.Count;
            int i;
            Item im;
            for (i=0;i< max;i++)
            {
                if (i >= c)
                    im = CreateNewItem();
                else im = li[i];
                ResizeItem(ref im);
            }
            for(i=c-1;i>= max;i--)
            {
                im = li[i];
                content.Children.Remove(im.img);
                content.Children.Remove(im.title);
                content.Children.Remove(im.detail);
                content.Children.Remove(im.bor);
                content.Children.Remove(im.mark);
                content.Children.Remove(im.button);
                Component.RecycleBorder(im.bor);
                Component.RecycleButton(im.button);
                Component.RecycleImage(im.img);
                Component.RecycleImage(im.mark);
                Component.RecycleTextBlock(im.title);
                Component.RecycleTextBlock(im.detail);
                li.RemoveAt(i);
            }
            //pos.index = -1;
            if (data.Count > 0)
                OrderItem();
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
            double w = m.Right - m.Left;
            double h = m.Bottom - m.Top;
            isa.svw= sv.Width = w;
            isa.svh= sv.Height = h;
            sv.Margin = m;
            content.Width = w;
            double cw = w / minX;
            int c = (int)cw;
            isa.cw = cw = w / c;
            isa.ox_i = cw * 0.05f;
            isa.iw = cw*0.9f;
            isa.ih = isa.iw*1.3f;
            isa.oy_t = isa.ih+20;
            isa.oy_d = isa.ih-36;
            double ch = isa.ih + 50;
            isa.ch = ch;

            int r = (int)(h / ch);
            r++;
            pos.row = r;
            pos.column = c;
            pos.maxcount =(r + 2)* c;
        }
        public void Dispose()
        {
            parent.Children.Remove(sv);
            content.Children.Clear();
            int c = li.Count;
            for(int i=0;i<c;i++)
            {
                Item im = li[i];
                Component.RecycleBorder(im.bor);
                Component.RecycleButton(im.button);
                Component.RecycleImage(im.img);
                Component.RecycleImage(im.mark);
                Component.RecycleTextBlock(im.title);
                Component.RecycleTextBlock(im.detail);
                li.RemoveAt(i);
            }
            li.Clear();
            sv.Content = null;
            GC.SuppressFinalize(content);
            GC.SuppressFinalize(sv);
            GC.SuppressFinalize(li);
            GC.SuppressFinalize(this);
        }
        public void ShowBorder()
        {
            sv.BorderBrush = Component.bor_brush;
            sv.BorderThickness = new Thickness(1, 1, 1, 1);
        }
    }

    class ScrollViewC:Component, IDisposable
    {
        struct Item
        {
            public Head head;
            public TextBlock title;
            public TextBlock content;
            public TextBlock time;
            public Button button;
        }
        struct Position
        {
            public int maxcount;
            public int index;
            public int ss;
            public double sio;
            public double start;
            public double offset;
        }
        struct Head
        {
            public Ellipse ell;
            public ImageBrush brush;
            public BitmapImage map;
        }
        struct Size
        {
            public double width;
            public double height;
        }
        ScrollViewer sv;
        Canvas content;
        Canvas parent;
        Thickness margin;
        double width;
        double height;
        bool min, max;
        const double rh = 20;
        const double os_max = 2000000;
        const double os_sta = 200000;
        Position pos;
        List<Item> li;
        List<Size> ls;
        Size si;
        public Action<object> itemclick;
        public bool LoadOver { get; set; }
        public Action Getmore;
        public ScrollViewC()
        {
            sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            sv.ViewChanged += (o, e) => { SlidingY(); };
            content = new Canvas();
            sv.Content = content;
            content.Height = os_max;
            pos.offset = os_max;
            li = new List<Item>();
            lci = new List<CommentInfo>();
            ls = new List<Size>();
        }
        void SlidingY()
        {
            double oy = pos.offset- sv.VerticalOffset;
            pos.offset = sv.VerticalOffset;
            if (oy != 0 &lci.Count>0)
            {
                if (lci.Count < pos.maxcount)
                {
                    content.Height = height;
                    pos.start = 0;
                    pos.index = 0;
                    pos.offset = 0;
                    Update();
                    return;
                }
                SlidingEx(oy);
                Update();
            }
        }
        void Update()
        {
            int index = pos.ss;
            int len = li.Count;
            int ss = index % len;
            double dy = pos.start;
            double ch;
            CalculSize();
            for (int c = 0; c < len; c++)
            {
                Item cc = li[ss];
                CommentInfo ci = lci[index];
                double ox = 0;
                if (ci.m_r_id!= "0")
                    ox += 40;
                cc.head.ell.Margin = new Thickness(ox, dy + 5, 0, 0);
                cc.title.Margin = new Thickness(ox + 54, dy, 0, 0);
                cc.time.Margin = new Thickness(ox + 54, dy + 24, 0, 0);
                cc.content.Margin = new Thickness(ox, dy + 54, 0, 0);
                cc.button.Margin = new Thickness(ox, dy, 0, 0);
                if (cc.head.map.UriSource==null|| cc.head.map.UriSource.OriginalString!=ci.url)
                {
                    cc.title.DataContext = index;
                    if(ci.url!=null & ci.url!="")
                    cc.head.map.UriSource = new Uri(ci.url);
                    string str = ci.nick + "  uid:" + ci.u_id;// + "  mid:" + buff_com[i].uidB;
                    cc.title.Text = str;
                    cc.time.Text = ci.time + ci.region;
                    cc.content.Text = ci.content+"\r\n"+ci.count;
                    ch = (int)(ci.content.Length * 16 / width + 1) * rh+24;
                    cc.content.Width = ls[index].width;
                    cc.content.Height = ch;
                    cc.button.Width = ls[index].width;
                    cc.button.Height = ls[index].height;
                }
                dy += ls[index].height;
                index++;
                if (index >= lci.Count)
                    break;
                ss++;
                if (ss >= len)
                    ss = 0;
            }
        }
        public List<CommentInfo> lci { get; set; }
        public void Refresh()
        {
            int max = pos.maxcount;
            int c = li.Count;
            int i;
            Item cc;
            for (i = 0; i < max; i++)
            {
                if (i >= c)
                    cc = CreateNewItem();
                else cc = li[i];
            }
            for (i = c - 1; i >= max; i--)
            {
                cc = li[i];
                RecycleItem(ref cc);
                li.RemoveAt(i);
            }
            for (i = ls.Count; i < lci.Count; i++)
                ls.Add(si);
            if (lci.Count > 0)
            {
                content.Visibility = Visibility.Visible;
                Timer.Delegate(() => { Update(); Timer.Stop(); }, 10);
            }
            else content.Visibility=Visibility.Collapsed;
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
            sv.Width =width;
            sv.Height = height;
            sv.Margin = m;
            content.Width = width;
            int count =(int)( height / 90);
            pos.maxcount = count + 3;
        }
        public void ReSet()
        {
            pos.index = 0;
            pos.start = 0;
            pos.offset = os_sta;
            pos.ss = 0;
            pos.sio = 0;
            lci.Clear();
            ls.Clear();
            LoadOver = false;
            content.Height = os_max;
        }
        public void Dispose()
        {
            parent.Children.Remove(sv);
            content.Children.Clear();
            int c = li.Count;
            for (int i = c - 1; i > 0; i--)
            {
                Item idb = li[i];
                RecycleTextBlock(idb.time);
                RecycleTextBlock(idb.title);
                RecycleTextBlock(idb.content);
                RecycleButton(idb.button);
                GC.SuppressFinalize(idb.head.ell);
                GC.SuppressFinalize(idb.head.brush);
                GC.SuppressFinalize(idb.head.map);
            }
            li.Clear();
            sv.Content = null;
            GC.SuppressFinalize(content);
            GC.SuppressFinalize(sv);
            GC.SuppressFinalize(li);
            GC.SuppressFinalize(this);
        }
        void SlidingEx(double oy)
        {
            if (min & oy >= 0)
            {
                pos.start = pos.offset;
                pos.sio = 0;
                return;
            } else min = false;
            if (max & oy < 0)
            {
                pos.start = pos.offset + pos.sio;
                return;
            } else max = false;
            double os = oy + pos.sio;
            int s=pos.index;
            double h;
            if (os>=0)
            {
                if (s > 0)
                {
                    s--;
                    pos.sio = os - ls[s].height;
                    pos.index = s;
                }
                else pos.sio = os;
            }
            else
            {
                double o =os+ ls[pos.index].height;
                if (o < 0)
                {
                    s++;
                    if (s + li.Count <= lci.Count)
                    {
                        pos.sio = o;
                        pos.index = s;
                        if(!LoadOver)
                        if (s + li.Count >= lci.Count - 10)
                            if (Getmore != null)
                                Getmore();
                    }
                    else
                    {
                        pos.sio = os;
                        return;
                    }
                }
                else { pos.sio = os; return; }
            }
            if (pos.index > 0)
            {
                pos.ss = pos.index - 1;
                CalculSize(pos.ss);
                h= ls[pos.ss].height;
                if (pos.index + li.Count >= lci.Count)
                {
                    double ah = CalculSize();
                    if (ah + pos.start-oy > pos.offset + height)
                    {
                        pos.sio = height - ah;
                        pos.start = pos.offset +pos.sio;
                        max = true;
                        return;
                    }
                }
                pos.start = pos.offset + pos.sio - h;
            }
            else {
                pos.ss = 0;
                if (pos.sio >= 0)
                { pos.sio = 0; min = true; }
                pos.start = pos.offset + pos.sio;
            }
        }
        double CalculSize()
        {
            int s = pos.ss;
            int max = li.Count;
            int dmax = lci.Count;
            double sw = width - 40;
            double h = 0;
            for(int i=0;i<max;i++)
            {
                if (s >= dmax)
                    break;
                if (s >= ls.Count)
                {
                    double oy;
                    int c = lci[s].content.Length;
                    if (lci[s].m_r_id != "0")
                    { si.width = sw; si.height = (c * 16 / sw + 1) * rh + 104; }
                    else { si.width = width; oy = (c * 16 / width + 1) * rh + 104; }
                    ls.Add(si);
                    h += si.height;
                }
                else
                { CalculSize(s);h += ls[s].height; }
                s++;
            }
            return h;
        }
        void CalculSize(int s)
        {
            if (lci[s].m_r_id != "0")
            {
                double sw = width - 40;
                if (ls[s].width != sw)
                {
                    si.width = sw;
                    si.height = (lci[s].content.Length * 16 / sw + 1) * rh + 84;
                    ls[s] = si;
                }
            }
            else
            {
                if (ls[s].width != width)
                {
                    si.width = width;
                    si.height = (lci[s].content.Length * 16 / width + 1) * rh + 84;
                    ls[s] = si;
                }
            }
        }
        Head CreateNewHead()
        {
            Head h = new Head();
            h.ell = new Ellipse();
            h.ell.Width = 36;
            h.ell.Height = 36;
            h.brush = new ImageBrush();
            h.map = new BitmapImage();
            h.ell.Fill = h.brush;
            h.brush.ImageSource = h.map;
            content.Children.Add(h.ell);
            return h;
        }
        Item CreateNewItem()
        {
            Item i = new Item();
            i.head = CreateNewHead();
            i.time = CreateTextBlockNext();
            i.time.Foreground = font_brush;
            i.title = CreateTextBlockNext();
            i.title.Foreground = title_brush;
            i.content = CreateTextBlockNext();
            i.content.Foreground = font_brush;
            i.content.TextWrapping = TextWrapping.Wrap;
            i.button = CreateButtonNext();
            i.button.Background = trans_brush;
            i.button.BorderBrush = filter_brush;
            i.button.BorderThickness = new Thickness(0,1,0,0);
            content.Children.Add(i.time);
            content.Children.Add(i.title);
            content.Children.Add(i.content);
            content.Children.Add(i.button);
            li.Add(i);
            return i;
        }
        void RecycleItem(ref Item i)
        {
            content.Children.Remove(i.time);
            content.Children.Remove(i.title);
            content.Children.Remove(i.content);
            content.Children.Remove(i.button);
            content.Children.Remove(i.head.ell);
            RecycleTextBlock(i.time);
            RecycleTextBlock(i.title);
            RecycleTextBlock(i.content);
            RecycleButton(i.button);
            GC.SuppressFinalize(i.head.ell);
            GC.SuppressFinalize(i.head.brush);
            GC.SuppressFinalize(i.head.map);
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
    }

    class ScrollViewD:Component, IDisposable
    {
        struct Item
        {
            public Image img;
            public BitmapImage map;
            public TextBlock title;
            public Button button;
        }
        struct Position
        {
            public int column;
            public int row;
            public int maxcount;
            public int index;
        }
        struct ItemSizeA
        {
            public double svw;
            public double cw;
            public double ch;
            public double iw;
            public double ih;
            public double ox_i;
            public double oy_t;
        }
        ScrollViewer sv;
        Canvas content;
        Canvas parent;
        Thickness margin;
        ItemSizeA isa;
        Position pos;
        List<Item> li;
        public Action<object> itemclick;
        public ScrollViewD()
        {
            sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            content = new Canvas();
            sv.Content = content;
            sv.ViewChanged += Sliding;
            li = new List<Item>();
            data = new List<ItemDataA>();
        }
        void Sliding(object o, ScrollViewerViewChangedEventArgs e)
        {
            double os = sv.VerticalOffset;
            int row = (int)((os - isa.ch * 0.3f) / isa.ch);
            int index = row * pos.column;
            if (index != pos.index)
            {
                pos.index = index;
                double dy = row * isa.ch;
                double dx = 0;
                Item imb;
                int max = li.Count;

                if (index + max <= data.Count + pos.column)
                {
                    int s = index % max;
                    for (int i = 0; i < max; i++)
                    {
                        if (dx + isa.cw > isa.svw)
                        {
                            dx = 0;
                            dy += isa.ch;
                        }
                        imb = li[s];
                        imb.img.Margin = new Thickness(dx + isa.ox_i, dy, 0, 0);
                        imb.title.Margin = new Thickness(dx + 5, isa.oy_t + dy, 0, 0);
                        //imb.detail.Margin = new Thickness(dx + 5, isa.oy_d + dy, 0, 0);
                        imb.button.Margin = new Thickness(dx, dy, 0, 0);
                        int di = (int)imb.img.DataContext;
                        if (di != index)
                        {
                            imb.img.DataContext = index;
                            ItemClick ic = new ItemClick();
                            ic.click = itemclick;
                            ic.tag = data[index].href;
                            imb.button.DataContext = ic;
                            (imb.img.Source as BitmapImage).UriSource = new Uri(data[index].src);
                            imb.title.Text = data[index].title;
                            //imb.detail.Text = data[index].detail;
                        }
                        dx += isa.cw;
                        index++;
                        if (index >= data.Count)
                            break;
                        s++;
                        if (s >= max)
                            s = 0;
                    }
                }
            }
        }
        void ResizeItem(ref Item im)
        {
            im.img.Width = isa.iw;
            im.img.Height = isa.ih;
            im.title.Width = isa.iw;
            im.button.Width = isa.cw;
            im.button.Height = isa.ch;
        }
        void OrderItem()
        {
            if (li.Count == 0)
                return;
            double os = sv.VerticalOffset;
            int row = (int)((os - isa.ch * 0.3f) / isa.ch);
            int index = row * pos.column;
            pos.index = index;
            double dy = row * isa.ch;
            double dx = 0;
            Item imb;
            int max = li.Count;
            int s = index % max;
            if (index + max <= data.Count + pos.column)
                for (int i = 0; i < max; i++)
                {
                    imb = li[s];
                    if (dx + isa.cw > isa.svw)
                    {
                        dx = 0;
                        dy += isa.ch;
                    }
                    imb.img.Margin = new Thickness(dx + isa.ox_i, dy, 0, 0);
                    imb.title.Margin = new Thickness(dx + 5, isa.oy_t + dy, 0, 0);
                    //imb.detail.Margin = new Thickness(dx + 5, isa.oy_d + dy, 0, 0);
                    imb.button.Margin = new Thickness(dx, dy, 0, 0);
                    dx += isa.cw;
                    imb.img.DataContext = index;
                    (imb.img.Source as BitmapImage).UriSource = new Uri(data[index].src);
                    imb.title.Text = data[index].title;
                    //imb.detail.Text = data[index].detail;
                    ItemClick ic = new ItemClick();
                    ic.click = itemclick;
                    ic.tag = data[index].href;
                    imb.button.DataContext = ic;
                    index++;
                    if (index >= data.Count)
                        break;
                    s++;
                    if (s >= max)
                        s = 0;
                }
        }
        public List<ItemDataA> data { set; get; }
        public void Refresh()
        {
            int max = data.Count;
            if (pos.column < 1)
                pos.column = 1;
            int r = max % pos.column;
            int row = max / pos.column;
            if (r > 0)
                row++;
            double h = row * isa.ch;
            content.Height = h;
            pos.index = 0;
            if (max > pos.maxcount)
                max = pos.maxcount;
            int c = li.Count;
            int i;
            Item imb;
            for (i = 0; i < max; i++)
            {
                if (i >= c)
                    imb = CreateNewItem();
                else imb = li[i];
                ResizeItem(ref imb);
            }
            for (i = c - 1; i >= max; i--)
            {
                imb = li[i];
                content.Children.Remove(imb.img);
                content.Children.Remove(imb.title);
                content.Children.Remove(imb.button);
                RecycleImage(imb.img);
                RecycleTextBlock(imb.title);
                RecycleButton(imb.button);
                li.RemoveAt(i);
            }
            if (data.Count > 0)
                OrderItem();
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
            double w = m.Right - m.Left;
            double h = m.Bottom - m.Top;
            sv.Width = w;
            sv.Height = h;
            sv.Margin = m;
            content.Width = w;
            isa.svw = w;
            double cw = w / minX;
            int c = (int)cw;
            isa.cw = cw = w / c;
            isa.ox_i = cw * 0.05f;
            isa.ih = isa.iw = cw * 0.9f;
            isa.oy_t = isa.ih;
            //isa.oy_d = isa.ih;
            double ch = isa.ch = cw * 1.1f;

            int r = (int)(h / ch);
            r++;
            pos.row = r;
            pos.column = c;
            pos.maxcount = (r + 2) * c;
        }
        public void Dispose()
        {
            parent.Children.Remove(sv);
            content.Children.Clear();
            int c = li.Count;
            for (int i = c - 1; i > 0; i--)
            {
                Item imb = li[i];
                RecycleImage(imb.img);
                RecycleTextBlock(imb.title);
                RecycleButton(imb.button);
            }
            sv.Content = null;
            GC.SuppressFinalize(content);
            GC.SuppressFinalize(sv);
            GC.SuppressFinalize(li);
            GC.SuppressFinalize(this);
        }    
        Item CreateNewItem()
        {
            Item i = new Item();
            i.img = CreateImageNext();
            i.map = i.img.Source as BitmapImage;
            i.title = CreateTextBlockNext();
            i.title.Foreground = title_brush;
            i.button = CreateButtonNext();
            i.button.BorderBrush = filter_brush;
            i.button.BorderThickness = new Thickness(1,1,1,1);
            i.button.Background = trans_brush;
            content.Children.Add(i.img);
            content.Children.Add(i.title);
            content.Children.Add(i.button);
            li.Add(i);
            return i;
        }
    }

    class DownView:Component
    {
        const int ih = 80;
        static string[] time = {"时长:","Time:" };
        static string[] part = { "分段:","Part:"};
        struct Item
        {
            public Border bor;
            public TextBlock detail;
            public AppBarButton play;
            public AppBarButton delete;
            //public AppBarButton down;
        }
        ScrollViewer sv;
        Canvas content;
        Canvas parent;
        Thickness margin;
        int maxcount;
        double width, height;
        public Action<int> Play;
        public Action<int> Delete;
        public DownView()
        {
            sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            sv.ViewChanged += Sliding;
            sv.Background = bk_brush;
            content = new Canvas();
            sv.Content = content;
            li = new List<Item>();
        }
        public void SetParent(Canvas p)
        {
            p.Children.Add(sv);
            parent = p;
        }
        public List<Mission> data { set; get; }
        List<Item> li;
        Item CreateNewItem()
        {
            Item it = new Item();
            it.bor = CreateBorderNext();
            it.bor.BorderBrush = filter_brush;
            it.bor.BorderThickness = new Thickness(0,0,0,1);
            it.bor.Height = ih;

            it.detail = CreateTextBlockNext();
            it.detail.Foreground = font_brush;
            it.detail.TextWrapping = TextWrapping.Wrap;
            it.play = new AppBarButton();
            it.play.Width = 48;
            it.play.Height = 48;
            it.play.Foreground = font_brush;
            it.play.Click += (o, e) =>
            {
                if (Play != null)
                    Play((int)(o as AppBarButton).DataContext);
            };
            it.play.Icon = new SymbolIcon(Symbol.Play);
            it.delete = new AppBarButton();
            it.delete.Width = 48;
            it.delete.Height = 48;
            it.delete.Foreground = font_brush;
            it.delete.Click += (o, e) =>
            {
                if (Delete != null)
                    Delete((int)(o as AppBarButton).DataContext);
            };
            it.delete.Icon = new SymbolIcon(Symbol.Delete);
            content.Children.Add(it.bor);
            content.Children.Add(it.detail);
            content.Children.Add(it.play);
            content.Children.Add(it.delete);
            return it;
        }
        void Sliding(object o, ScrollViewerViewChangedEventArgs e)
        {
            OrderItem();
        }
        public void Refresh()
        {
            if (data == null)
                return;
            int max = data.Count;
            double h= ih * max;
            if (h < height)
                h = height;
            content.Height = h;
            if (max < maxcount)
                maxcount = max;
            int c = li.Count;
            Item it;
            for(int i=0;i<maxcount;i++)
            {
                if(i>=c)
                {
                    it = CreateNewItem();
                    li.Add(it);
                }
                it = li[i];
                it.detail.Width = width;
                it.bor.Width = width;
            }
            for(int i=c-1;i>=maxcount;i--)
            {
                it = li[i];
                li.RemoveAt(i);
                content.Children.Remove(it.detail);
                content.Children.Remove(it.play);
                content.Children.Remove(it.delete);
                content.Children.Remove(it.bor);
                RecycleTextBlock(it.detail);
                RecycleBorder(it.bor);
                GC.SuppressFinalize(it.play);
                GC.SuppressFinalize(it.delete);
            }
            if (data.Count == 0)
                return;
            OrderItem();
        }
        void OrderItem()
        {
            double os = sv.VerticalOffset;
            int index = (int)((os - ih * 0.3f) / ih);
           
            double dy = index * ih;
            Item it;
            int max = li.Count;
            int s = index % max;
            double ox = width - 100;
            double ox2 = width - 50;
            if (index + max <= data.Count)
                for (int i = 0; i < max; i++)
                {
                    it = li[s];
                    it.bor.Margin = new Thickness(0,dy,0,0);
                    it.detail.Margin = new Thickness(0, dy, 0, 0);
                    it.play.Margin = new Thickness(ox2, dy, 0, 0);
                    it.delete.Margin = new Thickness(ox, dy, 0, 0);
                    dy += ih;
                    string str = data[index].name+"\r\n"+time[language]+data[index].time.ToString()+"s "+
                        part[language]+data[index].done.ToString()+"/"+ data[index].max.ToString();
                    str += "\r\n"+GetError(data[index]);
                    it.detail.Text = str;
                    it.play.DataContext = index;
                    it.delete.DataContext = index;
                    index++;
                    if (index >= data.Count)
                        break;
                    s++;
                    if (s >= max)
                        s = 0;
                }
        }
        string GetError(Mission m)
        {
            int c = m.done;
            int len = m.max;
            byte[] b = m.progress;
            int o = 0;
            string str = "";
            for(int i=0;i<len;i++)
            {
                if (o >= c)
                    break;
                if (b[i] == 0)
                    str += " " + i.ToString();
                else o++;
            }
            if (str != "")
                str = "第" + str + "分段下载错误";
            return str;
        }
        public void Resize(Thickness m)
        {
            if (margin == m)
                return;
            margin = m;
            width= m.Right - m.Left;
            height = m.Bottom - m.Top;
            sv.Width = width;
            sv.Height = height;
            sv.Margin = m;
            content.Width = width;
            maxcount =(int) height / 50 + 20;
        }
        public void Dispose()
        {
            int c = li.Count;
            Item it;
            content.Children.Clear();
            for (int i = c - 1; i >= 0; i--)
            {
                it = li[i];
                li.RemoveAt(i);
                RecycleBorder(it.bor);
                RecycleTextBlock(it.detail);
                GC.SuppressFinalize(it.play);
                GC.SuppressFinalize(it.delete);
            }
            sv.Content = null;
            parent.Children.Remove(sv);
            GC.SuppressFinalize(sv);
            GC.SuppressFinalize(content);
            GC.SuppressFinalize(this);
        }  
        public void ShowBorder()
        {
            sv.BorderBrush = bor_brush;
            sv.BorderThickness = new Thickness(1, 1, 1, 1);
        }
    }

    class Scroll_ex :Component, IDisposable
    {
        static string[] noname = { "无标题", "No name" };
        static string[] next = { "换一组", "Change" };
        static string[] nochange = { "不选择", "No Change" };
        struct Item
        {
            public Image img;
            public BitmapImage map;
            public Button button;
            public TextBlock title;
            public TextBlock detail;
            public TextBlock[] tag;
            public Border[] tag_bk;
        }
        struct Position
        {
            public int ac;// areacolumn
            public int column;
            public int row;
            public int maxcount;
            public int index;
            public int areaindex;
            public int datacount;
        }
        struct ItemSizeA
        {
            public double svw;
            public double svh;
            public double aw;//area width
            public double cw;
            public double ch;
            public double iw;//image width
            public double ih;//image height
            public double ox_i;
            public double oy_s;
            public double oy_t;
            public double oy_d;
        }
        ScrollViewer sv;
        Canvas content;
        Canvas parent;
        Thickness margin;
        public Area_m[] area;
        ItemSizeA isa;
        Position pos;
        Ellipse ell;
        ListBox lb;
        List<Item> li;
        List<Button> ar_title;
        List<Button> change;
        List<Border> ar_bor;
        public int maxcolumn;
        public bool UsingCustomTitle;
        public bool ForceUpdateOnce;
        public Action<object> PageClick;
        public Action<object> ItemClick;
#if phone
        public Action Lock;
        public Action UnLock;
#endif

        long time;
        Point eo;
        public Scroll_ex()
        {
            sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            sv.ViewChanged += (o, e) => { UpdateArea(sv.VerticalOffset); };
            content = new Canvas();
            sv.Content = content;
            li = new List<Item>();
            ar_title = new List<Button>();
            ar_bor = new List<Border>();
            change = new List<Button>();
            ell = new Ellipse();
            ell.Width = 48;
            ell.Height = 48;
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("ms-appx:///Pic/am64.png"));
            ell.Fill = ib;
            ell.Margin = new Thickness(10,40,0,0);
            ell.PointerPressed += (o, e) => { time = DateTime.Now.Ticks;
                PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                eo = pp.Position;
#if phone
                if (Lock != null)
                    Lock();
#endif
            };
            ell.PointerMoved += (o, e) => {
                PointerPoint pp = PointerPoint.GetCurrentPoint(e.Pointer.PointerId);
                if(pp.IsInContact)
                {
                    Point p = pp.Position;
                    double ox = p.X - eo.X;
                    double oy = p.Y - eo.Y;
                    eo = p;
                    Thickness tk = ell.Margin;
                    tk.Left += ox;
                    if (tk.Left < 0 || tk.Left > isa.svw - 48)
                        return;
                    tk.Top += oy;
                    if (tk.Top < 0 || tk.Top > sv.Height - 48)
                        return;
                    ell.Margin = tk;
                }
            };
            ell.PointerReleased += (o, e) => {
                if (DateTime.Now.Ticks - time < Component.presstime)
                    Jump();
                };
#if phone
            ell.PointerExited += (o,e)=> {if (UnLock != null)
                    UnLock();
            };
#endif
        }
        public void SetParent(Canvas p)
        {
            parent = p;
            p.Children.Add(sv);
            p.Children.Add(ell);
        }
        Item CreateNewItem()
        {
            Item i = new Item();
            i.img = Component.CreateImageNext();
            i.map = i.img.Source as BitmapImage;
            i.title = Component.CreateTextBlockNext();
            i.title.Foreground = Component.title_brush;
            i.detail = Component.CreateTextBlockNext();
            i.detail.Foreground = Component.font_brush;

            i.button = Component.CreateButtonNext();
            i.button.Foreground = Component.trans_brush;
            content.Children.Add(i.img);
            content.Children.Add(i.title);
            content.Children.Add(i.detail);
            i.tag = new TextBlock[3];
            i.tag_bk = new Border[3];
            for (int c = 0; c < 3; c++)
            {
                Border bor = Component.CreateBorderNext();
                bor.Height = 20;
                content.Children.Add(bor);
                i.tag_bk[c] = bor;
                TextBlock tb = Component.CreateTextBlockNext();
                tb.Foreground = Component.font_brush;
                content.Children.Add(tb);
                i.tag[c] = tb;
            }
            i.tag_bk[0].Background = Component.tag_brush_g;
            i.tag_bk[1].Background = Component.tag_brush_y;
            i.tag_bk[2].Background = Component.tag_brush_b;
            content.Children.Add(i.button);
            li.Add(i);
            return i;
        }
        void ResizeItem(ref Item i)
        {
            i.img.Width = isa.iw;
            i.img.Height = isa.ih;
            i.title.Width = isa.iw;
            i.detail.Width = isa.iw;
            i.button.Width = isa.iw;
            i.button.Height = isa.ch - 8;
        }
        public void Refresh()
        {
            pos.index = -1;
            pos.areaindex = 0;
            int max = pos.datacount;
            if (max > pos.maxcount)
                max = pos.maxcount;
            int c = li.Count;
            Item imb;
            for (int i = 0; i < max; i++)
            {
                if (i >= c)
                {
                    imb = CreateNewItem();
                }
                imb = li[i];
                ResizeItem(ref imb);
            }
            for (int i = c - 1; i >= max; i--)
            {
                imb = li[i];
                li.RemoveAt(i);
                content.Children.Remove(imb.img);
                content.Children.Remove(imb.title);
                content.Children.Remove(imb.detail);
                content.Children.Remove(imb.button);
                for (int d = 0; d < 3; d++)
                {
                    content.Children.Remove(imb.tag[d]);
                    content.Children.Remove(imb.tag_bk[d]);
                    RecycleTextBlock(imb.tag[d]);
                    RecycleBorder(imb.tag_bk[d]);
                }
                RecycleImage(imb.img);
                RecycleTextBlock(imb.title);
                RecycleTextBlock(imb.detail);
                RecycleButton(imb.button);
            }
            UpdateArea(sv.VerticalOffset);
        }
        public void ReSize(Thickness m)
        {
            if (margin == m)
                return;
            margin = m;
            double w = m.Right - m.Left;
            double h = m.Bottom - m.Top;
            sv.Width = w;
            sv.Height = h;
            sv.Margin = m;
            content.Width = w;
            isa.svw = w;
            double cw = w / minX;
            int c = (int)cw;
            isa.cw = cw = w / c;
            isa.ox_i = cw * 0.05f;
            isa.oy_d = isa.ih = isa.iw = cw * 0.9f;
            isa.oy_t = isa.ih - 20;
            isa.oy_s = isa.ih - 40;
            double ch = isa.ch = cw * 1.1f;

            int r = (int)(h / ch);
            r++;
            pos.row = r;
            pos.column = c;
            pos.maxcount = (r + 2) * c;
            if (maxcolumn > 0 & c > maxcolumn)
                isa.aw = cw * maxcolumn;
            else isa.aw = w;
            CaculAreaHeight();
        }
        void CaculAreaHeight()
        {
            int len = area.Length;
            double all_h = 0;
            int co = pos.column;
            pos.ac = 0;
            if (maxcolumn != 0)
                pos.ac = co / maxcolumn - 1;
            if (pos.ac < 0)
                pos.ac = 0;
            if (pos.ac == 0)
            {
                ell.Visibility = Visibility.Visible;
                Thickness tk = ell.Margin;
                if (tk.Left > sv.Width-48)
                    tk.Left = sv.Width - 48;
                if (tk.Top > sv.Height-48)
                    tk.Top = sv.Height - 48;
                ell.Margin = tk;
            }
            else ell.Visibility = Visibility.Collapsed;
            int max = 0;
            double dx = 0;
            for (int i = 0; i < len; i++)
            {
                area[i].start = max;
                int c = area[i].count;
                if (c > maxcolumn)
                    c = maxcolumn;
                max += c;
                int t = c / co;
                if (c % co > 0)
                    t++;
                double h = t * isa.ch + 30;
                area[i].height = h;
                area[i].dx = 0;
                area[i].dy = all_h;
                for (int o = 0; o < pos.ac; o++)
                {
                    i++;
                    if (i >= len)
                        goto label0;
                    area[i].start = max;
                    c = area[i].count;
                    if (c > maxcolumn)
                        c = maxcolumn;
                    max += c;
                    dx += isa.aw;
                    area[i].dx = dx;
                    area[i].height = h;
                    area[i].dy = all_h;
                }
                all_h += h;
                dx = 0;
            }
            label0:;
            pos.datacount = max;
            if (all_h < isa.svh)
                content.Height = isa.svh;
            else content.Height = all_h;
        }
        void UpdateArea(double os)
        {
            bool up = false;
            int s = 0;
            int len = area.Length;
            for (int i = 0; i < len; i++)
            {
                if (os >= area[i].dy)
                    s = i;
                else break;
            }
            s -= pos.ac;
            if (pos.areaindex != s)
            { pos.areaindex = s; up = true; }
            double aos = os - area[s].dy;
            int oi = (int)((aos - isa.ch * 0.3f - 30) / isa.ch);
            oi *= pos.column;
            if (ForceUpdateOnce)
                goto label0;
            if (!up)
            {
                if (oi == pos.index)
                    return;
            }
            label0:;
            pos.index = oi;
            int index = oi / pos.column;
            int c = oi + area[s].start;
            int ss = c % li.Count;
            int max = pos.maxcount;
            Area_m aex = area[s];
            int ac = aex.count;
            int si = area[s].start + oi;
            si = pos.datacount - si;
            if (ForceUpdateOnce|| si >= max - pos.column)
            {
                int lc = li.Count;
                int i = 0;
                int e = ss - 1;
                if (e < 0)
                    e = lc - 1;
                int a = 1;
                int d = s;
                while (i < lc)
                {
                    ss = UpdateArea(oi, ss, s, e);
                    if (ss < 0)
                        break;
                    oi = 0;
                    if (area[s].count < maxcolumn)
                        i += area[s].count;
                    else i += maxcolumn;
                    s++;
                    a++;
                    if (s >= area.Length)
                        break;
                }
                UpdateAreaTitle(d, a, 0);
            }
            ForceUpdateOnce = false;
        }
        int UpdateArea(int oi, int itemindex, int areaindex, int end)
        {
            area[areaindex].com_index = itemindex;
            ItemData_m[] data = area[areaindex].data;
            int len = area[areaindex].count;
            double dx;
            double dy = area[areaindex].dy + 30;
            int start = area[areaindex].start;
            double cx = 2;
            for (int i = 0; i < oi; i++)
            {
                cx += isa.cw;
                if (cx + isa.iw > isa.aw)
                {
                    cx = 2;
                    dy += isa.ch;
                }
            }
            int o = area[areaindex].showindex;
            if (len > o + maxcolumn)
                len = o + maxcolumn;
            o += oi;
            for (int i = o; i < len; i++)
            {
                Item ima = li[itemindex];
                dx = cx + area[areaindex].dx;
                if (ima.map.UriSource==null || ima.map.UriSource.OriginalString != data[i].src)
                {
                    ima.img.Margin = new Thickness(dx, dy - 8, 0, 0);
                    ima.title.Margin = new Thickness(dx, dy + isa.oy_t, 0, 0);
                    ima.detail.Margin = new Thickness(dx, dy + isa.oy_d, 0, 0);
                    ima.button.Margin = new Thickness(dx, dy, 0, 0);
                    ima.button.DataContext = data[i].href;
                    if (data[i].src != null)
                        ima.map.UriSource = new Uri(data[i].src);//gif no effect
                    if (data[i].title != null)
                        ima.title.Text = data[i].title;
                    else ima.title.Text = "";
                    if (data[i].detail != null)
                        ima.detail.Text = data[i].detail;
                    else ima.detail.Text = "";
                    ItemClick ic = new ItemClick();
                    ic.click = ItemClick;
                    ic.tag = data[i].href;
                    ima.button.DataContext = ic;
                    string[] str = data[i].tag;
                    if (str != null)
                        for (int t = 0; t < 3; t++)
                        {
                            TextBlock tb = ima.tag[t];
                            Border bor = ima.tag_bk[t];
                            if (str[t] != null)
                            {
                                bor.Visibility = Visibility.Visible;
                                tb.Visibility = Visibility.Visible;
                                tb.Text = str[t];
                                Thickness tk;
                                switch (t)
                                {
                                    case 0:
                                        int px = str[t].Length * 15;
                                        tb.Width = px;
                                        bor.Width = px;
                                        tk = new Thickness(dx, dy, 0, 0);
                                        tb.Margin = tk;
                                        bor.Margin = tk;
                                        break;
                                    case 1:
                                        px = str[t].Length * 15;
                                        tb.Width = px;
                                        bor.Width = px;
                                        double tx = dx + isa.iw - px;
                                        tk = new Thickness(tx, dy, 0, 0);
                                        tb.Margin = tk;
                                        bor.Margin = tk;
                                        break;
                                    case 2:
                                        //tb.Foreground = Data.font_brush;
                                        tb.Width = isa.iw;
                                        bor.Width = isa.iw;
                                        bor.Margin = tb.Margin = new Thickness(dx, dy + isa.oy_s, 0, 0);
                                        break;
                                }
                            }
                            else
                            {
                                tb.Visibility = Visibility.Collapsed;
                                bor.Visibility = Visibility.Collapsed;
                            }
                        }
                }
                cx += isa.cw;
                if (cx + isa.iw > isa.aw)
                {
                    cx = 2;
                    dy += isa.ch;
                }
                if (itemindex == end)
                { itemindex = -1; break; }
                itemindex++;
                if (itemindex >= li.Count)
                    itemindex = 0;
                oi++;
            }
            return itemindex;
        }
        void UpdateAreaTitle(int areaindex, int maxcount, int ss)
        {
            Button b;
            Border bo;
            int max = ar_title.Count;
            for (int i = max; i < maxcount; i++)
            {
                bo = Component.CreateBorderNext();
                content.Children.Insert(0, bo);
                bo.BorderBrush = Component.bor_brush;
                bo.BorderThickness = new Thickness(2, 2, 2, 2);
                bo.Width = isa.svw - 4;
                ar_bor.Add(bo);
                b = Component.CreateButtonNext();
                b.Foreground = Component.nav_brush;
                b.Background = Component.trans_brush;
                b.HorizontalContentAlignment = HorizontalAlignment.Left;
                ar_title.Add(b);
                content.Children.Add(b);
            }
            int s = areaindex;
            for (int i = 0; i < maxcount; i++)
            {
                b = ar_title[ss];
                if (UsingCustomTitle)
                {
                    b.Content = area[s].title[Component.language];
                    b.Width = area[s].title[Component.language].Length * 15+20;
                    ItemClick ic = new ItemClick();
                    ic.click = PageClick;
                    ic.tag = area[s].page;
                    b.DataContext = ic;
                }
                else
                {
                    if (area[s].alt != null)
                    { b.Content = area[s].alt; }
                    else { b.Content = noname[Component.language]; b.Width = noname[Component.language].Length * 15+20; }
                    b.DataContext = null;
                }
                b.Margin = new Thickness(area[s].dx, area[s].dy, 0, 0);
                bo = ar_bor[ss];
                bo.Margin = new Thickness(area[s].dx, area[s].dy, 0, 0);
                bo.Width = isa.aw - 4;
                bo.Height = area[s].height - 5;
                s++;
                if (s >= area.Length)
                    break;
                ss++;
                if (ss >= maxcount)
                    ss = 0;
            }
            for (int i = max - 1; i >= maxcount; i--)
            {
                b = ar_title[i];
                content.Children.Remove(b);
                Component.RecycleButton(b);
                ar_title.RemoveAt(i);
                bo = ar_bor[i];
                content.Children.Remove(bo);
                Component.RecycleBorder(bo);
                ar_bor.RemoveAt(i);
            }
            s = areaindex;
            int t = 0;
            for (int i = s; i < maxcount; i++)
            {
                int c = area[i].count;
                if (c / maxcolumn > 1)
                {
                    if (t >= change.Count)
                    {
                        b = Component.CreateButtonNext();
                        b.Foreground = Component.nav_brush;
                        b.Background = Component.trans_brush;
                        b.Content = next[Component.language];
                        b.HorizontalAlignment = HorizontalAlignment.Left;
                        change.Add(b);
                        content.Children.Add(b);
                        b.Width = 70;
                    }
                    else b = change[t];
                    b.Margin = new Thickness(area[i].dx + isa.aw - 70, area[i].dy, 0, 0);
                    ItemClick ic = new ItemClick();
                    ic.click = ChangeItem;
                    ic.tag = i;
                    b.DataContext = ic;
                    t++;
                }
            }
            max = change.Count - 1;
            for (int i = max; i > t; i--)
            {
                b = change[i];
                content.Children.Remove(b);
                Component.RecycleButton(b);
                change.RemoveAt(i);
            }
        }
        void ChangeItem(object index)
        {
            int s = (int)index;
            int ss = area[s].showindex + maxcolumn;
            if (ss + maxcolumn > area[s].count)
                ss = 0;
            area[s].showindex = ss;
            int oi = area[s].com_index;
            ItemData_m[] data = area[s].data;
            double dx;
            double dy = area[s].dy + 30;
            double cx = 2;
            for (int i = 0; i < 5; i++)
            {
                dx = area[s].dx + cx;
                Item ima = li[oi];
                ima.button.DataContext = data[ss].href;
                if (data[ss].src != null)
                    (ima.img.Source as BitmapImage).UriSource = new Uri(data[ss].src);//gif no effect
                if (data[ss].title != null)
                    ima.title.Text = data[ss].title;
                if (data[ss].detail != null)
                    ima.detail.Text = data[ss].detail;
                ItemClick ic = new ItemClick();
                ic.click = ItemClick;
                ic.tag = data[ss].href;
                ima.button.DataContext = ic;
                string[] str = data[ss].tag;
                if (str != null)
                    for (int t = 0; t < 3; t++)
                    {
                        TextBlock tb = ima.tag[t];
                        Border bor = ima.tag_bk[t];
                        if (str[t] != null)
                        {
                            bor.Visibility = Visibility.Visible;
                            tb.Visibility = Visibility.Visible;
                            tb.Text = str[t];
                            Thickness tk;
                            switch (t)
                            {
                                case 0:
                                    int px = str[t].Length * 15;
                                    tb.Width = px;
                                    bor.Width = px;
                                    tk = new Thickness(dx, dy, 0, 0);
                                    tb.Margin = tk;
                                    bor.Margin = tk;
                                    break;
                                case 1:
                                    px = str[t].Length * 15;
                                    tb.Width = px;
                                    bor.Width = px;
                                    double tx = dx + isa.iw - px;
                                    tk = new Thickness(tx, dy, 0, 0);
                                    tb.Margin = tk;
                                    bor.Margin = tk;
                                    break;
                                case 2:
                                    //tb.Foreground = Data.font_brush;
                                    tb.Width = isa.iw;
                                    bor.Width = isa.iw;
                                    tb.Margin = new Thickness(dx, dy + isa.oy_s, 0, 0);
                                    break;
                            }
                        }
                        else
                        {
                            tb.Visibility = Visibility.Collapsed;
                            bor.Visibility = Visibility.Collapsed;
                        }
                    }
                cx += isa.cw;
                if (cx + isa.iw > isa.aw)
                {
                    cx = 2;
                    dy += isa.ch;
                }
                oi++;
                ss++;
            }
        }
        void Jump()
        {
            lb = new ListBox();
            lb.Items.Add(nochange[Component.language]);
            lb.SelectionChanged += (o, e) => { Jump(lb.SelectedIndex); };
            parent.Children.Add(lb);
            lb.Width = sv.Width;
            lb.Height = sv.Height;
            lb.Background = Component.tag_brush_b;
            lb.Foreground = Component.nav_brush;
            int c = area.Length;
            for (int i = 0; i < c; i++)
            {
                if (area[i].alt != null)
                    lb.Items.Add(area[i].alt);
                else lb.Items.Add(noname[Component.language]);
            }
            ell.Visibility = Visibility.Collapsed;
        }
        void Jump(int index)
        {
            parent.Children.Remove(lb);
            GC.SuppressFinalize(lb);
            ell.Visibility = Visibility.Visible;
            lb = null;
            if (index == 0)
                return;
            index--;
            ForceUpdateOnce = true;
            sv.ChangeView(0, area[index].dy, 1);
            //sv.ScrollToVerticalOffset(area[index].dy);
        }
        public void Dispose()
        {
            sv.Content = null;
            content.Children.Clear();
            parent.Children.Remove(sv);
            parent.Children.Remove(ell);
            GC.SuppressFinalize(content);
            GC.SuppressFinalize(sv);
            GC.SuppressFinalize(ell);
            int c = li.Count;
            for (int i = 0; i < c; i++)
            {
                Item im = li[i];
                Component.RecycleButton(im.button);
                Component.RecycleImage(im.img);
                Component.RecycleTextBlock(im.title);
                Component.RecycleTextBlock(im.detail);
                for (int t = 0; t < 3; t++)
                {
                    Component.RecycleTextBlock(im.tag[t]);
                    Component.RecycleBorder(im.tag_bk[t]);
                }
            }
            li.Clear();
            c = ar_title.Count;
            for (int i = 0; i < c; i++)
                Component.RecycleButton(ar_title[i]);
            ar_title.Clear();
            c = change.Count;
            for (int i = 0; i < c; i++)
                Component.RecycleButton(change[i]);
            change.Clear();
            c = ar_bor.Count;
            for (int i = 0; i < c; i++)
                Component.RecycleBorder(ar_bor[i]);
            ar_bor.Clear();
            if (lb != null)
            {
                parent.Children.Remove(lb);
                GC.SuppressFinalize(lb);
            }
            GC.SuppressFinalize(this);
        }
        public void Hide()
        {
            sv.Visibility = Visibility.Collapsed;
            ell.Visibility = Visibility.Collapsed;
        }
        public void Show()
        {
            sv.Visibility = Visibility.Visible;
            if (pos.ac == 0)
                ell.Visibility = Visibility.Visible;
        }
        public bool Back()
        {
            if(lb!=null)
            {
                parent.Children.Remove(lb);
                lb.Visibility = Visibility.Collapsed;
                ell.Visibility = Visibility.Visible;
                GC.SuppressFinalize(lb);
                lb = null;
                return true;
            }
            return false;
        }
    }

    class Scroll_S:Component,IDisposable
    {
        struct Item
        {
            public Image img;
            public BitmapImage map;
            public Button button;
            public TextBlock title;
            public TextBlock detail;
        }
        struct Position
        {
            public int column;
            public int row;
            public int maxcount;
        }
        struct ItemSizeA
        {
            public double svw;
            public double svh;
            public double cw;
            public double tw;
        }
        ScrollViewer sv;
        Canvas content;
        Canvas parent;
        Thickness margin;
        ItemSizeA isa;
        Position pos;
        List<Item> li;
        public bool LoadOver { get; set; }
        public Action Getmore;
        const double itemwidth = 280;
        public Action<object> itemclick;
        public Scroll_S()
        {
            sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            content = new Canvas();
            sv.Content = content;
            sv.ViewChanged += Sliding;
            li = new List<Item>();
            data = new List<ItemDataA>();
        }
        void Sliding(object o, ScrollViewerViewChangedEventArgs e)
        {
            OrderItem();
        }
        void ResizeItem(ref Item i)
        {
            i.img.Width = 138;
            i.img.Height = 138;
            i.title.Width = isa.tw;
            i.detail.Width = isa.tw;
            i.detail.Height = 120;
            i.button.Width = isa.cw-4;
            i.button.Height = 140;
        }
        void OrderItem()
        {
            if (li.Count == 0)
                return;
            double os = sv.VerticalOffset;
            int row = (int)((os - 142 * 0.3f) / 142);
            int index = row * pos.column;
            double dy = row * 142;
            double dx = 0;
            Item ima;
            int max = li.Count;
            int s = index % max;
            if (index + max <= data.Count + pos.column)
                for (int i = 0; i < max; i++)
                {
                    ima = li[s];
                    if (ima.map.UriSource == null || ima.map.UriSource.OriginalString != data[index].src)
                    {
                        ima.img.Margin = new Thickness(dx+2, dy, 0, 0);
                        ima.title.Margin = new Thickness(dx+144, dy , 0, 0);
                        ima.detail.Margin = new Thickness(dx+144, dy+20, 0, 0);
                        ima.button.Margin = new Thickness(dx, dy, 0, 0);
                        ima.button.DataContext = data[index].href;
                        if (data[index].src != null)
                            ima.map.UriSource = new Uri(data[index].src);//gif no effect
                        if (data[index].title != null)
                            ima.title.Text = data[index].title;
                        if (data[index].detail != null)
                            ima.detail.Text = data[index].detail;
                        ItemClick ic = new ItemClick();
                        ic.click = itemclick;
                        ic.tag = data[index].href;
                        ima.button.DataContext = ic;
                    }
                    dx += isa.cw;
                    if (dx + isa.cw-5 > isa.svw)
                    {
                        dx = 0;
                        dy += 142;
                    }
                    index++;
                    if (index >= data.Count)
                        break;
                    s++;
                    if (s >= max)
                        s = 0;
                }
            if(index+max>=data.Count-10)
            {
                if (!LoadOver)
                    if (Getmore != null)
                        Getmore();
            }
        }
        public List<ItemDataA> data { set; get; }
        public void Refresh()
        {
            int max = data.Count;
            if (pos.column < 1)
                pos.column = 1;
            int r = max % pos.column;
            int row = max / pos.column;
            if (r > 0)
                row++;
            double h = row * 142+5;
            if (h < isa.svh)
                content.Height = isa.svh;
           else content.Height = h;
            if (max > pos.maxcount)
                max = pos.maxcount;
            int c = li.Count;
            int i;
            Item imb;
            for (i = 0; i < max; i++)
            {
                if (i >= c)
                    imb = CreateNewItem();
                else imb = li[i];
                ResizeItem(ref imb);
            }
            for (i = c - 1; i >= max; i--)
            {
                imb = li[i];
                li.RemoveAt(i);
                content.Children.Remove(imb.img);
                content.Children.Remove(imb.title);
                content.Children.Remove(imb.detail);
                content.Children.Remove(imb.button);
                RecycleImage(imb.img);
                RecycleTextBlock(imb.title);
                RecycleTextBlock(imb.detail);
                RecycleButton(imb.button);
            }
            if (data.Count > 0)
                OrderItem();
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
            double w = m.Right - m.Left;
            double h = m.Bottom - m.Top;
            sv.Width = w;
            sv.Height = h;
            sv.Margin = m;
            content.Width = w;
            isa.svw = w;
            double cw = w / itemwidth;
            int c = (int)cw;
            isa.cw = cw = w / c;
            isa.tw = cw - 150;
            double ch = 140;
            int r = (int)(h / ch);
            r++;
            pos.row = r;
            pos.column = c;
            pos.maxcount = (r + 2) * c;
        }
        public void Dispose()
        {
            parent.Children.Remove(sv);
            content.Children.Clear();
            int c = li.Count;
            for (int i = c - 1; i > 0; i--)
            {
                Item imb = li[i];
                li.RemoveAt(i);
                content.Children.Remove(imb.img);
                content.Children.Remove(imb.title);
                content.Children.Remove(imb.detail);
                content.Children.Remove(imb.button);
                RecycleImage(imb.img);
                RecycleTextBlock(imb.title);
                RecycleTextBlock(imb.detail);
                RecycleButton(imb.button);
            }
            sv.Content = null;
            GC.SuppressFinalize(content);
            GC.SuppressFinalize(sv);
            GC.SuppressFinalize(li);
            GC.SuppressFinalize(this);
        }
        Item CreateNewItem()
        {
            Item i = new Item();
            i.img = CreateImageNext();
            i.map = i.img.Source as BitmapImage;
            i.title = CreateTextBlockNext();
            i.title.Foreground = title_brush;
            i.detail = CreateTextBlockNext();
            i.detail.Foreground = font_brush;

            i.button = CreateButtonNext();
            i.button.Foreground = trans_brush;
            i.button.BorderBrush = filter_brush;
            i.button.BorderThickness = new Thickness(1,1,1,1);
            content.Children.Add(i.img);
            content.Children.Add(i.title);
            content.Children.Add(i.detail);
            content.Children.Add(i.button);
            li.Add(i);
            return i;
        }
        public void ShowBorder()
        {
            sv.BorderBrush = bor_brush;
            sv.BorderThickness = new Thickness(2, 2, 2, 2);
        }
    }

}
