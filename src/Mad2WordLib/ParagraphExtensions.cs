﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using DocumentFormat.OpenXml.Wordprocessing;

namespace Mad2WordLib
{
    internal static class ParagraphExtensions
    {
        internal static void SetStyle(this Paragraph p, string styleId)
        {
            if (p.ParagraphProperties == null)
            {
                p.ParagraphProperties = new ParagraphProperties();
            }

            p.ParagraphProperties.RemoveAllChildren<ParagraphStyleId>();
            p.ParagraphProperties.PrependChild(new ParagraphStyleId() { Val = styleId });
        }
    }
}
