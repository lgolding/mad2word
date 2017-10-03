﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System.IO;

namespace Mad2WordLib
{
    public static class StringExtensions
    {

        public static string RootedPath(this string path, IEnvironment environment)
        {
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(environment.CurrentDirectory, path);
            }

            return path;
        }
    }
}
