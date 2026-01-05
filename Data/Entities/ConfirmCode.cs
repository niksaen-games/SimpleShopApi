namespace SimpleShopApi.Data.Entities
{
    public class ConfirmCode
    {
        public enum Action
        {
            ConfirmEmail,
            ResetPassword
        }
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Action ActionType { get; set; }
    }
}
