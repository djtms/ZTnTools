using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ZTn.Tools.Stream.Helpers
{
    [TestFixture]
    public class AsyncReadWriteStreamUnitTests
    {
        private readonly string[] _expectedLines = Enumerable.Range(0, 5).Select(i => $"i is {i}").ToArray();

        [Test]
        public void SynchronousWriteAllThenReadAll()
        {
            var readLines = new List<string>();

            using (var rws = new AsyncReadWriteStream())
            {
                var ws = new StreamWriter(rws);
                for (var i = 0; i < 5; i++)
                {
                    ws.WriteLine(_expectedLines[i]);
                }
                ws.Flush();
                rws.FinalizeWritings();

                var rs = new StreamReader(rws);
                while (!rs.EndOfStream)
                {
                    readLines.Add(rs.ReadLine());
                }
            }

            CollectionAssert.AreEqual(_expectedLines, readLines);
        }

        [Test]
        public void AsynchronousReadingAndWriting()
        {
            var readLines = new List<string>();

            var rws = new AsyncReadWriteStream();

            var readTask = Task.Run(() =>
            {
                var rs = new StreamReader(rws);
                while (!rs.EndOfStream)
                {
                    readLines.Add(rs.ReadLine());
                }
            });

            var writeTask = Task.Run(() =>
            {
                Task.Delay(500);
                var ws = new StreamWriter(rws);
                for (var i = 0; i < 5; i++)
                {
                    ws.WriteLine(_expectedLines[i]);
                }
                ws.Flush();
                rws.FinalizeWritings();
            });

            Task.WaitAll(readTask, writeTask);

            CollectionAssert.AreEqual(_expectedLines, readLines);
        }
    }
}
