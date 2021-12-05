namespace Heka.DataAccess.Migrations
{
    using Heka.DataAccess.Context;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Heka.DataAccess.Context.HekaEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Heka.DataAccess.Context.HekaEntities context)
        {
            try
            {
                #region CREATE DEFAULT PLANT
                if (!context.Plant.Any())
                {
                    context.Plant.Add(new Plant
                    {
                        PlantCode = "001",
                        PlantName = "PLANT",
                    });
                }
                #endregion

                #region CREATE USER AUTH TYPES
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "POApproval"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "POApproval", AuthTypeName = "Satınalma Talep Onayı" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "POPriceApproval"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "POPriceApproval", AuthTypeName = "Satınalma Sipariş Fiyat Onayı" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "PWOApproval"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "PWOApproval", AuthTypeName = "Satınalma İrsaliye - Sipariş Onayı" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "POCreate"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "POCreate", AuthTypeName = "Satınalma Sipariş Oluşturma" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "MobileProductionUser"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "MobileProductionUser", AuthTypeName = "Mobil Üretim Terminali" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "MobileMechanicUser"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "MobileMechanicUser", AuthTypeName = "Mobil Bakım Terminali" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "MobileWarehouseUser"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "MobileWarehouseUser", AuthTypeName = "Mobil Depo Terminali" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "ModuleQuality"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "ModuleQuality", AuthTypeName = "Kalite Modülü" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "ModuleItems"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "ModuleItems", AuthTypeName = "Stok Modülü" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "ModuleProduction"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "ModuleProduction", AuthTypeName = "Üretim Modülü" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "ModuleDefinitions"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "ModuleDefinitions", AuthTypeName = "Tanımlar" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "IsSystemAdmin"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "IsSystemAdmin", AuthTypeName = "Sistem Yöneticisi" });
                if (!context.UserAuthType.Any(d => d.AuthTypeCode == "IsProductionChief"))
                    context.UserAuthType.Add(new UserAuthType { AuthTypeCode = "IsProductionChief", AuthTypeName = "Üretim Şefi" });
                #endregion

                #region CREATE SYSTEM ADMIN ROLE & USER
                if (!context.UserRole.Any(d => d.RoleName == "System Admin"))
                {
                    var adminRole = new UserRole
                    {
                        Plant = context.Plant.FirstOrDefault(),
                        RoleName = "System Admin",
                    };

                    context.UserRole.Add(adminRole);

                    foreach (var item in context.UserAuthType)
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
                        Plant = context.Plant.FirstOrDefault(),
                        Login = "SysAdmin",
                        UserName = "SysAdmin",
                        Password = "root",
                        CreatedDate = DateTime.Now,
                        UserRole = context.UserRole.FirstOrDefault(m => m.RoleName == "System Admin"),
                    });
                }
                #endregion

                if (context.ChangeTracker.HasChanges())
                    context.SaveChanges();
            }
            catch (Exception)
            {

            }

            
        }
    }
}
