#This builds executable from the PdfToHtmlPy.py
import subprocess
from pathlib import Path

pyScriptPath = str(Path.cwd()) + '\\PdfToHtmlPy.py'
outputDir = str(Path(f"..\\Executable").resolve())
cmd = f'pyinstaller --noconfirm --onefile --console "{pyScriptPath}" --distpath "{outputDir}"'
out = subprocess.Popen(cmd, 
           stdout=subprocess.PIPE, 
           stderr=subprocess.STDOUT)
stdout,stderr = out.communicate()
print(stdout.decode("utf-8"))
if (stderr != None):
    print(stderr.decode("utf-8"))
