using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVWP.Class
{
    class ParseData:CharOperation
    {
        //public static void Analyze_Home(string data, int start, int end, int s ,ref Area[] area)
        //{
        //    char[] c_buff = data.ToCharArray();
        //    c_buff = DeleteChar(ref c_buff, '\\');
        //    int st = s;
        //    for (int i = start; i < end; i++)
        //    {
        //        char[] tmp = nav_all[i].title[0].ToCharArray();
        //        int ts = FindCharArray(ref c_buff, ref tmp, st);
        //        if (ts > 0)
        //        {
        //            ItemDataA[] icb = area[i].data;
        //            int l = nav_all[i].count;
        //            s = ts;
        //            for (int d = 0; d < l; d++)
        //            {
        //                s = FindCharArray(ref c_buff, ref Key_list_item, s);
        //                s = FindCharArray(ref c_buff, ref Key_href, s);
        //                char[] tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
        //                string aa;
        //                if (FindCharArray(ref tt, ref Key_http, 0) > -1)
        //                    aa = new string(tt);
        //                else aa = "http:" + new string(tt);
        //                icb[d].href = aa;

        //                s = FindCharArray(ref c_buff, ref Key_title, s);
        //                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
        //                icb[d].title = new string(tt);

        //                s = FindCharArray(ref c_buff, ref Key_src, s);
        //                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
        //                if (FindCharArray(ref tt, ref Key_about, 0) > -1)
        //                    break;
        //                if (FindCharArray(ref tt, ref Key_http, 0) > -1)
        //                    aa = new string(tt);
        //                else aa = "http:" + new string(tt);
        //                icb[d].src = aa;
        //            }
        //        }
        //    }
        //}
        public static bool Analyze_Nav(ref char[] c_buff, List<ItemDataB> lid, int count)
        {
            int s = FindCharArray(ref c_buff, ref Key_figures_list, 0);
            if (s < 0)
                return false;
            int i = 0, e;
            s = FindCharArray(ref c_buff, ref Key_list_item_hover, s);
            if (s > 0)
                for (i = 0; i < count; i++)
                {
                    e = FindCharArray(ref c_buff, ref Key_list_item_hover, s);
                    if (e < 0)
                    {
                        e = c_buff.Length;
                        lid.Add(GetItemData(ref c_buff, s, e));
                        i++;
                        break;
                    }
                    lid.Add(GetItemData(ref c_buff, s, e));
                    s = e;
                }
            return true;
        }
        public static bool Analyze_NavA(ref char[] t, ref char[] key, string bhref ,int count, List<ItemDataB> lid)
        {
            int s = FindCharArray(ref t, ref Key_mod_video_list, 0);
            if (s < 0)
                return false;
            s = FindCharArray(ref t, ref key, s);
            ItemDataB im = new ItemDataB();
            int i = 0;
            if (s > 0)
                for (i = 0; i < count; i++)
                {
                    s = FindCharArray(ref t, ref Key_href, s);
                    string href = bhref + new string(FindCharArrayA(ref t, '\"', '\"', ref s));
                    im.href = href;
                    s = FindCharArray(ref t, ref Key_title, s);
                    im.title = new string(FindCharArrayA(ref t, '\"', '\"', ref s));
                    s = FindCharArray(ref t, ref Key_src, s);
                    im.src = new string(FindCharArrayA(ref t, '\"', '\"', ref s));
                    im.detail = "";
                    s = FindCharArray(ref t, ref key, s);
                    lid.Add(im);
                    if (s < 0)
                    {
                        i++;
                        break;
                    }
                }
            return true;
        }
        static ItemDataB GetItemData(ref char[] c_buff, int s, int e)
        {
            ItemDataB im = new ItemDataB();
            s = FindCharArray(ref c_buff, ref Key_href, s);
            string str = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
            im.href = str;
            s = FindCharArray(ref c_buff, ref Key_lazyload, s);
            char[] t=FindCharArrayA(ref c_buff, '\"', '\"', ref s);
            if (FindCharArray(ref t, ref Key_http, 0) > -1)
                im.src = new string(t);
            else im.src = "http:" + new string(t);
            s = FindCharArray(ref c_buff, ref Key_alt, s);
            im.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));

            int ae = FindCharArray(ref c_buff,ref Key_a_e,s);
            int ss= FindCharArray(ref c_buff,ref Key_mask,s,ae);
            string mask="";
            if (ss > 0)
                mask = new string(FindCharArrayA(ref c_buff, '>', '<', ref ss))+"\r\n";
            ss = FindCharArray(ref c_buff, ref Key_mark, s, ae);
            if (ss > 0)
            {
                ss += 2;
                t = FindCharArrayA(ref c_buff, '\"', '\"', ref ss);
                if (FindCharArray(ref t, ref Key_http, 0) > -1)
                    im.mark = new string(t);
                else im.mark = "http:" + new string(t);
            }

            int d = FindCharArray(ref c_buff, ref Key_figure_desc, s, e);
            if (d > 0)
            {
                char[] tt = FindCharArrayA(ref c_buff, '>', '<', ref d);
                tt = DeleteChar(ref tt, (char)9, '\r', '\n');
                str = new string(tt);
                int end = FindCharArray(ref c_buff, ref Key_div_e, d);
                int o = d;
                for (int c = 0; c < 4; c++)
                {
                    o = FindCharArray(ref c_buff, ref Key_title, o, end);
                    if (o < 0)
                        break;
                    str += " " + new string(FindCharArrayA(ref c_buff, '\"', '\"', ref o));
                }
                s = d;
            }
            else str = "";
            d = FindCharArray(ref c_buff, ref Key_info_inner, s, e);
            if (d > 0)
                str = str + "\r\n播放数:" + new string(FindCharArrayA(ref c_buff, '>', '<', ref d));
            im.detail =mask+ str;
            return im;
        }
        public static void GetFilterData(ref char[] c_buff, ref FilterItemData[] fid)
        {
            int s = FindCharArray(ref c_buff, ref Key_mod_filter_list, 0);
            s = FindCharArray(ref c_buff, ref Key_item_toggle, s);
            int e = s;
            string str;
            List<ItemContext> ls;
            ItemContext ic = new ItemContext();
            int i = 0;
            if (s > 0)
                for (i = 0; i < 8; i++)
                {
                    e = FindCharArray(ref c_buff, ref Key_div_e, s);
                    if (fid[i].data == null)
                        fid[i].data = new List<ItemContext>();
                    ls = fid[i].data;
                    ls.Clear();
                    int c = s;
                    c = FindCharArray(ref c_buff, ref Key_label, c, e);
                    fid[i].tilte = new string(FindCharArrayA(ref c_buff, '>', '<', ref c));
                    for (int d = 0; d < 20; d++)
                    {
                        c = FindCharArray(ref c_buff, ref Key_boss, c, e);
                        if (c < 0)
                            break;
                        str = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref c));
                        c = FindCharArray(ref c_buff, ref Key_index, c, e);
                        str += "=" + new string(FindCharArrayA(ref c_buff, '\"', '\"', ref c));
                        ic.tag = str;
                        ic.text = new string(FindCharArrayA(ref c_buff, '>', '<', ref c));
                        ls.Add(ic);
                    }
                    s = FindCharArray(ref c_buff, ref Key_item_toggle, e);
                    if (s < 0)
                        break;
                }
            for (int c = i; c < 8; c++)
                fid[c].tilte = "";
        }
        public static void VideoCover(ref char[] c_buff,List<ItemDataA> eic,List<ItemDataA> lic,int start, int ec )
        {
            int e = FindCharArray(ref c_buff, ref Key_listinfoE, start);
            if (e < 0)
                e = c_buff.Length;
            eic.Clear();
            lic.Clear();
            int s = start;
            s = FindCharArray(ref c_buff, ref Key_data, s);
            s = FindCharArray(ref c_buff, ref Key_vidB, s, e);
            int index = ec;
            int count = 0;
            ItemDataA ic = new ItemDataA();
            while (s > 0)
            {
                ic.detail = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                s = FindCharArray(ref c_buff, ref Key_titleA, s);
                s++;
                ic.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                s = FindCharArray(ref c_buff, ref Key_preview, s);
                if (s < 0)
                    break;
                char[] t = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                if (FindCharArray(ref t, ref Key_http, 0) > -1)
                    ic.src = new string(t);
                else ic.src = "http:" + new string(t);
                if (count < index)
                    eic.Add(ic);
                else lic.Add(ic);
                s = FindCharArray(ref c_buff, ref Key_vidB, s, e);
                count++;
            }
        }
        public static void VideoPage(ref char[] c_buff,List<ItemDataA> eic,List<ItemDataA> lic)
        {
            int s = FindCharArray(ref c_buff, ref Key_mod_playlist, 0);
            if (s < 0)
                return;
            eic.Clear();
            lic.Clear();
            ItemDataA ic = new ItemDataA();
            s = FindCharArray(ref c_buff, ref Key_list_itemA, s);
            int e = FindCharArray(ref c_buff, ref Key_ul_e, s);
            int c;
            while (s > 0)
            {
                c = FindCharArray(ref c_buff, ref Key_href, s);
                char[] href = FindCharArrayA(ref c_buff, '\"', '\"', ref c);
                int a = FindCharArray(ref href, ref Key_vidA, 0);
                if (a > 0)
                    ic.detail = new string(CopyCharArry(ref href, a, href.Length - a));
                ic.href = "http://v.qq.com" + new string(href);
                c = FindCharArray(ref c_buff, ref Key_src, c);
                char[] t = FindCharArrayA(ref c_buff, '\"', '\"', ref c);
                if (FindCharArray(ref t, ref Key_http, 0) > -1)
                    ic.src = new string(t);
                else ic.src = "http:" + new string(t);
                c = FindCharArray(ref c_buff, ref Key_alt, c);
                ic.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref c));
                lic.Add(ic);
                s = FindCharArray(ref c_buff, ref Key_list_itemA, c);
                if (s > e)
                    break;
            }
        }

        static char[] Key_up = "up\"".ToCharArray();
        static char[] Key_rep = "orireplynum\"".ToCharArray();
        static char[] Key_poke = "poke\"".ToCharArray();
        static char[] Key_score = "score\"".ToCharArray();
        public static int AnalyComment(ref char[] c_buff,List<CommentInfo> lci)
        {
            int s = 0, ss = lci.Count;
            CommentInfo ci = new CommentInfo();
            int i, rid = ss;
            for (i = 0; i < 20; i++)
            {
                s = FindCharArray(ref c_buff, ref Key_js_id, s);
                if (s < 0)
                    break;
                ci.m_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                int r = FindCharArray(ref c_buff, ref Key_js_rootid, s, s + 20);
                if (r > 0)
                {
                    ci.m_r_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref r));
                    ci.rid = rid;
                    s = r;
                }
                else { ci.m_r_id = null; rid = i + ss; };
                s = FindCharArray(ref c_buff, ref Key_js_timeD, s);
                char[] tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.time = GetString16A(ref tt);
                s = FindCharArray(ref c_buff, ref Key_js_content, s);
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.content = GetString16(ref tt);

                s = FindCharArray(ref c_buff,ref Key_up,s);
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.approval = CharToInt(ref tt);
                string str ="("+ new string(tt)+")赞 (";
                s = FindCharArray(ref c_buff, ref Key_rep, s);
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.replay = CharToInt(ref tt);
                string rs = new string(tt);

                s = FindCharArray(ref c_buff, ref Key_js_userid, s);
                ci.u_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                s = FindCharArray(ref c_buff, ref Key_poke, s);
                tt = FindCharArrayA(ref c_buff, ':', ',', ref s);
                ci.against = CharToInt(ref tt);
                str += new string(tt) + ")反对 (" + rs + ")回复";
                ci.count = str;
                s = FindCharArray(ref c_buff, ref Key_js_nick, s);
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.nick = GetString16(ref tt);
                s = FindCharArray(ref c_buff, ref Key_js_head, s);
                int t = s;
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref t);
                if(tt!=null)
                   ci.url = new string(DeleteChar(ref tt, '\\'));
                s = FindCharArray(ref c_buff, ref Key_js_vip, s);
                ci.vip = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                s = FindCharArray(ref c_buff, ref Key_js_region, s);
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.region = GetString16(ref tt);
                lci.Add(ci);
            }
            return i;
        }
        public static int AnalyUpComment(ref char[] c_buff,List<UpCommentInfo> luc)
        {
            int s = 0, ss = luc.Count;
            UpCommentInfo ci = new UpCommentInfo();
            int i, rid = ss;
            for (i = 0; i < 10; i++)
            {
                s = FindCharArray(ref c_buff, ref Key_js_id, s);//id
                if (s < 0)
                    break;
                ci.m_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));
                int r = FindCharArray(ref c_buff, ref Key_js_rootid, s, s + 20);//rootid
                if (r > 0)
                {
                    ci.m_r_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref r));
                    ci.rid = rid;
                    s = r;
                }
                else { ci.m_r_id = null; rid = i + ss; };

                s = FindCharArray(ref c_buff, ref Key_js_userid, s);//userid
                ci.u_id = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref s));

                s = FindCharArray(ref c_buff, ref Key_up, s);//up
                char[] tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.approval = CharToInt(ref tt);
                string str = "(" + new string(tt) + ")赞 (";

                s = FindCharArray(ref c_buff, ref Key_poke, s);//poke
                tt = FindCharArrayA(ref c_buff, ':', ',', ref s);
                ci.against = CharToInt(ref tt);
                str += new string(tt) + ")反对 (";

                s = FindCharArray(ref c_buff, ref Key_rep, s);//rep
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.replay = CharToInt(ref tt);
                str += new string(tt) + ")回复";
                ci.count = str;

                s = FindCharArray(ref c_buff,ref Key_titleB,s);//title
                tt = FindCharArrayA(ref c_buff,'\"','\"',ref s);
                ci.title = GetString16A(ref tt);

                s = FindCharArray(ref c_buff,':',s);
                tt= FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                s = FindCharArray(ref c_buff, ref Key_content, s);//content
                ci.detail_s = new List<UpContent>();
                int o= GetUpCom_Content(ref c_buff,s,ci.detail_s);
                if (o > 0)
                    s = o;
                if(ci.detail_s.Count==0)
                {
                    UpContent u = new UpContent();
                    tt = GetCharArray16A(ref tt);
                    u.text = tt;
                    u.type = 't';
                    ci.detail_s.Add(u);
                }

                s = FindCharArray(ref c_buff, ref Key_js_timeD, s);//timeDifference
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.time = GetString16A(ref tt);
        
                s = FindCharArray(ref c_buff, ref Key_js_nick, s);//nick
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.nick = GetString16(ref tt);

                s = FindCharArray(ref c_buff, ref Key_js_head, s);//head
                int t = s;
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref t);
                if (tt != null)
                    ci.url = new string(DeleteChar(ref tt, '\\'));

                s = FindCharArray(ref c_buff, ref Key_js_region, s);//region
                tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                ci.region = GetString16(ref tt);

                s = FindCharArray(ref c_buff, ref Key_js_vip, s);//viptype
                ci.vip = new string(FindCharArrayA(ref c_buff, ':', ',', ref s));

                s = FindCharArray(ref c_buff,ref Key_score,s);//score
                tt = FindCharArrayA(ref c_buff,':',',',ref s);
                ci.score = CharToInt(ref tt);

                luc.Add(ci);
            }
            return i;
        }
        static char[] Key_p_e = "/p>\"".ToCharArray();
        static char[] Key_data_width = "data-width".ToCharArray();
        static int GetUpCom_Content(ref char[] c_buff, int s ,List<UpContent> luc)
        {
            int e = FindCharArray(ref c_buff,ref Key_p_e,s);
            UpContent uc = new UpContent();
            for(int i=0;i<40;i++)
            {
                char[] tt = FindCharArrayA(ref c_buff,'>','<',ref s);
                if (s > e - 10)
                    return e;
                if(tt!=null)
                {
                    uc.text= GetCharArray16A(ref tt);
                    uc.type = 't';
                    uc.content = null;
                    luc.Add(uc);
                }
                s++;
                if(c_buff[s]=='i')//<img
                {
                    tt=FindCharArrayA(ref c_buff,'\"','\"',ref s);
                    tt = DeleteChar(ref tt,'\\');
                    if (tt[0] != 'h')
                        uc.content = "http:" + new string(tt);
                    else uc.content = new string(tt);
                    uc.type = 'i';
                    s= FindCharArray(ref c_buff, ref Key_data_width, s);
                    tt= FindCharArrayA(ref c_buff,'\"','\"',ref s);
                    uc.width = CharToInt(ref tt);
                    s++;
                    tt = FindCharArrayA(ref c_buff, '\"', '\"', ref s);
                    uc.height = CharToInt(ref tt);
                    luc.Add(uc);
                }
            }
            return e;
        }

        static char[] Key_mod_video_info = "mod_video_info".ToCharArray();
        static char[] Key_video_tit = "video_tit".ToCharArray();
        static char[] Key_video_types = "video_types".ToCharArray();
        static char[] Key_intro_line = "intro_line".ToCharArray();
        static char[] Key_mod_sideslip_episodes = "mod_sideslip_episodes".ToCharArray();
        static char[] Key_liA = "<li".ToCharArray();
        static char[] Key_i = "<i".ToCharArray();
        static char[] Key_video = "'$video'".ToCharArray();
        static char[] Key_videos = "'$videos'".ToCharArray();
        static char[] Key_clips = "'$clips'".ToCharArray();
        static char[] Key_m_e = ");".ToCharArray();
        public static ImageContext Des_PlayPage(char[] c_buff,List<EP_Info> lep,List<ItemDataA> lid)
        {
            c_buff = DeleteChar(ref c_buff,'\\');
            ImageContext ic = new ImageContext();
            int s = FindCharArray(ref c_buff,ref Key_mod_video_info,0);
            if (s < 0)
                return ic;
            int e = FindCharArray(ref c_buff,ref Key_section_e,s);
            int t = FindCharArray(ref c_buff,ref Key_video_tit,s);
            string str="";
            if(t>0)
            {
                ic.title = new string(FindCharArrayA(ref c_buff,'>','<',ref t ))+"\r\n";
                s = t;
            }
            t = FindCharArray(ref c_buff, ref Key_video_types, s);
            if (t > 0)
            {
                str += new string(FindCharArrayA(ref c_buff, '>', '<', ref t))+ "\r\n";
                s = t;
            }
            for(int i=0;i<3;i++)
            {
                t = FindCharArray(ref c_buff, ref Key_intro_line, s);
                if (t > 0)
                {
                    str += new string(FindCharArrayA(ref c_buff, '>', '<', ref t))+ "\r\n";
                    s = t;
                }
                else break;
            }
            ic.detail = str;
            t = FindCharArray(ref c_buff,ref Key_mod_sideslip_episodes,s);
            if(t>0)
            {
                EP_Info ep = new EP_Info();
                e = FindCharArray(ref c_buff, ref Key_section_e, t);
                int c = 1;
                while(t>0)
                {
                    t = FindCharArray(ref c_buff,ref Key_li,s,e);
                    if (t < 0)
                        break;
                    t = FindCharArray(ref c_buff,ref Key_vidA,t);
                    ep.vid = new string(FindCharArrayA(ref c_buff,'\"','\"',ref t));
                    int le = FindCharArray(ref c_buff,ref Key_a_e,t);
                    t = FindCharArray(ref c_buff,ref Key_i,t,le);
                    if (t > 0)
                        ep.title =c.ToString()+ new string(FindCharArrayA(ref c_buff, '>', '<', ref t));
                    else ep.title = c.ToString();
                    c++;
                    s = le;
                    lep.Add(ep);
                }
            }
            t = FindCharArray(ref c_buff,ref Key_video,s);
            if(t>0)
            {
                t = FindCharArray(ref c_buff,ref Key_vidB,t);
                ic.vid = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                t = FindCharArray(ref c_buff,ref Key_pic,t);
                t = FindCharArray(ref c_buff,':',t);
                ic.src = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                s = t;
            }
            t = FindCharArray(ref c_buff,ref Key_clips,s);
            if(t>0)
            {
                ItemDataA ida = new ItemDataA();
                e = FindCharArray(ref c_buff,ref Key_m_e,t);
                while (t > 0)
                {
                    t = FindCharArray(ref c_buff, ref Key_vidB, s, e);
                    if (t < 0)
                        break;
                    if (e - s < 200)
                        break;
                    ida.detail = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                    t = FindCharArray(ref c_buff, ref Key_titleA, t);
                    t++;
                    ida.title = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                    t = FindCharArray(ref c_buff, ref Key_pic, t);
                    t = FindCharArray(ref c_buff, ':', t);
                    ida.src = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref t));
                    int le = FindCharArray(ref c_buff, '}', t);
                    s = le;
                    le = FindCharArray(ref c_buff, ref Key_duration, t, s);
                    if (le > 0)
                    {
                        le++; ida.href = new string(FindCharArrayA(ref c_buff, '\"', '\"', ref le));
                    }
                    else ida.href = null;
                    lid.Add(ida);
                }
            }
            return ic;
        }

        static void GetMark(ref char[] buff,int s,ref string[] tag)
        {
            int ec = FindCharArray(ref buff, ref Key_span_e, s);
            for (int i = 0; i < 3; i++)
            {
                s = FindCharArray(ref buff, '<', s);
                s++;
                char[] p = FindCharArrayA(ref buff, '\"', '\"', s, ec);
                if (p == null)
                    break;
                char[] c = FindCharArrayA(ref buff, '>', '<', s, ec);
                if (c != null)
                {
                    switch (p[2])
                    {
                        case 's'://mask
                            tag[2] = new string(c);
                            break;
                        case 'r'://mark
                            if (p[5] == 'i')
                                tag[1] = new string(c);
                            else tag[0] = new string(c);
                            break;
                    }
                }
            }
        }
        public static void AnalyzeData(char[] buff,ref Area_m[] a)
        {
            int s = 0;
            int e;
            int len = a.Length;
            for (int i = 0; i < len; i++)
            {
                s = FindCharArray(ref buff, ref Key_section, s);
                if (s < 0)
                    break;
                s = FindCharArray(ref buff, ref Key_h2_e, s);
                if (s > 0)
                {
                    e = FindCharArray(ref buff, ref Key_title, s);
                    if (e > 0)
                        a[i].alt = new string(FindCharArrayA(ref buff, '\"', '\"', ref e));
                }
                else break;
                e = FindCharArray(ref buff, ref Key_section_e, s);
                int t = s;
                int c = a[i].count;
                for (int o = 0; o < c; o++)
                {
                    t = FindCharArray(ref buff, ref Key_a, t, e);
                    if (t < 0)
                        break;
                    a[i].data[o] = AnalyeItem(ref buff, t);
                }
            }
        }
        static ItemData_m AnalyeItem(ref char[] buff, int s)
        {
            ItemData_m d = new ItemData_m();
            d.tag = new string[3];
            //d.tag_p = new char[3][];
            int t = FindCharArray(ref buff, ref Key_href, s);
            char[] c = FindCharArrayA(ref buff, '\"', '\"', ref t);
            if (c[0] != 'h')
                d.href = "http:" + new string(c);
            else d.href = new string(c);
            t = FindCharArray(ref buff, ref Key_src, t);
            c = FindCharArrayA(ref buff, '\"', '\"', ref t);
            if (c[0] != 'h')
                d.src = "http:" + new string(c);
            else d.src = new string(c);
            int ec = FindCharArray(ref buff, ref Key_span_e, t);
            for (int i = 0; i < 3; i++)
            {
                t = FindCharArray(ref buff, '<', t);
                t++;
                char[] p = FindCharArrayA(ref buff, '\"', '\"', t, ec);
                if (p == null)
                    break;
                c = FindCharArrayA(ref buff, '>', '<', t, ec);
                if (c != null)
                {
                    switch (p[2])
                    {
                        case 's'://mask
                            d.tag[2] = new string(c);
                            break;
                        case 'r'://mark
                            if (p[5] == 'i')
                                d.tag[1] = new string(c);
                            else d.tag[0] = new string(c);
                            break;
                    }
                }
            }
            t = FindCharArray(ref buff, ref Key_div, t);
            t = FindCharArray(ref buff, '<', t);
            t++;
            d.title = new string(FindCharArrayA(ref buff, '>', '<', ref t));
            t = FindCharArray(ref buff, ref Key_p, t);
            d.detail = new string(FindCharArrayA(ref buff, '>', '<', ref t));
            return d;
        }

        static char[] Key_posterPic = "posterPic\"".ToCharArray();
        static char[] Key_videoCategory = "videoCategory".ToCharArray();
        static char[] Key_publishDate = "publishDate".ToCharArray();
        static char[] Key_webPlayUrl = "webPlayUrl".ToCharArray();
        public static int Search_ex(char[] buff,List<ItemDataA> lia)
        {
            ItemDataA im = new ItemDataA();
            int s = 0 ;
            string detail;
            int i;
            for( i=0;i<15;i++)
            {
                s = FindCharArray(ref buff,ref Key_idA,s);
                if (s < 0)
                    break;
                detail = "";
                s = FindCharArray(ref buff,ref Key_posterPic,s);
                im.src = new string(FindCharArrayA(ref buff, '\"', '\"', ref s));
                detail +="播放量:" +new string(FindCharArrayA(ref buff,':',',',ref s));
                s = FindCharArray(ref buff,ref Key_duration,s);
                detail+="\r\n时长:"+ new string(FindCharArrayA(ref buff, ':', ',', ref s));
                s = FindCharArray(ref buff,ref Key_titleA,s);
                s++;
                char[] t = FindCharArrayA(ref buff, '\"', '\"', ref s);
                im.title = GetString16A(ref t);
                s = FindCharArray(ref buff,ref Key_videoCategory,s);
                s++;
                detail+="S\r\n分类:"+ new string(FindCharArrayA(ref buff, '\"', '\"', ref s));
                s = FindCharArray(ref buff ,':',s);
                t = FindCharArrayA(ref buff, '\"', '\"', ref s);
                if(t!=null)
                   detail += "\r\n区域:" + GetString16A(ref t);
                s = FindCharArray(ref buff, ':', s);
                detail += "\r\n" + new string(FindCharArrayA(ref buff, '\"', '\"', ref s));
                s = FindCharArray(ref buff,ref Key_webPlayUrl,s);
                s++;
                im.href = new string(FindCharArrayA(ref buff, '\"', '\"', ref s));
                s = FindCharArray(ref buff,ref Key_publishDate,s);
                s++;
                detail += "\r\n" + new string(FindCharArrayA(ref buff, '\"', '\"', ref s));
                im.detail = detail;
                lia.Add(im);
            }
            return i;
        }
    }
}
