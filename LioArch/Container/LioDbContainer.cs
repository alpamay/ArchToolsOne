using System;
using System.Collections.Generic;
using System.Text;
using LioArch.Entities;
using Microsoft.EntityFrameworkCore.Query.Internal;


namespace LioArch.Container
{
    public class LioRelation
    {
        public string ShortIdName { get; set; }
        public string DisplayName { get; set; }
        public LioElement From { get; set; }
        public LioElement To { get; set; }
    }

    [Flags]
    public enum LioElementType
    {
        SoftwareSystemExternal= 2,
        SoftwareSystemInternal = 4,
        LioContainer = 8,
        LioComponent = 16,
        LioDeploymentNode = 32,
        LioModule = 64,
    }

    public class LioElement
    {
        public LioElement(string shortIdName, string displayName, LioElement type, TechnologyDescriptor technology, IList<LioRelation> related)
        {
            ShortIdName = shortIdName;
            DisplayName = displayName;
            Type = type;
            Technology = technology;
            Related = related;
        }

        public virtual string ShortIdName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual LioElement Type { get; set; }
        public virtual TechnologyDescriptor Technology { get; set;}
        public virtual IList<LioRelation> Related { get; set; }
    }
}
