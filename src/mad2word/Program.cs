// Licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CommandLine;
using Mad2WordLib;

namespace Mad2Word
{
    class Program
    {
        static int Main(string[] args)
        {
            Banner();

            return Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(
                    options => Run(options),
                    err => 1);

        }

        private static int Run(CommandLineOptions options)
        {
            MadokoToWordConverter.Convert(options.InputPath, options.OutputPath);

            return 0;
        }

        private static void Banner()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            IEnumerable<Attribute> attributes = entryAssembly.GetCustomAttributes();

            var titleAttribute = attributes.Single(a => a is AssemblyTitleAttribute) as AssemblyTitleAttribute;
            string programName = titleAttribute.Title;

            string version = entryAssembly.GetName().Version.ToString();

            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, Resources.Banner, programName, version));
            Console.WriteLine();
        }
    }
}
