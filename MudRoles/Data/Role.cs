using Microsoft.AspNetCore.Identity;

namespace MudRoles.Data
{
    public class Role : IdentityRole
    {
        public Role() : base()
        {
        }

        public Role(string roleName) : base(roleName)
        {
        }

    }
}
