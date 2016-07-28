﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Mad2WordLib
{
    public class MadokoToWordConverter
    {
        public static void Convert(MadokoDocument madokoDocument, string outputPath)
        {
            using (WordprocessingDocument wordDocument =
                WordprocessingDocument.Open(outputPath, true))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;

                // If the template document already contains an empty paragraph, remove it.
                body.RemoveAllChildren();

                foreach (MadokoBlock madokoBlock in madokoDocument.Blocks)
                {
                    var madokoHeading = madokoBlock as MadokoHeading;
                    if (madokoHeading != null)
                    {
                        Paragraph para = ConvertMadokoHeadingToParagraph(madokoHeading);
                        body.AppendChild(para);
                    }
                    else
                    {
                        Paragraph para = ConvertMadokoBlockToParagraph(madokoBlock);
                        body.AppendChild(para);
                    }
                }
            }
        }

        private static Paragraph ConvertMadokoHeadingToParagraph(MadokoHeading madokoHeading)
        {
            Paragraph heading = ConvertMadokoBlockToParagraph(madokoHeading);
            heading.SetHeadingLevel(madokoHeading.Level);
            return heading;
        }

        internal static Paragraph ConvertMadokoBlockToParagraph(MadokoBlock madokoBlock)
        {
            Run[] runs = madokoBlock.Runs.Select(ConvertMadokoRunToRun).ToArray();
            return new Paragraph(runs);
        }

        private static Run ConvertMadokoRunToRun(MadokoRun madokoRun)
        {
            Text text = new Text
            {
                Text = madokoRun.Text,
                Space = SpaceProcessingModeValues.Preserve
            };

            var run = new Run();
            if (madokoRun.RunType == MadokoRunType.Code)
            {
                run.SetStyle(StyleNames.CodeChar);
            }

            run.Append(text);

            return run;
        }
    }
}