using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AD.Data
{
    using AD.Model;
    using Microsoft.CodeAnalysis.Scripting;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection.Emit;

    public class AuthDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación muchos a muchos entre Perfil y Permiso
            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Permissions)
                .WithMany(p => p.Profiles)
                .UsingEntity(j => j.ToTable("ProfilePermissions"));

            // Configuración de la relación uno a muchos entre Perfil y Usuario
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.ProfileId);

            // Seeds iniciales
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Permisos
            var viewInventoryPermission = new Permission
            {
                Id = Guid.NewGuid(),
                RoleName = "InventoryView",
                Description = "Permiso para visualizar inventario"
            };

            var manageInventoryPermission = new Permission
            {
                Id = Guid.NewGuid(),
                RoleName = "InventoryManage",
                Description = "Permiso para gestionar inventario"
            };

            var pickingPermission = new Permission
            {
                Id = Guid.NewGuid(),
                RoleName = "Picking",
                Description = "Permiso para realizar operaciones de picking"
            };

            var receivingPermission = new Permission
            {
                Id = Guid.NewGuid(),
                RoleName = "Receiving",
                Description = "Permiso para recepción de mercancía"
            };

            var shiftManagementPermission = new Permission
            {
                Id = Guid.NewGuid(),
                RoleName = "ShiftManagement",
                Description = "Permiso para gestionar turnos de trabajo"
            };

            var adminPermission = new Permission
            {
                Id = Guid.NewGuid(),
                RoleName = "Admin",
                Description = "Permiso de administrador del sistema"
            };

            modelBuilder.Entity<Permission>().HasData(
                viewInventoryPermission,
                manageInventoryPermission,
                pickingPermission,
                receivingPermission,
                shiftManagementPermission,
                adminPermission
            );

            // Perfiles
            var pickerProfile = new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Picker",
                Description = "Personal de picking en almacén",
                ValidUntil = DateTime.Now.AddYears(1)
            };

            var warehouseWorkerProfile = new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Mozo de Almacén",
                Description = "Personal general de almacén",
                ValidUntil = DateTime.Now.AddYears(1)
            };

            var shiftManagerProfile = new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Jefe de Turno",
                Description = "Responsable de turno en almacén",
                ValidUntil = DateTime.Now.AddYears(1)
            };

            var adminProfile = new Profile
            {
                Id = Guid.NewGuid(),
                Name = "Administrador",
                Description = "Administrador del sistema",
                ValidUntil = DateTime.Now.AddYears(1)
            };

            modelBuilder.Entity<Profile>().HasData(
                pickerProfile,
                warehouseWorkerProfile,
                shiftManagerProfile,
                adminProfile
            );

            // Usuarios (con contraseñas hasheadas)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "picker1",
                    Password = BCrypt.Net.BCrypt.HashPassword("Picker123"),
                    FullName = "Juan Pérez",
                    Email = "jperez@warehouse.com",
                    EmployeeNumber = "E001",
                    ContactPhone = "600123456",
                    ProfileId = pickerProfile.Id
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "mozo1",
                    Password = BCrypt.Net.BCrypt.HashPassword("Mozo123"),
                    FullName = "Ana García",
                    Email = "agarcia@warehouse.com",
                    EmployeeNumber = "E002",
                    ContactPhone = "600123457",
                    ProfileId = warehouseWorkerProfile.Id
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "jefe1",
                    Password = BCrypt.Net.BCrypt.HashPassword("Jefe123"),
                    FullName = "Carlos Martínez",
                    Email = "cmartinez@warehouse.com",
                    EmployeeNumber = "E003",
                    ContactPhone = "600123458",
                    ProfileId = shiftManagerProfile.Id
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                    FullName = "María Rodríguez",
                    Email = "mrodriguez@warehouse.com",
                    EmployeeNumber = "E004",
                    ContactPhone = "600123459",
                    ProfileId = adminProfile.Id
                }
            );

            // Relaciones entre Perfiles y Permisos
            // Estas se deben agregar como datos semilla para la tabla de unión
            modelBuilder.Entity("ProfilePermission").HasData(
                // Picker tiene permisos de visualización y picking
                new { ProfilesId = pickerProfile.Id, PermissionsId = viewInventoryPermission.Id },
                new { ProfilesId = pickerProfile.Id, PermissionsId = pickingPermission.Id },

                // Mozo de almacén tiene permisos de visualización, picking y recepción
                new { ProfilesId = warehouseWorkerProfile.Id, PermissionsId = viewInventoryPermission.Id },
                new { ProfilesId = warehouseWorkerProfile.Id, PermissionsId = pickingPermission.Id },
                new { ProfilesId = warehouseWorkerProfile.Id, PermissionsId = receivingPermission.Id },

                // Jefe de turno tiene todos los permisos excepto admin
                new { ProfilesId = shiftManagerProfile.Id, PermissionsId = viewInventoryPermission.Id },
                new { ProfilesId = shiftManagerProfile.Id, PermissionsId = manageInventoryPermission.Id },
                new { ProfilesId = shiftManagerProfile.Id, PermissionsId = pickingPermission.Id },
                new { ProfilesId = shiftManagerProfile.Id, PermissionsId = receivingPermission.Id },
                new { ProfilesId = shiftManagerProfile.Id, PermissionsId = shiftManagementPermission.Id },

                // Administrador tiene todos los permisos
                new { ProfilesId = adminProfile.Id, PermissionsId = viewInventoryPermission.Id },
                new { ProfilesId = adminProfile.Id, PermissionsId = manageInventoryPermission.Id },
                new { ProfilesId = adminProfile.Id, PermissionsId = pickingPermission.Id },
                new { ProfilesId = adminProfile.Id, PermissionsId = receivingPermission.Id },
                new { ProfilesId = adminProfile.Id, PermissionsId = shiftManagementPermission.Id },
                new { ProfilesId = adminProfile.Id, PermissionsId = adminPermission.Id }
            );
        }
    }
