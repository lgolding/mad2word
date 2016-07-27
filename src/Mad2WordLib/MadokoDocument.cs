// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;

namespace Mad2WordLib
{
    public class MadokoDocument
    {
        internal static object Open(string inputPath)
        {
            var madokoDocument = new MadokoDocument();

            using (var reader = new StreamReader(File.OpenRead(inputPath)))
            {
            }

            return madokoDocument;
        }
    }
}
