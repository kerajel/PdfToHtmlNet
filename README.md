# PdfToHtmlNet
Free opensource library for converting .PDF to .HTML in .NET

https://www.nuget.org/packages/PdfToHtmlNet/

Usage:
```
string sourcePdf = "path to the source .pdf";
string targetHtml = "path to the resulting html";
PdfToHtmlNet.Converter c = new PdfToHtmlNet.Converter();
c.ConvertToFile(sourcePdf, targetHtml);
```

By default all document pages are converted. If you want to specify a single page:
```
int pageID = 1;
c.ConvertToFile(sourcePdf, targetHtml, pageID);
```
If you want to specify multiple pages:
```
int[] pageIDs = new int[]{ 1, 3 };
c.ConvertToFile(sourcePdf, targetHtml, pageIDs);
```
Index of pages is 1-based.

It is possible to specify Converter's encoding:
```
c.Encoding = Encoding.UTF32;
```
UTF8 is used by default.

The actual converting is done by a python executable which is deployed to the executing machine. You can specify the directory in which it will be stored before calling the 'Convert' method:
```
c.ExecutableDirectory = "directory path";
```

If no directory is provided, then current user's temp folder is used. Python executable has all the required dependencies within itself, so there is no need to install any pyenvs.
Source code of the executable is provided in this repository aswell.

The library does not overwrite the executable each time it is called. Instead it verifies its binary content and updates it only if a newer version is present.
