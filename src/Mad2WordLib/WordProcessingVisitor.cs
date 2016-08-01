// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Mad2WordLib
{
    public class WordProcessingVisitor : IMadokoVisitor
    {
        private readonly Body _body;

        public WordProcessingVisitor(Body body)
        {
            _body = body;
        }

        public void Visit(MadokoBlock block)
        {
            Paragraph para = ConvertMadokoBlockToParagraph(block);
            _body.AppendChild(para);
        }

        public void Visit(MadokoBulletListItem madokoBulletListItem)
        {
            Paragraph para = ConvertMadokoBlockToParagraph(madokoBulletListItem);
            para.PrependChild(
                new ParagraphProperties(
                  new NumberingProperties(
                    new NumberingLevelReference() { Val = 0 },
                    new NumberingId() { Val = 1 })));
            _body.AppendChild(para);
        }

        public void Visit(MadokoCodeBlock codeBlock)
        {
            AppendStyledBlock(codeBlock, StyleIds.Code);
        }

        public void Visit(MadokoHeading madokoHeading)
        {
            AppendStyledBlock(madokoHeading, GetHeadingStyleId(madokoHeading.Level));
        }

        public void Visit(MadokoTitle madokoTitle)
        {
            AppendStyledBlock(madokoTitle, StyleIds.Title);
        }

        private void AppendStyledBlock(MadokoBlock block, string styleId)
        {
            Paragraph para = ConvertMadokoBlockToParagraph(block);
            para.SetStyle(styleId);
            _body.AppendChild(para);
        }

        private static Paragraph ConvertMadokoBlockToParagraph(MadokoBlock madokoBlock)
        {
            Run[] runs = madokoBlock.Runs.Select(ConvertMadokoRunToRun).ToArray();
            return new Paragraph(runs);
        }

        internal static string GetHeadingStyleId(int level)
        {
            return "Heading" + level.ToString(CultureInfo.InvariantCulture);
        }

        private static readonly string[] s_lineSplitters = new string[] {Environment.NewLine };

        private static Run ConvertMadokoRunToRun(MadokoRun madokoRun)
        {
            var run = new Run();
            switch (madokoRun.RunType)
            {
                case MadokoRunType.Code:
                    run.SetStyle(StyleIds.CodeChar);
                    break;

                case MadokoRunType.Italic:
                    break;

                default:
                    break;
            }

            string[] softLines = madokoRun.Text.Split(s_lineSplitters, StringSplitOptions.None);
            for (int i = 0; i < softLines.Length; ++i)
            {
                Text text = new Text
                {
                    Text = softLines[i],
                    Space = SpaceProcessingModeValues.Preserve
                };

                run.Append(text);

                if (i < softLines.Length - 1)
                {
                    run.Append(new Break());
                }
            }

            return run;
        }
    }
}
