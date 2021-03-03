using MvvmCross.Core;
using MvvmCross.Platforms.Wpf.Core;
using MvvmCross.Platforms.Wpf.Views;


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
        }
    }
}
