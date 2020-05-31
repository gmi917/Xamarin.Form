using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Extended;
using static ARGuide.ShowPointLogDataService;

namespace ARGuide.ViewModels
{
    public class PointLogViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        public int TotalCount { get; private set; }
        AppValue app = new AppValue();
        readonly ShowPointLogDataService pointLogDataService = new ShowPointLogDataService();
        public InfiniteScrollCollection<MyItemListData> LogItems { get; set; }

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
        public PointLogViewModel()
        {
            LogItems = new InfiniteScrollCollection<MyItemListData>
            {
                OnLoadMore = async () =>
                {
                    IsBusy = true;
                    FooterText = "載入中...";
                    // load the next page
                    var page = LogItems.Count / app.PointLogPageSize;

                    var items = await pointLogDataService.GetItemsAsync(page, app.PointLogPageSize);

                    IsBusy = false;

                    // return the items that need to be added
                    return items;
                },
                OnCanLoadMore = () =>
                {
                    IsBusy = true;
                    FooterText = "沒有資料...";
                    return LogItems.Count < pointLogDataService.TotlaCount;
                }
            };

            Device.BeginInvokeOnMainThread(async () =>
            await DownloadDataAsync());

        }

        private async Task DownloadDataAsync()
        {
            UserDialogs.Instance.ShowLoading("");
            var items = await pointLogDataService.GetItemsAsync(pageIndex: 0, pageSize: app.PointLogPageSize);
            UserDialogs.Instance.HideLoading();
            if (items != null && items.Count > 0)
            {
                IsEmptyView = false;
                LogItems.AddRange(items);
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
