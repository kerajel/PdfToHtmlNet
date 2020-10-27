# PdfToHtmlNet
A free opensource library for converting .PDF into .HTML in .NET

Usage:
```
string sourcePdf = "path to the source .pdf";
string targetHtml = "path to the resulting html";
int pageID = 0; //id of the page to be converted. This parameter is 0 by default which targets all pages in the document
PdfToHtmlNet.Converter c = new PdfToHtmlNet.Converter();
c.Convert(sourcePdf, targetHtml, pageId);
```


The actual converting is done by a python executable which is deployed to your PC. You can specify the directory in which it would be stored before calling the 'Convert' method:
```
c.ExecutableDirectory = "directory path";
```

If no directory is provided, then current user's temp folder is used. Python executable has all the required dependencies within itself, so there is no need to install any pyenvs.
Source code of the executable is provided in this repository aswell.

The library does not overwrite the executable each time it is called. Instead it verifies its binary content and updates it only if a newer version is present.
