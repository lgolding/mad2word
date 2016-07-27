// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Mad2WordLib
{
    public class MadokoToWordConverter
    {
        public static void Convert(
            string inputPath,
            string templatePath,
            string outputPath)
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            File.Copy(templatePath, outputPath);

            using (WordprocessingDocument wordDocument =
                WordprocessingDocument.Open(outputPath, true))
            {
                Body body = wordDocument.MainDocumentPart.Document.Body;

                // If the template document already contains an empty paragraph, remove it.
                body.RemoveAllChildren();

                using (var reader = new StreamReader(File.OpenRead(inputPath)))
                {
                    string line;
                    Paragraph para = null;
                    MadokoHeading madokoHeading = null;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            para = null;
                        }
                        else if ((madokoHeading = MadokoHeading.CreateFrom(line)) != null)
                        { 
                            AddHeading(line, body);
                            para = null;
                        }
                        else
                        {
                            line = line.Trim();

                            if (para == null)
                            {
                                para = body.AppendChild(new Paragraph());
                            }
                            else
                            {
                                // This paragraph is continued from the preceding source line,
                                // so make sure there's a blank space between the end of that
                                // line and the start of this one.
                                line = " " + line;
                            }

                            Run[] runs = ConvertLineToRuns(line);
                            para.Append(runs);
                        }
                    }
                }
            }
        }

        internal static Run[] ConvertLineToRuns(string line)
        {
            MadokoRun[] madokoRuns = MadokoLine.Parse(line);
            return madokoRuns.Select(ConvertMadokoRunToRun).ToArray();
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

        private static void AddHeading(string line, Body body)
        {
            MadokoHeading madokoHeading = MadokoHeading.CreateFrom(line);

            Paragraph heading = body.AppendChild(new Paragraph());
            heading.SetHeadingLevel(madokoHeading.Level);

            Run run = heading.AppendChild(new Run());
            run.AppendChild(new Text(madokoHeading.Text));
        }
    }
}