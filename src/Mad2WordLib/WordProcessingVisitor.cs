// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

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

        public void Visit(MadokoCodeBlock codeBlock)
        {
            Paragraph para = ConvertMadokoBlockToParagraph(codeBlock);
            para.SetStyle(StyleNames.Code);
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

        public void Visit(MadokoHeading madokoHeading)
        {
            Paragraph para = ConvertMadokoBlockToParagraph(madokoHeading);
            para.SetHeadingLevel(madokoHeading.Level);
            _body.AppendChild(para);
        }

        private static Paragraph ConvertMadokoBlockToParagraph(MadokoBlock madokoBlock)
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
