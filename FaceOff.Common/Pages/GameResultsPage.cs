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

            var indicatior = new ActivityIndicator
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 50,
                WidthRequest = 50
            };

            indicatior.SetBinding(ActivityIndicator.IsRunningProperty, nameof(_viewModel.IsBusy));


            listView.ItemTemplate = new DataTemplate(typeof(TextCell));

            listView.SetBinding(ListView.ItemsSourceProperty, nameof(_viewModel.Data));
            listView.ItemTemplate.SetBinding(TextCell.TextProperty, nameof(GameResult.WinnerName));
            listView.ItemTemplate.SetBinding(TextCell.DetailProperty, nameof(GameResult.WinnerScore));

            RelativeLayout layout = new RelativeLayout();

            layout.Children.Add(listView,
                                Constraint.RelativeToParent((parent) =>
                                {
                                    return parent.X;
                                }), Constraint.RelativeToParent((parent) =>
                                {
                                    return parent.Y;
                                }));

            layout.Children.Add(indicatior,
                                Constraint.RelativeToParent((parent) =>
                                {
                                    return parent.Width / 2 - 25;
                                }),Constraint.RelativeToParent((parent) =>
                                {
                                    return parent.Height / 2 - 25;
                                }));

            Content = layout;
        }

        protected async override void OnAppearing()
        {
            if (_viewModel.Data.Count == 0)
                await _viewModel.GetAllGames();
        }
                                                                        
    }
}
