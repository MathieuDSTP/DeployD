﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deployd.Agent.Bootstrap;
using Deployd.Agent.WebUi.Models;
using Deployd.Core.AgentConfiguration;
using Ninject;
using Nancy.Responses;
using Nancy;
using Nancy.ModelBinding;
using NuGet;

namespace Deployd.Agent.WebUi.Modules
{
    public class UpgradedModule : SecureModule
    {
        public UpgradedModule() : base("/upgraded")
        {
            Get["/configuration"] = ctx =>
                {
                    var agentWatchList = RequestScope.Get<IAgentWatchListManager>();
                    var agentSettings = RequestScope.Get<IAgentSettingsManager>();

                    var configurationViewModel = new ConfigurationViewModel()
                    {
                        WatchList = agentWatchList.Load(),
                        Settings = agentSettings.Settings
                    };

                    return Negotiate.WithView("upgraded/configuration.cshtml").WithModel(configurationViewModel);
                };
            Post["/configuration"] = x =>
            {
                var settingsManager = RequestScope.Get<IAgentSettingsManager>();
                settingsManager.UpdateSettings(this.Bind<AgentSettings>());
                return Response.AsRedirect("~/upgraded/watchList");
            };

            Get["/watchList"] = x =>
            {
                var watchList = RequestScope.Get<IAgentWatchListManager>().Load();
                var settings = RequestScope.Get<IAgentSettingsManager>().Settings;
                var feed = RequestScope.Get<IPackageRepositoryFactory>().CreateRepository(settings.NuGetRepository);
                var packages = feed.GetPackages().ToList();
                var groupedTags = packages.Select(p => (p.Tags ?? "").Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                var allTags = new HashSet<string>();
                foreach (var tagGroup in groupedTags)
                {
                    foreach (var tag in tagGroup)
                        if (!allTags.Contains(tag))
                        {
                            allTags.Add(tag);
                        }
                }
                var viewModel = new WatchListConfigurationViewModel();
                viewModel.Groups = allTags
                    .OrderBy(t => t)
                    .Select(t => new WatchListItemViewModel()
                    {
                        Name = t,
                        Selected = watchList.Groups.Any(group => group.Equals(t, StringComparison.InvariantCultureIgnoreCase))
                    });
                viewModel.Packages = packages
                    .GroupBy(p => p.Id).OrderBy(p => p.Key)
                    .Select(p => new WatchListItemViewModel()
                    {
                        Name = p.Key,
                        Selected = watchList.Packages.Any(package => package.Name.Equals(p.Key, StringComparison.InvariantCultureIgnoreCase)),
                        AutoDeploy = watchList.Packages.Any(package => package.Name.Equals(p.Key, StringComparison.InvariantCultureIgnoreCase) && package.AutoDeploy)
                    });

                return Negotiate.WithView("upgraded/watchlist.cshtml").WithModel(viewModel);
            };

            Post["/watchList"] = x =>
            {
                var settings = RequestScope.Get<IAgentSettingsManager>().Settings;
                var feed = RequestScope.Get<IPackageRepositoryFactory>().CreateRepository(settings.NuGetRepository);
                var packages = feed.GetPackages().ToList();
                var groupedTags = packages.Select(p => (p.Tags ?? "").Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                var allTags = new HashSet<string>();
                foreach (var tagGroup in groupedTags)
                {
                    foreach (var tag in tagGroup)
                        if (!allTags.Contains(tag))
                        {
                            allTags.Add(tag);
                        }
                }
                var groups = allTags.OrderBy(t => t).ToList();
                var packageIds = packages.GroupBy(p => p.Id).OrderBy(p => p.Key).Select(p => p.Key);

                var watchLIstManager = RequestScope.Get<IAgentWatchListManager>();
                var watchList = new AgentWatchList();
                foreach (var group in groups)
                {
                    bool selected = false;
                    bool.TryParse(Request.Form[group], out selected);
                    if (selected)
                    {
                        watchList.Groups.Add(group);
                    }
                }
                foreach (var packageId in packageIds)
                {
                    bool selected = false;
                    bool.TryParse(Request.Form[packageId], out selected);
                    if (selected)
                    {
                        bool auto = false;
                        bool.TryParse(Request.Form[packageId + "_auto"], out auto);
                        watchList.Packages.Add(new WatchPackage() { Name = packageId, AutoDeploy = auto });
                    }
                }

                watchLIstManager.SaveWatchList(watchList);

                var versionManager = RequestScope.Get<IApplicationVersionManager>();
                versionManager.SetVersion(versionManager.Version, false);

                return Response.AsRedirect("/");
            };
        }
    }
}