using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace TVWP.Class
{
    class CharOperation
    {
        #region function
        public static char[] CharInsertSpace(ref char[] source, int row,int count)
        {
            int c = source.Length;
            int l= c + row * count;
            char[] buff = new char[l];
            int t = 0;
            int s = 0;
            int r = 0;
            for(int i=0;i<c;i++)
            {
                if(r<count)
                {
                    if(t<row)
                    {
                        for(r=0;r<count;r++)
                        {
                            buff[s] = ' ';
                            s++;
                        }
                        t++;
                    }
                }
                if(source[i]=='\r')
                {
                    buff[s] = '\r';
                    s++;
                    buff[s] = '\n';
                    i ++;
                    r = 0;
                }else
                {
                    buff[s] = source[i];
                    s++;
                }
            }
            return buff;
        }
        public static char[] CharInsert(ref char[] source, char[] target, int index)
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
        public static int FindCharArray(ref char[] source, ref char[] content, int index)
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
        public static int FindCharArray(ref char[] source, ref char[] content, int start, int end)
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
        public static int FindCharArray(ref char[] source, ref char[] content, int start, char end)
        {
            for (int i = start; i < end; i++)
            {
                if (source[i] == end)
                    return -1;
                if (content[0] == source[i])
                {
                    int t = i;
                    t++;
                    for (int c = 1; c < content.Length; c++)
                    {
                        if (source[t] == end)
                            return -1;
                        if (content[c] != source[t])
                            goto label1;
                        t++;
                    }
                    return t;
                }
                label1:;
            }
            return -1;
        }
        public static char[] FindCharArray(ref char[] source, ref char[] sc, ref char[] ec, int index)
        {
            return FindCharArrayA(ref source, ref sc, ref ec, ref index);
        }
        public static int FindCharArray(ref char[] source, char c1, int index)
        {
            for (int i = index; i < source.Length; i++)
            {
                if (source[i] == c1)
                    return i;
            }
            return -1;
        }
        public static int FindCharArray(ref char[] source, char c1, char c2, int index)
        {
            for (int i = index; i < source.Length; i++)
            {
                if (source[i] == c1 | source[i] == c2)
                    return i;
            }
            return -1;
        }
        public static char[] FindCharArrayA(ref char[] source, char sc, char ec, ref int index)
        {
            //if (index < 0)
            //    index = 0;
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
                        if (l <= 0)
                        { index = i; return null; }
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
        public static char[] FindCharArrayA(ref char[] source, char sc, char ec, int start, int end)
        {
            //if (start < 0)
            //    start = 0;
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
                        if (l <= 0)
                            return null;
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
        public static char[] FindCharArrayA(ref char[] source, ref char[] sc, ref char[] ec, ref int index)
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
                        if (l <= 0)
                            return null;
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
        public static char[] CopyCharArry(ref char[] source, int index, int count)
        {
            char[] temp = new char[count];
            for (int i = 0; i < count; i++)
            {
                temp[i] = source[index];
                index++;
            }
            return temp;
        }
        public static int FallFindCharArray(ref char[] source, char sc, int index)
        {
            for (int i = index; i > 0; i--)
            {
                if (source[i] == sc)
                    return i;
            }
            return -1;
        }
        public static int FallFindCharArray(ref char[] source, ref char[] sc, int index)
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
        public static char[] FallFindCharArray(ref char[] source, char sc, char ec, ref int index)
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
        public static char[] FallFindCharArray(ref char[] source, ref char[] sc, ref char[] ec, int index)
        {
            return FallFindCharArrayA(ref source, ref sc, ref ec, ref index);
        }
        public static char[] FallFindCharArrayA(ref char[] source, ref char[] sc, ref char[] ec, ref int index)
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
        public static string GetString16(ref char[] source)
        {
            if (source == null)
                return "";
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
        public static string GetString16A(ref char[] source)
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
        public static char[] DeleteChar(ref char[] source, char c)
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
        public static char[] DeleteChar(ref char[] source, params char[] c)
        {
            int len = source.Length;
            int l = c.Length;
            char[] temp = new char[len];
            int s = 0;
            for (int i = 0; i < len; i++)
            {
                for (int t = 0; t < l; t++)
                {
                    if (source[i] == c[t])
                        goto label1;
                }
                temp[s] = source[i];
                s++;
                label1:;
            }
            char[] temp2 = new char[s];
            for (int i = 0; i < s; i++)
                temp2[i] = temp[i];
            return temp2;
        }
        public static int CharToInt(ref char[] source)
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
        public static int FindCharCount(ref char[] source, char sc, int s)
        {
            int count = 0;
            for (int i = s; i < source.Length; i++)
            {
                if (source[i] == sc)
                    count++;
            }
            return count;
        }
        public static int FindCharArrayCount(ref char[] source, ref char[] sc, int s)
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
        public static int FindCharArrayCount(ref char[] source, ref char[] sc, int s, int e)
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
        public static char[] CharWarp(char[] source,int count,out int row)
        {
            int len = source.Length;
            char[] buff = new char[len+256];
            int max = 0;
            int c = 0;
            row = 0;
            for(int i=0;i<len;i++)
            {
                if (source[i] == '\r')
                {
                    c = 0;
                    buff[max] = '\r';
                    max++;
                    buff[max] = '\n';
                    max++;
                    i += 2;
                    row++;
                }
                else
                {
                    buff[max] = source[i];
                    max++;
                    c++;
                    if(c>=count)
                    {
                        c = 0;
                        buff[max] = '\r';
                        max++;
                        buff[max] = '\n';
                        max++;
                        row++;
                    }
                }
            }
            char[] temp = new char[max];
            for (int i = 0; i < max; i++)
                temp[i] = buff[i];
            return temp;
        }
        public static char[] CharWarp(char[] source,int count,int row,int count2,out int allrow)
        {
            allrow = 0;
            int len = source.Length;
            char[] buff = new char[len + 256];
            int max = 0;
            int c = 0;
            int c2 = count;
            for (int i = 0; i < len; i++)
            {
                if (source[i] == '\r')
                {
                    c = 0;
                    buff[max] = '\r';
                    max++;
                    buff[max] = '\n';
                    max++;
                    i += 2;
                    allrow++;
                    if (allrow == row)
                        c2 = count2;
                }
                else
                {
                    buff[max] = source[i];
                    max++;
                    c++;
                    if (c >= c2)
                    {
                        c = 0;
                        buff[max] = '\r';
                        max++;
                        buff[max] = '\n';
                        max++;
                        allrow++;
                        if (allrow == row)
                            c2 = count2;
                    }
                }
            }
            char[] temp = new char[max];
            for (int i = 0; i < max; i++)
                temp[i] = buff[i];
            return temp;
        }
        public static char[] GetCharArray16A(ref char[] source)
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
            return temp2;
        }
        #endregion

        #region keyword
        public static char[] Key_interactionCount = "interactionCount".ToCharArray();
        public static char[] Key_datePublished = "datePublished".ToCharArray();
        public static char[] Key_varietyDate = "varietyDate".ToCharArray();
        public static char[] Key_title = "title=".ToCharArray();
        public static char[] Key_titleA = "title".ToCharArray();
        public static char[] Key_titleB = "\"title\"".ToCharArray();
        public static char[] Key_pic = "pic".ToCharArray();
        public static char[] Key_less = "<".ToCharArray();
        public static char[] Key_list_item = "list_item".ToCharArray();
        public static char[] Key_list_itemA = "\"list_item\"".ToCharArray();
        public static char[] Key_href = "href=".ToCharArray();
        public static char[] Key_quote = "\"".ToCharArray();
        public static char[] Key_src = "src=".ToCharArray();
        public static char[] Key_http = "http".ToCharArray();
        public static char[] Key_content = "content".ToCharArray();
        public static char[] Key_count = "Count".ToCharArray();
        public static char[] Key_data = "data".ToCharArray();
        public static char[] Key_date = "date".ToCharArray();
        public static char[] Key_img = "image".ToCharArray();
        public static char[] Key_x = ".com/x/".ToCharArray();
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
        public static char[] Key_fc = "\"fc\"".ToCharArray();
        public static char[] Key_fn = "\"fn\"".ToCharArray();
        public static char[] Key_key = "<key>".ToCharArray();
        public static char[] Key_fvkey = "\"fvkey\"".ToCharArray();
        public static char[] Key_fmt = "<fmt>".ToCharArray();
        public static char[] Key_name = "\"name\"".ToCharArray();
        public static char[] Key_filename = "<filename>".ToCharArray();
        public static char[] Key_dtc = "<dtc>".ToCharArray();
        public static char[] Key_coverinfo = "COVER_INFO".ToCharArray();
        public static char[] Key_videoinfo = "VIDEO_INFO".ToCharArray();
        public static char[] Key_listinfo = "LIST_INFO".ToCharArray();
        public static char[] Key_listinfoE = "}}}".ToCharArray();
        public static char[] Key_vid = "vid:".ToCharArray();
        public static char[] Key_vidA = "vid=".ToCharArray();
        public static char[] Key_vidB = "vid\"".ToCharArray();
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
        public static char[] Key_alt = "alt=".ToCharArray();
        public static char[] Key_mod_box_series = "mod_box_series".ToCharArray();
        public static char[] Key_mod_box_stage = "mod_box_stage".ToCharArray();
        public static char[] Key_mod_video_list = "mod_video_list".ToCharArray();
        public static char[] Key_mod_item = "mod_item\"".ToCharArray();
        public static char[] Key_a = "<a".ToCharArray();
        public static char[] Key_a_e = "</a".ToCharArray();
        public static char[] Key_em = "<em".ToCharArray();
        public static char[] Key_desc_text = "desc_text".ToCharArray();
        public static char[] Key_replace = "replace".ToCharArray();
        public static char[] Key_replaceA = "url.replace".ToCharArray();
        public static char[] Key_keyid = "keyid".ToCharArray();
        public static char[] Key_fs = "\"fs\"".ToCharArray();
        public static char[] Key_td = "\"td\"".ToCharArray();
        public static char[] Key_ti = "\"ti\"".ToCharArray();
        public static char[] Key_cmd5 = "\"cmd5\"".ToCharArray();
        public static char[] Key_info_inner = "info_inner".ToCharArray();
        public static char[] Key_list_item_hover = "list_item_hover".ToCharArray();
        public static char[] Key_figure_desc = "figure_desc".ToCharArray();
        public static char[] Key_lazyload = "lazyload=".ToCharArray();
        public static char[] Key_mod_filter_list = "mod_filter_list".ToCharArray();
        public static char[] Key_label = "label".ToCharArray();
        public static char[] Key_item_toggle = "item_toggle".ToCharArray();
        public static char[] Key_boss = "_boss".ToCharArray();
        public static char[] Key_index = "index".ToCharArray();
        public static char[] Key_type = "type".ToCharArray();
        public static char[] Key_li = "<li>".ToCharArray();
        public static char[] Key_qq = "qq.com".ToCharArray();
        public static char[] Key_preview = "preview\"".ToCharArray();
        public static char[] Key_ul_e = "</ul>".ToCharArray();
        public static char[] Key_split = "split".ToCharArray();
        public static char[] Key_site_container = "site_container".ToCharArray();
        public static char[] Key_c_over = "<!--".ToCharArray();
        #endregion

        #region Keyword
        public static char[] Key_weekline_title = "weekline_title".ToCharArray();
        public static char[] Key_curPlaysrc = "curPlaysrc".ToCharArray();
        public static char[] Key_datavid = "data-vid".ToCharArray();
        public static char[] Key_dispatch = "dispatch".ToCharArray();
        public static char[] Key_span = "<span".ToCharArray();
        public static char[] Key_span_e = "</span".ToCharArray();
        public static char[] Key_div = "<div".ToCharArray();
        public static char[] Key_div_e = "</div>".ToCharArray();
        public static char[] Key_p = "<p".ToCharArray();
        public static char[] Key_section = "<section".ToCharArray();
        public static char[] Key_section_e = "</section".ToCharArray();
        public static char[] Key_h2 = "<h2".ToCharArray();
        public static char[] Key_h2_e = "</h2".ToCharArray();
        public static char[] Key_mask = "mask_txt".ToCharArray();
        public static char[] Key_mark = "mark_v".ToCharArray();
        public static char[] Key_leaf_id = "leaf_id\"".ToCharArray();
        public static char[] Key_now = "now\"".ToCharArray();
        #endregion
    }
    class Component
    {
        #region  global set
        protected const double minX = 140;
        public const int presstime = 1800000;
        public const double pressoffset = 30;
        public static double PixRratio { get; set; }
        public static int language { get; set; }
        public static SolidColorBrush bk_brush { get; set; }
        public static SolidColorBrush font_brush { get; set; }
        public static SolidColorBrush title_brush { get; set; }
        public static SolidColorBrush trans_brush { get; set; }
        public static SolidColorBrush filter_brush { get; set; }
        public static SolidColorBrush nav_brush { get; set; }
        public static SolidColorBrush half_t_brush { get; set; }
        public static SolidColorBrush bor_brush { get; set; }
        public static SolidColorBrush warning_brush { get; set; }
        public static SolidColorBrush tag_brush_y { get; set; }
        public static SolidColorBrush tag_brush_g { get; set; }
        public static SolidColorBrush tag_brush_b { get; set; }
        public static double screenX, screenY;
        protected static double X { get; set; }
        protected static double Y { get; set; }
        protected static double OffsetX { get; set; }
        protected static double OffsetY { get; set; }

        public static void Initail()
        {
            language = Setting.language;
            bk_brush = new SolidColorBrush(Setting.bk_color);
            font_brush = new SolidColorBrush(Setting.font_color);
            title_brush = new SolidColorBrush(Setting.title_color);
            bor_brush = new SolidColorBrush(Setting.bor_color);
            filter_brush = new SolidColorBrush(Setting.filter_color);
            nav_brush = new SolidColorBrush(Setting.nav_color);

            half_t_brush = new SolidColorBrush(Color.FromArgb(64, 128, 128, 128));
            trans_brush =new SolidColorBrush(Colors.Transparent);
            warning_brush = new SolidColorBrush(Colors.Red);
            tag_brush_y = new SolidColorBrush(Colors.OrangeRed);
            tag_brush_g = new SolidColorBrush(Colors.DarkGreen);
            tag_brush_b = new SolidColorBrush(Color.FromArgb(128,0,0,0));
            InitialBuff();
        }
        public static void CreatePivot(ref PivotPage pp,ref string[][] s)
        {
            Pivot p = new Pivot();
            pp.pivot = p;
            int l = s.Length;
            pp.items = new PivotItem[l];
            pp.son = new Canvas[l];
            pp.head = new Button[l];
            for (int i = 0; i < l; i++)
            {
                Button b = new Button();
                b.Content= s[i][language];
                b.Foreground = nav_brush;
                b.Background = bk_brush;
                pp.head[i] = b;
                Canvas can = new Canvas();
                can.Background = half_t_brush;
                pp.son[i] = can;
                can.Margin = new Thickness(-10, 0, 0, 0);
                PivotItem pi = new PivotItem();
                pi.Header = b;
                pi.Content = can;
                pp.items[i] = pi;
                p.Items.Add(pi);
            }
            pp.index = -1;
        }
        public static void ResizePivot(ref PivotPage pp, Thickness m)
        {
            double h = m.Bottom - m.Top;
            Pivot p = pp.pivot;
            p.Margin = m;
            p.Width = screenX;
            p.Height = h;
            h -= 30;
            if (pp.items == null)
                return;
            int l = pp.items.Length;
            //double sw = screenX / l - 6;
            for (int i = 0; i < l; i++)
            {
                //pp.head[i].Width = sw;
                pp.son[i].Width = screenX;
                pp.son[i].Height = h;
            }
        }
        #endregion

        #region itemomd manage
        static ItemModE[] item_buff = new ItemModE[320];
        static int max;
        public static void ReCycleItemMod(int index)
        {
            item_buff[index].reg = false;
            item_buff[index].can.Visibility = Visibility.Collapsed;
        }
        public static void ClearItemMod(ItemModE im)
        {
            im.can.Children.Clear();
            GC.SuppressFinalize(im.button);
            GC.SuppressFinalize(im.img);
            GC.SuppressFinalize(im.title);
            //if (im.ico != null)
            //    GC.SuppressFinalize(im.ico);
            if (im.content != null)
                GC.SuppressFinalize(im.content);
            GC.SuppressFinalize(im.can);
        }
        public static ItemModE CreateItemModE()
        {
            ItemModE mod = new ItemModE();
            Canvas can = new Canvas();
            mod.can = can;
            Image img = new Image();
            can.Children.Add(img);
            mod.img = img;
            BitmapImage bi = new BitmapImage();
            img.Source = bi;
            TextBlock tb = new TextBlock();
            tb.FontSize = 18;
            tb.Foreground = title_brush;
            tb.Height = 50;
            can.Children.Add(tb);
            mod.title = tb;
            //tb = new TextBlock();
            //tb.Foreground = font_brush;
            tb.TextWrapping = TextWrapping.Wrap;
            //ca.Children.Add(tb);
            //mod.content = tb;
            Button b = new Button();
            b.Background = trans_brush;
            can.Children.Add(b);
            mod.button = b;
            return mod;
        }
        public static ItemModE CreateItemMod()
        {
            for(int i=0;i<max;i++)
            {
                if (!item_buff[i].reg)
                {
                    item_buff[i].reg = true;
                    item_buff[i].can.Visibility = Visibility.Visible;
                    return item_buff[i];
                }
            }
            ItemModE mod = CreateItemModE();
            mod.reg = true;
            mod.index = max;
            item_buff[max] = mod;
            max++;
            return mod;
        }
        protected static void CalculItemSize(ref ItemSize its, double w)
        {
            its.w = w;
            double sw = w / minX;
            int c = (int)sw;
            its.sw = sw = w / c;
            double sh = sw * 1.1f;
            its.sh = sh;
            its.iw = sw - 10;
            its.ih = its.iw;
            its.oy_t = its.ih * 0.8f;
            //its.oy_c = its.oy_t + 30;
            //its.ch = sh - its.oy_c;
        }
        #endregion

        #region conponent manage
        struct Com
        {
            public object obj;
            public bool reg;
        }
        struct Com_Buff
        {
            public Com[] btn;
            public Com[] img;
            public Com[] tbk;
            public Com[] bor;
        }
        static Com_Buff com;
        static int btn_max, img_max, tbk_max, bor_max;
        static int btn_pit, img_pit, tbk_pit, bor_pit;
        public static void InitialBuff()
        {
            com.btn = new Com[512];
            com.img = new Com[512];
            com.tbk = new Com[1024];
            com.bor = new Com[512];
            btn_max = img_max = tbk_max = bor_max = 0;
        }
#if desktop
        const int prebuff = 60;
#else
        const int prebuff = 20;
#endif
        public static void Buffcomponent()
        {
            for (int i = 0; i < prebuff; i++)
            { CreateNewButton(); com.btn[btn_max-1].reg = false; }
            for (int i = 0; i < prebuff; i++)
            { CreateNewBorder(); com.bor[bor_max - 1].reg = false; }
            for (int i = 0; i < prebuff; i++)
            { CreateNewImage(); com.img[img_max - 1].reg = false; }
            for (int i = 0; i < prebuff*2; i++)
            { CreateNewTextBlock(); com.tbk[tbk_max - 1].reg = false; }
        }
        static Button ReStoreButton()
        {
            Com[] cp = com.btn;
            while (btn_pit < btn_max)
            {
                if (!cp[btn_pit].reg)
                {
                    cp[btn_pit].reg = true;
                    return cp[btn_pit].obj as Button;
                }
                btn_pit++;
            }
            return null;
        }
        public static Button CreateNewButton()
        {
            Button btn = new Button();
            btn.Tag = btn_max;
            com.btn[btn_max].obj = btn;
            com.btn[btn_max].reg = true;
            btn_max++;
            btn.Click += (o, e) => {
                object obj = (o as Button).DataContext;
                if(obj!=null)
                {
                    ItemClick ic = (ItemClick)obj;
                    if(ic.click!=null &ic.tag!=null)
                      ic.click(ic.tag);
                }
            };
            return btn;
        }
        public static Button CreateButtonNext()
        {
            Button btn = ReStoreButton();
            if (btn != null)
            { btn.Visibility = Visibility.Visible; return btn; }
            return CreateNewButton();
        }
        public static void RecycleButton(Button btn)
        {
            btn.BorderBrush = null;
            btn.Foreground = null;
            btn.Background = null;
            btn.Height = 30;
            btn.Content = null;
            btn.Visibility = Visibility.Collapsed;
            btn.DataContext = null;
            int index =(int) btn.Tag;
            com.btn[index].reg = false;
            if (index < btn_pit)
                btn_pit = index;
        }

        static Image ReStoreImage()
        {
            Com[] cp = com.img;
            while (img_pit < img_max)
            {
                if (!cp[img_pit].reg)
                {
                    cp[img_pit].reg = true;
                    return cp[img_pit].obj as Image;
                }
                img_pit++;
            }
            return null;
        }
        public static Image CreateNewImage()
        {
            Image img = new Image();
            img.Source = new BitmapImage();
            img.Tag = img_max;
            com.img[img_max].obj = img;
            com.img[img_max].reg = true;
            img_max++;
            return img;
        }
        public static Image CreateImageNext()
        {
            Image img = ReStoreImage();
            if (img != null)
            { img.Visibility = Visibility.Visible; return img; }
            return CreateNewImage();
        }
        public static void RecycleImage(Image img)
        {
            img.Visibility = Visibility.Collapsed;
            int index = (int)img.Tag;
            com.img[index].reg = false;
            if (index < img_pit)
                img_pit = index;
            (img.Source as BitmapImage).UriSource = null;
        }

        static TextBlock ReStoreTextBlock()
        {
            Com[] cp = com.tbk;
            while (tbk_pit < tbk_max)
            {
                if (!cp[tbk_pit].reg)
                {
                    cp[tbk_pit].reg = true;
                    return cp[tbk_pit].obj as TextBlock;
                }
                tbk_pit++;
            }
            return null;
        }
        public static TextBlock CreateTextBlock()
        {
            tbk_pit = 0;
            return CreateTextBlockNext();
        }
        public static TextBlock CreateNewTextBlock()
        {
            TextBlock tbk = new TextBlock();
            tbk.Tag = tbk_max;
            com.tbk[tbk_max].obj = tbk;
            com.tbk[tbk_max].reg = true;
            tbk_max++;
            return tbk;
        }
        public static TextBlock CreateTextBlockNext()
        {
            TextBlock tbk = ReStoreTextBlock();
            if (tbk != null)
            { tbk.Visibility = Visibility.Visible; return tbk; }
            return CreateNewTextBlock();
        }
        public static void RecycleTextBlock(TextBlock tbk)
        {
            tbk.Visibility = Visibility.Collapsed;
            int index = (int)tbk.Tag;
            com.tbk[index].reg = false;
            if (index < tbk_pit)
                tbk_pit = index;
            tbk.Text = "";
        }

        static Border ReStoreBorder()
        {
            Com[] cp = com.bor;
            while (bor_pit < bor_max)
            {
                if (!cp[bor_pit].reg)
                {
                    cp[bor_pit].reg = true;
                    return cp[bor_pit].obj as Border;
                }
                bor_pit++;
            }
            return null;
        }
        public static Border CreateNewBorder()
        {
            Border bor = new Border();
            bor.Tag = bor_max;
            com.bor[bor_max].obj = bor;
            com.bor[bor_max].reg = true;
            bor_max++;
            return bor;
        }
        public static Border CreateBorderNext()
        {
            Border bor = ReStoreBorder();
            if (bor != null)
            { bor.Visibility = Visibility.Visible; return bor; }
            return CreateNewBorder();
        }
        public static void RecycleBorder(Border bor)
        {
            bor.Background = null;
            bor.BorderBrush = null;
            bor.Visibility = Visibility.Collapsed;
            int index = (int)bor.Tag;
            com.bor[index].reg = false;
            if (index < bor_pit)
                bor_pit = index;
        }
#endregion
    }
}