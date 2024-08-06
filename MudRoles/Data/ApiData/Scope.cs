namespace MudRoles.Data.ApiData
{
    public class Scope
    {
        public string? Controller { get; set; }
        public string? Method { get; set; }
        public string? Verb { get; set; }
        public string? Endpoint { get; set; }
        public bool IsChecked { get; set; } // Add this property to indicate if the scope is checked
    }

}
