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

        internal static readonly MadokoAttribute[] EmptyAttributes = new MadokoAttribute[0];

        public MadokoAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        internal static MadokoAttribute[] Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return EmptyAttributes;
            }

            var attributes = new List<MadokoAttribute>();

            IEnumerable<string> attributeSpecifiers = input.Trim()
                .Split(';')
                .Select(s => s.Trim());

            foreach (string specifier in attributeSpecifiers)
            {
                string key = null;
                string value = null;

                int colonIndex = specifier.IndexOf(':');
                if (colonIndex < 0)
                {
                    if (specifier.StartsWith("#"))
                    {
                        key = IdAttribute;
                        value = specifier.Substring(1);
                    }
                }
                else if (colonIndex > 0 && colonIndex < specifier.Length - 1)
                {
                    key = specifier.Substring(0, colonIndex);
                    value = specifier.Substring(colonIndex + 1);
                }

                if (key == null || value == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.ErrorInvalidAttributeSyntax,
                            input));
                }

                key = key.Trim();
                value = value.Trim();

                attributes.Add(new MadokoAttribute(key, value));
            }

            return attributes.ToArray();
        }
    }
}