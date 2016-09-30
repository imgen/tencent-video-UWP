using Windows.UI.Xaml.Media;

namespace TVWP.Class
{
    class Data
    {

        #region  global set
        public const int presstime = 1800000;
        public static long timenow { get; set; }
        public static int language { get; set; }
        protected static SolidColorBrush bk_brush { get; set; }
        protected static SolidColorBrush font_brush { get; set; }
        protected static double screenX, screenY;
        protected static double X { get; set; }
        protected static double Y { get; set; }
        protected static double OffsetX { get; set; }
        protected static double OffsetY { get; set; }
        #endregion

        #region char filter
        protected static char[] main_buff;
        protected static char[] CharInsert(ref char[] source, char[] target, int index)
        {
            int x = source.Length;
            int y = target.Length;
            int l = x + y;
            char[] temp = new char[l];
            for (int i = 0; i < index; i++)
                temp[i] = source[i];
            int s = index;
            for (int i = 0; i < y; i++)
            {
                temp[s] = target[i];
                s++;
            }
            for (int i = s; i < l; i++)
            {
                temp[i] = source[index];
                index++;
            }
            return temp;
        }
        protected static int FindCharArray(ref char[] content, int index)
        {
            if (index < 0)
                index = 0;
            for (int i = index; i < main_buff.Length; i++)
            {
                if (content[0] == main_buff[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < content.Length; c++)
                    {
                        if (content[c] != main_buff[t])
                            goto label1;
                        t++;
                        if (t >= main_buff.Length)
                            return -1;
                    }
                    return t;
                }
                label1:;
            }
            return -1;
        }
        protected static int FindCharArray(ref char[] source, ref char[] content, int index)
        {
            for (int i = index; i < source.Length; i++)
            {
                if (content[0] == source[i])
                {
                    int t = i;
                    t++;
                    if (t >= source.Length)
                        return -1;
                    for (int c = 1; c < content.Length; c++)
                    {
                        if (content[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= source.Length)
                            return -1;
                    }
                    return t;
                }
                label1:;
            }
            return -1;
        }
        protected static int FindCharArray(ref char[] source, ref char[] content, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                if (content[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < content.Length; c++)
                    {
                        if (content[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= end)
                            return -1;
                    }
                    return t;
                }
                label1:;
            }
            return -1;
        }
        protected static string FindString(ref char[] sc, ref char[] ec, ref int index)
        {
            if (index < 0)
                index = 0;
            char[] temp = sc;
            bool o = false;
            int s = 0;
            for (int i = index; i < main_buff.Length; i++)
            {
                if (temp[0] == main_buff[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < temp.Length; c++)
                    {
                        if (temp[c] != main_buff[t])
                            goto label1;
                        t++;
                        if (t >= main_buff.Length)
                            return null;
                    }
                    if (o)
                    {
                        int l = t - s - 1;
                        temp = new char[l];
                        for (int c = 0; c < l; c++)
                        { temp[c] = main_buff[s]; s++; }
                        index = t;
                        return new string(temp);
                    }
                    else { o = true; s = t; i = t; temp = ec; }
                }
                label1:;
            }
            return null;
        }
        protected static char[] FindCharArray(ref char[] source, ref char[] sc, ref char[] ec, int index)
        {
            return FindCharArrayA(ref source, ref sc, ref ec, ref index);
        }
        protected static int FindCharArray(ref char[] source, char c1, int index)
        {
            for (int i = index; i < source.Length; i++)
            {
                if (source[i] == c1)
                    return i;
            }
            return -1;
        }
        protected static int FindCharArray(ref char[] source, char c1, char c2, int index)
        {
            for (int i = index; i < source.Length; i++)
            {
                if (source[i] == c1 | source[i] == c2)
                    return i;
            }
            return -1;
        }
        protected static char[] FindCharArrayA(ref char[] source, char sc, char ec, ref int index)
        {
            if (index < 0)
                index = 0;
            char temp = sc;
            bool o = false;
            int s = 0;
            for (int i = index; i < source.Length; i++)
            {
                if (temp == source[i])
                {
                    if (o)
                    {
                        int l = i - s;
                        char[] cc = new char[l];
                        for (int c = 0; c < l; c++)
                        { cc[c] = source[s]; s++; }
                        index = i;
                        return cc;
                    }
                    else { o = true; s = i; s++; temp = ec; }
                }
            }
            return null;
        }
        protected static char[] FindCharArrayA(ref char[] source, char sc, char ec, int start, int end)
        {
            if (start < 0)
                start = 0;
            char temp = sc;
            bool o = false;
            int s = 0;
            for (int i = start; i < end; i++)
            {
                if (temp == source[i])
                {
                    if (o)
                    {
                        int l = i - s;
                        char[] cc = new char[l];
                        for (int c = 0; c < l; c++)
                        { cc[c] = source[s]; s++; }
                        return cc;
                    }
                    else { o = true; s = i; s++; temp = ec; }
                }
            }
            return null;
        }
        protected static char[] FindCharArrayA(ref char[] source, ref char[] sc, ref char[] ec, ref int index)
        {
            if (index < 0)
                index = 0;
            char[] temp = sc;
            bool o = false;
            int s = 0;
            for (int i = index; i < source.Length; i++)
            {
                if (temp[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < temp.Length; c++)
                    {
                        if (temp[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= source.Length)
                            return null;
                    }
                    if (o)
                    {
                        int l = t - s - 1;
                        temp = new char[l];
                        for (int c = 0; c < l; c++)
                        { temp[c] = source[s]; s++; }
                        index = t;
                        return temp;
                    }
                    else { o = true; s = t; i = t; temp = ec; }
                }
                label1:;
            }
            return null;
        }
        protected static char[] CopyCharArry(int index, int count)
        {
            char[] temp = new char[count];
            for (int i = 0; i < count; i++)
            {
                temp[i] = main_buff[index];
                index++;
            }
            return temp;
        }
        protected static char[] CopyCharArry(ref char[] source, int index, int count)
        {
            char[] temp = new char[count];
            for (int i = 0; i < count; i++)
            {
                temp[i] = source[index];
                index++;
            }
            return temp;
        }
        protected static int FallFindCharArray(ref char[] source, ref char[] sc, int index)
        {
            for (int i = index; i > 0; i--)
            {
                if (sc[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < sc.Length; c++)
                    {
                        if (sc[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= source.Length)
                            goto label1;
                    }
                    return i;
                }
                label1:;
            }
            return -1;
        }
        protected static char[] FallFindCharArray(ref char[] source, char sc, char ec, ref int index)
        {
            bool o = false;
            int end = 0;
            for (int i = index; i > 0; i--)
            {
                if (source[i] == ec)
                {
                    if (o)
                    {
                        i++;
                        int l = end - i;
                        char[] temp = new char[l];
                        for (int c = 0; c < l; c++)
                        {
                            temp[c] = source[i];
                            i++;
                        }
                        return temp;
                    }
                    else
                    {
                        end = i; o = true; ec = sc;
                    }
                }
            }
            return null;
        }
        protected static char[] FallFindCharArray(ref char[] source, ref char[] sc, ref char[] ec, int index)
        {
            return FallFindCharArrayA(ref source, ref sc, ref ec, ref index);
        }
        protected static char[] FallFindCharArrayA(ref char[] source, ref char[] sc, ref char[] ec, ref int index)
        {
            char[] temp = sc;
            bool o = false;
            int s = 0;
            for (int i = index; i > 0; i--)
            {
                if (temp[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < temp.Length; c++)
                    {
                        if (temp[c] != source[t])
                            goto label1;
                        t++;
                        if (i >= source.Length)
                            goto label1;
                    }
                    if (o)
                    {
                        int l = t - s - 1;
                        temp = new char[l];
                        for (int c = 0; c < l; c++)
                        { temp[c] = source[s]; s++; }
                        index = i;
                        return temp;
                    }
                    else { o = true; s = t; temp = ec; }
                }
                label1:;
            }
            return null;
        }
        protected static string GetString16(ref char[] source)
        {
            int len = source.Length;
            len /= 6;
            char[] temp = new char[len];
            int t = 2;
            for (int i = 0; i < len; i++)
            {
                int c = (int)source[t];
                if (c > 58)
                    c -= 87;
                else
                    c &= 15;
                c <<= 12;
                t++;
                int d = (int)source[t];
                if (d > 58)
                    d -= 87;
                else
                    d &= 15;
                d <<= 8;
                c |= d;
                t++;
                d = (int)source[t];
                if (d > 58)
                    d -= 87;
                else
                    d &= 15;
                d <<= 4;
                c |= d;
                t++;
                d = (int)source[t];
                if (d > 58)
                    d -= 87;
                else
                    d &= 15;
                c |= d;
                t++;
                t += 2;
                temp[i] = (char)c;
            }
            return new string(temp);
        }
        protected static string GetString16A(ref char[] source)
        {
            int len = source.Length;
            char[] temp = new char[len];
            int t = 0, s = 0;
            while (t < len)
            {
                if (source[t] == '\\')
                    t++;
                if (source[t] == 'u')
                {
                    t++;
                    int c = (int)source[t];
                    if (c > 58)
                        c -= 87;
                    else
                        c &= 15;
                    c <<= 12;
                    t++;
                    int d = (int)source[t];
                    if (d > 58)
                        d -= 87;
                    else
                        d &= 15;
                    d <<= 8;
                    c |= d;
                    t++;
                    d = (int)source[t];
                    if (d > 58)
                        d -= 87;
                    else
                        d &= 15;
                    d <<= 4;
                    c |= d;
                    t++;
                    d = (int)source[t];
                    if (d > 58)
                        d -= 87;
                    else
                        d &= 15;
                    c |= d;
                    t += 2;
                    temp[s] = (char)c;
                    s++;
                }
                else
                {
                    label0:;
                    temp[s] = source[t];
                    s++;
                    t++;
                    if (t >= len)
                        break;
                    if (source[t] != '\\')
                        goto label0;
                    t++;
                }
            }
            char[] temp2 = new char[s];
            for (int i = 0; i < s; i++)
                temp2[i] = temp[i];
            return new string(temp2);
        }
        protected static char[] DeleteChar(ref char[] source, char c)
        {
            int len = source.Length;
            char[] temp = new char[len];
            int s = 0;
            for (int i = 0; i < len; i++)
            {
                if (source[i] != c)
                {
                    temp[s] = source[i];
                    s++;
                }
            }
            char[] temp2 = new char[s];
            for (int i = 0; i < s; i++)
                temp2[i] = temp[i];
            return temp2;
        }
        protected static int CharToInt(ref char[] source)
        {
            int c;
            if (source.Length < 10)
                c = source.Length;
            else c = 10;
            int r = 0;
            for (int i = 0; i < c; i++)
            {
                int t = source[i];
                if (t > 57 || t < 48)
                    return r;
                t -= 48;
                r *= 10;
                r += t;
            }
            return r;
        }
        protected static int FindCharCount(ref char[] source, char sc, int s)
        {
            int count = 0;
            for (int i = s; i < source.Length; i++)
            {
                if (source[i] == sc)
                    count++;
            }
            return count;
        }
        protected static int FindCharArrayCount(ref char[] source, ref char[] sc, int s)
        {
            int count = 0;
            for (int i = s; i < source.Length; i++)
            {
                if (source[i] == sc[0])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < sc.Length; c++)
                    {
                        if (sc[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= source.Length)
                            return count;
                    }
                    count++;
                }
                label1:;
            }
            return count;
        }
        protected static int FindCharArrayCount(ref char[] source, ref char[] sc, int s, int e)
        {
            int count = 0;
            for (int i = s; i < e; i++)
            {
                if (source[i] == sc[0])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < sc.Length; c++)
                    {
                        if (sc[c] != source[t])
                            goto label1;
                        t++;
                        if (t >= e)
                            return count;
                    }
                    count++;
                }
                label1:;
            }
            return count;
        }
        #endregion

