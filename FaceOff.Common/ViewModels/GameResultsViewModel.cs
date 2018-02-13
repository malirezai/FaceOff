using System;
using System.Collections.ObjectModel;
using CosmosDBResourceTokenBroker.Shared.Models;
using System.Threading.Tasks;
using CosmosDBResourceTokenBroker.Shared;
using Xamarin.Forms;
namespace FaceOff.ViewModels
{
    public class GameResultsViewModel : BaseViewModel
    {

        ObservableCollection<GameResult> _data;
        public ObservableCollection<GameResult> Data
        {
            get
            {
                if (_data == null)
                    _data = new ObservableCollection<GameResult>();
                return _data;
            }
            set
            {
                SetProperty(ref _data, value);
            }

        }

        public GameResultsViewModel()
        {
        }

        public async Task GetAllGames()
        {
            IsBusy = true;
            var dataList = await CosmosDBRepository.Instance.GetAllItemsAsync<GameResult>();

            _data.Clear();
            foreach (var item in dataList)
            {
                Data.Add(item);
            }
            IsBusy = false;
        }
    }
}
