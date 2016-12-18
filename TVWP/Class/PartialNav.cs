using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace TVWP.Class
{
    enum PageAddress :int
    {
        ExclusiveSeletion, OriginalSelection, SynchronousTheater, PopularVariety, MovieBlockbuster,
        AmericanTV, KoreanDrama, Animation, Children, Documentary, Microfilm, Entertainment, laughPoint,
        Fashion, Living, MotherAndChild, Automotive, Finance, Technology, RealEstate, Travel, Game, None
    }

    class PartialNav:CharOperation,Navigation
    {
#region Exclusive selection
        static string[] nav_es0 ={"腾讯出品地带", "Tencent production zone" };
        static string[] nav_es1 = { "精彩节目", "Wonderful program" } ;
        static string[] nav_es2 = { "自制剧", "Self-made drama" } ;
        static string[] nav_es3 = { "出品电影", "Produced film" } ;
        static string[] nav_es4 =  { "出品短片", "Produced short films" } ;
        static string[] nav_es5 ={ "更多精彩", "More exciting" } ;
        static string[][] nav_es = {nav_es0,nav_es1,nav_es2,nav_es3,nav_es4,nav_es5};
#endregion

#region Original selection
        //static string[] nav_os0 = { "原创精选", "Original selection" };
        //static string[] nav_os1 = { "一周更新时间表", "" };
        //static string[] nav_os2 = { "微电影 · 网络剧", "" };
        //static string[] nav_os3 = { "综艺 · 娱乐", "" };
        //static string[] nav_os4 = { "体育", "" };
        //static string[] nav_os5 = { "资讯 · 记录片", "" };
        //static string[] nav_os6 = { "搞笑", "" };
        //static string[] nav_os7 = { "时尚 · 生活", "" };
        //static string[] nav_os8 = { "联合出品", "" };
        //static string[][] nav_os = { nav_os0, nav_os1 };//, nav_os2, nav_os3, nav_os4, nav_os5 ,nav_os6,nav_os7,nav_os8};
#endregion

#region Synchronous theater
        static string[] nav_st0 = { "全球追剧", "Global chase drama" };
        static string[] nav_st1 = { "腾讯自制", "Tencent made" };
        static string[] nav_st2 = { "卫视热播", "TV hit" };
        static string[] nav_st3 = { "美剧强档", "US drama strong file" };
        static string[] nav_st4 = { "独家英剧", "Exclusive English drama" };
        static string[] nav_st5 = { "时尚韩风", "Fashion Korean style" };
        static string[] nav_st6 = { "港台经典", "Hong Kong and Taiwan classic" };
        static string[] nav_st7 = { "预告 · 片花", "Trailer · Trailer" };
        static string[] nav_st8 = { "英美课堂", "English and American classrooms" };
        static string[][] nav_st = {nav_st0,nav_st1,nav_st2,nav_st3,nav_st4,nav_st5,nav_st6,nav_st7,nav_st8 };
#endregion

#region Popular Variety
        static string[] nav_pv0 = { "今日更新", "Updated today" };
        static string[] nav_pv1 = { "热门栏目", "Popular sections" };
        static string[] nav_pv2 = { "娱乐爆点", "Entertainment burst point" };
        static string[] nav_pv3 = { "腾讯出品", "Tencent produced" };
        static string[] nav_pv4 = { "韩国综艺", "South Korea Variety" };
        static string[] nav_pv5 = { "卫视强档", "TV strong file" };
        static string[] nav_pv6 = { "江苏卫视", "Jiangsu Satellite TV" };
        static string[] nav_pv7 = { "东方卫视", "Dragon TV" };
        static string[][] nav_pv = {nav_pv0,nav_pv1,nav_pv2,nav_pv3,nav_pv4,nav_pv5,nav_pv6,nav_pv7 };
#endregion

#region Movie blockbuster
        //static string[] nav_mb0 = { "资讯 · 预告片", "" };
        static string[] nav_mb1 = { "首播影院", "Premiere theaters" };
        static string[] nav_mb2 = { "经典太空片盘点", "Classic space inventory" };
        static string[] nav_mb3 = { "微电影", "Microfilm" };
        static string[] nav_mb4 = { "特色策划", "Features planning" };
        static string[] nav_mb5 = { "精彩专题", "Wonderful topic" };
        static string[] nav_mb6 = { "华语强档", "Chinese strong file" };
        static string[] nav_mb7 = { "异色欧美", "Different colors in Europe and America" };
        static string[] nav_mb8 = { "亚洲精选", "Asia Featured" };
        static string[][] nav_mb = {nav_mb1,nav_mb2,nav_mb3,nav_mb4,nav_mb5,nav_mb6,nav_mb7,nav_mb8 };
#endregion

#region American TV series
        static string[] nav_at0 = { "一周追剧时间表", "One week chase play schedule" };
        static string[] nav_at1 = { "王者英美剧场", "King Anglo - American Theater" };
        static string[] nav_at2 = { "青春 · 偶像", "Youth and idols" };
        static string[] nav_at3 = { "家庭 · 喜剧", "Family comedy" };
        static string[] nav_at4 = { "科幻 · 魔幻", "Science fiction magic" };
        static string[] nav_at5 = { "罪案 · 推理", "Crime and Reasoning" };
        static string[] nav_at6 = { "经典 · 口碑", "Classic word of mouth" };
        static string[] nav_at7 = { "开门遇见好莱坞", "Open the door to meet Hollywood" };
        static string[] nav_at8 = { "英美明星中国行", "British and American stars in China" };
        static string[] nav_at9 = { "明星来荐剧", "Star to recommend opera" };
        static string[] nav_ata = { "英美课堂", "English and American classrooms" };
        static string[][] nav_at = {nav_at0,nav_at1,nav_at2,nav_at3,nav_at4,nav_at5,nav_at6,nav_at7,nav_at8,nav_at9,nav_ata };
#endregion

#region Korean drama
        static string[] nav_kd0 = { "同步热播", "Synchronous hit" };
        static string[] nav_kd1 = { "精彩片花", "Wonderful film flowers" };
        static string[] nav_kd2 = { "花痴偶像", "Idiot idol" };
        static string[] nav_kd3 = { "都市情感", "Urban sentiment" };
        static string[] nav_kd4 = { "娱乐综艺", "Entertainment variety" };
        static string[] nav_kd5 = { "家庭伦理", "Family ethics" };
        static string[] nav_kd6 = { "古装武侠", "Costume martial arts" };
        static string[] nav_kd7 = { "重温经典", "Review the classic" };
        static string[] nav_kd8 = { "韩星门面担当", "Han Xing facade play" };
        static string[][] nav_kd = { nav_kd0,nav_kd1,nav_kd2,nav_kd3,nav_kd4,nav_kd5,nav_kd6,nav_kd7,nav_kd8};
#endregion

#region Animation selection
        static string[] nav_as0 = { "热播推荐", "Recommended" };
        static string[] nav_as1 = { "儿童动画", "Children Animation" };
        static string[] nav_as2 = { "日韩动漫", "Japanese and Korean animation" };
        static string[] nav_as3 = { "国产动漫", "Domestic animation" };
        static string[] nav_as4 = { "网络动漫", "Network animation" };
        static string[] nav_as5 = { "动画电影", "Animated film" };
        static string[] nav_as6 = { "流金经典", "Flow of the classic" };
        static string[] nav_as7 = { "漫迷世界", "Mang fans of the world" };
        static string[][] nav_as = { nav_as0,nav_as1,nav_as2,nav_as3,nav_as4,nav_as5,nav_as6,nav_as7 };
#endregion

#region Children
        static string[] nav_ch0 = { "热播推荐", "Recommended" };
        static string[] nav_ch1 = { "宝宝乐园", "Baby paradise" };
        static string[] nav_ch2 = { "金色童年", "Golden childhood" };
        static string[] nav_ch3 = { "激燃青春", "Stimulate youth" };
        static string[] nav_ch4 = { "动画电影", "Animated film" };
        static string[] nav_ch5 = { "流金经典", "Flow of the classic" };
        static string[] nav_ch6 = { "少儿综艺", "Children's Arts" };
        static string[][] nav_ch = { nav_ch0,nav_ch1,nav_ch2,nav_ch3,nav_ch4,nav_ch5,nav_ch6};
#endregion

#region Documentary
        static string[] nav_do0 = { "顶级首播", "Top premiere" };
        static string[] nav_do1 = { "铁血军事", "Jagged military" };
        static string[] nav_do2 = { "社会 · 刑侦", "Social and criminal investigation" };
        static string[] nav_do3 = { "时政 · 财经", "Politics, finance and economics" };
        static string[] nav_do4 = { "历史风云", "Historical situation" };
        static string[] nav_do5 = { "自然 · 地理", "Nature and Geography" };
        static string[] nav_do6 = { "科技 · 环保", "Technology · Environmental protection" };
        static string[] nav_do7 = { "旅游 · 人文", "Tourism · Humanities" };
        static string[][] nav_do = {nav_do0,nav_do1,nav_do2,nav_do3,nav_do4,nav_do5,nav_do6,nav_do7 };
#endregion

#region Microfilm
        static string[] nav_mf0 = { "最新上映", "The latest release" };
        static string[] nav_mf1 = { "微电影", "Microfilm" };
        static string[] nav_mf2 = { "网络剧时间表", "Network play schedule" };
        static string[] nav_mf3 = { "完结剧集", "End episode" };
        static string[] nav_mf4 = { "网络电影", "Internet Movie" };
        static string[] nav_mf5 = { "赛事展厅", "Event Hall" };
        static string[] nav_mf6 = { "独播影院", "Independent broadcast theater" };
        static string[] nav_mf7 = { "原创短片", "Original Short Film" };
        static string[] nav_mf8 = { "精彩专题", "Wonderful topic" };
        static string[] nav_mf9 = { "热门频道", "Popular channels" };
        static string[][] nav_mf = { nav_mf0,nav_mf1,nav_mf2,nav_mf3,nav_mf4,nav_mf5,nav_mf6,nav_mf7,nav_mf8,nav_mf9};
#endregion

#region Entertainment hot spots
        static string[] nav_eh0 = { "今日娱乐热点", "Today entertainment hot spots" };
        static string[] nav_eh1 = { "所谓娱乐", "The so-called entertainment" };
        static string[] nav_eh2 = { "尖叫吧路人", "Screaming passers-by" };
        static string[] nav_eh3 = { "周一见", "meet on Monday" };
        static string[] nav_eh4 = { "明星课程表", "Star curriculum" };
        static string[] nav_eh5 = { "存照", "On record" };
        static string[] nav_eh6 = { "青年问大师", "Youth asked the master" };
        static string[] nav_eh7 = { "星星侦探社", "Star Detective Agency" };
        static string[] nav_eh8 = { "娱乐挖掘机", "Recreational Excavator" };
        static string[] nav_eh9 = { "封面人物", "Cover character" };
        static string[][] nav_eh = {nav_eh0,nav_eh1,nav_eh2,nav_eh3,nav_eh4,nav_eh5,nav_eh6,nav_eh7,nav_eh8,nav_eh9 };
#endregion

#region The strongest laugh point
        static string[] nav_ts0 = { "最强笑点", "The strongest laugh point" };
        static string[] nav_ts1 = { "搞乐剧场", "Engage in musical theater" };
        static string[] nav_ts2 = { "原创恶搞", "Original spoof" };
        static string[] nav_ts3 = { "雷人糗事", "Ridiculous embarrassing things" };
        static string[] nav_ts4 = { "奇闻趣事", "Gonzo" };
        static string[] nav_ts5 = { "萌宝萌宠", "Meng Po Chong pet" };
        static string[] nav_ts6 = { "栏目策划", "Column planning" };
        static string[][] nav_ts = {nav_ts0,nav_ts1,nav_ts2,nav_ts3,nav_ts4,nav_ts5,nav_ts6};
#endregion

#region Fashion heat list
        static string[] nav_fh0 = { "时尚热度榜", "Fashion heat list" };
        static string[] nav_fh1 = { "原创节目", "Original program" };
        static string[] nav_fh2 = { "时装周", "Fashion Week" };
        static string[] nav_fh3 = { "秀场直击", "Show field direct hit" };
        static string[] nav_fh4 = { "潮衣风向标", "Tide clothes vane" };
        static string[] nav_fh5 = { "后天美人养成记", "Acquired the day after tomorrow" };
        static string[] nav_fh6 = { "型男养成记", "Profile of a man" };
        static string[] nav_fh7 = { "视觉盛宴", "Visual feast" };
        static string[][] nav_fh = { nav_fh0,nav_fh1,nav_fh2,nav_fh3,nav_fh4,nav_fh5,nav_fh6,nav_fh7};
#endregion

#region Living Information
        static string[] nav_li0 = { "热门资讯", "Top news" };
        static string[] nav_li1 = { "健康", "Health" };
        static string[] nav_li2 = { "美食", "Food" };
        static string[] nav_li3 = { "百科", "Encyclopedia" };
        static string[] nav_li4 = { "育儿", "Parenting" };
        static string[] nav_li5 = { "旅游", "Tourism" };
        static string[] nav_li6 = { "生活", "Life" };
        static string[][] nav_li = { nav_li0,nav_li1,nav_li2,nav_li3,nav_li4,nav_li5,nav_li6};
#endregion

#region Mother and child knowledge
        static string[] nav_ma0 = { "备孕孕期手册", "Preparing pregnancy manual" };
        static string[] nav_ma1 = { "分娩产后指导", "Postpartum guidance" };
        static string[] nav_ma2 = { "育儿喂养百科", "Parenting feeding Encyclopedia" };
        static string[] nav_ma3 = { "保健早教宝箱", "Health care early education chest" };
        static string[] nav_ma4 = { "宝宝秀", "Baby show" };
        static string[] nav_ma5 = { "宝宝TV", "Baby TV" };
        static string[] nav_ma6 = { "母婴", "Mother and child" };
        static string[][] nav_ma = { nav_ma0, nav_ma1, nav_ma2, nav_ma3, nav_ma4, nav_ma5, nav_ma6 };
#endregion

#region Automotive Information
        static string[] nav_ca0 = { "品牌专区", "Brand Zone" };
        static string[] nav_ca1 = { "原创节目", "Original program" };
        static string[] nav_ca2 = { "新车", "New car" };
        static string[] nav_ca3 = { "试驾", "Test drive" };
        static string[] nav_ca4 = { "车展", "Auto Show" };
        static string[] nav_ca5 = { "用车", "Car" };
        static string[] nav_ca6 = { "赛事", "Events" };
        static string[] nav_ca7 = { "交通", "Traffic" };
        static string[] nav_ca8 = { "改装", "Modified" };
        static string[] nav_ca9 = { "车旅", "Car travel" };
        static string[][] nav_ca = { nav_ca0,nav_ca1,nav_ca2,nav_ca3,nav_ca4,nav_ca5,nav_ca6,nav_ca7,nav_ca8,nav_ca9};
#endregion

#region Finance Classroom
        static string[] nav_fc0 = { "大牌精品", "Big boutique" };
        static string[] nav_fc1 = { "投资理财", "Investment and financial management" };
        static string[] nav_fc2 = { "财经课堂", "Finance Classroom" };
        static string[] nav_fc3 = { "创业天堂", "Entrepreneurial Paradise" };
        static string[][] nav_fc = {nav_fc0,nav_fc1,nav_fc2,nav_fc3 };
#endregion

#region Technology hot spots
        static string[] nav_th0 = { "热门视频", "Popular videos" };
        static string[] nav_th1 = { "原创栏目", "Original section" };
        static string[] nav_th2 = { "科学探索", "Scientific Exploration" };
        static string[] nav_th3 = { "数码体验", "Digital experience" };
        static string[] nav_th4 = { "科技风云", "Science and technology situation" };
        static string[] nav_th5 = { "原创精选", "Original selection" };
        static string[] nav_th6 = { "原创自媒体", "Original from the media" };
        static string[][] nav_th = {nav_th0,nav_th1,nav_th2,nav_th3,nav_th4,nav_th5,nav_th6 };
#endregion

#region Real estate information
        static string[] nav_ho0 = { "热门推荐", "Popular Recommended" };
        static string[] nav_ho1 = { "家具装修", "Furniture renovation" };
        static string[] nav_ho2 = { "房生百态", "Housing students attitudes" };
        static string[] nav_ho3 = { "风水趣谈", "Feng shui interesting" };
        static string[] nav_ho4 = { "楼市动态", "Property market dynamics" };
        static string[][] nav_ho = { nav_ho0,nav_ho1,nav_ho2,nav_ho3,nav_ho4};
#endregion

#region Travel
        static string[] nav_tr0 = { "国内游", "Domestic Travel" };
        static string[] nav_tr1 = { "国外游", "Overseas travel" };
        static string[] nav_tr2 = { "环球趣闻", "Global Interest" };
        static string[] nav_tr3 = { "旅行达人", "Travel up to people" };
        static string[] nav_tr4 = { "精彩专题", "Wonderful topic" };
        static string[] nav_tr5 = { "旅游", "Tourism" };
        static string[][] nav_tr = {nav_tr0,nav_tr1,nav_tr2,nav_tr3,nav_tr4,nav_tr5 };
#endregion

#region Game
        static string[] nav_ga0 = { "今日推荐", "Recommended today" };
        static string[] nav_ga1 = { "精品游戏", "Boutique game" };
        static string[] nav_ga2 = { "当前爆款", "The current explosion" };
        static string[] nav_ga3 = { "我的世界", "My world" };
        static string[] nav_ga4 = { "英雄联盟", "League of legends" };
        static string[] nav_ga5 = { "游戏达人", "Game up people" };
        static string[] nav_ga6 = { "热门网游", "Popular online games" };
        static string[] nav_ga7 = { "热门手游", "Popular hand tour" };
        static string[] nav_ga8 = { "VR游戏", "VR games" };
        static string[] nav_ga9 = { "美女主播", "Beauty anchor" };
        static string[] nav_gaa = { "游戏八卦", "Game gossip" };
        static string[] nav_gab = { "二次元", "The second element" };
        static string[] nav_gac = { "合作媒体", "Cooperative Media" };
        static string[][] nav_ga = {nav_ga0,nav_ga1,nav_ga2,nav_ga3,nav_ga4,nav_ga5,nav_ga6,
            nav_ga7,nav_ga8,nav_ga9,nav_gaa,nav_gab,nav_gac };
        #endregion

        #region code
        struct SonPage
        {
            public Canvas Main;
            public Button head;
            public ScrollViewD svd;
        }
        delegate SonPartialPage GetObject();
        struct Nav_data
        {
            public bool special;
            public GetObject GetPage;
            public SonPartialPage son;
            public string[][] headers;
            public string href;
        }
        static Nav_data[] nav_buff = {
            new Nav_data {headers=nav_es,href="http://v.qq.com/program/" },
            new Nav_data {special=true, GetPage=()=> {return new OriginalPage(); },href="http://v.qq.com/videoplus/"},
            new Nav_data {headers=nav_st,href="http://v.qq.com/tv/" },
            new Nav_data {headers=nav_pv,href="http://v.qq.com/variety/" },
            new Nav_data {headers=nav_mb,href="http://v.qq.com/movie/" },
            new Nav_data {headers=nav_at,href="http://v.qq.com/tv/yingmei/" },
            new Nav_data {headers=nav_kd,href="http://v.qq.com/tv/korea/" },
            new Nav_data {headers=nav_as,href="http://v.qq.com/cartoon/" },
            new Nav_data {headers=nav_ch,href="http://v.qq.com/child/" },
            new Nav_data {headers=nav_do,href="http://v.qq.com/doco/" },
            new Nav_data {headers=nav_mf,href="http://v.qq.com/dv/" },
            new Nav_data {headers=nav_eh,href="http://v.qq.com/ent/" },
            new Nav_data {headers=nav_ts,href="http://v.qq.com/fun/" },
            new Nav_data {headers=nav_fh,href="http://v.qq.com/fashion/" },
            new Nav_data {headers=nav_li,href="http://v.qq.com/life/" },
            new Nav_data {headers=nav_ma,href="http://v.qq.com/baby/index.html" },
            new Nav_data {headers=nav_ca,href="http://v.qq.com/auto/" },
            new Nav_data {headers=nav_fc,href="http://v.qq.com/finance/" },
            new Nav_data {headers=nav_th,href="http://v.qq.com/tech/" },
            new Nav_data {headers=nav_ho,href="http://v.qq.com/house/" },
            new Nav_data {headers=nav_tr,href="http://v.qq.com/travel/" },
            new Nav_data {headers=nav_ga,href="http://v.qq.com/games/" }
        };
        static char[] c_buff;
        static PivotPage pp;
        static List<PivotItem> pi_buff;
        static List<SonPage> pi_son;
        static bool task, load;
        static PageAddress current;
        static Thickness margin;
        static void CreateA(Canvas p,Thickness m)
        {
            int c = (int)current;
            if (nav_buff[c].special)
            {
                if (nav_buff[c].son == null)
                    nav_buff[c].son = nav_buff[c].GetPage();
                nav_buff[c].son.Create(p,m);
            }
            else CreateDef(p,m);
        }
        static void CreateDef(Canvas p, Thickness m)
        {
            if (pp.pivot != null)
            {
                pp.pivot.Visibility = Visibility.Visible;
                pp.pivot.SelectedIndex = 0;
                Resize(m);
                return;
            }
            margin = m;
            Pivot pi = new Pivot();
            p.Children.Add(pi);
            pp.pivot = pi;
            pp.pivot.SelectionChanged += (o, e) => {
                int index = pp.pivot.SelectedIndex;
                if (index < 0)
                    return;
                Thickness tk = margin;
                tk.Top = 0;tk.Bottom -= 40;
                pi_son[index].svd.ReSize(tk);
                pi_son[index].svd.Refresh();
            };
            pi_buff = new List<PivotItem>();
            pi_son = new List<SonPage>();
            task = true;
            if (load)
            {
                InitialPivot();
                Anlayze();
                pi_son[0].svd.Refresh();
            }
        }
        static void InitialPivot()
        {
            Nav_data nd = nav_buff[(int)current];
            int len = nd.headers.Length;
            int a = pi_son.Count;
            int t = 0;
            if (pp.pivot.Items != null)
                t = pp.pivot.Items.Count;
            for(int i=0;i<len;i++)
            {
                if(i>=a)
                {
                    PivotItem pi = new PivotItem();
                    pi_buff.Add(pi);
                    SonPage son = new SonPage();
                    son.Main = new Canvas();
                    Button b = new Button();
                    b.Background = Component.trans_brush;
                    b.Foreground = Component.nav_brush;
                    b.FontSize = 18;
                    b.Content = nd.headers[i][Component.language];
                    son.head = b;
                    pi.Header = b;
                    pi.Content = son.Main;
                    son.svd = new ScrollViewD();
                    son.svd.itemclick = (o) => {
                        VideoPage.SetAdress(o as string);
                        PageManageEx.CreateNewPage(PageTag.videopage);
                    };
                    son.svd.SetParent(son.Main);
                    pi_son.Add(son);
                    pp.pivot.Items.Add(pi);
                }
                else
                    pi_son[i].head.Content= nd.headers[i][Component.language];
            }
            for (int o = t-1; o >= len; o--)
                pp.pivot.Items.RemoveAt(o);
            Pivot p = pp.pivot;
            p.Margin = margin;
            p.Width = Component.screenX;
            p.Height = margin.Bottom-margin.Top;
            Thickness tk = margin;
            tk.Top = 0;
            tk.Bottom -= 40;
        }
        public static void LoadPage(PageAddress pa)
        {
            current = pa;
            load = false;
            Nav_data nd = nav_buff[(int)pa];
            WebClass.TaskGet(nd.href,LoadData);
        }
        static void LoadData(string data)
        {
            int c = (int)current;
            if (nav_buff[c].special)
            {
                nav_buff[c].son.UpdatePage(data);
                return;
            }
            c_buff = data.ToCharArray();
            c_buff = DeleteChar(ref c_buff,'\\');
            if (task)
            {
                InitialPivot();
                Anlayze();
                pi_son[0].svd.Refresh();
                pp.pivot.SelectedIndex = 0;
            }
            else load = true;
        }
        static void Anlayze()
        {
            int s=0;
            Nav_data nd = nav_buff[(int)current];
            int end = nd.headers.Length;
            ItemDataA ic = new ItemDataA();
            for (int i = 0; i < end; i++)
            {
                List<ItemDataA> lic = pi_son[i].svd.data;
                lic.Clear();
                char[] tmp = nd.headers[i][0].ToCharArray();
                int ts = FindCharArray(ref c_buff, ref tmp, s);
                if (ts > 0)
                {
                    s = FindCharArray(ref c_buff,ref Key_figures_list,ts);
                    if (s < 0)
                        return;
                    int e = FindCharArray(ref c_buff,ref Key_c_over,s);
                    if (e < 0)
                        e = c_buff.Length;
                    while (s>0)
                    {
                        s = FindCharArray(ref c_buff, ref Key_list_item, s, e);
                        if (s < 0)
                            break;
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
                        if (FindCharArray(ref tt, ref Key_about, 0) > -1)
                            break;
                        if (FindCharArray(ref tt, ref Key_http, 0) > -1)
                            aa = new string(tt);
                        else aa = "http:" + new string(tt);
                        ic.src = aa;
                        lic.Add(ic);
                    }
                    s = e;
                }
            }
        }
        static void Resize(Thickness m)
        {
            margin = m;
            int c = (int)current;
            if (nav_buff[c].special)
                nav_buff[c].son.ReSize(m);
            else
            {
                Pivot p = pp.pivot;
                p.Margin = margin;
                p.Width = Component.screenX;
                p.Height = m.Bottom - m.Top;
                Thickness tk = margin;
                tk.Top = 0;
                tk.Bottom -= 40;
                pi_son[p.SelectedIndex].svd.ReSize(tk);
                pi_son[p.SelectedIndex].svd.Refresh();
            }
        }
        public void Hide()
        {
            int c = (int)current;
            if (nav_buff[c].special)
            {
                nav_buff[c].son.Hide();
            }else
            pp.pivot.Visibility = Visibility.Collapsed;
        }
        public void Show()
        {
            int c = (int)current;
            if (nav_buff[c].special)
            {
                
            }
            else
                pp.pivot.Visibility = Visibility.Visible;
        }
        public void ReSize(Thickness m)
        {
            Resize(m);
        }
        public bool Back()
        {
            int c = (int)current;
            if (nav_buff[c].special)
                nav_buff[c].son.Dispose();
            pp.pivot.Visibility = Visibility.Collapsed;
            return false;
        }
        public void Create(Canvas p, Thickness m)
        {
            CreateA(p,m);
        }
        #endregion

        //http://sports.qq.com/nbavideo/
    }

    enum PageAddress_m : int
    {
        TV, Movie, Variety, Animation, Entertainment, News, Sports, Life, Game, Documentary, Function, Automotive, None
    }
    class PageNav_m: CharOperation ,Navigation
    {   
        static string tv = "113";
        //"http://m.v.qq.com/tv.html";
        static string movie = "173";
        static string variety = "109";
        static string animation = "119";
        static string ent = "111";
        static string news = "107";
        static string sports = "103";
        static string life = "114";
        static string game= "115";
        static string doc= "105";
        static string fun= "102";
        static string car= "161";
        static string[] Address = {tv,movie,variety,animation,ent,news,sports,life,game,doc,fun,car };
        static Scroll_ex sv;
        static PageAddress_m current;
        static Thickness margin;
        public static void CreateA(Canvas p,Thickness m)
        {
            margin = m;
            if (sv == null)
            {
                sv = new Scroll_ex();
#if desktop
                sv.maxcolumn = 5;
#else
                sv.maxcolumn = 4;
#endif
                sv.SetParent(p);
                sv.ItemClick = (o) =>
                {
                    if (o != null)
                    {
                        VideoPage.SetAdress(o as string);
                        PageManageEx.CreateNewPage(PageTag.videopage);
                    }
                };
            }
            else sv.Show();
        }
        public static void LoadPage(PageAddress_m pa)
        {
            current = pa;
            Load( Address[(int)pa],Update);
            //sv.PreLoad(margin);
        }
        static void Update()
        {
            sv.area = area;
            sv.ForceUpdateOnce = true;
            sv.ReSize(margin);
            sv.Refresh();
        }
        public static void Dispose()
        {
            //sv.Hide();
            sv.Dispose();
            sv = null;
        }
        #region data analyze
        static string url = "http://mobile.video.qq.com/fcgi-bin/getjimudata?otype=json&platform=103&version=2&ztid=100";
        static char[] Key_focus = "focus\"".ToCharArray();
        static char[] Key_tag = "tag_".ToCharArray();
        static char[] Key_publish_time = "publish_time\"".ToCharArray();
        static char[] Key_stitle = "stitle\"".ToCharArray();
        static char[] Key_web_url = "web_url\"".ToCharArray();
        static char[] Key_extend = "_extend\"".ToCharArray();
        static char[] Key_text = "text\"".ToCharArray();
        static char[] Key_score = "score\"".ToCharArray();
        static char[] Key_zt_head = "ZT_leaf_head\"".ToCharArray();
        static List<string> ls;
        public static Area_m[] area;
        static Action callback;
        static string address;
        static int part;
        static int allpart;
        public static void Load(string type, Action back)
        {
            if (ls == null)
                ls = new List<string>();
            address = url + type;
            WebClass.TaskGet(address, GetList);
            callback = back;
        }
        static void GetList(string str)
        {
            ls.Clear();
            char[] buff = str.ToCharArray();
            int s = 0;
            //string now = "";
            while (true)
            {
                int e = FindCharArray(ref buff, ref Key_leaf_id, s);
                if (e < 1)
                {
                    //e= FindCharArray(ref buff, ref Key_now, s);
                    //now = new string(FindCharArrayA(ref buff, ':', '}', ref e));
                    break;
                }
                else s = e;
                ls.Add(new string(FindCharArrayA(ref buff, '\"', '\"', ref s)));
            }
            int c = ls.Count;
            area = new Area_m[c];
            c--;
            int o = c / 10;
            allpart = o + 1;
            part = 0;
            int ss = 0;
            string r;
            for (int t = 0; t < o; t++)
            {
                r = address + "&type=1&leafids=";
                for (int i = 0; i < 9; i++)
                { r += ls[ss] + "%2B"; ss++; }
                r += ls[ss];
                ss++;
                WebClass.TaskGet(r, GetData, t * 10);
            }
            r = address + "&type=1&leafids=";
            for (int i = ss; i < c; i++)
            { r += ls[i] + "%2B"; }
            r += ls[c];
            WebClass.TaskGet(r, GetData, o * 10);
        }
        static void GetData(string str, int p)
        {
            //Debug.WriteLine(str);
            AnalyzeArea(str.ToCharArray(), p, ref area);
            part++;
            if (part == allpart)
            {
                int a = 0;
                int c = ls.Count;
                for (int i = 0; i < c; i++)
                {
                    if (area[i].count > 0)
                        a++;
                }
                Area_m[] t = new Area_m[a];
                a = 0;
                for (int i = 0; i < c; i++)
                {
                    if (area[i].count > 0)
                    {
                        t[a] = area[i];
                        a++;
                    }
                }
                area = t;
                callback();
            }
        }
        static void AnalyzeArea(char[] buff, int index, ref Area_m[] a)
        {
            int max = a.Length;
            int o = max - index;
            if (o > 10)
                max = index + 10;
            int s = FindCharArray(ref buff, ref Key_zt_head, 0);
            int end;
            List<ItemData_m> li = new List<ItemData_m>();
            for (int i = index; i < max; i++)
            {
                end = FindCharArray(ref buff, ref Key_zt_head, s);
                if (end < 0)
                    end = buff.Length;
                if (buff[s + 2] != 'n')
                    a[i].alt = new string(FindCharArrayA(ref buff, ':', ',', ref s));
                s = FindCharArray(ref buff, ref Key_extend, s);
                int e;
                do
                {
                    e = FindCharArray(ref buff, ref Key_extend, s + 100, end);
                    if (e < 0)
                        e = end;
                    ItemData_m idm = AnalyeItem(ref buff, s, e);
                    if (idm.href != null)
                        li.Add(idm);
                    s = e;
                } while (e != end);
                a[i].data = li.ToArray();
                a[i].count = li.Count;
                li.Clear();
            }
        }
        static ItemData_m AnalyeItem(ref char[] buff, int s, int e)
        {
            ItemData_m idm = new ItemData_m();
            idm.tag = new string[3];
            int c = FindCharArray(ref buff, ref Key_focus, s);
            char[] ot = FindCharArrayA(ref buff, '\"', '\"', ref c);
            if (ot[0] != 'h')
                idm.src = "http:" + new string(ot);
            else idm.src = new string(ot);
            int t = c;
            for (int i = 0; i < 3; i++)
            {
                t = FindCharArray(ref buff, ref Key_tag, t, e);
                if (t < 0)
                    break;
                t = FindCharArray(ref buff, ref Key_text, t);
                char[] cc = FindCharArrayA(ref buff, '\"', '\"', ref t);
                if (cc != null)
                    idm.tag[i] = new string(cc);
                c = t;
            }
            t = FindCharArray(ref buff, ref Key_score, c, e);
            if (t > 0)
            { c = t; idm.tag[2] += "  " + new string(FindCharArrayA(ref buff, '\"', '\"', ref t)); }
            t = FindCharArray(ref buff, ref Key_stitle, c);
            if (t > 0)
            { c = t; idm.detail = new string(FindCharArrayA(ref buff, '\"', '\"', ref t)); }
            t = FindCharArray(ref buff, ref Key_titleB, c);
            if (t > 0)
            { t++; c = t; idm.title = new string(FindCharArrayA(ref buff, '\"', '\"', ref t)); }
            t = FindCharArray(ref buff, ref Key_web_url, c, e);
            if (t > 0)
            {
                ot = FindCharArrayA(ref buff, '\"', '\"', ref t);
                if (ot[0] != 'h')
                    idm.href = "http:" + new string(ot);
                else idm.href = new string(ot);
            }
            return idm;
        }
        #endregion

        public void Create(Canvas p, Thickness m)
        {
            CreateA(p, m);
        }
        public void Hide()
        {
            sv.Hide();
        }
        public void Show()
        {
            sv.Show();
        }
        public void ReSize(Thickness m)
        {
            sv.ReSize(m);
            sv.Refresh();
        }
        public bool Back()
        {
            if (sv.Back())
                return true;
            Dispose();
            return false;
        }
    }

    class PartialPage : Component
    {
        struct PageProperty
        {
            public string url;
            public string[] title;
        }

        #region main img area
        static Cla_data nav_fy = new Cla_data { count = 9, title = new string[] { "为你精选", "For you selection" }, page = PageAddress.None };
        static Cla_data nav_es = new Cla_data { count = 6, title = new string[] { "独家精选", "Exclusive selection" }, page = PageAddress.ExclusiveSeletion };
        static Cla_data nav_os = new Cla_data { count = 6, title = new string[] { "原创精选", "Original selection" }, page = PageAddress.OriginalSelection };
        static Cla_data nav_st = new Cla_data { count = 9, title = new string[] { "同步剧场", "Synchronous theater" }, page = PageAddress.SynchronousTheater };
        static Cla_data nav_pv = new Cla_data { count = 9, title = new string[] { "热门综艺", "Popular Variety" }, page = PageAddress.PopularVariety };
        static Cla_data nav_sk = new Cla_data { count = 6, title = new string[] { "韩国综艺", "South Korea Variety" }, page = PageAddress.None };
        static Cla_data nav_mb = new Cla_data { count = 12, title = new string[] { "电影大片", "Movie blockbuster" }, page = PageAddress.MovieBlockbuster };
        static Cla_data nav_at = new Cla_data { count = 9, title = new string[] { "美剧·英剧", "American TV series" }, page = PageAddress.AmericanTV };
        static Cla_data nav_kd = new Cla_data { count = 9, title = new string[] { "韩剧", "Korean drama" }, page = PageAddress.KoreanDrama };
        static Cla_data nav_as = new Cla_data { count = 9, title = new string[] { "动漫精选", "Animation selection" }, page = PageAddress.Animation };
        static Cla_data nav_ch = new Cla_data { count = 6, title = new string[] { "少儿", "Children" }, page = PageAddress.Children };
        static Cla_data nav_doc = new Cla_data { count = 6, title = new string[] { "纪录片", "Documentary" }, page = PageAddress.Documentary };
        static Cla_data nav_mf = new Cla_data { count = 6, title = new string[] { "微电影", "Microfilm" }, page = PageAddress.Microfilm };
        static Cla_data nav_eh = new Cla_data { count = 6, title = new string[] { "娱乐热点", "Entertainment hot spots" }, page = PageAddress.Entertainment };
        static Cla_data nav_si = new Cla_data { count = 6, title = new string[] { "体育资讯", "Sports information" }, page = PageAddress.None };
        static Cla_data nav_nb = new Cla_data { count = 6, title = new string[] { "NBA专区", "NBA area" }, page = PageAddress.None };
        static Cla_data nav_fuc = new Cla_data { count = 6, title = new string[] { "最强笑点", "The strongest laugh point" }, page = PageAddress.laughPoint };
        static Cla_data nav_mv = new Cla_data { count = 6, title = new string[] { "MV八音盒", "MV music box" }, page = PageAddress.None };
        static Cla_data nav_ec = new Cla_data { count = 6, title = new string[] { "独家演唱会", "Exclusive concert" }, page = PageAddress.None };
        static Cla_data nav_fh = new Cla_data { count = 6, title = new string[] { "时尚热度榜", "Fashion heat list" }, page = PageAddress.Fashion };
        static Cla_data nav_lic = new Cla_data { count = 6, title = new string[] { "生活资讯", "Living Information" }, page = PageAddress.Living };
        static Cla_data nav_ba = new Cla_data { count = 6, title = new string[] { "母婴常识", "Mother and child knowledge" }, page = PageAddress.MotherAndChild };
        static Cla_data nav_ca = new Cla_data { count = 6, title = new string[] { "汽车资讯", "Automotive Information" }, page = PageAddress.Automotive };
        static Cla_data nav_fc = new Cla_data { count = 6, title = new string[] { "财经课堂", "Finance Classroom" }, page = PageAddress.Finance };
        static Cla_data nav_te = new Cla_data { count = 6, title = new string[] { "科技热点", "Technology hot spots" }, page = PageAddress.Technology };
        static Cla_data nav_ho = new Cla_data { count = 6, title = new string[] { "房产资讯", "Real estate information" }, page = PageAddress.RealEstate };
        static Cla_data nav_br = new Cla_data { count = 6, title = new string[] { "品牌专区", "Brand Zone" }, page = PageAddress.None };
        static Cla_data nav_tr = new Cla_data { count = 6, title = new string[] { "旅游", "Travel" }, page = PageAddress.Travel };
        static Cla_data nav_gac = new Cla_data { count = 6, title = new string[] { "精品游戏", "Fine game" }, page = PageAddress.Game };
        static Cla_data nav_tc = new Cla_data { count = 6, title = new string[] { "腾讯拍客", "Tencent shoot off" }, page = PageAddress.None };
        protected static Cla_data[] nav_all = {nav_fy,nav_es,nav_os,nav_st,nav_pv,nav_sk,nav_mb,nav_at,nav_kd,
        nav_as,nav_ch,nav_doc,nav_mf,nav_eh,nav_si,nav_nb,nav_fuc,nav_mv,nav_ec,nav_fh,nav_lic,nav_ba,nav_ca,
        nav_fc,nav_te,nav_ho,nav_br,nav_tr, nav_gac,nav_tc};

        #endregion

        #region main img area
        static PageProperty nav_tv = new PageProperty { title = new string[] { "TV", "TV" }, url = "ms-appx:///Pic/tv64.png" };
        static PageProperty nav_mo = new PageProperty { title = new string[] { "电影", "Movie" }, url = "ms-appx:///Pic/mov64.png" };
        static PageProperty nav_va = new PageProperty { title = new string[] { "综艺", "Variety" }, url = "ms-appx:///Pic/User.png" };
        static PageProperty nav_an = new PageProperty { title = new string[] { "动漫", "Animation" }, url = "ms-appx:///Pic/ani64.png" };
        static PageProperty nav_en = new PageProperty { title = new string[] { "娱乐", "Entertainment" }, url = "ms-appx:///Pic/User.png" };
        static PageProperty nav_ne = new PageProperty { title = new string[] { "新闻", "News" }, url = "ms-appx:///Pic/new64.png" };
        static PageProperty nav_sp = new PageProperty { title = new string[] { "体育", "Sports" }, url = "ms-appx:///Pic/spo64.png" };
        static PageProperty nav_li = new PageProperty { title = new string[] { "生活", "Living" }, url = "ms-appx:///Pic/User.png" };
        static PageProperty nav_ga = new PageProperty { title = new string[] { "游戏", "Game" }, url = "ms-appx:///Pic/gam64.png" };
        static PageProperty nav_do = new PageProperty { title = new string[] { "纪录片", "Documentary" }, url = "ms-appx:///Pic/doc64.png" };
        static PageProperty nav_fu = new PageProperty { title = new string[] { "搞笑", "Fun" }, url = "ms-appx:///Pic/fun64.png" };
        static PageProperty nav_au = new PageProperty { title = new string[] { "汽车", "Automotiven" }, url = "ms-appx:///Pic/car64.png" };
        static PageProperty[] pag_all = { nav_tv, nav_mo, nav_va, nav_an, nav_en, nav_ne, nav_sp, nav_li, nav_ga, nav_do, nav_fu, nav_au };
        #endregion

        struct Item
        {
            public Canvas can;
            public Image img;
            public TextBlock tb;
            public Button button;
        }
        static ScrollViewer sv;
        static Canvas con;
        static Thickness margin;
        static List<Item> li;
        static void CreateItems()
        {

            int c = nav_all.Length;
            for (int i = 0; i < c; i++)
            {
                if (nav_all[i].page != PageAddress.None)
                {
                    Item t = new Item();
                    Canvas can = new Canvas();
                    con.Children.Add(can);
                    t.can = can;
                    Image img = new Image();
                    t.img = img;
                    BitmapImage bi = new BitmapImage(new Uri("ms-appx:///Pic/User.png"));
                    img.Source = bi;
                    can.Children.Add(img);
                    TextBlock tb = new TextBlock();
                    tb.Foreground = nav_brush;
                    tb.FontSize = 18;
                    t.tb = tb;
                    tb.TextAlignment = TextAlignment.Center;
                    tb.Text = nav_all[i].title[language];
                    can.Children.Add(tb);
                    Button b = new Button();
                    b.Background = trans_brush;
                    can.Children.Add(b);
                    t.button = b;
                    b.DataContext = nav_all[i].page;
                    b.Click += (o, e) => {
                        PartialNav.LoadPage((PageAddress)(o as Button).DataContext);
                        PageManageEx.CreateNewPage(PageTag.partial);
                    };
                    li.Add(t);
                }
            }
        }
        static void CreateItems_m()
        {
            int c = (int)PageAddress_m.None;
            for (int i = 0; i < c; i++)
            {
                Item t = new Item();
                Canvas can = new Canvas();
                con.Children.Add(can);
                t.can = can;
                Image img = new Image();
                t.img = img;
                BitmapImage bi = new BitmapImage(new Uri(pag_all[i].url));
                img.Source = bi;
                can.Children.Add(img);
                TextBlock tb = new TextBlock();
                tb.Foreground = nav_brush;
                tb.FontSize = 18;
                t.tb = tb;
                tb.TextAlignment = TextAlignment.Center;
                tb.Text = pag_all[i].title[language];
                can.Children.Add(tb);
                Button b = new Button();
                b.Background = trans_brush;
                can.Children.Add(b);
                t.button = b;
                b.DataContext = i;
                b.Click += (o, e) => {
                    PageManageEx.CreateNewPage(PageTag.page_m);
                    PageNav_m.LoadPage((PageAddress_m)(o as Button).DataContext);
                };
                li.Add(t);
            }
        }
        public static void Create(Canvas p, Thickness m)
        {
            if (sv != null)
            {
                sv.Visibility = Visibility.Visible;
                Resize(m);
                return;
            }
            sv = new ScrollViewer();
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            p.Children.Add(sv);
            con = new Canvas();
            sv.Content = con;
            li = new List<Item>();
            CreateItems_m();
            CreateItems();
            Resize(m);
        }
        static void ResizeItems(double w)
        {
            double sw = w / 80;
            int c = (int)sw;
            sw = w / c;
            int len = li.Count;
            double oy = sw - 24;
            double dx = 0;
            double dy = 30;
            double ow = sw * 1.3f;
            for (int i = 0; i < len; i++)
            {
                if (dx + ow > w)
                {
                    dx = 0; dy += ow;
                }
                Item t = li[i];
                t.img.Width = sw - 20;
                t.img.Height = sw - 20;
                t.img.Margin = new Thickness(10, 0, 0, 0);
                t.can.Margin = new Thickness(dx, dy, 0, 0);
                t.tb.Margin = new Thickness(0, oy, 0, 0);
                t.tb.Width = sw;
                t.button.Width = sw;
                t.button.Height = sw;
                dx += ow;
            }
            dy += ow;
            con.Height = dy + 50;
        }
        public static void Resize(Thickness m)
        {
            if (margin == m)
                return;
            margin = m;
            double w = m.Right - m.Left;
            double h = m.Bottom - m.Top;
            sv.Width = w;
            sv.Height = h;
            con.Width = w;
            con.Height = h;
            ResizeItems(w);
        }
        public static void Hide()
        {

        }
        public static void Show()
        {

        }
    }

    class SportPage
    {
        //"http://sports.qq.com/nbavideo/"
        static string[] nav_sp0 = { "为你推荐", "Recommended for you" };
        static string[] nav_sp1 = { "英超", "Premier League" };//"http://sports.qq.com/video/yc/"
        static string[] nav_sp2 = { "欧冠", "Champions League" };//"http://sports.qq.com/video/og/"
        static string[] nav_sp3 = { "中超", "Super" };//"http://sports.qq.com/video/zcvideo/"
        static string[] nav_sp4 = { "拳力联盟", "Boxing Union" };
        static string[] nav_sp5 = { "综合体育", "Comprehensive sports" };
        static string[] nav_sp6 = { "功夫搏击", "Kung Fu fighting" };
        static string[] nav_sp7 = { "极限运动", "Extreme sport" };
        static string[] nav_sp8 = { "酷炫体坛", "Cool sports" };
        static string[] nav_sp9 = { "体育原创视频", "Sports Original Video" };
        static string[] nav_spa = { "赛程直播表", "Schedule live table" };
        static string[] nav_spb = { "赛程视频查询 ", "Schedule video query" };
        static string[][] nav_sp = { nav_sp0, nav_sp1, nav_sp2, nav_sp3, nav_sp4, nav_sp5, nav_sp6, nav_sp7, nav_sp8, nav_sp9 };
        static char[] Key_focus = "site_focus".ToCharArray();
        public void Create(Canvas p, Thickness m)
        {

        }
        public void Dispose()
        {

        }
        void LoadFocus()
        {

        }
        //http://v.qq.com/sports/
    }
}