        #region search filter
        public static object[] SearchScope = new object[] {"分类", "电影", "电视剧", "综艺", "动漫","音乐", "纪录片", "其他",
        "原创","热享","拍客","新闻","娱乐","财经","体育","教育","16","游戏","18","19","母婴","汽车"};
        public static object[] SearchDate = new object[] { "最近", "天", "周", "月", "年" };
        public static object[] SearchTime = new object[] { "时长", "10分钟", "10-30分钟", "30-60分钟", "60分钟" };
        #endregion

        #region keyword
        public static char[] Key_interactionCount = "interactionCount".ToCharArray();
        public static char[] Key_datePublished = "datePublished".ToCharArray();
        public static char[] Key_varietyDate = "varietyDate".ToCharArray();
        public static char[] Key_title = "title=".ToCharArray();
        public static char[] Key_pic = "pic".ToCharArray();
        public static char[] Key_less = "<".ToCharArray();
        public static char[] Key_list_item = "list_item".ToCharArray();
        public static char[] Key_list_itemA = "\"list_item\"".ToCharArray();
        public static char[] Key_href = "href=".ToCharArray();
        public static char[] Key_quote = "\"".ToCharArray();
        public static char[] Key_src = "src=".ToCharArray();
        public static char[] Key_http = "http:".ToCharArray();
        public static char[] Key_content = "content".ToCharArray();
        public static char[] Key_count = "Count".ToCharArray();
        public static char[] Key_date = "date".ToCharArray();
        public static char[] Key_img = "image".ToCharArray();
        public static char[] Key_trans = "/x/".ToCharArray();
        public static char[] Key_com = ".com".ToCharArray();
        public static char[] Key_refresh = "refresh".ToCharArray();
        public static char[] Key_url = "url=".ToCharArray();
        public static char[] Key_urlA = "\"url\"".ToCharArray();
        public static char[] Key_quote_s = "'".ToCharArray();
        public static char[] Key_curvid = "curVid ==".ToCharArray();
        public static char[] Key_id = "id".ToCharArray();
        public static char[] Key_idA = "\"id\"".ToCharArray();
        public static char[] Key_left_brace = "(".ToCharArray();
        public static char[] Key_right_brace = ")".ToCharArray();
        public static char[] Key_P = "P)".ToCharArray();
        public static char[] Key_fn = "\"fn\"".ToCharArray();
        public static char[] Key_key = "<key>".ToCharArray();
        public static char[] Key_fvkey = "\"fvkey\"".ToCharArray();
        public static char[] Key_fmt = "<fmt>".ToCharArray();
        public static char[] Key_name = "\"name\"".ToCharArray();
        public static char[] Key_filename = "<filename>".ToCharArray();
        public static char[] Key_dtc = "<dtc>".ToCharArray();
        public static char[] Key_coverinfo = "COVER_INFO".ToCharArray();
        public static char[] Key_videoinfo = "VIDEO_INFO".ToCharArray();
        public static char[] Key_vid = "vid:".ToCharArray();
        public static char[] Key_slash = "//".ToCharArray();
        public static char[] Key_about = "about:".ToCharArray();
        public static char[] Key_comment_id = "comment_id\":".ToCharArray();
        public static char[] Key_return = "retnum\"".ToCharArray();
        public static char[] Key_fristid = "first\"".ToCharArray();
        public static char[] Key_lastid = "last\"".ToCharArray();
        public static char[] Key_js_id = "\"id\"".ToCharArray();
        public static char[] Key_js_rootid = "rootid\"".ToCharArray();
        public static char[] Key_js_content = "content\"".ToCharArray();
        public static char[] Key_js_time = "time\"".ToCharArray();
        public static char[] Key_js_timeD = "timeDifference\"".ToCharArray();
        public static char[] Key_js_userid = "userid\"".ToCharArray();
        public static char[] Key_js_useridA = "userid".ToCharArray();
        public static char[] Key_js_nick = "nick\"".ToCharArray();
        public static char[] Key_js_head = "head\"".ToCharArray();
        public static char[] Key_js_vip = "viptype\"".ToCharArray();
        public static char[] Key_js_region = "region\"".ToCharArray();
        public static char[] Key_titleA = "title".ToCharArray();
        public static char[] Key_equal = "=".ToCharArray();
        public static char[] Key_and = "&".ToCharArray();
        #endregion

