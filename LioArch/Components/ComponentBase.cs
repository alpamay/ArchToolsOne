using Structurizr;
using Structurizr.Core;

namespace LioArch.Components
{
    public enum ComponentType
    {
        LioCppProcess,
        LioCppClrProcess,
        LioCppService,
        LioCppClrService,
        LioManagedProcess,
        LioManagedService,

    }

    public class ComponentBase
    {
        public ComponentBase(string canonicalName)
        {
            CanonicalName = canonicalName;
        }

        public string CanonicalName { get; private set; }
        public string IdName => CanonicalName;
    }

    public class ComponentWrapper
    {
        private Component _wrappedComponent;
        private ComponentWrapper(Component wrappedComponent)
        {
            _wrappedComponent = wrappedComponent;
        }

        public static ComponentWrapper Wrap(Component insideComponent)
        {
            return new ComponentWrapper(insideComponent);
        }


    }
}