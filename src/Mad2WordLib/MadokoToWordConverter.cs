// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

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

                var visitor = new WordProcessingVisitor(body);

                foreach (MadokoBlock madokoBlock in madokoDocument.Blocks)
                {
                    madokoBlock.Accept(visitor);
                }
            }
        }
    }
}