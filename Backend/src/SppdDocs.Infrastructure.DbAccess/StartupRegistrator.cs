﻿using System;
using System.Linq;
using System.Reflection;

using log4net;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SppdDocs.Core;
using SppdDocs.Core.Config;
using SppdDocs.Core.Repositories;
using SppdDocs.Infrastructure.DbAccess.Config;
using SppdDocs.Infrastructure.DbAccess.EntityMetadataProviders;
using SppdDocs.Infrastructure.DbAccess.Repositories;
using SppdDocs.Infrastructure.DbAccess.Seeders;

namespace SppdDocs.Infrastructure.DbAccess
{
    public class StartupRegistrator : IStartupRegistrator
    {
        private static readonly ILog s_logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public int Priority => 1000;

        public void RegisterService(IServiceCollection services)
        {
            var databaseConfig = GetDatabaseConfig(services.BuildServiceProvider());

            // SppdContext
            services.AddDbContext<SppdContext>(options => options.UseSqlServer(databaseConfig.ConnectionString));

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IVersionedEntityRepository<>), typeof(VersionedEntityRepository<>));

            services.AddScoped(typeof(ICardRepository), typeof(CardRepository));

            // Entity metadata providers
            services.AddScoped(typeof(IEntityMetadataProvider), typeof(BaseEntityMetadataProvider));
            services.AddScoped(typeof(IEntityMetadataProvider), typeof(VersionedEntityMetadataProvider));

            // DB Seeders
            if (databaseConfig.ManageDatabase)
            {
                services.AddScoped(typeof(IDbSeeder), typeof(CardDbSeeder));
                services.AddScoped(typeof(IDbSeeder), typeof(RarityDbSeeder));
                services.AddScoped(typeof(IDbSeeder), typeof(CardClassDbSeeder));
                services.AddScoped(typeof(IDbSeeder), typeof(CardEffectDbSeeder));
                services.AddScoped(typeof(IDbSeeder), typeof(CardStatusEffectDbSeeder));
                services.AddScoped(typeof(IDbSeeder), typeof(CardThemeDbSeeder));
                services.AddScoped(typeof(IDbSeeder), typeof(CardAttributeDbSeeder));
                services.AddScoped(typeof(IDbSeeder), typeof(CardCastAreaDbSeeder));
            }
        }

        public void ConfigureService(IServiceProvider serviceProvider)
        {
            var databaseConfig = GetDatabaseConfig(serviceProvider);

            using (var serviceScope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<SppdContext>();

                if (databaseConfig.ManageDatabase)
                {
                    s_logger.Debug("Start recreating database");
                    if (context.Database.EnsureDeleted())
                    {
                        s_logger.Debug("Database has been deleted");
                    }

                    //DbInterception.Add(new CreateDatabaseCollationInterceptor("Latin1_General_100_CI_AS"));
                    if (context.Database.EnsureCreated())
                    {
                        s_logger.Debug("Database has been created");
                        foreach (var seeder in serviceScope.ServiceProvider.GetServices<IDbSeeder>().OrderBy(seeder => seeder.Priority))
                        {
                            seeder.Seed();
                        }

                        s_logger.Debug("Data has been seeded to database");
                    }

                    s_logger.Info("Database has been recreated and data has been seeded");
                }
                else
                {
                    context.Database.Migrate();
                    if (context.Database.GetAppliedMigrations().Any())
                    {
                        s_logger.Info($"Pending migrations have been applied: {string.Join(", ", context.Database.GetAppliedMigrations())}");
                    }
                }
            }
        }

        private static DatabaseConfig GetDatabaseConfig(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<IConfigProvider<DatabaseConfig>>().Config;
        }
    }
}