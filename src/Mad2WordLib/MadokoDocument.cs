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

            string[] lines = ReadAllLines(reader, fileSystem);

            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];
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

        internal static string[] ReadAllLines(
            TextReader reader,
            IFileSystem fileSystem)
        {
            var lines = new List<string>();

            ReadAllLines(reader, lines, fileSystem);
            
            return lines.ToArray();
        }

        private static void ReadAllLines(
            TextReader reader,
            List<string> lines,
            IFileSystem fileSystem)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var includeDirective = IncludeDirective.CreateFrom(line);
                if (includeDirective != null)
                {
                    string path = includeDirective.Path;
                    if (string.IsNullOrEmpty(Path.GetExtension(path)))
                    {
                        path = Path.ChangeExtension(path, "mdk"); 
                    }

                    using (TextReader innerReader = fileSystem.OpenText(path))
                    {
                        ReadAllLines(innerReader, lines, fileSystem);
                    }
                }
                else
                {
                    lines.Add(line);
                }
            }
        }

        private MadokoDocument()
        {
            Blocks = new List<MadokoBlock>();
        }

        public List<MadokoBlock> Blocks { get; }
    }
}
