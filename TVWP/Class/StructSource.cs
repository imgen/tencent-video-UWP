using System;
using System.Collections.Generic;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    interface SonPartialPage
    {
        void Create(Canvas p,Thickness m);
        void UpdatePage(string data);
        void ReSize(Thickness m);
        void Hide();
        void Dispose();
    }
    interface Navigation
    {
        void Create(Canvas p, Thickness m);
        bool Back();
        void Hide();
        void Show();
        void ReSize(Thickness m);
    }
    struct ItemData_m
    {
        public string href;
        public string src;
        public string title;
        public string detail;
        public string[] tag;
    }
    struct Area_m
    {
        public PageAddress page;
        public double dx;
        public double dy;
        public double height;
        public int start;
        public int count;
        public string[] title;
        public string alt;
        public int com_index;
        public int showindex;
        public ItemData_m[] data;
    }
    struct ItemContext
    {
        public string tag;
        public string text;
    }
    struct FilterItemData
    {
        public string tilte;
        public List<ItemContext> data;
    }
    struct ImageContext
    {
        public string href;
        public string src;
        public string vid;
        public string title;
        public string detail;
        public int time;
    }
    struct VideoInfo
    {
        public int type;
        public int alltime;
        public int part;
        public int site;
        public string vid;
        public string tilte;
        public string href;
        public string vkey;
        public string[] sharp;
        public string[] cmd5;
    }
    struct CommentInfo
    {
        public string nick;
        public string content;
        public string url;
        public string time;
        public string m_id;//message id
        public string m_r_id;// message root id
        public string vip;
        public string u_id;//user id
        public string region;
        public string count;
        public int rid;
        public int approval;
        public int against;
        public int replay;
    }
    struct UpContent
    {
        public double width;
        public double height;
        public int type;
        public char[] text;
        public string content;
    }
    struct UpCommentInfo
    {
        public string nick;
        public string title;
        public string url;
        public string time;
        public string m_id;//message id
        public string m_r_id;// message root id
        public string vip;
        public string u_id;//user id
        public string region;
        public string count;
        public int rid;
        public int approval;
        public int against;
        public int replay;
        public int score;
        public List<UpContent> detail_s;
    }
    struct ItemDataA
    {
        public string detail;
        public string src;
        public string title;
        public string href;
    }
    struct ItemDataB
    {
        //public string vid;
        public string src;
        public string title;
        public string detail;
        public string href;
        //public string mask;
        public string mark;
    }
    struct ItemModE
    {
        public int index;
        public bool reg;
        public Canvas can;
        public Image img;
        //public Image ico;
        public TextBlock title;
        public TextBlock content;
        public Button button;
    }
    struct ItemSize
    {
        public double w;
        public double sw;
        public double sh;
        public double iw;
        public double ih;
        //public double oy_i;
        public double oy_t;
        //public double oy_c;
        //public double ch;
    }
    struct Rect
    {
        public double x;
        public double y;
        public double w;
        public double h;
    }
    struct PivotPage
    {
        public Pivot pivot;
        public PivotItem[] items;
        public Canvas[] son;
        public Button[] head;
       // public PivotHeader[] header;
        public Thickness item_margin;
        public int index;
    }
    struct Cla_data
    {
        public int count;//item count
        public string[] title;
        public PageAddress page;
    }
    struct ItemClick
    {
        public Action<object> click;
        public object tag;
    }
    struct History
    {
        //public bool watch;
        public int time;
        public string name;
        public string vid;
        public string href;
    }
    struct EP_Info
    {
        public string vid;
        public string title;
    }
}
