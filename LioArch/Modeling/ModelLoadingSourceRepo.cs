using System.Collections.Generic;
using System.IO;

namespace LioArch.Modeling
{
    /// <summary>
    /// //TODO #todo static sources repo - due to the dev rush
    /// </summary>
    public static class ModelLoadingSourceRepo
    {
        private static readonly Dictionary<string, (string DbName, string FolderPath)> ModelingSources = 
            new Dictionary<string, (string, string)>();
        private static readonly string DraftModelingRootPath = @"Modeling";
        private static readonly string ModelsRootPath = @"Models";

        static ModelLoadingSourceRepo()
        {
            ModelingSources.Add("LioSystem", ("lioarch", Path.Combine(DraftModelingRootPath, "LioSoftwareSystem")));
            ModelingSources.Add("LBSLSystem", ("lioarch-lbsl", Path.Combine(DraftModelingRootPath, "LioSoftwareSystemLbsl")));
            
            // Next step approach - getting more detailed level for software systems with SubSystems used in Trapeze dev in 2020
            ModelingSources.Add("Lio-DAC", ("lioarch-DAC", Path.Combine(ModelsRootPath, @"Lio-DAC")));

        }

        public static ModelLoadingSource Get(string id)
        {
            if (ModelingSources.TryGetValue(id, out var loadingSource))
            {
                return new ModelLoadingSource
                {
                    DbName = loadingSource.DbName,
                    FolderPath = loadingSource.FolderPath,
                };
            }

            return null;
        }
    }
}
