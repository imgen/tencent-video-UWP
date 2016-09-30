using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace TVWP.Class
{

    struct ImageContext
    {
        public string href;
        public string src;
        public string vid;
        public string title;
        public string detail;
        public int time;
        public int type;
    }
    struct VideoInfo
    {
        public string sharp;
        public string fid;
        public string id;
        public string fmt;
        public string vkey;
        public string fn;
    }
    struct VideoInfoA
    {
        public int alltime;
        public int fregment;
        public string vid;
        public VideoInfo[] vi;
        public string[] http;
    }
    struct VideoItem
    {
        public Image img;
        public TextBlock textblock;
    }
    struct VideoContainer
    {
        public List<VideoItem> LVI;
        public StackPanel parent;
        public ScrollViewer sv;
    }

    struct CommentInfo
    {
        public string username;
        public string content;
        public string url;
        public string time;
        public string uid;
        public string vip;
        public string uidA;
        //public string uidB;
        public string region;
    }
    struct CommentConponent
    {
        public Canvas can;
        public Image head;
        public TextBlock title;
        public TextBlock content;
        public TextBlock time;
    }
    struct CommentContainer
    {
        public StackPanel stack;
        public ScrollViewer sv;
        public List<CommentConponent> son;
    }

    struct SearchContainer
    {
        public Canvas can;
        public ScrollViewer sv;
        public List<SearchItem> items;
    }
    struct SearchItem
    {
        public Image img;
        public TextBlock title;
        //public TextBlock detail;
    }
    struct Nav_Data
    {
        public string[] text;
        public string href;
        public Nav_Data(string[] t, string h) { text = t; href = h; }
    }
}
