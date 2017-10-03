// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace Mad2WordLib.UnitTests
{
    internal class FakeFileSystem : IFileSystem
    {
        private readonly Dictionary<string, string> _fileContentsDictionary;
        public readonly IEnvironment _environment;

        public FakeFileSystem(IEnvironment environment)
        {
            _environment = environment;
            _fileContentsDictionary = new Dictionary<string, string>();
        }

        public bool FileExists(string path)
        {
            return _fileContentsDictionary.ContainsKey(path.RootedPath(_environment));
        }

        public TextReader OpenText(string path)
        {
            string rootedPath = path.RootedPath(_environment);

            if (!FileExists(rootedPath))
            {
                throw new FileNotFoundException("The specified file was not found: " + rootedPath, rootedPath);
            }

            return new StringReader(_fileContentsDictionary[rootedPath]);
        }

        public string[] ReadAllLines(string path)
        {
            string rootedPath = path.RootedPath(_environment);

            if (!FileExists(rootedPath))
            {
                throw new FileNotFoundException("The specified file was not found: " + rootedPath, rootedPath);
            }

            var reader = new StringReader(_fileContentsDictionary[rootedPath]);
            return reader.ReadAllLines();
        }

        internal void AddFile(string path, string fileContents)
        {
            _fileContentsDictionary.Add(path.RootedPath(_environment), fileContents);
        }
    }
}