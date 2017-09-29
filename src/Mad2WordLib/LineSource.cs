// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace Mad2WordLib
{
    public class LineSource
    {
        private readonly IFileSystem _fileSystem;
        private readonly IEnvironment _environment;
        private readonly string[] _lines;

        // The index of the next line to be retrieved from the source.
        private int _nextIndex;
        
        public LineSource(TextReader reader, IFileSystem fileSystem, IEnvironment environment, string inputPath)
        {
            _fileSystem = fileSystem;
            _environment = environment;
            var lines = new List<string>();

            ReadAllLines(lines, reader, inputPath);

            _lines = lines.ToArray();
        }

        public int LineNumber => _nextIndex + 1;

        public bool AtEnd => _nextIndex == _lines.Length;

        public string[] GetAllLines()
        {
            string[] allLines = new string[_lines.Length];
            Array.Copy(_lines, allLines, _lines.Length);
            return allLines;
        }

        public string GetLine()
        {
            return _lines[_nextIndex++];
        }

        internal string PeekLine()
        {
            return _lines[_nextIndex];
        }

        public void Advance()
        {
            if (_nextIndex < _lines.Length)
            {
                ++_nextIndex;
            }
            else
            {
                throw new InvalidOperationException("Unexpectedly encountered end of input.");
            }
        }

        public void BackUp()
        {
            if (_nextIndex > 0)
            {
                --_nextIndex;
            }
            else
            {
                throw new InvalidOperationException("Attempted to back up past the beginning of the input.");
            }
        }

        private void ReadAllLines(List<string> lines, TextReader reader, string inputPath)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var includeDirective = IncludeDirective.CreateFrom(line);
                if (includeDirective != null)
                {
                    string path = includeDirective.Path;
                    if (string.IsNullOrEmpty(Path.GetExtension(path)))
                    {
                        path = Path.ChangeExtension(path, "mdk");
                    }

                    var searchPaths = new List<string>();
                    if (Path.IsPathRooted(path))
                    {
                        searchPaths.Add(path);
                    }
                    else
                    {
                        searchPaths.Add(Path.Combine(_environment.CurrentDirectory, path));

                        string inputDirectory = Path.GetDirectoryName(inputPath);
                        searchPaths.Add(Path.Combine(inputDirectory, path));
                    }

                    bool found = false;
                    foreach (string searchPath in searchPaths)
                    {
                        if (_fileSystem.FileExists(searchPath))
                        {
                            using (TextReader innerReader = _fileSystem.OpenText(searchPath))
                            {
                                ReadAllLines(lines, innerReader, searchPath);
                            }

                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        throw new MadokoReaderException(
                            StringUtil.Format(
                                Resources.ErrorIncludeFileNotFound,
                                includeDirective.Path,
                                string.Join(", ", searchPaths)));
                    }
                }
                else
                {
                    lines.Add(line);
                }
            }
        }
    }
}
