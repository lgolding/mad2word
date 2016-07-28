// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class ParagraphExtensionsTests
    {
        [Fact(DisplayName = nameof(Paragraph_SetsStyle))]
        public void Paragraph_SetsStyle()
        {
            const string StyleId = "CustomStyle";

            var p = new Paragraph();
            p.SetStyle(StyleId);

            p.ParagraphProperties.ParagraphStyleId.Val.Value.Should().Be(StyleId);
        }

        [Fact(DisplayName = nameof(Paragraph_SetsHeadingLevel))]
        public void Paragraph_SetsHeadingLevel()
        {
            const int Level = 2;

            var p = new Paragraph();
            p.SetHeadingLevel(Level);

            p.ParagraphProperties.ParagraphStyleId.Val.Value.Should().Be("Heading2");
        }
    }
}
