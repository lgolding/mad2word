// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace Mad2WordLib
{
    public class MadokoDocument
    {
        public static object Read(string inputPath)
        {
            using (var reader = new StreamReader(File.OpenRead(inputPath)))
            {
                return Read(reader);
            }
        }

        public static MadokoDocument Read(TextReader reader)
        {
            var madokoDocument = new MadokoDocument();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var madokoHeader = MadokoHeading.CreateFrom(line);
                if (madokoHeader != null)
                {
                    madokoDocument.Blocks.Add(madokoHeader);
                }
            }

            return madokoDocument;
        }

        private MadokoDocument()
        {
            Blocks = new List<MadokoBlock>();
        }

        public List<MadokoBlock> Blocks { get; }
    }
}
