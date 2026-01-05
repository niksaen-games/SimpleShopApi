using Microsoft.EntityFrameworkCore;
using SimpleShopApi.Data.Entities;
using System;

namespace SimpleShopApi.Data.Repositories
{
    public class ConfirmCodeRepository(AppDbContext appDbContext)
    {
        public async Task<ConfirmCode?> GetByIdAsync(Guid id) =>
            await (from code in appDbContext.ConfirmCodes where code.Id == id select code).FirstOrDefaultAsync();
        public async Task<ConfirmCode?> GetByEmailAsync(string email) =>
            await (from code in appDbContext.ConfirmCodes where code.Email == email select code).FirstOrDefaultAsync();

        public async Task CleanupCodesAsync()
        {
            await appDbContext.ConfirmCodes
                .Where(code => code.CreatedAt.AddMinutes(10) < DateTime.UtcNow)
                .ExecuteDeleteAsync();
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return;
            appDbContext.ConfirmCodes.Remove(user);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<ConfirmCode> UpdateAsync(ConfirmCode code)
        {
            appDbContext.ConfirmCodes.Update(code);
            await appDbContext.SaveChangesAsync();
            return code;
        }

        public async Task<ConfirmCode> AddAsync(ConfirmCode code)
        {
            await appDbContext.ConfirmCodes.AddAsync(code);
            await appDbContext.SaveChangesAsync();
            return code;
        }
    }
}
