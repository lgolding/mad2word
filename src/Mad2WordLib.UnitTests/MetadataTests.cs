// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MetadataTests
    {
        [Fact(DisplayName = nameof(Metadata_ReadsOneValue))]
        public void Metadata_ReadsOneValue()
        {
            const string Input = "Title: My Document";

            Metadata metadata = ReadMetadata(Input);

            metadata.GetValue("Title").Should().Be("My Document");
        }

        [Fact(DisplayName = nameof(Metadata_AcceptsCaseInsensitiveKeys))]
        public void Metadata_AcceptsCaseInsensitiveKeys()
        {
            const string Input = "Title: My Document";

            Metadata metadata = ReadMetadata(Input);

            metadata.GetValue("TITLE").Should().Be("My Document");
        }

        [Fact(DisplayName = nameof(Metadata_TrimsName))]
        public void Metadata_TrimsName()
        {
            const string Input = "   Title  : My Document";

            Metadata metadata = ReadMetadata(Input);

            metadata.GetValue("Title").Should().Be("My Document");
        }

        [Fact(DisplayName = nameof(Metadata_TrimsValue))]
        public void Metadata_TrimsValue()
        {
            const string Input = "Title: My Document \t ";

            Metadata metadata = ReadMetadata(Input);

            metadata.GetValue("Title").Should().Be("My Document");
        }

        [Fact(DisplayName = nameof(Metadata_ReadsValueWithSpaceInName))]
        public void Metadata_ReadsValueWithSpaceInName()
        {
            const string Input = "Heading Base: 2";

            Metadata metadata = ReadMetadata(Input);

            metadata.GetValue("Heading Base").Should().Be("2");
        }

        [Fact(DisplayName = nameof(Metadata_ReadsMultipleValues))]
        public void Metadata_ReadsMultipleValues()
        {
            const string Input =
@"Title: My Document
Heading Base: 2";

            Metadata metadata = ReadMetadata(Input);

            metadata.GetValue("Title").Should().Be("My Document");
            metadata.GetValue("Heading Base").Should().Be("2");
        }

        [Fact(DisplayName = nameof(Metadata_SkipsBlankLines))]
        public void Metadata_SkipsBlankLines()
        {
            const string Input =
@"Title: My Document

Heading Base: 2";

            Metadata metadata = ReadMetadata(Input);

            metadata.GetValue("Title").Should().Be("My Document");
            metadata.GetValue("Heading Base").Should().Be("2");
        }

        [Fact(DisplayName = nameof(Metadata_StopsAfterFirstNonMetadataLine))]
        public void Metadata_StopsAfterFirstNonMetadataLine()
        {
            const string Input =
@"Title: My Document

Heading Base: 2

Not metadata";

            Metadata metadata = ReadMetadata(Input);

            metadata.GetValue("Title").Should().Be("My Document");
            metadata.GetValue("Heading Base").Should().Be("2");
        }

        [Fact(DisplayName = nameof(Metadata_StopsAtFirstCustomBlock))]
        public void Metadata_StopsAtFirstCustomBlock()
        {
            const string Input =
@"Title: My Document

Heading Base: 2

~Note: .block before='[NOTE&nbsp;&nbsp;&nbsp;]' font-size=small";

            Metadata metadata = ReadMetadata(Input);

            metadata.GetValue("Title").Should().Be("My Document");
            metadata.GetValue("Heading Base").Should().Be("2");
        }

        [Fact(DisplayName = nameof(Metadata_ValueCanSpanMultipleLinesWithIndentation))]
        public void Metadata_ValueCanSpanMultipleLinesWithIndentation()
        {
            const string Input =
@"Title: My Document
 With a Long Title";

            Metadata metadata = ReadMetadata(Input);

            metadata.GetValue("Title").Should().Be("My Document With a Long Title");
        }

        private static Metadata ReadMetadata(string input)
        {
            Metadata metadata = null;

            var fileSystem = new FakeFileSystem();
            using (TextReader reader = new StringReader(input))
            {
                var lineSource = new LineSource(reader, fileSystem);

                metadata = Metadata.Read(lineSource);
            }

            return metadata;
        }
    }
}
