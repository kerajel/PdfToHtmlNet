using System;
using System.Linq;
using System.Xml.Linq;
using BaseDeployableNamespace;

namespace PdfToHtmlNet
{
    public class Converter : BaseDeployable
    {
        protected override string ExecutableExtenstion { get; set; } = ".exe";
        protected override string ExecutableName { get; set; } = "PdfToHtmlNet";
        protected override string ExecutableEmbeddedResourceName { get; set; } = "PdfToHtmlNet.Repository.PdfToHtmlNet.exe";

        public void Convert(string pdfPath, string htmlPath, int pageId = 0)
        {
            InvokeExecutable(out string log, pdfPath, htmlPath, pageId);
            XDocument xdoc;
            try
            {
                xdoc = XDocument.Parse(log);
                var parsedCorrectly = xdoc.Root.Elements()
                    .All(r => r.Name == "OperationStatus" || r.Name == "ErrorMessage");
                if (!parsedCorrectly)
                    throw new Exception();
            }
            catch
            {
                throw new Exception("Failed to get response from the python executable");
            }
            if (xdoc.Root.Element("OperationStatus").Value == "Faulted")
            {
                string exceptionDetails = string.Concat("Python executable failed with exception:", Environment.NewLine, xdoc.Root.Element("ErrorMessage").Value);
                throw new Exception(exceptionDetails);
            }
        }
    }
}
