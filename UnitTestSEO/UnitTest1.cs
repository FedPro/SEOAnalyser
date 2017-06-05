using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEO_Analyser;
using System.Net;
using System.IO;

namespace UnitTestSEO
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {            
            var pUrl = "http://www.sitecore.net/";
            var response = HtmlParserHelper.GetWebRequestContent(pUrl);
            HtmlParserHelper.GetMetaTagsContent(response.Content);
            HtmlParserHelper.ParseHtml(response.Content); 
        }
    }
}
