using System;
using System.IO;
using System.Threading;

namespace ZTn.Tools.Stream.Helpers
{
    /// <summary>
    /// Defines a simple stream that can be concurrently written and read.
    /// </summary>
    /// <example>
    /// using (var rws = new AsyncReadWriteStream())
    ///	{
    ///    Task.Run(() =>
    ///	   {
    ///       var rs = new StreamReader(rws);
    ///	      while (!rs.EndOfStream)
    ///	      {
    ///	         rs.ReadLine().Dump();
    ///       }
    ///       rws.FinalizeWritings();
    ///	   });
    ///	   Task.Run(() =>
    ///	   {
    ///       var ws = new StreamWriter(rws);
    ///	      for (int i = 0; i &lt; 5; i++)
    ///	      {
    ///	         ws.WriteLine($"i is {i}");
    ///	      }
    ///	      ws.Flush();
    ///	   });
    ///	   Console.ReadLine();
    ///	}
    /// </example>
    public class AsyncReadWriteStream : System.IO.Stream
    {
        private MemoryStream _innerStream = new MemoryStream();
        private readonly ManualResetEventSlim _dataAvailable = new ManualResetEventSlim(false);
        private readonly object _synchro = new Object();

        private long _readPosition;
        private long _writePosition;
        private bool _isWritingsFinalized;

        #region >> Stream

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanSeek => false;

        /// <inheritdoc />
        public override bool CanWrite => true;

        /// <inheritdoc />
        public override void Close()
        {
            FinalizeWritings();

            base.Close();
        }

        /// <inheritdoc />
        public override void Flush()
        {
            lock (_synchro)
            {
                _innerStream.Flush();
            }
        }

        /// <inheritdoc />
        public override long Length
        {
            get
            {
                lock (_synchro)
                {
                    return _innerStream.Length;
                }
            }
        }

        /// <inheritdoc />
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            _dataAvailable.Wait();

            lock (_synchro)
            {
                if (_isWritingsFinalized && _readPosition == _writePosition)
                {
                    _innerStream.Close();
                    return 0;
                }

                _innerStream.Position = _readPosition;
                var read = _innerStream.Read(buffer, offset, count);
                _readPosition = _innerStream.Position;

                if (!_isWritingsFinalized && _readPosition == _writePosition)
                {
                    _dataAvailable.Reset();
                }

                return read;
            }
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (_synchro)
            {
                if (_isWritingsFinalized)
                {
                    throw new EndOfStreamException("No Write can be done once FinalizeWritings has been called.");
                }

                _innerStream.Position = _writePosition;
                _innerStream.Write(buffer, offset, count);
                _writePosition = _innerStream.Position;
                _dataAvailable.Set();
            }
        }

        #endregion

        #region >> IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _innerStream?.Close();
                }
            }
            finally
            {
                if (_innerStream != null)
                {
                    _innerStream = null;
                    base.Dispose(disposing);
                }
            }
        }

        #endregion

        /// <summary>
        /// Finalizes the writings: no call to <see cref="Write"/> can be done now.
        /// </summary>
        public void FinalizeWritings()
        {
            lock (_synchro)
            {
                _isWritingsFinalized = true;
                _dataAvailable.Set();
            }
        }
    }
}
