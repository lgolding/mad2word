// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace Mad2WordLib
{
    public class MadokoDocument
    {
        public static MadokoDocument Read(string inputPath, IFileSystem fileSystem)
        {
            using (var reader = new StreamReader(File.OpenRead(inputPath)))
            {
                return Read(reader, fileSystem);
            }
        }

        public static MadokoDocument Read(TextReader reader, IFileSystem fileSystem)
        {
            var document = new MadokoDocument();

            MadokoBlock block = null;
            MadokoHeading heading = null;
            MadokoBulletListItem bulletListItem = null;

            var lineSource = new LineSource(reader, fileSystem);

            while (lineSource.MoreLines)
            {
                string line = lineSource.GetNextLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    block = null;
                }
                else if ((heading = MadokoHeading.CreateFrom(line)) != null)
                {
                    document.Blocks.Add(heading);
                    block = heading;
                }
                else if ((bulletListItem = MadokoBulletListItem.CreateFrom(line)) != null)
                {
                    document.Blocks.Add(bulletListItem);
                    block = bulletListItem;
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
