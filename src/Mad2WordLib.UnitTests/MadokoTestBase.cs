// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;

namespace Mad2WordLib.UnitTests
{
    public abstract class MadokoTestBase
    {
        protected static MadokoDocument ReadDocument(string input)
        {
            MadokoDocument document;
            using (var reader = new StringReader(input))
            {
                var environment = new FakeEnvironment();
                var fileSystem = new FakeFileSystem(environment);
                document = MadokoDocument.Read(reader, fileSystem, environment, inputPath: null);
            }

            return document;
        }

        protected static LineSource MakeLineSource(string input)
        {
            LineSource lineSource;
            using (var reader = new StringReader(input))
            {
                var environment = new FakeEnvironment();
                var fileSystem = new FakeFileSystem(environment);
                lineSource = new LineSource(reader, fileSystem, environment, inputPath: null);
            }

            return lineSource;
        }
    }
}
