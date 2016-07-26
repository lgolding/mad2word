// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using CommandLine;

namespace Mad2Word
{
    public class CommandLineOptions
    {
        [Option(
            'i',
            "input-path",
            HelpText = "Input Madoko document file path",
            Required = true)]
        public string InputPath { get; set; }

        [Option(
            'o',
            "output-path",
            HelpText = "Output word processing document file path",
            Required = true)]
        public string OutputPath { get; set; }
    }
}
