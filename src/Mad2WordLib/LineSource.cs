// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace Mad2WordLib
{
    public class LineSource
    {
        private readonly string _fileName;
        private readonly IFileSystem _fileSystem;
        private readonly IEnvironment _environment;
        private readonly string[] _lines;

        // The index of the next line to be retrieved from the source.
        private int _nextIndex;
        
        public LineSource(TextReader reader, string fileName, IFileSystem fileSystem, IEnvironment environment)
        {
            _fileName = fileName;
            _fileSystem = fileSystem;
            _environment = environment;
            var lines = new List<string>();

            ReadAllLines(lines, reader, fileName);

            _lines = lines.ToArray();
        }

        public string FileName => _fileName;

        public int LineNumber => _nextIndex;

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

        private void ReadAllLines(List<string> lines, TextReader reader, string fileName)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var includeDirective = IncludeDirective.CreateFrom(line);
                if (includeDirective != null)
                {
                    ReadIncludedFile(includeDirective.Path, fileName, lines);
                }
                else
                {
                    lines.Add(line);
                }
            }
        }

        private void ReadIncludedFile(string includedFilePath, string fileName, List<string> lines)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(includedFilePath)))
            {
                includedFilePath = Path.ChangeExtension(includedFilePath, "mdk");
            }

            // If the INCLUDE directive specifies an absolute path, search only for that
            // file. Otherwise, search for the specified file in the directories specified
            // by the Madoko manual, in this order:
            //    1) The currect directory.
            //    2) The directory containing the including file.
            //    3) The styles directory (NYI).
            //    4) The Madoko installation directory (NYI).
            var searchPaths = new List<string>();
            if (Path.IsPathRooted(includedFilePath))
            {
                searchPaths.Add(includedFilePath);
            }
            else
            {
                searchPaths.Add(Path.Combine(_environment.CurrentDirectory, includedFilePath));

                string inputDirectory = Path.GetDirectoryName(fileName);
                searchPaths.Add(Path.Combine(inputDirectory, includedFilePath));
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
                        includedFilePath,
                        string.Join(", ", searchPaths)));
            }
        }
    }
}
