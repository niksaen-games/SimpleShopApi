using Microsoft.EntityFrameworkCore;
using SimpleShopApi.Data.Entities;

namespace SimpleShopApi.Data.Repositories
{
    public class ApiKeyRepository(AppDbContext appDbContext)
    {
        public async Task<ApiKey?> GetByIdAsync(Guid id)
        {
            return await (from key in appDbContext.ApiKeys where key.Id == id select key).FirstOrDefaultAsync();
        }
        public async Task DeleteByIdAsync(Guid id) 
        {
            var key = await GetByIdAsync(id);
            if (key == null) return;
            appDbContext.ApiKeys.Remove(key);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<ApiKey> UpdateAsync(ApiKey apiKey)
        {
            appDbContext.ApiKeys.Update(apiKey);
            await appDbContext.SaveChangesAsync();
            return apiKey;
        }

        public async Task<ApiKey> AddAsync(ApiKey apiKey)
        {
            await appDbContext.ApiKeys.AddAsync(apiKey);
            await appDbContext.SaveChangesAsync();
            return apiKey;
        }
    }
}
