namespace MudRoles.Client.Models
{
    public class AvatarItem<T>
    {
        public string ImageUrl { get; set; } = string.Empty;
        public T Value { get; set; } = default!;
    }
}
