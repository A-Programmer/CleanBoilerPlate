﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Project.Common.Utilities;
using Project.Entities;
using Project.Entities.EntityClasses.TestEntities;
using Project.Data.Extensions;

namespace Project.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<TestEntity> TestEntities { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ////Add entities as a DbSet dynamically
            //var entitiesAssembly = typeof(IEntity).Assembly;
            //modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);

            #region Register Model Configurations
            //if TypeConfigurations used
            //modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
            #endregion

            //Manage delete behevior
            modelBuilder.AddRestrictDeleteBehaviorConvention();

            //If Guid used for id for entity in SQL Server database
            //modelBuilder.AddSequentialGuidForIdConvention();

            ////Pluraliazing table names if RegisterAllEntities used
            //modelBuilder.AddPluralizingTableNameConvention();


        }

        #region Override SaveChanges methods
        public override int SaveChanges()
        {
            FixYeke();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            FixYeke();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            FixYeke();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            FixYeke();
            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion

        #region Fix Persian Chars
        public void FixYeke()
        {
            var changedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var propName = property.Name;
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
        }
        #endregion
    }
}
