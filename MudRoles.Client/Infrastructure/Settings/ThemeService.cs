using MudBlazor;

namespace MudRoles.Client.Infrastructure.Settings
{
    public class ThemeService
    {
        private bool _isDarkMode = false;
        private MudTheme _currentTheme = MudRolesTheme.DefaultTheme;
        public event Func<Task>? OnThemeChanged;
        public MudTheme CurrentTheme => _currentTheme;
        public bool IsDarkMode()
        {
            return _isDarkMode;
        }
        public string GetSecondaryColorHex(MudTheme theme)
        {
            string secondaryColor;
            if (_isDarkMode)
            {
                secondaryColor = theme.PaletteDark.Secondary.ToString();
            }
            else
            {
                secondaryColor = theme.PaletteLight.Secondary.ToString();
            }
            return secondaryColor;
        }
        public string GetPrimaryColorHex(MudTheme theme)
        {
            string primaryColor;
            if (_isDarkMode)
            {
                primaryColor = theme.PaletteDark.Primary.ToString();
            }
            else
            {
                primaryColor = theme.PaletteLight.Primary.ToString();
            }
            return primaryColor;
        }
        public async void ToggleDarkMode()
        {
            _isDarkMode = !_isDarkMode;
            _currentTheme = _isDarkMode ? MudRolesTheme.DarkTheme : MudRolesTheme.DefaultTheme;
            if (OnThemeChanged != null)
            {
                await OnThemeChanged.Invoke();
            }
        }
    }
}
