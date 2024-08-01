using Microsoft.AspNetCore.Mvc.ApiExplorer;
using MudRoles.Data.ApiData;

namespace MudRoles.Infrastructure.Api
{
    public class ApiEndpoints
    {
        private static List<EndPointInfo> _endpoints = new List<EndPointInfo>();

        public static void Initialize(IServiceProvider serviceProvider)
        {
            var apiDescriptionGroupCollectionProvider = serviceProvider.GetRequiredService<IApiDescriptionGroupCollectionProvider>();
            _endpoints = apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items
                .SelectMany(group => group.Items)
                .Select(apiDescription => new EndPointInfo
                {
                    Method = apiDescription.HttpMethod?.ToUpper(),
                    Path = apiDescription.RelativePath?.ToLower()
                })
                .ToList();
        }

        public static List<EndPointInfo> GetEndpoints()
        {
            return _endpoints;
        }
    }
}
