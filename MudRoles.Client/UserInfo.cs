using MudRoles.Client.Components;

namespace MudRoles.Client;

// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
public class UserInfo
{
    public string UserId { get; set; } = "";
    public string Email { get; set; } = "";
    public List<RoleItem> Roles { get; set; } = new List<RoleItem>();
}
