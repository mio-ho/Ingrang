using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace UGDR
{
    class Suzip
    {
        private string[] nibu(string mizuki, string akari)
        {



            return mizuki.Split(new string[] { akari },StringSplitOptions.None);
        }
        private string asuka(string mizuki,Regex pattern,int option)
        {
            MatchCollection resultColl = pattern.Matches(mizuki);
            Match match = resultColl[0];
            return match.Groups[option].ToString();
        }
        private string[] Asuka(string mizuki,Regex pattern,int option)
        {
            MatchCollection resultColl = pattern.Matches(mizuki);
            string[] saito = new string[resultColl.Count];
            for(int i= 0; i< resultColl.Count;i++)
            {
                saito[i] = resultColl[0].Value;

            }
            return saito;
        }
        public void GetB(int num)
        {

            string url = "https://m.dcinside.com/board/japan_entertainment/" + num;


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 30 * 1000;
            request.Headers["Upgrade-Insecure-Requests"] = "1";
            request.Referer = "https://m.dcinside.com/board/japan_entertainment/";
            request.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_1_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1";

            string responseText = string.Empty;
            try
            {
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }
            }
            catch { }


            string timeY="";
            string timeM="";
            string timeD="";
            string timeH="";
            string timeMi="";
            string ipN="";
            string nickN="";
            string title="";
            string content = "";
            string C_cmt="";
            string idN="";
            string recomN="";
            string hantai="";

            try
            {
                title = asuka(responseText, new Regex(@"<title>\t(.*)-"),1);//제목
            }
            catch
            {
                title = null;
            }
            string kitano = asuka(responseText, new Regex(@"thum-txtin\"">(.*)<div class=\""reco-search\""",RegexOptions.Singleline),1);
            //내용 원본

            kitano = kitano.Replace("\n", ""); kitano = kitano.Replace("\t", ""); kitano = kitano.Replace("\r", "");
            try
            {
                string kitano2 = asuka(kitano, new Regex(@"</div></div><p></p><div><span style=(.*)</span></div></div></div>"), 0); //꼬린말컷
                kitano = kitano.Replace(kitano2, "");
            }
            catch { }
            Boolean hinako = false;
            foreach(char a in kitano)
            {
                if (a == '<') hinako = true;
                if (a == '>')
                {
                    hinako = false;
                    continue;
                }
                content += hinako == false ? a.ToString() : "";
            }
            try
            {
                string dog = asuka(content, new Regex(@" var (.*)}\);"), 0);
                content = content.Replace(dog, "");
            }
            catch { };

            recomN = asuka(responseText, new Regex(@"recomm_btn"">(.*)</span>"), 1);//추천수
            hantai = asuka(responseText, new Regex(@"nonrecomm_btn"">(.*)</span>"), 1);//비추
            
            try
            {
                idN = asuka(responseText, new Regex(@"dcinside.com/gallog/(.*)\"" class="), 1); //ID
            }
            catch
            {
                idN = "X";
            }
            try
            {
                timeY = asuka(responseText, new Regex(@"<li>20(.*)</li>"), 1);
                timeM = timeY.Substring(3, 2);
                timeD = timeY.Substring(6, 2);
                timeH = timeY.Substring(9, 2);
                timeMi = timeY.Substring(12, 2);
                timeY = "20" + timeY.Substring(0, 2);

            }
            catch { }
            try
            {//닉네임
                nickN = asuka(responseText, new Regex(@"ginfo2(.*)\t<li>20",RegexOptions.Singleline), 1);
                nickN = asuka(nickN, new Regex(@"<li>(.*)</li>",RegexOptions.Singleline), 1);
                try
                {
                    nickN = asuka(nickN, new Regex(@"(.*)<span", RegexOptions.Singleline), 1);
                }
                catch { }
                ipN = asuka(nickN,new Regex(@"\((.*)\)"),1);
                nickN = asuka(nickN, new Regex(@"(.*)\("),1);
            }
            catch 
            { }

            try
            {
                C_cmt = asuka(responseText, new Regex(@"<span class=""ct"">\[(.{0,5})\]</span><span class=""sp-reload"""), 1);
            }
            catch { }
            string sql = "INSERT OR REPLACE INTO Ranking VALUES(";
            sql += num + "," + Int32.Parse(timeY) +","+ Int32.Parse(timeM) + "," + Int32.Parse(timeD) + "," + Int32.Parse(timeH) + "," + Int32.Parse(timeMi) + ",\"" + ipN +"\",\"" + nickN + "\",\"" + title + "\",\"" + content + "\"," + Int32.Parse(C_cmt) + ",\"" + idN + "\"," + Int32.Parse(recomN) + "," + Int32.Parse(hantai) + ")";
            udb.save(sql);
            if (C_cmt == "0") return;


            string cmmenturl = "https://m.dcinside.com/ajax/response-comment";
            string[] messages = new string[6];
            messages[0] = "japan_entertainment";
            messages[1] =  num.ToString();
            messages[2] =  "1";
            messages[3] = "";
            messages[4] = "";
            messages[5] = "";
            string postData = string.Format("id={0}&no={1}&cpage={2}&managerskill={3}&del_scope={4}&csort={5}", messages[0], messages[1], messages[2], messages[3], messages[4], messages[5]);

            byte[] sendData = UTF8Encoding.UTF8.GetBytes(postData);


            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(cmmenturl);
            request2.Method = "POST";
            request2.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request2.Headers["X-Requested-With"] = "XMLHttpRequest";
            request2.Referer = url;
            request2.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_1_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1";

            request2.ContentLength = sendData.Length;
            using (Stream reqStream = request2.GetRequestStream())
            {
                reqStream.Write(sendData, 0, sendData.Length);
            }

            string responseText2 = string.Empty;
            try
            {
                using (WebResponse resp = request2.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText2 = sr.ReadToEnd();
                    }
                }
            }
            catch(Exception ex)
            { 
                MessageBox.Show(ex.ToString());
            }


            string[] c_id;
            c_id = nibu(responseText2,"\"nick\">");

            for(int bemiho = 1;bemiho <= Int32.Parse(C_cmt);bemiho++)
            {
                string cc_nick = asuka(c_id[bemiho].Substring(0, 20), new Regex("(.*)<span class"),1);
                string cc_content = asuka(c_id[bemiho], new Regex("<p class=\"txt\">(.*)</p>"), 1);
                string cc_date = asuka(c_id[bemiho], new Regex("<span class=\"date\">(.*)</span>  "), 1);
                string cc_id="X";
                string cc_ip="0";
                try
                {
                     cc_id = asuka(c_id[bemiho], new Regex("data-info=(.*) ></span>"), 1);
                }
                catch
                {
                    cc_ip = asuka(c_id[bemiho], new Regex("ip blockCommentIp\">(.*)</sapn>"), 1); //sapn?????????
                    cc_ip = cc_ip.Substring(1, cc_ip.Length - 2);
                }
                sql = "INSERT OR REPLACE INTO c_Ranking VALUES(";
                sql += num + ",\"" + nickN + "\",\"" + cc_nick + "\",\"" + cc_content + "\",\"" + cc_date + "\",\"" + cc_ip + "\",\"" + cc_id + "\")";
                udb.save(sql);

            }

        }
    }
}
