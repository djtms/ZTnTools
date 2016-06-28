using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZTn.Tools.CommandLine.Helpers
{
    public class FunctionsChain : IFunction
    {
        readonly string _functionName = string.Empty;
        readonly List<IFunction> _functions;

        int _errorCode = 0;
        TimeSpan _time;

        System.IO.Stream _stream = System.IO.Stream.Null;

        /// <inheritdoc/>
        public System.IO.Stream Stream
        {
            get { return _stream; }
            set
            {
                _stream = value;
                foreach (var pf in _functions)
                {
                    pf.Stream = value;
                }
            }
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
        public FunctionsChain(string functionName)
        {
            _functionName = functionName;
            _functions = new List<IFunction>();
        }

        /// <inheritdoc/>
        public FunctionsChain AddFunction(IFunction pf)
        {
            _functions.Add(pf);
            return this;
        }

        /// <inheritdoc/>
        public IFunction Execute()
        {
            var sw = new System.IO.StreamWriter(Stream) { AutoFlush = true };

            _time = TimeSpan.Zero;
            sw.WriteLine("[Chain     ] " + _functionName + " starting");

            foreach (var pf in _functions)
            {
                pf.Execute();
                _time += pf.Time;
                _errorCode = pf.ErrorCode;
                if (_errorCode != 0) break;
            }
            sw.WriteLine("[Chain     ] " + _functionName + " ended with ErrorCode " + ErrorCode);

            return this;
        }

        /// <inheritdoc/>
        public IFunction SetPathToExecutable(string path)
        {
            foreach (var pf in _functions)
            {
                pf.SetPathToExecutable(path);
            }
            return this;
        }

        /// <inheritdoc/>
        public IFunction SetParameters(string parameters)
        {
            throw new Exception("setParameters must not be called from a ExecutablesChain object");
        }

        /// <inheritdoc/>
        public IFunction SetAlias(string alias, string value)
        {
            foreach (var pf in _functions)
            {
                pf.SetAlias(alias, value);
            }
            return this;
        }

        /// <inheritdoc/>
        public IFunction SetStream(System.IO.Stream outputStream)
        {
            Stream = outputStream;
            return this;
        }

        public IFunction SetWorkingDirectory(string workingDirectory)
        {
            foreach (var pf in _functions)
            {
                pf.SetWorkingDirectory(workingDirectory);
            }
            return this;
        }
    }
}