        #region Keyword
        public static char[] Key_duration = "duration".ToCharArray();
        public static char[] Key_tl = "tl=".ToCharArray();
        public static char[] Key_description = "description".ToCharArray();
        public static char[] Key_mod_episode = "mod_episode".ToCharArray();
        public static char[] Key_mod_playlist = "mod_playlist".ToCharArray();
        public static char[] Key_result_item = "result_item".ToCharArray();
        public static char[] Key_player_figure = "player_figure".ToCharArray();
        public static char[] Key_playlist = "_playlist".ToCharArray();
        public static char[] Key_figures_list = "figures_list".ToCharArray();
        public static char[] Key_vidA = "vid=".ToCharArray();
        public static char[] Key_alt = "alt=".ToCharArray();
        public static char[] Key_mod_box_series = "mod_box_series".ToCharArray();
        public static char[] Key_mod_box_stage = "mod_box_stage".ToCharArray();
        public static char[] Key_mod_video_list = "mod_video_list".ToCharArray();
        public static char[] Key_mod_item = "mod_item\"".ToCharArray();
        public static char[] Key_a = "<a".ToCharArray();
        public static char[] Key_em = "<em".ToCharArray();
        public static char[] Key_desc_text = "desc_text".ToCharArray();
        public static char[] Key_replace = "replace".ToCharArray();
        public static char[] Key_replaceA = "url.replace".ToCharArray();
        public static char[] Key_keyid = "keyid".ToCharArray();
        public static char[] Key_fs = "\"fs\"".ToCharArray();
        public static char[] Key_td = "\"td\"".ToCharArray();
        public static char[] Key_cmd5 = "\"cmd5\"".ToCharArray();
        public static char[] Key_info_inner = "info_inner".ToCharArray();
        public static char[] Key_list_item_hover = "list_item_hover".ToCharArray();
        public static char[] Key_figure_desc = "figure_desc".ToCharArray();
        public static char[] Key_lazyload = "lazyload=".ToCharArray();
        public static char[] Key_mod_filter_list = "mod_filter_list".ToCharArray();
        public static char[] Key_label = "label".ToCharArray();
        public static char[] Key_item_toggle = "item_toggle".ToCharArray();
        public static char[] Key_div_e = "</div>".ToCharArray();
        public static char[] Key_boss= "_boss".ToCharArray();
        public static char[] Key_index = "index".ToCharArray();
        public static char[] Key_type = "type".ToCharArray();
        public static char[] Key_li = "<li>".ToCharArray();
        public static char[] Key_vqq = "v.qq.com/cover".ToCharArray();
        #endregion

