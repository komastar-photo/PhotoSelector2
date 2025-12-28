using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace PhotoSelector2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
      builder
         .UseMauiApp<App>()
         .UseMauiCommunityToolkit()
           .ConfigureFonts(fonts =>
      {
           fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
    {
      events.AddWindows(wndLifeCycleBuilder =>
     {
wndLifeCycleBuilder.OnWindowCreated(window =>
 {
      var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
       var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
         var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);

    // 컴팩트한 창 크기 설정 (너비 500, 높이 700)
     appWindow.Resize(new Windows.Graphics.SizeInt32(500, 700));
    });
       });
            });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

          return builder.Build();
      }
    }
}
