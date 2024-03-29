﻿using CanastaChampions.DataAccess.Models;
using MvvmCross.Platforms.Wpf.Views;
using MvxCanastaChampions.Core.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MvxCanastaChampions.Wpf.Views
{
    /// <summary>
    /// Interaction logic for TeamsView.xaml
    /// </summary>
    public partial class TeamsView : MvxWpfView
    {
        public TeamsView()
        {
            InitializeComponent();
        }

        private void TeamNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TeamsViewModel tmv = DataContext as TeamsViewModel;
            IList items = (IList)TeamNames.SelectedItems;
            tmv.SelectedTeamMembers = items.Cast<GamePlayerModel>();
        }
    }
}
