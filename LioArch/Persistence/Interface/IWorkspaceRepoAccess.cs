using Structurizr;

namespace LioArch.Persistence.Interface
{
    public interface IWorkspaceRepoAccess
    {
        WorkspaceTarget GetTarget(long workspaceId);
        WorkspaceTarget GetTargetByName(string workspaceName);
        Workspace GetWorkspace(long id);
        Workspace CloneWorkspace(string canonicalName);
        Workspace CreateWorkspace(string newCanonicalName);
    }
}