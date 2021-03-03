using CanastaChampions.Data.Models;
using CanastaChampions.Domain.Models;
using CanastaChampions.Domain.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CanastaChampions.Domain.ViewModel
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Competition> Competitions = new ObservableCollection<Competition>();
        private CompetitionRepository _compRepo = new CompetitionRepository();

        private Competition _selectedCompetition = new Competition();

        public event PropertyChangedEventHandler PropertyChanged;

        public Competition SelectedCompetition
        {
            get { return _selectedCompetition; }
            set {
                _selectedCompetition = value;
                OnPropertyChanged();
            }
        }


        public AppViewModel()
        {
            foreach (CompetitionModel cm in _compRepo.Competitions)
            {
                Competitions.Add(new Competition(cm));
            }
        }

        public void CreateCompetition()
        {
            CompetitionModel competitionModel = _compRepo.CreateCompetition(SelectedCompetition.CompetitionName, SelectedCompetition.FixedTeams, SelectedCompetition.RandomiseTeams);
            Competitions.Add(new Competition(competitionModel));
        }

        public void SaveCompetition()
        {
            _compRepo.UpdateCompetition(SelectedCompetition.GetCompetitionModel());
        }

        private void OnPropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        
    }
}
