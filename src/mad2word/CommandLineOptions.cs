// Licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CommandLine;

namespace Mad2Word
{
    public class CommandLineOptions
    {
        [Option(
            'o',
            "output-path",
            HelpText = "Output document file path",
            Required = true)]
        public string OutputPath { get; set; }
    }
}
