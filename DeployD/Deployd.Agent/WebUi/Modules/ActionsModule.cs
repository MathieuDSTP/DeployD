﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deployd.Agent.Services;
using Deployd.Agent.WebUi.Models;
using Deployd.Core.AgentManagement;
using Deployd.Core.Hosting;
using Deployd.Core.PackageCaching;
using Nancy;
using Nancy.Helpers;
using Nancy.Responses;

namespace Deployd.Agent.WebUi.Modules
{
    public class ActionsModule : NancyModule
    {
        public static Func<IIocContainer> Container { get; set; }
        public ActionsModule():base("/actions")
        {
            Get["/"] = x =>
            {
                var packageCache = Container().GetType<ILocalPackageCache>();

                var viewModel = packageCache.AllCachedPackages();

                return Negotiate.WithView("actions/index.cshtml").WithModel(viewModel);
            };

            Get["/{actionTaskId}"] = x =>
            {
                var actionsRepository = Container().GetType<ActionExecutionService>();
                ActionTask action = actionsRepository.GetAction(x.actionTaskId);

                return Negotiate.WithView("actions/task.cshtml").WithModel(action);
            };

            Get["/{packageId}/{action}"] = x =>
            {
                var pendingActions = Container().GetType<PendingActionsQueue>();
                var runningActions = Container().GetType<RunningActionsList>();
                var completedActions = Container().GetType<CompletedActionsList>();
                string unencodedScriptName = HttpUtility.UrlDecode(x.action);

                var viewModel = new ActionDetailsViewModel()
                {
                    Pending=pendingActions.Where(a=>a.PackageId==x.packageId && a.ScriptName==unencodedScriptName),
                    Running= runningActions.Where(a => a.PackageId == x.packageId && a.ScriptName == unencodedScriptName),
                    Completed= completedActions.Where(a => a.PackageId == x.packageId && a.ScriptName == unencodedScriptName),
                    PackageId = x.packageId,
                    Action = x.action
                };
                return Negotiate.WithView("actions/action.cshtml").WithModel(viewModel);
            };

            Post["/{packageId}/{action}/run"] = x =>
            {
                var actionQueue = Container().GetType<PendingActionsQueue>();
                var actionRepository = Container().GetType<IAgentActionsRepository>();
                string unencodedScriptName = HttpUtility.UrlDecode(x.action);
                var action = actionRepository.GetAction(x.packageId, unencodedScriptName);
                var task = actionQueue.Add(action.PackageId, unencodedScriptName, action.ScriptPath);
                string taskId = task.Id;
                return Response.AsRedirect("/actions/"+taskId);
            };
        }
    }
}
