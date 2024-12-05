// <copyright file="ViewGenerationSettings.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

namespace RapidXamlToolkit.Options;

public class ViewGenerationSettings
{
    [AllowedPlaceholders(Placeholder.ViewProject, Placeholder.ViewNamespace, Placeholder.ViewModelNamespace, Placeholder.ViewClass, Placeholder.ViewModelClass, Placeholder.GeneratedXAML)]
    public string XamlPlaceholder { get; set; }

    [AllowedPlaceholders(Placeholder.ViewProject, Placeholder.ViewNamespace, Placeholder.ViewModelNamespace, Placeholder.ViewClass, Placeholder.ViewModelClass, Placeholder.GeneratedXAML)]
    public string CodePlaceholder { get; set; }

    public string XamlFileSuffix { get; set; }

    public string ViewModelFileSuffix { get; set; }

    public string XamlFileDirectoryName { get; set; }

    public string ViewModelDirectoryName { get; set; }

    public bool AllInSameProject { get; set; }

    public string XamlProjectSuffix { get; set; }

    public string ViewModelProjectSuffix { get; set; }
}