        #region class
        static string[] movie = new string[] { "电影", "Movie" };
        static string[] tv = new string[] { "电视剧", "TV" };
        static string[] variety = new string[] { "综艺", "Variety" };
        static string[] animation = new string[] { "动漫", "Animation" };
        static string[] children = new string[] { "少儿", "Children" };
        static string[] mv = new string[] { "MV", "MV" };
        static string[] docoment = new string[] { "纪录片", "Docoment" };
        static string[] news = new string[] { "新闻", "News" };
        static string[] entertainment = new string[] { "娱乐", "Entertainment" };
        static string[] sports = new string[] { "体育", "Sports" };
        static string[] games = new string[] { "游戏", "Games" };
        static string[] fun = new string[] { "搞笑", "Fun" };
        static string[] cla = new string[] { "课程", "Class" };
        static string[] fashion = new string[] { "时尚", "Fashion" };
        static string[] other = new string[] { "其它","Other"};

        public static string[] type = new string[] {"分类","Type" };
        public static string[] sort = new string[] { "排序","Sort"};
        #endregion

        #region href
        static string movie_href = "http://v.qq.com/x/movielist/";// /?sort=4 &offset=20|40.....
        static string tv_href = "http://v.qq.com/x/teleplaylist/";
        static string variety_href = "http://v.qq.com/x/varietylist/";
        static string animation_href = "http://v.qq.com/x/cartoonlist/";
        static string children_href = "http://v.qq.com/x/childrenlist/";
        static string mv_href = "http://v.qq.com/x/musiclist/";
        static string docoment_href = "http://v.qq.com/x/documentarylist/";
        static string news_href = "http://v.qq.com/x/newslist/";
        static string entertainment_href = "http://v.qq.com/x/entlist/";
        static string sports_href = "http://v.qq.com/x/sportlist/";
        static string games_href = "http://v.qq.com/x/gamelist/";
        static string fun_href = "http://v.qq.com/x/funnylist/";

