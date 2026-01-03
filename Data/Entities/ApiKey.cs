using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SimpleShopApi.Data.Entities
{
    [Flags]
    public enum ApiPermissions
    {
        None = 0,
        ViewProducts = 1,
        EditProducts = 2,
        All = ViewProducts | EditProducts
    }
    public class ApiKey
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiriesAt { get; set; }
        public ApiPermissions Permissions { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
