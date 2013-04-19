using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZTn.Tools.CommandLine.Helpers
{
    public class FunctionsChain : IFunction
    {
        String _functionName = String.Empty;
        List<IFunction> functions;

        int _errorCode = 0;
        TimeSpan _time;

        System.IO.Stream _stream = System.IO.Stream.Null;

        /// <inheritdoc/>
        public System.IO.Stream stream
        {
            get { return _stream; }
            set
            {
                _stream = value;
                foreach (IFunction pf in functions)
                {
                    pf.stream = value;
                }
            }
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
        public FunctionsChain(String functionName)
        {
            _functionName = functionName;
            functions = new List<IFunction>();
        }

        /// <inheritdoc/>
        public FunctionsChain addFunction(IFunction pf)
        {
            functions.Add(pf);
            return this;
        }

        /// <inheritdoc/>
        public IFunction execute()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(stream);
            sw.AutoFlush = true;

            _time = TimeSpan.Zero;
            sw.WriteLine("[Chain     ] " + _functionName + " starting");

            foreach (IFunction pf in functions)
            {
                pf.execute();
                _time += pf.time;
                _errorCode = pf.errorCode;
                if (_errorCode != 0) break;
            }
            sw.WriteLine("[Chain     ] " + _functionName + " ended with ErrorCode " + errorCode);

            return this;
        }

        /// <inheritdoc/>
        public IFunction setPathToExecutable(string path)
        {
            foreach (IFunction pf in functions)
            {
                pf.setPathToExecutable(path);
            }
            return this;
        }

        /// <inheritdoc/>
        public IFunction setParameters(string parameters)
        {
            throw new Exception("setParameters must not be called from a ExecutablesChain object");
        }

        /// <inheritdoc/>
        public IFunction setAlias(string alias, string value)
        {
            foreach (IFunction pf in functions)
            {
                pf.setAlias(alias, value);
            }
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
