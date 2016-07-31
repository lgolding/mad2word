// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace Mad2WordLib
{
    public class LineSource
    {
        private readonly string[] _lines;

        // The index of the next line to be retrieved from the source.
        private int _nextIndex;

        public LineSource(TextReader reader, IFileSystem fileSystem)
        {
            var lines = new List<string>();

            ReadAllLines(lines, reader, fileSystem);

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

        private void ReadAllLines(List<string> lines, TextReader reader, IFileSystem fileSystem)
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

                    using (TextReader innerReader = fileSystem.OpenText(path))
                    {
                        ReadAllLines(lines, innerReader, fileSystem);
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
