// <copyright file="GlobalSuppressions.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1652:Enable XML documentation output", Justification = "XML output not wanted or needed")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements must appear in the correct order", Justification = "Many unit tests require specific classes for tests. More readable if they appear before the relevant test method.")]
