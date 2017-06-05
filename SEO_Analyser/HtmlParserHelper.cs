
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
//pck included => Html Agility Pack (HAP) v1.4.9
//ref => http://html-agility-pack.net/

namespace SEO_Analyser
{
    public class HtmlParserHelper
    {
        /// <summary>
        /// get the values of all meta tags
        /// </summary>
        /// <param name="pHtml">the parse of the html page</param>
        /// <returns>string of all meta tags values</returns>
        public static string GetMetaTagsContent(string pHtml)
        {
            StringBuilder myMetaContents = new StringBuilder();
            // reg express to get the hmtl meta tag
            Regex regMeta = new Regex(@"<meta\s*(?:(?:\b(\w|-)+\b\s*(?:=\s*(?:""[^""]*""|'" +
                                  @"[^']*'|[^""'<> ]+)\s*)?)*)/?\s*>",
                                  RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

            foreach (Match metaMtch in regMeta.Matches(pHtml))
            {
                //reg express to get extract the content from the meta tag
                Regex regSubMeta = new Regex(@"(?<name>\b(\w|-)+\b)\" +
                                                  @"s*=\s*(""(?<value>" +
                                                  @"[^""]*)""|'(?<value>[^']*)'" +
                                                  @"|(?<value>[^""'<> ]+)\s*)+",
                                                  RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

                foreach (Match submetaMtch in regSubMeta.Matches(metaMtch.Value.ToString()))
                {
                    if ("content" == submetaMtch.Groups["name"].ToString().ToLower())
                        myMetaContents.Append(submetaMtch.Groups["value"].ToString() + ";");
                }
            }
            return myMetaContents.ToString();
        }

        public static HtmlRequestContent GetWebRequestContent(string pUrl)
        {
            var reqContent = new HtmlRequestContent();            
            // Create a request for the URL             
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(pUrl);
            req.Timeout = 10000;
            HttpWebResponse response = null;
            try
            {
                // Get the response;
                response = (HttpWebResponse)req.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new System.IO.StreamReader(stream);
                reqContent.Content = sr.ReadToEnd();
            }
            catch (WebException e)
            {
                ////handle the error
                reqContent.Error = true;
                reqContent.Status = e.Status.ToString();
                reqContent.Message = e.Message;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            return reqContent;
        }

        public static string ParseHtml(string pHtml)
        {                        
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pHtml);

            var sb = new StringBuilder();
            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//text()[normalize-space(.) != '']"))
            {
                sb.AppendLine(node.InnerText.Trim());
            }

            return sb.ToString();
        }


    }

    public class HtmlRequestContent
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Content { get; set; }
        public bool Error { get; set; }
    }
}