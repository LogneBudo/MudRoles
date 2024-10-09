using MudRoles.Client.Components;

namespace MudRoles.Infrastructure.Converter
{
    public static class RoleConverter
    {
        public static List<RoleItem> ConvertToRoleItems(IList<string> userRoles)
        {
            return userRoles.Select(role => new RoleItem
            {
                RoleName = role,
                IsAssigned = true // Since these roles are already assigned to the user
            }).ToList();
        }
    }
}
