using System.ComponentModel;
using MudBlazor;

namespace GameEditor.Client.Services;

public class ThemeService : INotifyPropertyChanged
{
    private bool _isDarkMode = true;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode != value)
            {
                _isDarkMode = value;
                OnPropertyChanged(nameof(IsDarkMode));
                OnPropertyChanged(nameof(CurrentTheme));
            }
        }
    }

    public MudTheme CurrentTheme => IsDarkMode ? DarkTheme : LightTheme;

    private static readonly MudTheme DarkTheme = new()
    {
        PaletteDark = new PaletteDark()
        {
            Primary = Colors.Blue.Lighten3,
            Background = Colors.Gray.Darken4,
            Surface = Colors.Gray.Darken3,
            AppbarBackground = Colors.Gray.Darken4,
            TextPrimary = Colors.Shades.White
        }
    };

    private static readonly MudTheme LightTheme = new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = Colors.Blue.Default,
            Background = Colors.Gray.Lighten5,
            Surface = Colors.Shades.White,
            AppbarBackground = Colors.Blue.Lighten5,
            TextPrimary = Colors.Shades.Black
        }
    };

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