        static string cla_href = "http://v.qq.com/v/type/list_";//type_x_order_mod_page
        static string fashion_href = "http://v.qq.com/fashion/list/1/";// type/type/type_offset
        static string other_href = "http://v.qq.com/worldcuplist/21_-1_-1_-1_-1_-1_";//type_page_count

        protected static Nav_Data[] nav_data = new Nav_Data[] {new Nav_Data(movie,movie_href),new Nav_Data(tv,tv_href),
        new Nav_Data(variety,variety_href),new Nav_Data(animation,animation_href),new Nav_Data(children,children_href),
        new Nav_Data(mv,mv_href),new Nav_Data(docoment,docoment_href),new Nav_Data(news,news_href),
        new Nav_Data(entertainment,entertainment_href),new Nav_Data(sports,sports_href),new Nav_Data(games,games_href),
        new Nav_Data(fun,fun_href),new Nav_Data(cla,cla_href),new Nav_Data(fashion,fashion_href),new Nav_Data(other,other_href)};
        #endregion

        #region filter12
        static string[] f12_t_l = new string[] { "全部", "All" };
        static string[] f12_t_l_0 = new string[] { "历史" ,"History" };
        static string[] f12_t_l_1 = new string[] { "经济", "Economics" };
        static string[] f12_t_l_2 = new string[] { "生活", "living" };
        static string[] f12_t_l_3 = new string[] { "百科", "encyclopedia" };
        static string[] f12_t_l_4 = new string[] { "互联网", "Internet" };
        static string[] f12_t_l_5 = new string[] { "晃眼" , "glare" };
        static string[] f12_t_l_6 = new string[] { "下午茶", "tea" };
        const string f12_t_v = "9_-1_";
        const string f12_t_v_0 = "1_0_";
        const string f12_t_v_1 = "1_1_";
        const string f12_t_v_2 = "1_2_";
        const string f12_t_v_3 = "1_3_";
        const string f12_t_v_4 = "1_4_";
        const string f12_t_v_5 = "1_5_";
        const string f12_t_v_6 = "1_6_";
        static string[] f12_o_l = new string[] { "更新", "Update" };
        static string[] f12_o_l_0 = new string[] { "热度", "Hot" };
        static string[] f12_o_l_1 = new string[] { "评分", "Score" };
        const string f12_o_v = "0";
        const string f12_o_v_0 = "1";
        const string f12_o_v_1 = "2";
        protected static Nav_Data[] filter12 = new Nav_Data[] {new Nav_Data(f12_t_l,f12_t_v) ,new Nav_Data(f12_t_l_0,f12_t_v_0),
        new Nav_Data(f12_t_l_1,f12_t_v_1),new Nav_Data(f12_t_l_2,f12_t_v_2),new Nav_Data(f12_t_l_3,f12_t_v_3),
        new Nav_Data(f12_t_l_4,f12_t_v_4),new Nav_Data(f12_t_l_5,f12_t_v_5),new Nav_Data(f12_t_l_6,f12_t_v_6)};
        protected static Nav_Data[] filter12_o = new Nav_Data[] {new Nav_Data(f12_o_l,f12_o_v),new Nav_Data(f12_o_l_0,f12_o_v_0),
        new Nav_Data(f12_o_l_1,f12_o_v_1)};
        #endregion

