namespace MudRoles.Client.Infrastructure.Settings
{
    public class ConsentService
    {
        private bool _bannerShown = false;
        public event Func<Task>? OnCookieShown;
        public async void ShowCookiesPolicy() 
        {
            _bannerShown = !_bannerShown;
            if (OnCookieShown != null)
            {
                await OnCookieShown.Invoke();
            }

        }
    }
}
