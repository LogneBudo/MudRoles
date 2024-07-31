using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MudRoles.Data
{
    public static class ScopeFetcher
    {
        private static IActionDescriptorCollectionProvider? _actionDescriptorCollectionProvider;

        public static void Initialize(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public static List<string> GetAllRoutes()
        {
            var routes = new List<string>();
            if (_actionDescriptorCollectionProvider == null)
            {
                return routes;
            }
            else
            {
                var actions = _actionDescriptorCollectionProvider.ActionDescriptors.Items;

                foreach (var action in actions)
                {
                    var attributeRoute = action.AttributeRouteInfo;

                    if (attributeRoute != null)
                    {
                        routes.Add(attributeRoute.Template ?? string.Empty);
                    }
                }
                return routes;
            }
        }
    }
}
