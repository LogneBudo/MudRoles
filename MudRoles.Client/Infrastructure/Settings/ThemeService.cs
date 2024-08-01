using MudBlazor;

namespace MudRoles.Client.Infrastructure.Settings
{
    public class ThemeService
    {
        private bool _isDarkMode = false;
        private MudTheme _currentTheme = MudRolesTheme.DefaultTheme;

        public MudTheme CurrentTheme => _currentTheme;

        public void ToggleDarkMode()
        {
            _isDarkMode = !_isDarkMode;
            _currentTheme = _isDarkMode ? MudRolesTheme.DarkTheme : MudRolesTheme.DefaultTheme;
        }
    }
}
