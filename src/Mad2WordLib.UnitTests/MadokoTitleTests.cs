// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoTitleTests : MadokoTestBase
    {
        [Fact(DisplayName = nameof(TitleBlock_IsEmptyIfTitleMetadataIsAbsent))]
        public void TitleBlock_IsEmptyIfTitleMetadataIsAbsent()
        {
            const string Input = "[TITLE]";

            MadokoDocument document = ReadDocument(Input);

            document.Blocks.Count.Should().Be(0);
        }

        [Fact(DisplayName = nameof(TitleBlock_IsEmptyIfTitleMetadataIsBlank))]
        public void TitleBlock_IsEmptyIfTitleMetadataIsBlank()
        {
            const string Input =
// There are blank spaces at the end of the Title metadata line (which shouldn't matter
// because the metadata values are trimmed before being stored.
@"Title :  
[TITLE]";

            MadokoDocument document = ReadDocument(Input);

            document.Blocks.Count.Should().Be(0);
        }

        [Fact(DisplayName = nameof(TitleBlock_DisplaysTitleMetadata))]
        public void TitleBlock_DisplaysTitleMetadata()
        {
            const string Input =
@"Title : My Document
[TITLE]";

            MadokoDocument document = ReadDocument(Input);

            document.Blocks.Count.Should().Be(1);
            MadokoBlock titleBlock = document.Blocks[0];
            titleBlock.Should().BeOfType<MadokoTitle>();
            titleBlock.Runs.Count.Should().Be(1);
            titleBlock.Runs[0].Text.Should().Be("My Document");
        }

        [Fact(DisplayName = nameof(TitleBlock_IgnoresTrailingWhiteSpace))]
        public void TitleBlock_IgnoresTrailingWhiteSpace()
        {
        }

        [Fact(DisplayName = nameof(TitleBlock_AllowsRunStyling))]
        public void TitleBlock_AllowsRunStyling()
        {
        }
    }
}
