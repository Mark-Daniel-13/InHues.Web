namespace InHues.Components.ViewModels.Auth
{
    public class Login
    {
        #if DEBUG
                public string Email { get; set; } = "admin@inhues.com";
                public string Password { get; set; } = "Pmmd.03162019!";
        #else
                public string Email { get; set; }
                public string Password { get; set; }
        #endif
    }
}
