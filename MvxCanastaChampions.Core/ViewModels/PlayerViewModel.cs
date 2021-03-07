using CanastaChampions.DataAccess.Models;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvxCanastaChampions.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvxCanastaChampions.Core.ViewModels
{
    public class PlayerViewModel : MvxViewModel<PlayerModel>
    {
        private readonly IMvxNavigationService _navigationService;

        private PlayerModel _initialisingPlayerModel = new PlayerModel();

        private PlayerDialogMode DialogMode;

        public PlayerModel InitialisingPlayer
        {
            get { return _initialisingPlayerModel; }
            set {
                _initialisingPlayerModel = value;
                SetProperty(ref _initialisingPlayerModel, value);
            }
        }

        private bool _registerToCompetition;

        public bool RegisterToCompetition
        {
            get { return _registerToCompetition; }
            set { 
                _registerToCompetition = value;
                SetProperty(ref _registerToCompetition, value);
            }
        }


        public PlayerViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
            SaveCommand = new MvxCommand(Save);
        }


        public override void Prepare(PlayerModel parameter)
        {
            _initialisingPlayerModel = parameter;
            DialogMode = (parameter.PlayerID == -1) ? PlayerDialogMode.ADD : PlayerDialogMode.EDIT;
        }

        #region BackCommand
        public IMvxCommand BackCommand { get; set; }

        public void Back()
        {
            //_navigationService.Navigate<TeamsViewModel, CompetitionModel>(SelectedCompetition);
        }
        #endregion

        #region SaveCommand
        public IMvxCommand SaveCommand { get; set; }

        public void Save()
        {
            if (DialogMode == PlayerDialogMode.ADD)
            {
                /*CompetitionDataAccess.InsertPlayer(InitialisingPlayer)
                    if (_registerToCompetition)
                        CompetitionDataAccess.RegisterPlayerToCompetition(InitialisingPlayer.CompetitionID, InitialisingPlayer.PlayerID)*/
            }
            else if (DialogMode == PlayerDialogMode.EDIT)
            {

            }
        }
        #endregion

        enum PlayerDialogMode
        {
            ADD,
            EDIT,
        }
    }
}
