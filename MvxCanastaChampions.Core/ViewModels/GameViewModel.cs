using CanastaChampions.DataAccess.Models;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class GameViewModel : MvxViewModel<List<TeamFormationModel>>
    {
        public override void Prepare(List<TeamFormationModel> parameter)
        {
            foreach (TeamFormationModel m in parameter)
            {
                System.Diagnostics.Debug.WriteLine($"==> Team Member is {m.PlayerName}, team # {m.TeamNumber}");
            }
        }
    }
}
