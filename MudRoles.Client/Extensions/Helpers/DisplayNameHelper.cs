using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace MudRoles.Client.Extensions.Helpers
{
    public static class DisplayNameHelper
    {
        public static string GetDisplayName<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                var displayAttribute = memberExpression.Member.GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute != null && !string.IsNullOrEmpty(displayAttribute.Name))
                {
                    return displayAttribute.Name;
                }
            }
            return string.Empty;
        }
    }
}
