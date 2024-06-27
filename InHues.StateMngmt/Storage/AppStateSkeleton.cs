namespace InHues.StateMngmt.Storage
{
    public class AppStateSkeleton
    {
        public string SelectedSidebarItem { get; set; }
        public bool IsRunningRequest { get; set; }
        public UserViewModel? CurrentUser { get; set; }

    }
}
