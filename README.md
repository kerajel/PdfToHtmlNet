# PdfToHtmlNet
A free opensource library for converting .pdf into .html in .net

Usage:
```
string sourcePdf = "path to the target .pdf";
string targetHtml = "path to the resulting html";
int pageID = 0; //id of the page to be converted. This parameter is 0 by default and targets all pages in the document
PdfToHtmlNet.Converter c = new PdfToHtmlNet.Converter();
c.Convert(sourcePdf, targetHtml, pageId);
```


The actual converting is done by a python executable which is deployed to your PC. You can specify the directory in which it would be stored before calling 'Convert' method:
```
c.ExecutableDirectory = "directory path";
```

If no directory is provided, then current user's temp folder is used. Python executable has all the required dependencies within itself, so there is no need to install any pyenvs.
Source code of the executable is provided in this repository aswell.
