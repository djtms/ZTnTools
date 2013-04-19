using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                .setParameters("/C dir %WORKINGDIR")
                // Set value for each alias to be used with next execute()
                .setAlias("%WORKINGDIR", @".\")
                // Set output stream to get live informations on execution (console)
                .setStream(Console.OpenStandardOutput())
                // Execute the chain
                .execute();
        }
    }
}
