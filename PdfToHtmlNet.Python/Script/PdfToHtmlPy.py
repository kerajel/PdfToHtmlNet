from pdfminer.pdfinterp import PDFResourceManager, PDFPageInterpreter
from pdfminer.converter import HTMLConverter
from pdfminer.layout import LAParams
from pdfminer.pdfpage import PDFPage
from dicttoxml import dicttoxml
from io import BytesIO
import sys

# TestData
#sys.argv = [None] * 5
#sys.argv[1] = r"../../PdfToHtmlNet.Tests/TestData/0055-CPC-4.1.8.20.201-AK1.PL-0002_01_ER.pdf"
#sys.argv[2] = r"../../PdfToHtmlNet.Tests/TestData/0055-CPC-4.1.8.20.201-AK1.PL-0002_01_ER.html"
#sys.argv[3] = ""
#sys.argv[4] = "utf-8"

try:
    log = {}
    pdf_path = sys.argv[1]
    html_path = sys.argv[2]
    page_id = "0" if sys.argv[3] == "" else sys.argv[3]
    encoding = sys.argv[4]
    pdf_page_IDs = []
    pdf_pages = []
    target_page_IDs = [int(x) for x in page_id.split(',')]
    rsrcmgr = PDFResourceManager()
    retstr = BytesIO()
    laparams = LAParams()
    device = HTMLConverter(rsrcmgr, retstr, codec=encoding, laparams=laparams)
    with open(pdf_path, "rb") as fp:
        interpreter = PDFPageInterpreter(rsrcmgr, device)
        for pdf_page_ID, pdf_page in enumerate(PDFPage.get_pages(fp, set(), maxpages=0, password='', caching=True, check_extractable=True)):
            pdf_page_IDs.append(pdf_page_ID)
            pdf_pages.append(pdf_page)
        if len(target_page_IDs) == 1 and target_page_IDs[0] == 0:
            target_page_IDs = [x + 1 for x in pdf_page_IDs]
        missing_pdf_page_IDs = [
            x for x in target_page_IDs if x - 1 not in pdf_page_IDs]
        if len(missing_pdf_page_IDs) > 0:
            raise ValueError(
                f"Following pages were not found in '{pdf_path}': {','.join(map(str, missing_pdf_page_IDs))}")
        for target_page_ID in target_page_IDs:
            page_idx = pdf_page_IDs.index(target_page_ID - 1)
            interpreter.process_page(pdf_pages[page_idx])
        device.close()
        html_string = retstr.getvalue()
        retstr.close()
    with open(html_path, "wb") as html_file:
        html_file.write(html_string)
    log["OperationStatus"] = "Completed"
    log["ErrorMessage"] = ""
except Exception as e:
    log["OperationStatus"] = "Faulted"
    log["ErrorMessage"] = str(e)
finally:
    print(dicttoxml(log).decode())
