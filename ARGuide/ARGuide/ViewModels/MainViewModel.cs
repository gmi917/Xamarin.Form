using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extended;
using static ARGuide.DataService;

namespace ARGuide.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        AppValue app = new AppValue();
        public int TotalCount { get; private set; }
        readonly DataService dataService = new DataService();

        //public InfiniteScrollCollection<string> Items { get; }
        public InfiniteScrollCollection<MyItemListData> Items { get; set; }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }
        private string footerText;
        public string FooterText
        {
            get { return footerText; }
            set
            {
                footerText = value;
                OnPropertyChanged(nameof(FooterText));
            }
        }

        private bool isEmptyView;
        public bool IsEmptyView
        {
            get { return isEmptyView; }
            set
            {
                isEmptyView = value;
                OnPropertyChanged(nameof(IsEmptyView));
            }
        }

        public MainViewModel()
        {
            Items = new InfiniteScrollCollection<MyItemListData>
            {
                OnLoadMore = async () =>
                {
                    IsBusy = true;
                    FooterText = "載入中...";
                    // load the next page
                    var page = Items.Count / app.PageSize;

                    var items = await dataService.GetItemsAsync(page, app.PageSize);

                    IsBusy = false;

                    // return the items that need to be added
                    return items;
                },
                OnCanLoadMore = () =>
                {
                    IsBusy = true;
                    FooterText = "沒有資料...";
                    return Items.Count < dataService.TotlaCount;
                }
            };

            Device.BeginInvokeOnMainThread(async () =>
            await DownloadDataAsync());

        }

        private async Task DownloadDataAsync()
        {
            UserDialogs.Instance.ShowLoading("");
            var items = await dataService.GetItemsAsync(pageIndex: 0, pageSize: app.PageSize);
            UserDialogs.Instance.HideLoading();
            if (items != null && items.Count > 0)
            {
                IsEmptyView = false;
                Items.AddRange(items);
            }
            else
            {
                IsEmptyView = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
