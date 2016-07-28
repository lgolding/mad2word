// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See the LICENSE file in the project root for license information.

namespace Mad2WordLib
{
    public interface IMadokoVisitor
    {
        void Visit(MadokoBlock block);
        void Visit(MadokoHeading heading);
        void Visit(MadokoBulletListItem listItem);
    }
}