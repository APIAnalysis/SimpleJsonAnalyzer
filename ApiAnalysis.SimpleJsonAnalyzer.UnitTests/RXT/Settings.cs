// <copyright file="Settings.cs" company="Matt Lacey">
// Copyright (c) Matt Lacey. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RapidXamlToolkit.Options;

public class Settings : CanNotifyPropertyChanged
{
    public string ActiveProfileName { get; set; }

    public List<Profile> Profiles { get; set; }

    public bool ExtendedOutputEnabled { get; set; }

    public ObservableCollection<ProfileSummary> ProfilesList
    {
        get
        {
            var list = new ObservableCollection<ProfileSummary>();

            // If multile profiles have the same name as the active profile, use the first one in the list with matcing name
            bool activeIndicated = false;

            for (var index = 0; index < Profiles.Count; index++)
            {
                var profile = Profiles[index];

                var summary = new ProfileSummary
                {
                    Index = index,
                    Name = profile.Name,
                    IsActive = !activeIndicated && profile.Name == ActiveProfileName,
                };

                if (summary.IsActive)
                {
                    activeIndicated = true;
                }

                list.Add(summary);
            }

            return list;
        }
    }

    public bool IsActiveProfileSet => !string.IsNullOrWhiteSpace(ActiveProfileName);

    public Profile GetActiveProfile()
    {
        Profile result = null;

        if (!string.IsNullOrEmpty(ActiveProfileName))
        {
            result = Profiles.FirstOrDefault(p => p.Name == ActiveProfileName);
        }

        return result ?? Profiles.FirstOrDefault();
    }

    public List<string> GetAllProfileNames()
    {
        return Profiles.Select(p => p.Name).ToList();
    }

    public void RefreshProfilesList()
    {
        OnPropertyChanged(nameof(ProfilesList));
    }
}