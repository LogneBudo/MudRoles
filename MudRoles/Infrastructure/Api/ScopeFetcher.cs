using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using MudRoles.Data.ApiData;

namespace MudRoles.Infrastructure.Api
{
    public static class ScopeFetcher
    {
        private static IActionDescriptorCollectionProvider? _actionDescriptorCollectionProvider;

        public static void Initialize(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        public static List<Scope> GetAllRoutes()
        {
            var scopes = new List<Scope>();
            if (_actionDescriptorCollectionProvider == null)
            {
                return scopes;
            }
            else
            {
                var actions = _actionDescriptorCollectionProvider.ActionDescriptors.Items;

                foreach (var action in actions)
                {
                    var attributeRoute = action.AttributeRouteInfo;
                    var controllerActionDescriptor = action as ControllerActionDescriptor;

                    if (attributeRoute != null && controllerActionDescriptor != null)
                    {
                        var httpMethodActionConstraint = action.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault();
                        var verb = httpMethodActionConstraint?.HttpMethods.FirstOrDefault() ?? "GET";

                        var scope = new Scope
                        {
                            Controller = controllerActionDescriptor.ControllerName,
                            Method = controllerActionDescriptor.ActionName,
                            Verb = verb,
                            Endpoint = attributeRoute.Template ?? string.Empty
                        };
                        scopes.Add(scope);
                    }
                }
                return scopes;
            }
        }
    }
}
