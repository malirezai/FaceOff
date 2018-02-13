using System;
using Xamarin.Forms;
using CosmosDBResourceTokenBroker.Shared.Models;
using System.Collections.ObjectModel;
using FaceOff.ViewModels;
namespace FaceOff.Pages
{
    public class GameResultsPage : ContentPage
    {
        GameResultsViewModel _viewModel;

        public GameResultsPage()
        {
            _viewModel = new GameResultsViewModel();
            BindingContext = _viewModel;

            ListView listView = new ListView();

            listView.ItemTemplate = new DataTemplate(typeof(TextCell));

            listView.SetBinding(ListView.ItemsSourceProperty, nameof(_viewModel.Data));
            listView.ItemTemplate.SetBinding(TextCell.TextProperty, nameof(GameResult.WinnerName));
            listView.ItemTemplate.SetBinding(TextCell.DetailProperty, nameof(GameResult.WinnerScore));

            StackLayout stackLayout = new StackLayout();

            stackLayout.Children.Add(listView);

            Content = stackLayout;
        }

        protected async override void OnAppearing()
        {
            if (_viewModel.Data.Count == 0)
                await _viewModel.GetAllGames();
        }
                                                                        
    }
}
