// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoDocumentTests
    {
        [Fact]
        public void MadokoDocument_ReadsHeaders()
        {
            const string Input = "# Chapter 1\n## Section 1.1\n# Chapter 2";

            using (var reader = new StringReader(Input))
            {
                var document = MadokoDocument.Read(reader);

                document.Blocks.Count.Should().Be(3);
                var headings = document.Blocks.Cast<MadokoHeading>().ToList();
                headings[0].Level.Should().Be(1);
                headings[0].Runs[0].Text.Should().Be("Chapter 1");
                headings[1].Level.Should().Be(2);
                headings[1].Runs[0].Text.Should().Be("Section 1.1");
                headings[2].Level.Should().Be(1);
                headings[2].Runs[0].Text.Should().Be("Chapter 2");
            }
        }
    }
}
