// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace Mad2WordLib.UnitTests
{
    internal class FakeFileSystem: IFileSystem
    {
        private readonly Dictionary<string, string> _fileContentsDictionary;

        public FakeFileSystem()
        {
            _fileContentsDictionary = new Dictionary<string, string>();
        }

        public TextReader OpenText(string path)
        {
            return new StringReader(_fileContentsDictionary[path]);
        }

        internal void AddFile(string path, string fileContents)
        {
            _fileContentsDictionary.Add(path, fileContents);
        }
    }
}