using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace InHues.Web.Pages.Host
{
    [Authorize]
    public class AdminBase : ComponentBase
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
    }
}
