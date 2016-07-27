// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using DocumentFormat.OpenXml.Wordprocessing;

namespace Mad2WordLib
{
    internal static class RunExtensions
    {
        internal static void SetStyle(this Run r, string styleId)
        {
            if (r.RunProperties == null)
            {
                r.RunProperties = new RunProperties();
            }

            r.RunProperties.RunStyle = new RunStyle { Val = styleId };
        }
    }
}
