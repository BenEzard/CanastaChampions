using MvvmCross;
using MvvmCross.Core;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Wpf.Core;
using MvvmCross.Platforms.Wpf.Views;
using MvxCanastaChampions.Core.ViewModels;

namespace MvxCanastaChampions.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : MvxApplication
    {
        protected override void RegisterSetup()
        {
            this.RegisterSetupType<MvxWpfSetup<MvxCanastaChampions.Core.App>>();
            //Mvx.IoCProvider.RegisterSingleton<GameViewModel>(GameViewModel);
        }
    }
}
