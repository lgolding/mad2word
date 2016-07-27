// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace Mad2WordLib
{
    public class MadokoDocument
    {
        public static object Read(string inputPath)
        {
            using (var reader = new StreamReader(File.OpenRead(inputPath)))
            {
                return Read(reader);
            }
        }

        public static MadokoDocument Read(TextReader reader)
        {
            var document = new MadokoDocument();

            string line;
            MadokoBlock block = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    block = null;
                }
                else if ((block = MadokoHeading.CreateFrom(line)) != null)
                {
                    document.Blocks.Add(block);
                }
                else
                {
                    line = line.Trim();

                    if (block == null)
                    {
                        block = new MadokoBlock();
                        document.Blocks.Add(block);
                    }
                    else
                    {
                        // This paragraph is continued from the preceding source line,
                        // so make sure there's a blank space between the end of that
                        // line and the start of this one.
                        line = " " + line;
                    }

                    MadokoRun[] runs = MadokoLine.Parse(line);
                    block.Runs.AddRange(runs);
                }
            }

            return document;
        }

        private MadokoDocument()
        {
            Blocks = new List<MadokoBlock>();
        }

        public List<MadokoBlock> Blocks { get; }
    }
}
