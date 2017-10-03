// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mad2WordLib
{
    public class LineSource
    {
        private string _filePath;
        private string[] _lines;
        private int _lineNumber;

        private readonly IFileSystem _fileSystem;
        private readonly IEnvironment _environment;

        private List<LineSourceState> _stateStack = new List<LineSourceState>();

        public LineSource(TextReader reader, string filePath, IFileSystem fileSystem, IEnvironment environment)
        {
            _filePath = filePath?.RootedPath(environment);
            _lines = reader.ReadAllLines();
            _lineNumber = 0;

            _fileSystem = fileSystem;
            _environment = environment;
        }

        public string FilePath => _filePath;

        public int LineNumber => _lineNumber;

        public bool AtEnd => _lineNumber == _lines.Length && !_stateStack.Any();

        public string[] GetAllLines()
        {
            var allLines = new List<string>();
            string line;
            while ((line = GetLine()) != null)
            {
                allLines.Add(line);
            }

            return allLines.ToArray();
        }

        public string GetLine()
        {
            if (_lineNumber < _lines.Length)
            {
                string line = _lines[_lineNumber];
                var includeDirective = IncludeDirective.CreateFrom(line);
                if (includeDirective != null)
                {
                    string resolvedIncludeFilePath = ResolveIncludedFilePath(includeDirective.Path);

                    // When we return from the nested file, we'll need to be pointing
                    // at the line after the [INCLUDE] directive.
                    ++_lineNumber;

                    PushState();
                    _filePath = resolvedIncludeFilePath;
                    _lines = _fileSystem.ReadAllLines(_filePath);
                    _lineNumber = 0;
                    return GetLine();
                }
                else
                {
                    ++_lineNumber;
                    return line;
                }
            }
            else if (_stateStack.Any())
            {
                PopState();
                return GetLine();
            }
            else
            {
                return null;
            }
        }

        public string PeekLine()
        {
            string[] lines = _lines;
            int lineNumber = _lineNumber;
            int stackPosition = _stateStack.Count - 1;

            // Look past the end of any included files we are currently positioned at the end of.
            while (lineNumber >= lines.Length && stackPosition >= 0)
            {
                LineSourceState state = _stateStack[stackPosition--];
                lines = state.Lines;
                lineNumber = state.LineNumber;
            }

            // Having found the next line, if it's an INCLUDE directive, look
            // into it.
            string line = lines[lineNumber];
            IncludeDirective includeDirective;
            while ((includeDirective = IncludeDirective.CreateFrom(line)) != null)
            {
                string resolvedIncludeFilePath = ResolveIncludedFilePath(includeDirective.Path);
                line = _fileSystem.OpenText(resolvedIncludeFilePath).ReadLine();
            }

            return line;
        }

        public void Advance()
        {
            string line = GetLine();
            if (line == null)
            {
                throw new InvalidOperationException("Unexpectedly encountered end of input.");
            }
        }

        public void BackUp()
        {
            if (_lineNumber > 0)
            {
                --_lineNumber;
            }
            else
            {
                throw new InvalidOperationException("Attempted to back up past the beginning of the input.");
            }
        }

        private string ResolveIncludedFilePath(string includedFilePath)
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

                string inputDirectory = Path.GetDirectoryName(_filePath);
                searchPaths.Add(Path.Combine(inputDirectory, includedFilePath));
            }

            foreach (string searchPath in searchPaths)
            {
                if (_fileSystem.FileExists(searchPath))
                {
                    return searchPath;
                }
            }

            throw new MadokoReaderException(
                StringUtil.Format(
                    Resources.ErrorIncludeFileNotFound,
                    includedFilePath,
                    string.Join(", ", searchPaths)));
        }

        private void PushState()
        {
            _stateStack.Add(new LineSourceState(_filePath, _lines, _lineNumber));
        }

        private void PopState()
        {
            LineSourceState state = _stateStack.Last();
            _filePath = state.FileName;
            _lines = state.Lines;
            _lineNumber = state.LineNumber;

            _stateStack.Remove(_stateStack.Last());
        }

        private class LineSourceState
        {
            public LineSourceState(string fileName, string[] lines, int lineNumber)
            {
                FileName = fileName;
                Lines = lines;
                LineNumber = lineNumber;
            }

            public string FileName { get; }

            public string[] Lines { get; }

            public int LineNumber { get; }
        }
    }
}
