// <copyright file="ProfileSummary.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

namespace RapidXamlToolkit.Options;

public class ProfileSummary : CanNotifyPropertyChanged
{
    private bool isActive;

    public int Index { get; set; }

    public string Name { get; set; }

    public bool IsActive
    {
        get
        {
            return isActive;
        }

        set
        {
            isActive = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayName));
        }
    }

    public string DisplayName
    {
        get
        {
            if (IsActive)
            {
                return Name;
            }
            else
            {
                return Name;
            }
        }
    }
}