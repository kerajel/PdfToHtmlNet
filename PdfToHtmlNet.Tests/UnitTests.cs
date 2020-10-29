using System;
using System.IO;
using System.Xml.Linq;
using HtmlAgilityPack;
using Xunit;


namespace PdfToHtmlNet.Tests
{
    public class UnitTests
    {
        [Fact]
        public void Test_1()
        {
            string sourcePdf = @"..\..\..\TestData\0055-CPC-4.1.8.20.201-AK1.PL-0002_01_ER.pdf";
            string targetHtml = @"..\..\..\TestData\0055-CPC-4.1.8.20.201-AK1.PL-0002_01_ER.html";
            int pageId = 0;
            if (File.Exists(targetHtml))
                File.Delete(targetHtml);
            PdfToHtmlNet.Converter c = new PdfToHtmlNet.Converter();
            c.Convert(sourcePdf, targetHtml, pageId);
            Exception excep = null;
            try
            {
                string html = File.ReadAllText(targetHtml);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
            }
            catch (Exception ex)
            {
                excep = ex;
            }
            Assert.True(excep is null);
        }
    }
}
