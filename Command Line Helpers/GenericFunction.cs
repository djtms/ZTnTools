using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ZTn.Tools.CommandLine.Helpers
{
    public class GenericFunction : IFunction
    {
        String _executableName = String.Empty;
        String _pathToExecutable = String.Empty;
        String _parameters = String.Empty;

        Dictionary<String, String> _aliases = new Dictionary<string, string>();

        System.IO.Stream _stream = System.IO.Stream.Null;

        int _errorCode = 0;
        TimeSpan _time = TimeSpan.Zero;

        /// <inheritdoc/>
        public System.IO.Stream stream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        /// <inheritdoc/>
        public int errorCode
        {
            get { return _errorCode; }
        }

        /// <inheritdoc/>
        public TimeSpan time
        {
            get { return _time; }
        }

        /// <inheritdoc/>
        public GenericFunction(String executableName)
        {
            this._executableName = executableName;
        }

        /// <inheritdoc/>
        public IFunction execute()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(stream);
            sw.AutoFlush = true;

            String parameters = _parameters;
            foreach (String aliasName in _aliases.Keys)
            {
                parameters = Regex.Replace(parameters, aliasName, _aliases[aliasName]);
            }

            sw.WriteLine("[Generic   ] " + _executableName + " " + parameters);

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = _pathToExecutable + _executableName;
            psi.Arguments = parameters;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;

            p.Start();
            sw.WriteLine(p.StandardOutput.ReadToEnd());
            p.WaitForExit();

            _time = p.TotalProcessorTime;
            _errorCode = p.ExitCode;

            sw.WriteLine("[Generic   ] " + _executableName + " ended with ErrorCode: " + errorCode + " after " + time);

            return this;
        }

        /// <inheritdoc/>
        public IFunction setPathToExecutable(String path)
        {
            this._pathToExecutable = path;
            return this;
        }

        /// <inheritdoc/>
        public IFunction setParameters(String parameters)
        {
            this._parameters = parameters;
            return this;
        }

        /// <inheritdoc/>
        public IFunction setAlias(String alias, String value)
        {
            _aliases.Add(alias, value);
            return this;
        }

        /// <inheritdoc/>
        public IFunction setStream(System.IO.Stream outputStream)
        {
            stream = outputStream;
            return this;
        }
    }
}
