// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;

namespace Mad2WordLib.UnitTests
{
    public abstract class MadokoTestBase
    {
        protected static MadokoDocument ReadDocument(string Input)
        {
            MadokoDocument document;
            using (var reader = new StringReader(Input))
            {
                IFileSystem fileSystem = new FakeFileSystem();
                document = MadokoDocument.Read(reader, fileSystem);
            }

            return document;
        }
    }
}