        #region filter13
        static string[] f13_t_l_1 = new string[] {"新闻","News" };
        static string[] f13_t_l_2 = new string[] { "访谈", "Interview" };
        static string[] f13_t_l_3 = new string[] { "大片", "Large" };
        static string[] f13_t_l_4 = new string[] { "街拍", "Street beat" };
        static string[] f13_t_l_5 = new string[] { "奢侈品", "Luxury" };
        const string f13_t_v_1 = "221";
        const string f13_t_v_2 = "222";
        const string f13_t_v_3 =  "224" ;
        const string f13_t_v_4 = "225" ;
        const string f13_t_v_5 ="227";
        protected static Nav_Data[] filter13 = new Nav_Data[] { new Nav_Data(f12_t_l,f12_o_v),new Nav_Data(f13_t_l_1,f13_t_v_1),
        new Nav_Data(f13_t_l_2,f13_t_v_2),new Nav_Data(f13_t_l_3,f13_t_v_3),new Nav_Data(f13_t_l_4,f13_t_v_4),
        new Nav_Data(f13_t_l_5,f13_t_v_5)};
        protected static Nav_Data[] filter13_o = new Nav_Data[] {new Nav_Data(f12_o_l,f12_o_v_0),new Nav_Data(f12_o_l_0,f12_o_v_1)};
        #endregion

        #region sort filter
        public static string[] order_4 = new string[] { "热播", "Hot" };
        public static string[] order_5 = new string[] {"最新","new" };
        public static string[] order_6 = new string[] { "评分", "Score" };
        public static string[] order_value = new string[] {"?sort=4","?sort=5","?sort=6" };
        #endregion
    }
}