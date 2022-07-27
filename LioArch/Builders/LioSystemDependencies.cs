using Structurizr;
using ILogger = NLog.ILogger;
using Model = Structurizr.Model;


namespace LioArch.Builders
{
    public static class LioSystemDependencies
    {
        public static SoftwareSystem CreateBiSystemInModel(ref Model model)
        {
            return model.AddSoftwareSystem(
                Location.External,
                @"BISystem",
                @"Business Intelligence");
        }

        public static SoftwareSystem CreateParitySystemInModel(ref Model model)
        {
            return model.AddSoftwareSystem(
                Location.External,
                "Parity Voice System",
                "Multi radio technology voice system for single and multiple endpoints");
        }

        public static SoftwareSystem CreateLioDataSystemInModel(ref Model model)
        {
            return model.AddSoftwareSystem(
                Location.External,
                "LioData",
                "Source of Data (offline)");
        }

        public static SoftwareSystem CreateIdrFleetSystemInModel(ref Model model)
        {
            return model.AddSoftwareSystem(
                Location.External,
                "IDR Fleet System",
                "A farm of mobile OBC and on-site service hosting");
        }

    }
}