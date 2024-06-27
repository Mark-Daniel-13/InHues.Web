using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace InHues.Web.Implementation
{
    public class AppKeys
    {
        private readonly IWebAssemblyHostEnvironment WAHE;
        public readonly string BackendOrigin = string.Empty;
        public AppKeys(IWebAssemblyHostEnvironment wAHE)
        {
            WAHE = wAHE;
            BackendOrigin = WAHE.Environment switch
            {
                //"Development" => "https://api.staging.dermtrics.com",
                "Development" => "https://localhost:44334",
                "Production" => "https://api.staging.dermtrics.com",
                "Staging" => "",
                _ => string.Empty
            };
        }
        public readonly string AccessToken = "ih_tkn";
        public readonly string RefreshToken = "ihr_tkn";

    }
}
