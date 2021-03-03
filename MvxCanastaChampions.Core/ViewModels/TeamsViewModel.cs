using CanastaChampions.DataAccess.Models;
using MvvmCross.ViewModels;
using MvxCanastaChampions.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class TeamsViewModel : MvxViewModel<CompetitionModel>
    {
        public CompetitionModel SelectedCompetition = null;

        private MvxObservableCollection<TeamFormationModel> _teamFormationList = new MvxObservableCollection<TeamFormationModel>();


        public MvxObservableCollection<TeamFormationModel> TeamFormationList
        {
            get => _teamFormationList;
            set { 
                _teamFormationList = value;
                System.Diagnostics.Debug.WriteLine($"updating the list");
                SetProperty(ref _teamFormationList, value);
            }
        }

        private TeamFormationModel _selectedTeam;
        private List<TeamFormationModel> _selectedTeams = new List<TeamFormationModel>();
        public TeamFormationModel SelectedTeam
        {
            get => _selectedTeam;
            set
            {
                System.Diagnostics.Debug.WriteLine($"Checking for  {value.PlayerName}, state is {_selectedTeams.Contains(value)}");
                if (_selectedTeams.Contains(value))
                {
                    System.Diagnostics.Debug.WriteLine($"Removing {value.PlayerName}, count is {_selectedTeams.Count}");
                    _selectedTeams.Remove(value);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Adding {value.PlayerName}, count is {_selectedTeams.Count}");
                    _selectedTeams.Add(value);
                }
                _selectedTeam = value;
            }
        }

        private static int _teamNumber;

        public static int TeamNumber
        {
            get { return _teamNumber; }
        }


        public override void Prepare(CompetitionModel parameter)
        {
            SelectedCompetition = parameter;
            List<TeamFormationModel> players = CompetitionServices.GetPlayerRoster(parameter.CompetitionID);
            TeamFormationList = new MvxObservableCollection<TeamFormationModel>(players);
        }

     



    }
}
