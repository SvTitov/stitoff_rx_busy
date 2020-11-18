using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.Reactive;
using System.Reactive.Linq;

namespace stitoff_rx_busy
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private bool isBusy;
        private string content;
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        #endregion

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                NotifyPropertyChanged();
            }
        }

        public string Content
        {
            get => content;
            set
            {
                content = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand DownloadCommand { get; }

        public ICommand DownloadRxCommand { get; }

        public MainPageViewModel()
        {
            DownloadCommand = new Command(Download);
            DownloadRxCommand = new Command(DownloadRx);
        }


        private async void Download(object obj)
        {
            try
            {
                Prepare();

                Content = await GetData(@"https://jsonplaceholder.typicode.com/todos/1")
                                    .ConfigureAwait(false);
            }
            finally
            {
                Finish();
            }
        }

        private void Finish()
        {
            IsBusy = false;
        }

        private void Prepare()
        {
            IsBusy = true;
            Content = string.Empty;
        }

        private void DownloadRx(object obj)
        {
            Observable.Start(Prepare)
                .Merge(Observable.FromAsync(() => GetData(@"https://jsonplaceholder.typicode.com/todos/1"))
                    .Do(Print)
                    .Select(_ => Unit.Default))
                .Finally(Finish)
                .Subscribe();
        }

        private void Print(string content)
        {
            Content = content;
        }

        private async Task<string> GetData(string url)
        {
            //delay
            await Task.Delay(5000);

            var client = new HttpClient();
            return await client.GetStringAsync(url);
        }
    }
}
