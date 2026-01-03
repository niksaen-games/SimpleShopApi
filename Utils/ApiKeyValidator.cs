using Microsoft.EntityFrameworkCore;
using SimpleShopApi.Data;
using SimpleShopApi.Data.Entities;
using SimpleShopApi.Data.Repositories;

namespace SimpleShopApi.Utils
{
    public class ApiKeyValidator(ApiKeyRepository repository)
    {
        public async Task<bool> IsValidAsync(string apiKey)
        {
            if (!Guid.TryParse(apiKey, out var keyId)) return false;

            var key = await repository.GetByIdAsync(keyId);
            if(key == null) return false;
            if(!key.IsActive) return false;
            if(key.ExpiriesAt < DateTime.UtcNow) return false; 

            return true;
        }
    }
}
