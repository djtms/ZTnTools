using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZTn.Tools.CommandLine.Helpers
{
    public class GenericFunction : IFunction
    {
        readonly string _executableName = string.Empty;
        private string _workingDirectory = ".";
        string _pathToExecutable = string.Empty;
        string _parameters = string.Empty;

        readonly Dictionary<string, string> _aliases = new Dictionary<string, string>();

        System.IO.Stream _stream = System.IO.Stream.Null;

        int _errorCode = 0;
        TimeSpan _time = TimeSpan.Zero;

        /// <inheritdoc/>
        public System.IO.Stream Stream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        /// <inheritdoc/>
        public int ErrorCode
        {
            get { return _errorCode; }
        }

        /// <inheritdoc/>
        public TimeSpan Time
        {
            get { return _time; }
        }

        /// <inheritdoc/>
        public GenericFunction(string executableName)
        {
            _executableName = executableName;
        }

        /// <inheritdoc/>
        public IFunction Execute()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(Stream);
            sw.AutoFlush = true;

            var parameters = _parameters;
            foreach (var aliasName in _aliases.Keys)
            {
                parameters = Regex.Replace(parameters, aliasName, _aliases[aliasName]);
            }

            sw.WriteLine("[Generic   ] " + _executableName + " " + parameters);

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = _pathToExecutable + _executableName,
                Arguments = parameters,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = _workingDirectory
            };
            System.Diagnostics.Process p = new System.Diagnostics.Process { StartInfo = psi };

            p.Start();
            sw.WriteLine(p.StandardOutput.ReadToEnd());
            p.WaitForExit();

            _time = p.TotalProcessorTime;
            _errorCode = p.ExitCode;

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
