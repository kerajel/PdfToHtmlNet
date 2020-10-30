using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;

namespace BaseDeployableNamespace
{
    public abstract class BaseDeployable
    {
        protected bool isExecutableDeployed;
        protected string executableDirectory;
        protected virtual string ExecutableExtenstion { get; set; }
        protected virtual string ExecutableName { get; set; }
        protected virtual string ExecutableEmbeddedResourceName { get; set; }
        public string ExecutableDirectory
        {
            get => string.IsNullOrEmpty(executableDirectory) ? Path.Combine(Path.GetTempPath(), ExecutableName) : executableDirectory;
            set
            {
                executableDirectory = value;
                isExecutableDeployed = false;
            }
        }

        protected string ExecutableFullPath => Path.Combine(ExecutableDirectory, $"{ExecutableName}{ExecutableExtenstion}");
        protected static string BuildCommandLineArgs(params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object arg in args)
            {
                sb.Append("\"\"");
                sb.Append(arg.ToString().Replace("\"", "\\\""));
                sb.Append("\"\" ");
            }
            return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : string.Empty;
        }
        private static byte[] ExtractResource(string resourceName)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream(resourceName))
            {
                if (resFilestream == null) return null;
                byte[] bytea = new byte[resFilestream.Length];
                resFilestream.Read(bytea, 0, bytea.Length);
                return bytea;
            }
        }

        protected void DeployExecutable()
        {
            if (isExecutableDeployed)
                return;
            Directory.CreateDirectory(ExecutableDirectory);
            Debug.Print("PdfToHtmlNet executable dir:");
            Debug.Print(ExecutableDirectory);
            byte[] bytea = ExtractResource(ExecutableEmbeddedResourceName);
            Binary packageBinary = new Binary(bytea);
            if (File.Exists(ExecutableFullPath))
            {
                Binary existingBinary;
                using (FileStream fs = new FileStream(ExecutableFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    existingBinary = new Binary(ms.ToArray());
                }
                if (!packageBinary.Equals(existingBinary))
                    File.Delete(ExecutableFullPath);
            }
            if (!File.Exists(ExecutableFullPath))
                using (var fs = new FileStream(ExecutableFullPath, FileMode.CreateNew, FileAccess.Write))
                    fs.Write(packageBinary.ToArray(), 0, packageBinary.ToArray().Length);
            isExecutableDeployed = true;
        }

        protected void InvokeExecutable(out string output, params object[] args)
        {
            DeployExecutable();
            output = string.Empty;
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExecutableFullPath,
                    Arguments = BuildCommandLineArgs(args),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
                output = proc.StandardOutput.ReadToEnd();
        }
    }
}
