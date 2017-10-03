// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Mad2WordLib
{
    public class MadokoAttribute
    {
        public const string IdAttribute = "id";
        public const string ClassAttribute = "class";

        internal static readonly IDictionary<string, MadokoAttribute> EmptyAttributes = new Dictionary<string, MadokoAttribute>();

        public MadokoAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        internal static IDictionary<string, MadokoAttribute> Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            input = input.Trim();
            if (input.Length > 0 && input[0] == '-')
            {
                input = input.Substring(1);
                input.Trim();
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return EmptyAttributes;
            }

            var attributes = new Dictionary<string, MadokoAttribute>();

            IEnumerable<string> attributeSpecifiers = input.Trim()
                .Split(';')
                .Select(s => s.Trim());

            foreach (string specifier in attributeSpecifiers)
            {
                string key = null;
                string value = null;

                // We don't handle all attribute forms; don't complain about the ones we don't handle.
                bool ignore = false;

                int colonIndex = specifier.IndexOf(':');
                if (colonIndex < 0)
                {
                    if (specifier.StartsWith("#"))
                    {
                        key = IdAttribute;
                        value = specifier.Substring(1);
                    }
                    else if (specifier.StartsWith("."))
                    {
                        key = ClassAttribute;
                        value = specifier.Substring(1);
                    }
                    else if (specifier.StartsWith("@"))
                    {
                        //ignore = true;
                    }
                }
                else if (colonIndex > 0 && colonIndex < specifier.Length - 1)
                {
                    key = specifier.Substring(0, colonIndex);
                    value = specifier.Substring(colonIndex + 1);
                }

                if (!ignore)
                {
                    if (key == null || value == null)
                    {
                        throw new MadokoParserException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.ErrorInvalidAttributeSyntax,
                                specifier));
                    }

                    key = key.Trim();
                    value = value.Trim();

                    attributes.Add(key, new MadokoAttribute(key, value));
                }
            }

            return attributes;
        }
    }
}