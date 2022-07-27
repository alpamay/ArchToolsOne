using LioArch.Modeling;

namespace LioArch.Persistence.Interface
{
    public interface IDeploymentNodesRepository
    {
        LioDeploymentNode GetByIdString(string idString);
    }
}