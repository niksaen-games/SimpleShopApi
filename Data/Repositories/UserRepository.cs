using Microsoft.EntityFrameworkCore;
using SimpleShopApi.Data.Entities;

namespace SimpleShopApi.Data.Repositories
{
    public class UserRepository(AppDbContext appDbContext)
    {
        public async Task<User?> GetByIdAsync(Guid id) => 
            await (from user in appDbContext.Users where user.Id == id select user).FirstOrDefaultAsync();

        public async Task<User?> GetByEmailAsync(string email) =>
            await (from user in appDbContext.Users where user.Email == email select user).FirstOrDefaultAsync();

        public async Task DeleteByIdAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return;
            appDbContext.Users.Remove(user);
            await appDbContext.SaveChangesAsync();
        }

        public async Task<User> UpdateAsync(User user)
        {
            appDbContext.Users.Update(user);
            await appDbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> AddAsync(User user)
        {
            await appDbContext.Users.AddAsync(user);
            await appDbContext.SaveChangesAsync();
            return user;
        }
    }
}
