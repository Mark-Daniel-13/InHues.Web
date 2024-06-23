namespace InHues.Components.ViewModels.Auth
{
    public class Login
    {
        #if DEBUG
                public string Email { get; set; } = "dev_test@gmail.com";
                public string Password { get; set; } = "Test123!";
        #else
                public string Email { get; set; }
                public string Password { get; set; }
        #endif
    }
}
