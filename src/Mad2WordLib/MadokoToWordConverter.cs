// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Globalization;
using System.IO;
using System.Linq;
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

                    while ((line = reader.ReadLine()) != null)
                    {
                        var madokoHeading = MadokoHeading.CreateFrom(line);
                        if (madokoHeading != null)
                        {
                            AddHeading(line, body);
                            para = null;
                        }
                        else
                        {
                            if (para == null)
                            {
                                para = body.AppendChild(new Paragraph());
                            }

                            Run run = para.AppendChild(new Run());
                            run.AppendChild(new Text(line));
                        }
                    }
                }
            }
        }

        private static void AddHeading(string line, Body body)
        {
            MadokoHeading madokoHeading = MadokoHeading.CreateFrom(line);

            string styleId = "Heading" + madokoHeading.Level.ToString(CultureInfo.InvariantCulture);

            Paragraph heading = body.AppendChild(new Paragraph());
            heading.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId() { Val = styleId });
            Run run = heading.AppendChild(new Run());
            run.AppendChild(new Text(madokoHeading.Text));
        }
    }
}
