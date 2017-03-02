using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZTn.Tools.CommandLine.Helpers
{
    public class GenericFunction : IFunction
    {
        private readonly string _executableName;
        private string _workingDirectory = ".";
        private string _pathToExecutable = string.Empty;
        private string _parameters = string.Empty;

        private readonly Dictionary<string, string> _aliases = new Dictionary<string, string>();

        /// <inheritdoc/>
        public System.IO.Stream Stream { get; set; } = System.IO.Stream.Null;

        /// <inheritdoc/>
        public int ErrorCode { get; private set; }

        /// <inheritdoc/>
        public TimeSpan Time { get; private set; } = TimeSpan.Zero;

        /// <inheritdoc/>
        public GenericFunction(string executableName)
        {
            _executableName = executableName;
        }

        /// <inheritdoc/>
        public IFunction Execute()
        {
            var sw = new System.IO.StreamWriter(Stream) { AutoFlush = true };

            var parameters = _parameters;
            foreach (var aliasName in _aliases.Keys)
            {
                parameters = Regex.Replace(parameters, aliasName, _aliases[aliasName]);
            }

            sw.WriteLine("[Generic   ] " + _executableName + " " + parameters);

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = _pathToExecutable + _executableName,
                Arguments = parameters,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = _workingDirectory
            };
            var p = new System.Diagnostics.Process { StartInfo = psi };

            p.Start();
            sw.WriteLine(p.StandardOutput.ReadToEnd());
            p.WaitForExit();

            Time = p.TotalProcessorTime;
            ErrorCode = p.ExitCode;

            sw.WriteLine("[Generic   ] " + _executableName + " ended with ErrorCode: " + ErrorCode + " after " + Time);

            return this;
        }

        /// <inheritdoc/>
        public IFunction SetPathToExecutable(string path)
        {
            _pathToExecutable = path;
            return this;
        }

        /// <inheritdoc/>
        public IFunction SetParameters(string parameters)
        {
            _parameters = parameters;
            return this;
        }

        /// <inheritdoc/>
        public IFunction SetAlias(string alias, string value)
        {
            _aliases.Add(alias, value);
            return this;
        }

        /// <inheritdoc/>
        public IFunction SetWorkingDirectory(string workingDirectory)
        {
            _workingDirectory = workingDirectory;
            return this;
        }

        /// <inheritdoc/>
        public IFunction SetStream(System.IO.Stream outputStream)
        {
            Stream = outputStream;
            return this;
        }
    }
}
