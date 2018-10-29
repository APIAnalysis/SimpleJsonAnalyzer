// <copyright file="Mapping.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;

namespace RapidXamlToolkit.Options
{
    public class Mapping : ICloneable
    {
        public string Type { get; set; }

        public string NameContains { get; set; }

        public bool IfReadOnly { get; set; }

        [AllowedPlaceholders(Placeholder.PropertyName, Placeholder.PropertyNameWithSpaces, Placeholder.PropertyType, Placeholder.IncrementingInteger, Placeholder.RepeatingInteger, Placeholder.EnumMembers, Placeholder.SubProperties)]
        public string Output { get; set; }

        public static Mapping CreateNew()
        {
            return new Mapping
            {
                Type = string.Empty,
                NameContains = string.Empty,
                Output = string.Empty,
                IfReadOnly = false,
            };
        }

        public object Clone()
        {
            return new Mapping
            {
                Type = this.Type,
                NameContains = this.NameContains,
                Output = this.Output,
                IfReadOnly = this.IfReadOnly,
            };
        }
    }
}
