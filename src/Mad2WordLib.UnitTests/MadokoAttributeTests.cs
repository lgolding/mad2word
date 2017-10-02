// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoAttributeTests
    {
        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_empty_attributes))]
        public void MadokoAttribute_Parse_parses_empty_attributes()
        {
            MadokoAttribute[] result = MadokoAttribute.Parse(string.Empty);
            result.Length.Should().Be(0);
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_all_white_space))]
        public void MadokoAttribute_Parse_parses_all_white_space()
        {
            MadokoAttribute[] result = MadokoAttribute.Parse("\t\t  \t  ");
            result.Length.Should().Be(0);
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_single_attribute))]
        public void MadokoAttribute_Parse_parses_single_attribute()
        {
            MadokoAttribute[] result = MadokoAttribute.Parse("key:value");
            result.Length.Should().Be(1);
            result[0].Key.Should().Be("key");
            result[0].Value.Should().Be("value");
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_multiple_attributes))]
        public void MadokoAttribute_Parse_parses_multiple_attributes()
        {
            MadokoAttribute[] result = MadokoAttribute.Parse("key1:value1;key2:value2");
            result.Length.Should().Be(2);
            result[0].Key.Should().Be("key1");
            result[0].Value.Should().Be("value1");
            result[1].Key.Should().Be("key2");
            result[1].Value.Should().Be("value2");
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_trims_parsed_attributes))]
        public void MadokoAttribute_Parse_trims_parsed_attributes()
        {
            MadokoAttribute[] result = MadokoAttribute.Parse("  key1  :   value1  ;  key2  :  value2  ");
            result.Length.Should().Be(2);
            result[0].Key.Should().Be("key1");
            result[0].Value.Should().Be("value1");
            result[1].Key.Should().Be("key2");
            result[1].Value.Should().Be("value2");
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_throws_on_null_input))]
        public void MadokoAttribute_Parse_throws_on_null_input()
        {
            Action action = () => MadokoAttribute.Parse(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_throws_on_leading_colon))]
        public void MadokoAttribute_Parse_throws_on_leading_colon()
        {
            Action action = () => MadokoAttribute.Parse(":value");

            action.ShouldThrowExactly<ArgumentException>();
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_throws_on_trailing_colon))]
        public void MadokoAttribute_Parse_throws_on_trailing_colon()
        {
            Action action = () => MadokoAttribute.Parse("key:");

            action.ShouldThrowExactly<ArgumentException>();
        }
    }
}
