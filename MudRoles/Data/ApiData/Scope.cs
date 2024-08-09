
namespace MudRoles.Data.ApiData
{
    /// <summary>  
    /// Represents an API scope with associated metadata.  
    /// </summary>  
    public class Scope
    {
        /// <summary>  
        /// Gets or sets the controller name associated with the scope.  
        /// </summary>  
        public string? Controller { get; set; }

        /// <summary>  
        /// Gets or sets the method name associated with the scope.  
        /// </summary>  
        public string? Method { get; set; }

        /// <summary>  
        /// Gets or sets the HTTP verb associated with the scope.  
        /// </summary>  
        public string? Verb { get; set; }

        /// <summary>  
        /// Gets or sets the endpoint associated with the scope.  
        /// </summary>  
        public string? Endpoint { get; set; }

        /// <summary>  
        /// Gets or sets a value indicating whether the scope is checked. 
        /// This property is not persisted it is only used for display purposes 
        /// </summary>  
        public bool IsChecked { get; set; }
    }
}
