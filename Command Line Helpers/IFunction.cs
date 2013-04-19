using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZTn.Tools.CommandLine.Helpers
{
    public interface IFunction
    {
        System.IO.Stream stream { get; set; }

        int errorCode { get; }

        TimeSpan time { get; }

        /// <summary>
        /// Execute the function, using external executables when needed
        /// </summary>
        /// <returns>Current <see cref="IFunction"/> instance</returns>
        IFunction execute();

        /// <summary>
        /// Set the path to the directory containing targeted executables
        /// </summary>
        /// <param name="path">Path to the directory containing targeted executables</param>
        /// <returns>Current <see cref="IFunction"/> instance</returns>
        IFunction setPathToExecutable(String path);

        /// <summary>
        /// Set parameters to personalize the function (if generic, it's the command line argument used for the executables)
        /// </summary>
        /// <param name="parameters">Parameters as a String</param>
        /// <returns>Current <see cref="IFunction"/> instance</returns>
        IFunction setParameters(String parameters);

        /// <summary>
        /// Define a value for the alias
        /// </summary>
        /// <param name="alias">Alias to be replaced</param>
        /// <param name="value">Value to use instead of the alias</param>
        /// <returns></returns>
        IFunction setAlias(String alias, String value);

        /// <summary>
        /// Set a stream to receive the output of the execution
        /// </summary>
        /// <param name="outputStream"></param>
        /// <returns></returns>
        IFunction setStream(System.IO.Stream outputStream);
    }
}
