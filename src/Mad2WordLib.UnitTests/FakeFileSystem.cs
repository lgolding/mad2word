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

        public bool FileExists(string path) =>
            _fileContentsDictionary.ContainsKey(path.RootedPath(_environment));

        public TextReader OpenText(string path) =>
            new StringReader(_fileContentsDictionary[GetRootedPath(path)]);

        public string[] ReadAllLines(string path) =>
            OpenText(path).ReadAllLines();

        internal void AddFile(string path, string contents) =>
            _fileContentsDictionary.Add(path.RootedPath(_environment), contents);

        public Stream OpenRead(string path) =>
            _fileContentsDictionary[GetRootedPath(path)].ToStream();

        private string GetRootedPath(string path)
        {
            string rootedPath = path.RootedPath(_environment);

            if (!FileExists(rootedPath))
            {
                throw new FileNotFoundException("The specified file was not found: " + rootedPath, rootedPath);
            }

            return rootedPath;
        }
    }
}