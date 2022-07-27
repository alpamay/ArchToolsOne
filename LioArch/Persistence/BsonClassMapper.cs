using System;
using System.Collections.Generic;
using System.Text;
using LioArch.Entities;
using LioArch.Modeling;
using MongoDB.Bson.Serialization;

namespace LioArch.Persistence
{
    public static class BsonClassMapper
    {
        public static void MapToBson()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(TechnologyDescriptor)))
                BsonClassMap.RegisterClassMap<TechnologyDescriptor>(cm =>
                {
                    cm.MapIdMember(td => td.IdName);
                    cm.AutoMap();
                });

            if (!BsonClassMap.IsClassMapRegistered(typeof(LioSoftwareSystem)))
                BsonClassMap.RegisterClassMap<LioSoftwareSystem>(cm =>
                {
                    cm.MapIdMember(swss => swss.Id);
                    cm.AutoMap();
                });

            if (!BsonClassMap.IsClassMapRegistered(typeof(LioContainer)))
                BsonClassMap.RegisterClassMap<LioContainer>(cm =>
                {
                    cm.MapIdMember(cnt => cnt.Id);
                    cm.AutoMap();
                });

            if (!BsonClassMap.IsClassMapRegistered(typeof(LioRelation)))
                BsonClassMap.RegisterClassMap<LioRelation>(cm =>
                {
                    cm.MapIdMember(rel => rel.Name);
                    cm.AutoMap();
                });

            if (!BsonClassMap.IsClassMapRegistered(typeof(LioChildRelation)))
                BsonClassMap.RegisterClassMap<LioChildRelation>(cm =>
                {
                    cm.MapIdMember(rel => rel.Id);
                    cm.AutoMap();
                });

            if (!BsonClassMap.IsClassMapRegistered(typeof(LioContainerChildRelation)))
                BsonClassMap.RegisterClassMap<LioContainerChildRelation>(cm =>
                {
                    cm.AutoMap();
                });

            if (!BsonClassMap.IsClassMapRegistered(typeof(LioPersona)))
                BsonClassMap.RegisterClassMap<LioPersona>(cm =>
                {
                    cm.MapIdMember(rel => rel.CanonicalName);
                    cm.AutoMap();
                });

            if (!BsonClassMap.IsClassMapRegistered(typeof(LioPersonaUsesRelation)))
                BsonClassMap.RegisterClassMap<LioPersonaUsesRelation>(cm =>
                {
                    cm.AutoMap();
                });
        }
    }
}
