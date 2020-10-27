using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseDeployableNamespace
{
    public abstract class BaseDeployable
    {
        protected bool isExecutableDeployed;
        protected string executableDirectory;
        protected virtual string ExecutableExtenstion { get; set; }
        protected virtual string ExecutableName { get; set; }
        protected virtual string ExecutableEmbeddedObjectName { get; set; }
        public string ExecutableDirectory
        {
            get => string.IsNullOrWhiteSpace(executableDirectory) ? Path.Combine(Path.GetTempPath(), ExecutableName) : executableDirectory;
            set
            {
                executableDirectory = value;
                isExecutableDeployed = false;
            }
        }
        private string ExecutableNameWithVersion => string.Concat(ExecutableName, "_", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        protected string ExecutableFullPath => Path.Combine(ExecutableDirectory, $"{ExecutableNameWithVersion}{ExecutableExtenstion}");
        protected static string BuildCommandLineArgs(params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object arg in args)
            {
                sb.Append("\"\"");
                sb.Append(arg.ToString().Replace("\"", "\\\""));
                sb.Append("\"\" ");
            }
            if (sb.Length > 0)
                sb = sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        protected static byte[] GetBytesFromEmbeddedObject(string objectName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            objectName = $"{assembly.GetName().Name}.{objectName}";
            using (Stream stream = assembly.GetManifestResourceStream(objectName))
            {
                if (stream == null)
                    return null;
                byte[] bytea = new byte[stream.Length];
                stream.Read(bytea, 0, bytea.Length);
                return bytea;
            }
        }

        protected void DeployExecutable()
        {
            if (isExecutableDeployed)
                return;
            Directory.CreateDirectory(ExecutableDirectory);
            Debug.Print("Executable folder:");
            Debug.Print(ExecutableDirectory);
            Directory.GetFiles(ExecutableDirectory)
                .Where(r => Regex.IsMatch(Path.GetFileName(r), $@"{ExecutableName}_\d\.\d\.\d\.\d\{ExecutableExtenstion}") & !(r == ExecutableFullPath))
                .ToList()
                .ForEach(r => File.Delete(r));
            byte[] bytea = GetBytesFromEmbeddedObject(ExecutableEmbeddedObjectName);
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

        protected void InvokeExecutable(out string log, params object[] args)
        {
            DeployExecutable();
            log = string.Empty;
            var proc = new Process
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
                log = proc.StandardOutput.ReadToEnd();
        }
    }
}
