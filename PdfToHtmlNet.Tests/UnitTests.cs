using System;
using System.IO;
using System.Text;
using HtmlAgilityPack;
using Xunit;
using System.Diagnostics;


namespace PdfToHtmlNet.Tests
{
    public class UnitTests
    {
        static string sourcePdf = @"..\..\..\TestData\0055-CPC-4.1.8.20.201-AK1.PL-0002_01_ER.pdf";
        static string targetHtml = @"..\..\..\TestData\0055-CPC-4.1.8.20.201-AK1.PL-0002_01_ER.html";

        [Fact]
        public void Test_1()
        {
            var pageID = new int[] { 2 };
            if (File.Exists(targetHtml)) File.Delete(targetHtml);
            PdfToHtmlNet.Converter c = new PdfToHtmlNet.Converter();
            c.Encoding = Encoding.UTF8;
            c.ConvertToFile(sourcePdf, targetHtml, pageID);
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
                if (excep.Message.Length > 0)
                    Debug.Print(excep.Message);
            }
            Assert.True(excep is null);
        }
    }
}