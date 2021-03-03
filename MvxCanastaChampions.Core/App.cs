using MvvmCross.ViewModels;
using MvxCanastaChampions.Core.ViewModels;

namespace MvxCanastaChampions.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<CompetitionViewModel>();
        }
    }
}
