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
            return _fileContentsDictionary.ContainsKey(RootedPath(path));
        }

        public TextReader OpenText(string path)
        {
            string rootedPath = RootedPath(path);

            if (!FileExists(rootedPath))
            {
                throw new FileNotFoundException("The specified file was not found: " + rootedPath, rootedPath);
            }

            return new StringReader(_fileContentsDictionary[rootedPath]);
        }

        internal void AddFile(string path, string fileContents)
        {
            _fileContentsDictionary.Add(RootedPath(path), fileContents);
        }

        private string RootedPath(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(_environment.CurrentDirectory, path);
            }

            return path;
        }
    }
}