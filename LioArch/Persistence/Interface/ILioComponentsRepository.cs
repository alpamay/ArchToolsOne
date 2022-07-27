using LioArch.Entities;

namespace LioArch.Persistence.Interface
{
    public interface ILioComponentsRepository
    {
        LioComponent GetByName(string canonicalName);
    }
}