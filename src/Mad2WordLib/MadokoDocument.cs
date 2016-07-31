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

            var lineSource = new LineSource(reader, fileSystem);
            var metadata = new Metadata(lineSource);

            while (!lineSource.AtEnd)
            {
                SkipBlankLines(lineSource);
                if (lineSource.AtEnd)
                {
                    break;
                }

                string nextLine = lineSource.PeekLine();
                if (MadokoHeading.MatchesLine(nextLine))
                {
                    document.Blocks.Add(new MadokoHeading(lineSource));
                }
                else if (MadokoBulletListItem.MatchesLine(nextLine))
                {
                    document.Blocks.Add(new MadokoBulletListItem(lineSource));
                }
                else if (MadokoCodeBlock.MatchesLine(nextLine))
                {
                    document.Blocks.Add(new MadokoCodeBlock(lineSource));
                }
                else if (MadokoTitle.MatchesLine(nextLine))
                {
                    string title = metadata.Title;
                    if (title != null)
                    {
                        document.Blocks.Add(new MadokoTitle(title));
                    }

                    lineSource.Advance();
                }
                else
                {
                    document.Blocks.Add(new MadokoBlock(lineSource));
                }
            }

            return document;
        }

        public List<MadokoBlock> Blocks { get; }

        private MadokoDocument()
        {
            Blocks = new List<MadokoBlock>();
        }

        private static void SkipBlankLines(LineSource lineSource)
        {
            while (!lineSource.AtEnd && string.IsNullOrWhiteSpace(lineSource.PeekLine()))
            {
                lineSource.Advance();
            }
        }
    }
}
