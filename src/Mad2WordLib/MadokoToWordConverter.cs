// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Mad2WordLib
{
    public class MadokoToWordConverter
    {
        public static void Convert(string inputPath, string outputPath)
        {

            using (WordprocessingDocument wordDocument =
                WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                Paragraph para = body.AppendChild(new Paragraph());

                using (var reader = new StreamReader(File.OpenRead(inputPath)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Run run = para.AppendChild(new Run());
                        run.AppendChild(new Text(line));
                    }
                }
            }
        }
    }
}
