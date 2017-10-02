﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Mad2WordLib
{
    public class MadokoAttribute
    {
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
                int colonIndex = specifier.IndexOf(':');
                if (colonIndex < 1 || colonIndex >= specifier.Length - 1)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.ErrorInvalidAttributeSyntax,
                            input));
                }

                string key = specifier.Substring(0, colonIndex).Trim();
                string value = specifier.Substring(colonIndex + 1).Trim();

                attributes.Add(new MadokoAttribute(key, value));
            }

            return attributes.ToArray();
        }
    }
}