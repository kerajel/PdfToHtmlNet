using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BaseDeployableNamespace;

namespace PdfToHtmlNet
{
    public class Converter : BaseDeployable
    {
        protected override string ExecutableExtenstion { get; set; } = ".exe";
        protected override string ExecutableName { get; set; } = "PdfToHtmlNet";
        protected override string ExecutableEmbeddedResourceName { get; set; } = "PdfToHtmlNet.Repository.PdfToHtmlNet.exe";

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public void Convert(string pdfPath, string htmlPath, int pageID = 0) => ProcessPipe(pdfPath, htmlPath, pageID);
        public void Convert(string pdfPath, string htmlPath, IEnumerable<int> pageIDs) => ProcessPipe(pdfPath, htmlPath, string.Join(',', pageIDs));

        private void ProcessPipe(string pdfPath, string htmlPath, object pageID)
        {
            InvokeExecutable(out string output, pdfPath, htmlPath, pageID, Encoding.BodyName);
            ParseExecutableOutput(output);
        }

        private static void ParseExecutableOutput(string output)
        {
            XDocument xdoc;
            try
            {
                xdoc = XDocument.Parse(output);
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
