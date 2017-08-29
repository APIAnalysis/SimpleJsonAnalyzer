// <copyright file="PropertyInfoHelper.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System;
using System.Reflection;

namespace ApiAnalysis.UnitTests.Helpers
{
    public static class PropertyInfoHelper
    {
        public static PropertyInfo Get(Type containingClass, string propertyName)
        {
            return containingClass.GetProperty(propertyName);
        }
    }
}
