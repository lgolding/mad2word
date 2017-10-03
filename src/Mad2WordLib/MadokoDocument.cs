// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mad2WordLib
{
    public class MadokoDocument
    {
        public static MadokoDocument Read(string inputPath, IFileSystem fileSystem, IEnvironment environment)
        {
            using (var reader = new StreamReader(File.OpenRead(inputPath)))
            {
                return Read(reader, fileSystem, environment, inputPath);
            }
        }

        internal static MadokoDocument Read(TextReader reader, IFileSystem fileSystem, IEnvironment environment, string inputPath)
        {
            var document = new MadokoDocument();
            var errors = new List<MadokoParserException>();

            var lineSource = new LineSource(reader, inputPath, fileSystem, environment);
            var metadata = new Metadata(lineSource);

            while (!lineSource.AtEnd)
            {
                try
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
                catch (MadokoParserException ex)
                {
                    if (ex.LineNumber == 0)
                    {
                        ex.LineNumber = lineSource.LineNumber;
                    }

                    errors.Add(ex);
                }
            }

            if (errors.Any())
            {
                throw new AggregateException(errors);
            }

            return document;
        }

        public List<MadokoBlock> Blocks { get; }

        public List<MadokoParserException> Errors { get; }

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
