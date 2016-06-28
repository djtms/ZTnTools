using System;

namespace ZTn.Tools.CommandLine.Helpers
{
    public interface IFunction
    {
        System.IO.Stream Stream { get; set; }

        int ErrorCode { get; }

        TimeSpan Time { get; }

        /// <summary>
        /// Execute the function, using external executables when needed
        /// </summary>
        /// <returns>Current <see cref="IFunction"/> instance</returns>
        IFunction Execute();

        /// <summary>
        /// Set the path to the directory containing targeted executables
        /// </summary>
        /// <param name="path">Path to the directory containing targeted executables</param>
        /// <returns>Current <see cref="IFunction"/> instance</returns>
        IFunction SetPathToExecutable(string path);

        /// <summary>
        /// Set parameters to personalize the function (if generic, it's the command line argument used for the executables)
        /// </summary>
        /// <param name="parameters">Parameters as a String</param>
        /// <returns>Current <see cref="IFunction"/> instance</returns>
        IFunction SetParameters(string parameters);

        /// <summary>
        /// Define a value for the alias
        /// </summary>
        /// <param name="alias">Alias to be replaced</param>
        /// <param name="value">Value to use instead of the alias</param>
        /// <returns></returns>
        IFunction SetAlias(string alias, string value);

        /// <summary>
        /// Set a stream to receive the output of the execution
        /// </summary>
        /// <param name="outputStream"></param>
        /// <returns></returns>
        IFunction SetStream(System.IO.Stream outputStream);

        /// <summary>
        /// Set working directory
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        IFunction SetWorkingDirectory(string workingDirectory);
    }
}
