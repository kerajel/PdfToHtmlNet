using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string sourcePdf = @"..\..\TestData\0055-CPC-4.1.8.20.201-AK1.PL-0002_01_ER.pdf";
            string targetHtml = @"..\..\TestData\0055-CPC-4.1.8.20.201-AK1.PL-0002_01_ER.html";
            int pageId = 0;
            if (File.Exists(targetHtml))
                File.Delete(targetHtml);
            PdfToHtmlNet.Converter c = new PdfToHtmlNet.Converter();
            c.Convert(sourcePdf, targetHtml, pageId);
            var htmlExists = File.Exists(targetHtml);
            Assert.IsTrue(htmlExists);
        }
    }
}
