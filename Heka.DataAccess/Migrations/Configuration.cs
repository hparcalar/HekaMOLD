namespace Heka.DataAccess.Migrations
{
    using Heka.DataAccess.Context;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<Heka.DataAccess.Context.HekaEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Heka.DataAccess.Context.HekaEntities context)
        {
            try
            {
                #region CREATE DEFAULT PLANT
                var defaultPlant = context.Plant.FirstOrDefault();
                if (!context.Plant.Any())
                {
                    defaultPlant = new Plant
                    {
                        PlantCode = "001",
                        PlantName = "PLANT",
                    };
                    context.Plant.Add(defaultPlant);
                }
                #endregion

                #region CREATE USER AUTH TYPES
                List<UserAuthType> currentAuthTypes = context.UserAuthType.ToList();
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "POApproval"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "POApproval", AuthTypeName = "Satınalma Talep Onayı" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }

                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "POPriceApproval"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "POPriceApproval", AuthTypeName = "Satınalma Sipariş Fiyat Onayı" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }

                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "PWOApproval"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "PWOApproval", AuthTypeName = "Satınalma İrsaliye - Sipariş Onayı" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "POCreate"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "POCreate", AuthTypeName = "Satınalma Sipariş Oluşturma" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "MobileProductionUser"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "MobileProductionUser", AuthTypeName = "Mobil Üretim Terminali" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "MobileMechanicUser"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "MobileMechanicUser", AuthTypeName = "Mobil Bakım Terminali" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "MobileWarehouseUser"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "MobileWarehouseUser", AuthTypeName = "Mobil Depo Terminali" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "ModuleQuality"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "ModuleQuality", AuthTypeName = "Kalite Modülü" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "ModuleItems"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "ModuleItems", AuthTypeName = "Stok Modülü" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "ModuleProduction"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "ModuleProduction", AuthTypeName = "Üretim Modülü" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "ModuleDefinitions"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "ModuleDefinitions", AuthTypeName = "Tanımlar" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "IsSystemAdmin"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "IsSystemAdmin", AuthTypeName = "Sistem Yöneticisi" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "IsProductionChief"))
                {
                    var aType = new UserAuthType { AuthTypeCode = "IsProductionChief", AuthTypeName = "Üretim Şefi" };
                    currentAuthTypes.Add(aType);
                    context.UserAuthType.Add(aType);
                }
                #endregion

                #region CREATE SYSTEM ADMIN ROLE & USER
                UserRole adminRole = context.UserRole.FirstOrDefault(d => d.RoleName == "System Admin");
                if (!context.UserRole.Any(d => d.RoleName == "System Admin"))
                {
                    adminRole = new UserRole
                    {
                        Plant = defaultPlant,
                        RoleName = "System Admin",
                    };

                    context.UserRole.Add(adminRole);

                    foreach (var item in currentAuthTypes)
                    {
                        if (item.AuthTypeCode.Contains("Mobile"))
                            continue;

                        context.UserAuth.Add(new UserAuth
                        {
                            UserAuthType = item,
                            IsGranted = true,
                            UserRole = adminRole,
                        });
                    }
                }

                if (!context.User.Any(d => d.UserCode == "SysAdmin"))
                {
                    context.User.Add(new User
                    {
                        UserCode = "SysAdmin",
                        Plant = defaultPlant,
                        Login = "SysAdmin",
                        UserName = "SysAdmin",
                        Password = "root",
                        CreatedDate = DateTime.Now,
                        UserRole = adminRole,
                    });
                }
                #endregion

                if (context.ChangeTracker.HasChanges())
                    context.SaveChanges();
            }
            catch (Exception ex)
            {

            }


        }
    }
}