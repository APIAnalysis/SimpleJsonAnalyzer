// <copyright file="Placeholder.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RapidXamlToolkit;

public class Placeholder
{
    public const string PropertyName = "$name$";

    public const string PropertyNameWithSpaces = "$namewithspaces$";

    public const string PropertyType = "$type$";

    public const string IncrementingInteger = "$incint$";

    public const string RepeatingInteger = "$repint$";

    public const string SubProperties = "$subprops$";

    public const string EnumMembers = "$members$";

    public const string EnumElement = "$element$";

    public const string EnumElementWithSpaces = "$elementwithspaces$";

    public const string EnumPropName = "$enumname$";

    public const string ViewProject = "$viewproject$";

    public const string ViewNamespace = "$viewns$";

    public const string ViewModelNamespace = "$viewmodelns$";

    public const string ViewClass = "$viewclass$";

    public const string ViewModelClass = "$viewmodelclass$";

    public const string GeneratedXAML = "$genxaml$";

    public const string NoOutput = "$nooutput$";

    private static List<string> all = null;

    public static List<string> All()
    {
        if (all == null)
        {
            Type type = typeof(Placeholder);
            var flags = BindingFlags.Static | BindingFlags.Public;
            var fields = type.GetFields(flags).Where(f => f.IsLiteral);

            all = fields.Select(f => f.GetValue(null).ToString()).ToList();
        }

        return all;
    }
}