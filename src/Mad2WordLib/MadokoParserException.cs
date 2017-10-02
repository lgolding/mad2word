// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Mad2WordLib
{
    public class MadokoParserException : Exception
    {
        public MadokoParserException() : base() { }

        public MadokoParserException(string message) : base(message) { }

        public MadokoParserException(string message, Exception innerException) : base(message, innerException) { }

        public MadokoParserException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public int LineNumber { get; set; }

        public override string Message
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, Resources.ParserErrorMessageFormat, LineNumber, base.Message);
            }
        }
    }
}
