// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
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
            try
            {
                return Parser.Default.ParseArguments<CommandLineOptions>(args)
                    .MapResult(
                        options => Run(options),
                        err => 1);
            }
            catch (AggregateException ex)
            {
                foreach (Exception inner in ex.InnerExceptions)
                {
                    Console.Error.WriteLine(inner.Message);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            return 1;
        }

        private static int Run(CommandLineOptions options)
        {
            Banner();

            if (File.Exists(options.OutputPath))
            {
                File.Delete(options.OutputPath);
            }

            File.Copy(options.TemplatePath, options.OutputPath);

            IEnvironment environment = new RealEnvironment();
            IFileSystem fileSystem = new FileSystem();
            var madokoDocument = MadokoDocument.Read(options.InputPath, fileSystem, environment);

            MadokoToWordConverter.Convert(madokoDocument, options.OutputPath);

            return 0;
        }

        private static void Banner()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            IEnumerable<Attribute> attributes = entryAssembly.GetCustomAttributes();

            var titleAttribute = attributes.Single(a => a is AssemblyTitleAttribute) as AssemblyTitleAttribute;
            string programName = titleAttribute.Title;

            string version = entryAssembly.GetName().Version.ToString();

            var copyrightAttribute = attributes.Single(a => a is AssemblyCopyrightAttribute) as AssemblyCopyrightAttribute;
            string copyright = copyrightAttribute.Copyright;

            Console.WriteLine(StringUtil.Format(Resources.Banner, programName, version));
            Console.WriteLine(copyright);
            Console.WriteLine();
        }
    }
}