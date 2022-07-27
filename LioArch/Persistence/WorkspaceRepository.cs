using System;
using System.Collections.Generic;
using System.Linq;
using LioArch.Persistence.Interface;
using Structurizr;

namespace LioArch.Persistence
{
    public class WorkspaceTarget
    {
        public Workspace StructWorkspace { get; }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public long WorkspaceId { get; private set; }
        public string ApiKey { get; private set; }
        public string ApiSecret { get; private set; }
        public string FocusedSystemName { get; private set; }

        public WorkspaceTarget(string name, string description, long workspaceId, string apiKey, string apiSecret, string focusedSystemName)
        {
            Name = name;
            Description = description;
            WorkspaceId = workspaceId;
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            FocusedSystemName = focusedSystemName;

            StructWorkspace = new Workspace(Name, Description);
        }

    }
   
    public class WorkspaceRepository : IWorkspaceRepoAccess
    {

        private readonly Dictionary<long, (string ApiKey, string ApiSecretPublic, string CanonicalName, string Description, string FocusedSystem)> _workplacesInfo = 
            new Dictionary<long, (string , string , string , string , string)>
        {
            {59811, 
                ("8023ed9a-f07a-4ade-9bb8-35a957d8af17", 
                    // TODO #todo take secretCookie from commandline "cd4b370b-c6f2-409b-84e6-secret", 
                    "c7e4ea64-a449-49e8-b8fb-6b5bab556ca9",
                    "Lio Architecture Modeling - Mainstream Perspective", 
                    "Lio World version 0.3 draft",
                    "LioSystem")},
            {55779, 
                ("efba3b8b-66fb-4fbb-a3bd-8226db0b71d7", 
                    "4fb48d2c-bf7d-4ee5-9b88-secret", 
                    "Lio LBSL Architecture Modeling - LBSL Perspective", 
                    "Lio World LBSL migrated to Mainstream - version 0.1 draft",
                    "LbslSystem")}
        };

       private Dictionary<long, WorkspaceTarget> _workspaceTargets = new Dictionary<long, WorkspaceTarget>();

        public WorkspaceRepository()
        {
            foreach (var info in _workplacesInfo)
            {
                _workspaceTargets.TryAdd(info.Key, new WorkspaceTarget(
                    info.Value.CanonicalName,
                    info.Value.Description,
                    info.Key,
                    info.Value.ApiKey,
                    info.Value.ApiSecretPublic,
                    info.Value.FocusedSystem));
            }
        }

        public WorkspaceTarget GetTarget(long workspaceId)
        {
            return _workspaceTargets.Select(row => row.Value).First(target => target.WorkspaceId == workspaceId);
        }

        public WorkspaceTarget GetTargetByName(string workspaceName)
        {
            var found = _workspaceTargets
                .Where(target => target.Value.Name.Equals(workspaceName))
                .Select(target => target.Value)
                .ToList();

            if (found.Any())
            {
                return found.First();
            }

            return null;
        }

        public Workspace GetWorkspace(long id)
        {
            if (_workplacesInfo.TryGetValue(id, out var workplaceInfo))
            {
                return new Workspace(workplaceInfo.CanonicalName, workplaceInfo.Description);
            }

            // TODO #todo Integrate with Maybe<>, Switch<> pattern matching
            return null;
        }

        public Workspace CloneWorkspace(string canonicalName)
        {
            throw new NotImplementedException();
        }

        public Workspace CreateWorkspace(string newCanonicalName)
        {
            throw new NotImplementedException();
        }
    }

}