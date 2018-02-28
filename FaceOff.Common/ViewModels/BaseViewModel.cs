using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
//using Plugin.Connectivity;

namespace FaceOff
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public BaseViewModel(){

           // Plugin.Connectivity.CrossConnectivity.Current.ConnectivityChanged+= Current_ConnectivityChanged;

        }

        //void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        //{

           
        //}

        bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                SetProperty(ref _isBusy, value);
            }
        }


		protected void SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyname = "", Action onChanged = null)
		{
			if (EqualityComparer<T>.Default.Equals(backingStore, value))
				return;

			backingStore = value;

			onChanged?.Invoke();

			OnPropertyChanged(propertyname);
		}

		void OnPropertyChanged([CallerMemberName]string name = "")
		{
			var handle = PropertyChanged;
			handle?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
