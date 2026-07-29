using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OzBiPortalCRM.Data;
using OzBiPortalCRM.Models;
using BCrypt.Net;

namespace OzBiPortalCRM.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public UserService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task SeedDefaultUserAsync()
        {
            var appDir = Path.Combine(Directory.GetCurrentDirectory(), "app");
            if (!Directory.Exists(appDir))
            {
                Directory.CreateDirectory(appDir);
            }

            using var db = await _dbFactory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();

            const string seedEmail = "eren@ozbiapp.com.tr";
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == seedEmail.ToLower());

            if (existingUser == null)
            {
                var seedUser = new PortalUser
                {
                    Email = seedEmail,
                    FullName = "Eren Çülcüoğlu",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                db.Users.Add(seedUser);
                await db.SaveChangesAsync();
            }
        }

        public async Task<PortalUser?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            using var db = await _dbFactory.CreateDbContextAsync();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower() && u.IsActive);

            if (user == null)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isValid)
                return null;

            user.LastLoginAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return user;
        }

        public async Task<List<PortalUser>> GetAllUsersAsync()
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Users.AsNoTracking().OrderByDescending(u => u.CreatedAt).ToListAsync();
        }

        public async Task<PortalUser?> GetUserByIdAsync(int id)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            return await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<(bool Success, string Message)> CreateUserAsync(PortalUser user, string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(user.FullName))
                return (false, "Ad Soyad alanı boş olamaz.");

            if (string.IsNullOrWhiteSpace(user.Email))
                return (false, "E-posta adresi boş olamaz.");

            if (string.IsNullOrWhiteSpace(plainPassword) || plainPassword.Length < 6)
                return (false, "Şifre en az 6 karakter olmalıdır.");

            using var db = await _dbFactory.CreateDbContextAsync();

            var normalizedEmail = user.Email.Trim().ToLower();
            if (await db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail))
                return (false, "Bu e-posta adresi ile zaten bir kullanıcı kayıtlı.");

            user.Email = normalizedEmail;
            user.FullName = user.FullName.Trim();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            user.CreatedAt = DateTime.UtcNow;

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return (true, "Kullanıcı başarıyla oluşturuldu.");
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(PortalUser user, string? newPassword = null)
        {
            if (string.IsNullOrWhiteSpace(user.FullName))
                return (false, "Ad Soyad alanı boş olamaz.");

            if (string.IsNullOrWhiteSpace(user.Email))
                return (false, "E-posta adresi boş olamaz.");

            using var db = await _dbFactory.CreateDbContextAsync();
            var existing = await db.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existing == null)
                return (false, "Güncellenecek kullanıcı bulunamadı.");

            var normalizedEmail = user.Email.Trim().ToLower();
            if (await db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.Id != user.Id))
                return (false, "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");

            existing.Email = normalizedEmail;
            existing.FullName = user.FullName.Trim();
            existing.Role = user.Role;
            existing.IsActive = user.IsActive;

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                if (newPassword.Length < 6)
                    return (false, "Yeni şifre en az 6 karakter olmalıdır.");

                existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            await db.SaveChangesAsync();
            return (true, $"{existing.FullName} kullanıcısı başarıyla güncellendi.");
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;

            // Protect seed user from deletion
            if (user.Email.Equals("eren@ozbiapp.com.tr", StringComparison.OrdinalIgnoreCase))
                return false;

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
