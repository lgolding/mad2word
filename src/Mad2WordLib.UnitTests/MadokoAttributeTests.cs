// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Mad2WordLib.UnitTests
{
    public class MadokoAttributeTests
    {
        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_empty_attributes))]
        public void MadokoAttribute_Parse_parses_empty_attributes()
        {
            IDictionary<string, MadokoAttribute> result = MadokoAttribute.Parse(string.Empty);
            result.Count.Should().Be(0);
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_all_white_space))]
        public void MadokoAttribute_Parse_parses_all_white_space()
        {
            IDictionary<string, MadokoAttribute> result = MadokoAttribute.Parse("\t\t  \t  ");
            result.Count.Should().Be(0);
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_single_attribute))]
        public void MadokoAttribute_Parse_parses_single_attribute()
        {
            IDictionary<string, MadokoAttribute> result = MadokoAttribute.Parse("key:value");
            result.Count.Should().Be(1);
            result["key"].Key.Should().Be("key");
            result["key"].Value.Should().Be("value");
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_multiple_attributes))]
        public void MadokoAttribute_Parse_parses_multiple_attributes()
        {
            IDictionary<string, MadokoAttribute> result = MadokoAttribute.Parse("key1:value1;key2:value2");
            result.Count.Should().Be(2);
            result["key1"].Key.Should().Be("key1");
            result["key1"].Value.Should().Be("value1");
            result["key2"].Key.Should().Be("key2");
            result["key2"].Value.Should().Be("value2");
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_trims_parsed_attributes))]
        public void MadokoAttribute_Parse_trims_parsed_attributes()
        {
            IDictionary<string, MadokoAttribute> result = MadokoAttribute.Parse("  key1  :   value1  ;  key2  :  value2  ");
            result.Count.Should().Be(2);
            result["key1"].Key.Should().Be("key1");
            result["key1"].Value.Should().Be("value1");
            result["key2"].Key.Should().Be("key2");
            result["key2"].Value.Should().Be("value2");
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_id_shorthand))]
        public void MadokoAttribute_Parse_parses_id_shorthand()
        {
            IDictionary<string, MadokoAttribute> result = MadokoAttribute.Parse("#chapter-1");
            result.Count.Should().Be(1);
            result[MadokoAttribute.IdAttribute].Key.Should().Be(MadokoAttribute.IdAttribute);
            result[MadokoAttribute.IdAttribute].Value.Should().Be("chapter-1");
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_class_shorthand))]
        public void MadokoAttribute_Parse_parses_class_shorthand()
        {
            IDictionary<string, MadokoAttribute> result = MadokoAttribute.Parse(".title");
            result.Count.Should().Be(1);
            result[MadokoAttribute.ClassAttribute].Key.Should().Be(MadokoAttribute.ClassAttribute);
            result[MadokoAttribute.ClassAttribute].Value.Should().Be("title");
        }

        [Fact(DisplayName = nameof(MadokoAttribute_Parse_parses_mixed_attributes))]
        public void MadokoAttribute_Parse_parses_mixed_attributes()
        {
            IDictionary<string, MadokoAttribute> result = MadokoAttribute.Parse("key: value; #chapter-1; .title");
            result.Count.Should().Be(3);
            result["key"].Key.Should().Be("key");
            result["key"].Value.Should().Be("value");
            result[MadokoAttribute.IdAttribute].Key.Should().Be(MadokoAttribute.IdAttribute);
            result[MadokoAttribute.IdAttribute].Value.Should().Be("chapter-1");
            result[MadokoAttribute.ClassAttribute].Key.Should().Be(MadokoAttribute.ClassAttribute);
            result[MadokoAttribute.ClassAttribute].Value.Should().Be("title");
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
