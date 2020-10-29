from pdfminer.pdfinterp import PDFResourceManager, PDFPageInterpreter
from pdfminer.converter import HTMLConverter
from pdfminer.layout import LAParams
from pdfminer.pdfpage import PDFPage
from dicttoxml import dicttoxml
from io import BytesIO
import sys

def convert_pdf_to_html(pdf_path, html_path, page_id, encoding):
    page_found = False
    page_id = int(page_id)
    rsrcmgr = PDFResourceManager()
    retstr = BytesIO()
    laparams = LAParams()
    device = HTMLConverter(rsrcmgr, retstr, codec=encoding, laparams=laparams)
    with open(pdf_path, 'rb') as fp:
        interpreter = PDFPageInterpreter(rsrcmgr, device)
        for pageNumber, page in enumerate(PDFPage.get_pages(fp, set(),
                                                            maxpages=0,  # 0 is for all
                                                            password='',
                                                            caching=True,
                                                            check_extractable=True)):
            if page_id == 0:
                interpreter.process_page(page)
                page_found = True
            else:
                if pageNumber == page_id - 1:
                    interpreter.process_page(page)
                    page_found = True
    device.close()
    html_string = retstr.getvalue()
    retstr.close()
    if page_found == False:
        raise ValueError(f'Page {page_id} was not found')
    with open(html_path, "wb") as Html_file:
        Html_file.write(html_string)


try:
    log = {}
    pdf_path = sys.argv[1]
    html_path = sys.argv[2]
    page_id = sys.argv[3]
    encoding = sys.argv[4]
    convert_pdf_to_html(pdf_path, html_path, page_id, encoding)
    log["OperationStatus"] = "Completed"
    log["ErrorMessage"] = ""
except Exception as e:
    log["OperationStatus"] = "Faulted"
    log["ErrorMessage"] = str(e)
finally:
    print(dicttoxml(log).decode())
