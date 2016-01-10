using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Epson
{
    class Program
    {
        static HttpListener _listener = new HttpListener();
        static Printer printer = new Printer();

        static public void GetContextCallback(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);
            var request = context.Request;
            var response = context.Response;

            var sb = new StringBuilder();

            sb.Append("");
            sb.Append(string.Format("HttpMethod: {0}", request.HttpMethod));
            sb.Append(string.Format("Uri:        {0}", request.Url.AbsoluteUri));
            sb.Append(string.Format("LocalPath:  {0}", request.Url.LocalPath));
            foreach (string key in request.QueryString.Keys)
            {
                sb.Append(string.Format("Query:      {0} = {1}", key, request.QueryString[key]));
            }
            sb.Append("");
            var responseString = sb.ToString();
            var buffer = Encoding.UTF8.GetBytes(responseString);

            response.ContentLength64 = buffer.Length;
            response.AddHeader("Access-Control-Allow-Origin", "*");

            Print(request.QueryString.Get("data"));

            using (System.IO.Stream outputStream = response.OutputStream)
            {
                outputStream.Write(buffer, 0, buffer.Length);
            }
            _listener.BeginGetContext(new AsyncCallback(GetContextCallback), null);

        }

        static void Print(string query)
        {
            var tango = false;
            try {
                Console.WriteLine("print!" + query);
                var buyList = new List<Quiz>();
                //buyList.Add(new Quiz("Domain Name Service", "53", "50"));
                //buyList.Add(new Quiz("Domain Name Service", "53", "53"));
                buyList.AddRange(query.Split('/')
                    .Select(s => s.Split(','))
                    .Select(s => new Quiz(s[0], s[1], s[2])));
                printer.TransactionStart();
                printer.PrintBitmap("./tutui.bmp");
                printer.Scroll(500);
                printer.PrintRow(("お疲れ様でした " + DateTime.Now.ToLocalTime()).AlignCenter());
                printer.PrintRow("関西オープンフォーラム2015".AlignCenter());
                printer.Scroll(1000);
                buyList.ForEach(t =>
                {
                    if (tango)
                    {
                        printer.PrintRow(t.title.AlignCenter().BoldStr());
                        printer.Scroll(1000);
                        printer.PrintRow(printer.RowRepeatStr("-"));
                        printer.Scroll(1000);
                        printer.PrintRow(printer.MakePrintString(t.isOk ? "正解！　" : "不正解　回答:" + t.ans, "正答:" + t.truth));
                        printer.Scroll(1000);
                        printer.ScrollCutPosition();
                        printer.CutPaper();
                    }
                    else
                    {
                        printer.PrintRow(t.title);
                        printer.PrintRow(printer.MakePrintString(t.isOk ? "正解！　" : "不正解　回答:" + t.ans, "正答:" + t.truth));
                        printer.Scroll(500);
                    }
                });
                printer.PrintRow(printer.RowRepeatStr("-"));
                printer.PrintRow(printer.MakePrintString("合計", buyList.Where(s => s.isOk).Count() + "問正解"));
                printer.Scroll(500);
                printer.ScrollCutPosition();
                printer.CutPaper();
                printer.TransactionSubmit();

            } catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        static void Main(string[] args)
        {

            _listener.Prefixes.Add("http://*:1234/");
            _listener.Start();
            Console.WriteLine("Listening, hit enter to stop");
            _listener.BeginGetContext(new AsyncCallback(GetContextCallback), null);
            Console.ReadLine();
            _listener.Stop();

        }
    }

    class Quiz
    {
        public string title;
        public string truth;
        public string ans;
        public bool isOk { get { return truth == ans; } }
        public Quiz(string title, string truth, string ans)
        {
            this.title = title;
            this.truth = truth;
            this.ans = ans;
        }
    }

    static class Extention
    {
        static public string BoldStr(this string str)
        {
            return "\u001b|bC" + str;
        }
        /// <summary>
        /// 横長の文字を得ます
        /// </summary>
        /// <param name="type">1～4の程度を指定する数字</param>
        static public string LongStr(this string str, int type)
        {
            switch (type)
            {
                case 1:
                    return "\u001b|1hC" + str;
                default:
                case 2:
                    return "\u001b|2hC" + str;
                case 3:
                    return "\u001b|3hC" + str;
                case 4:
                    return "\u001b|4hC" + str;
            }
        }
        /// <summary>
        /// 縦長の文字を得ます
        /// </summary>
        /// <param name="type">1～4の程度を指定する数字</param>
        static public string TallStr(this string str, int type)
        {
            switch (type)
            {
                case 1:
                    return "\u001b|1vC" + str;
                default:
                case 2:
                    return "\u001b|2vC" + str;
                case 3:
                    return "\u001b|3vC" + str;
                case 4:
                    return "\u001b|4vC" + str;
            }
        }
        static public string RepeatStr(this string parts, int lineChars)
        {
            string buff = "";
            int i = 0;

            List<char> charList = parts.ToCharArray().ToList();
            bool flag = true;

            while (flag)
            {
                charList.ForEach(delegate (char c)
                {
                    if (i < lineChars)
                    {
                        buff += c.ToString();
                        i++;
                    }
                    else
                    {
                        flag = false;
                    }
                });
            }
            return buff;
        }

        static public string AlignCenter(this string str)
        {
            return "\u001b|cA" + str;
        }
        static public string AlignRight(this string str)
        {
            return "\u001b|rA" + str;
        }
        static public string AlignLeft(this string str)
        {
            return "\u001b|lA" + str;
        }

    }
}