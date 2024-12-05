// <copyright file="AllowedPlaceholdersAttribute.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;

namespace RapidXamlToolkit.Options;

internal class AllowedPlaceholdersAttribute : Attribute
{
    public AllowedPlaceholdersAttribute(params string[] placeholders)
    {
        Placeholders = placeholders;
    }

    public string[] Placeholders { get; }
}