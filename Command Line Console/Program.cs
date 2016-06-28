using System;
using ZTn.Tools.CommandLine.Helpers;

namespace ZTn.Tools.CommandLine.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            IFunction executable = new GenericFunction("cmd.exe");
            executable
                // Set parameters
                .SetParameters("/C dir %WORKINGDIR")
                // Set value for each alias to be used with next execute()
                .SetAlias("%WORKINGDIR", @".\")
                // Set output stream to get live informations on execution (console)
                .SetStream(Console.OpenStandardOutput())
                // Set working directory
                .SetWorkingDirectory(@"..\")
                // Execute the chain
                .Execute();
        }
    }
}
