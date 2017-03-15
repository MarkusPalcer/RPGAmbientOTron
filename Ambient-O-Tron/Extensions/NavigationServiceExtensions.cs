using AmbientOTron.Views.Ambience;
using AmbientOTron.Views.Shell;
using AmbientOTron.Views.SoundBoard;
using Core.Navigation;

namespace AmbientOTron.Extensions
{
  public static class NavigationServiceExtensions
  {
    public static void CloseSoundBoard(this INavigationService service)
    {
      service.NavigateAsync<NewSoundBoardView>(ShellViewModel.LowerPane);
    }

    public static void CloseAmbience(this INavigationService service)
    {
      service.NavigateAsync<NewAmbienceView>(ShellViewModel.MainRegion);
    }
  }
}