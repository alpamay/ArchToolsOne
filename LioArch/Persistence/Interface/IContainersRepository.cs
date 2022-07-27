using LioArch.Entities;
using LioArch.Modeling;

namespace LioArch.Persistence.Interface
{
    public interface IContainersRepository
    {
        LioContainer GetByName(string canonicalName);
    }
}