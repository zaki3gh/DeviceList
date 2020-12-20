using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;


// アイテム詳細ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234232 を参照してください

namespace MyApps.DeviceList
{
    /// <summary>
    /// グループ内の単一のアイテムに関する詳細情報を表示するページです。同じグループに属する他の
    /// アイテムにフリップするジェスチャを使用できます。
    /// </summary>
    public sealed partial class DeviceItemDetailPage : MyApps.DeviceList.Common.LayoutAwarePage, IShareSourcePage
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public DeviceItemDetailPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///  選択されているデバイスを<see cref="LoadState()"/><see cref="SaveState()"/>で保存するときに指定するキー.
        /// </summary>
        private const string PageStateKeySelectedItemId = "SelectedItemId";

        /// <summary>
        /// このページには、移動中に渡されるコンテンツを設定します。前のセッションからページを
        /// 再作成する場合は、保存状態も指定されます。
        /// </summary>
        /// <param name="navigationParameter">このページが最初に要求されたときに
        /// <see cref="Frame.Navigate(Type, Object)"/> に渡されたパラメーター値。
        /// </param>
        /// <param name="pageState">前のセッションでこのページによって保存された状態の
        /// ディクショナリ。ページに初めてアクセスするとき、状態は null になります。</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // 保存されたページの状態で、表示する最初のアイテムをオーバーライドすることを許可します
            if (pageState != null && pageState.ContainsKey(PageStateKeySelectedItemId))
            {
                navigationParameter = pageState[PageStateKeySelectedItemId];
            }

            // 何か状況が変わった?
            //  表示していたデバイスがなくなっているかもしれないのでページを戻す.
            if (DeviceItemGroup.Instance.ShouldUpdate)
            {
                this.Frame.GoBack();
                return;
            }

            // バインド可能なアイテムのコレクションを this.DefaultViewModel["Items"] に割り当てます
            // 選択したアイテムを this.flipView.SelectedItem に割り当てます
            var param = navigationParameter as string;
            this.DefaultViewModel["Items"] = DeviceItemGroup.Instance.Items;
            this.flipView.SelectedItem = DeviceItemGroup.Instance.Items.FirstOrDefault(
                x => x.DeviceItem.Data.Device.Id.Equals(param));
        }

        /// <summary>
        /// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
        /// このページに関連付けられた状態を保存します。値は、
        /// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
        /// </summary>
        /// <param name="pageState">シリアル化可能な状態で作成される空のディクショナリ。</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            // save selected item id
            pageState[PageStateKeySelectedItemId] = DeviceItemGroup.Instance.SelectedItemId;
        }

        /// <summary>
        ///  flipView SelectionChanged event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = this.flipView.SelectedItem as DeviceItemVisual;
            if (item == null)
            {
                return;
            }

            DeviceItemGroup.Instance.SelectedItemId = item.DeviceItem.Data.Device.Id;
        }

        #region IShareSourcePage

        /// <summary>
        ///  共有ソースとして<c>DataRequested</c>イベントを処理する.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var requestData = args.Request.Data;
            var item = this.flipView.SelectedItem as DeviceItemVisual;
            if (item != null)
            {
                item.SetDataPackage(requestData, DataPackageTextSource.AllProperties);
            }
            else
            {
                var res = Windows.UI.Xaml.Application.Current.Resources["MyStringResource"] as MyStringResources;
                args.Request.FailWithDisplayText(res["ShareNoItemsSelectedText"]);
            }
        }

        #endregion
    }
}
