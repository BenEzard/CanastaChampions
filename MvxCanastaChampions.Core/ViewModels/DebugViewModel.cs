using CanastaChampions.DataAccess.Models;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class DebugViewModel : MvxViewModel<CompetitionModel>
    {
        public CompetitionModel SelectedCompetition = null;

        public override void Prepare(CompetitionModel parameter)
        {
            SelectedCompetition = parameter;
            System.Diagnostics.Debug.WriteLine($"Copying {parameter.CompetitionName}");
        }
    }
}
