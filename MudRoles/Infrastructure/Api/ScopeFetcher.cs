using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using MudRoles.Data.ApiData;

namespace MudRoles.Infrastructure.Api
{
    /// <summary>
    /// Static class to fetch all API routes and their associated scopes.
    /// </summary>
    public static class ScopeFetcher
    {
        private static IActionDescriptorCollectionProvider? _actionDescriptorCollectionProvider;

        /// <summary>
        /// Initializes the ScopeFetcher with the provided action descriptor collection provider.
        /// </summary>
        /// <param name="actionDescriptorCollectionProvider">The action descriptor collection provider.</param>
        public static void Initialize(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        /// <summary>
        /// Retrieves all API routes and their associated scopes.
        /// </summary>
        /// <returns>A list of <see cref="Scope"/> objects representing the API routes.</returns>
        public static List<Scope> GetAllRoutes()
        {
            var scopes = new List<Scope>();
            if (_actionDescriptorCollectionProvider == null)
            {
                return scopes;
            }
            else
            {
                // Retrieve all action descriptors from the provider
                var actions = _actionDescriptorCollectionProvider.ActionDescriptors.Items;

                // Iterate through each action descriptor to extract route information
                foreach (var action in actions)
                {
                    var attributeRoute = action.AttributeRouteInfo;
                    var controllerActionDescriptor = action as ControllerActionDescriptor;

                    // Check if the action has a route and is a controller action
                    if (attributeRoute != null && controllerActionDescriptor != null)
                    {
                        // Get the HTTP method (verb) for the action
                        var httpMethodActionConstraint = action.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault();
                        var verb = httpMethodActionConstraint?.HttpMethods.FirstOrDefault() ?? "GET";

                        // Create a new Scope object with the extracted information
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
