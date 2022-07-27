namespace LioArch.Entities
{
    public class TechnologyDescriptor
    {
        public string TechCanonicalName { get; private set; }
        public string IdName => TechCanonicalName;
        public string SuppliersName { get; set; }
        public decimal AnnualCostDevelopment { get; set; }
        public string License { get; set; }
        public string Version { get; set; }
    }

    public abstract class TechnologyBase
    {
        protected TechnologyBase(string techCanonicalName)
        {
            TechCanonicalName = techCanonicalName;
        }

        public string TechCanonicalName { get; private set; }
        public string IdName => TechCanonicalName;
        public string SuppliersName { get; set; }
        public decimal AnnualCostDevelopment { get; set; }
        public string License { get; set; }
        public string Version { get; set; }
    }

    public class DbTechnology : TechnologyBase
    {
        public DbTechnology(string techCanonicalName) : base(techCanonicalName)
        {
            
        }
    }
}
