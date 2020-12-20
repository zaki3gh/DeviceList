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

// グループ化されたアイテム ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234231 を参照してください

namespace MyApps.DeviceList
{
    /// <summary>
    /// グループ化されたアイテムのコレクションを表示するページです。
    /// </summary>
    public sealed partial class DeviceItemGroupedItemsPage : MyApps.DeviceList.Common.LayoutAwarePage, IShareSourcePage
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public DeviceItemGroupedItemsPage()
        {
            this.InitializeComponent();

            this.DeviceItemTreeView.NavigateToDeviceItemDetailPageHandler = this.NavigateToDeviceItemDetailPage;
        }

        /// <summary>
        ///  選択されているデバイスを<see cref="LoadState()"/><see cref="SaveState()"/>で保存するときに指定するキー.
        /// </summary>
        private const string PageStateKeySelectedItemId = "SelectedItemId";

        /// <summary>
        ///  グループ分けの種類を<see cref="LoadState()"/><see cref="SaveState()"/>で保存するときに指定するキー.
        /// </summary>
        private const string PageStateKeyGroupType = "GroupType";

        /// <summary>
        ///  非表示のデバイスを表示するかどうかを<see cref="LoadState()"/><see cref="SaveState()"/>で保存するときに指定するキー.
        /// </summary>
        private const string PageStateKeyShouldShowNotShownDevice = "ShouldShowNotShownDevice";

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
            if (pageState != null)
            {
                if (String.IsNullOrEmpty(DeviceItemGroup.Instance.SelectedItemId))
                {
                    object seletedItemId;
                    if (pageState.TryGetValue(PageStateKeySelectedItemId, out seletedItemId))
                    {
                        DeviceItemGroup.Instance.SelectedItemId = seletedItemId as string;
                    }
                }

                object groupType;
                if (pageState.TryGetValue(PageStateKeyGroupType, out groupType))
                {
                    DeviceItemGroup.Instance.GroupType = (groupType as GroupType?).GetValueOrDefault();
                }

                object shouldShowNotShownDevice;
                if (pageState.TryGetValue(PageStateKeyShouldShowNotShownDevice, out shouldShowNotShownDevice))
                {
                    DeviceItemGroup.Instance.ShouldShowNotShownDevice = (shouldShowNotShownDevice as bool?).GetValueOrDefault(false);
                }
            }

            // TODO: バインド可能なグループのコレクションを this.DefaultViewModel["Groups"] に割り当てます
            this.DefaultViewModel["Groups"] = DeviceItemGroup.Instance;

            UpdateItemsAsync(false);
        }

        /// <summary>
        /// アプリケーションが中断される場合、またはページがナビゲーション キャッシュから破棄される場合、
        /// このページに関連付けられた状態を保存します。値は、
        /// <see cref="SuspensionManager.SessionState"/> のシリアル化の要件に準拠する必要があります。
        /// </summary>
        /// <param name="pageState">シリアル化可能な状態で作成される空のディクショナリ。</param>
        protected override void SaveState(Dictionary<string, object> pageState)
        {
            // empty
            //base.SaveState(pageState);

            System.Diagnostics.Debug.Assert(pageState != null);

            // save selected item
            pageState[PageStateKeySelectedItemId] = DeviceItemGroup.Instance.SelectedItemId;
            pageState[PageStateKeyGroupType] = DeviceItemGroup.Instance.GroupType;
            pageState[PageStateKeyShouldShowNotShownDevice] = DeviceItemGroup.Instance.ShouldShowNotShownDevice;
        }

        /// <summary>
        ///  デバイスの表示を更新する.
        /// </summary>
        /// <param name="forceUpdate">更新が不要だとしても更新する場合に<c>true</c>を指定する</param>
        private async void UpdateItemsAsync(bool forceUpdate)
        {
            if (forceUpdate || DeviceItemGroup.Instance.ShouldUpdate)
            {
                await DeviceItemGroup.Instance.Update();
            }

            // 接続別表示では特殊な処理が必要
            if (DeviceItemGroup.Instance.GroupType == GroupType.ByConnection)
            {
                this.DeviceItemTreeView.Update();
            }

            // select device
            var selectedItem = this.groupedItemsViewSource.View.FirstOrDefault(
                x => (x as DeviceItemVisual).DeviceItem.Data.Device.Id.Equals(DeviceItemGroup.Instance.SelectedItemId));
            if (selectedItem != null)
            {
                // このdelayがないと選択されたアイテムにスクロールされない
                // 推測ではこのdelayの間にgridview/listviewへのbindingが処理されて
                // アイテムが作られる, それが終わるまではviewにおける選択が成立していない?

                // 選択したアイテムにフォーカスが来ない, 何故だ?
                //   --> SelectionModeをNoneからSingleにしたら来た!
                //       見栄えの問題でテーマをLightにするのでSingleだと具合が悪い
                //       ということで普段はNoneだがスクロールさせる間だけSingleに変える.
                this.itemListView.SelectionMode = ListViewSelectionMode.Single;

                await System.Threading.Tasks.Task.Delay(100);

                if (this.itemGridView.Visibility == Windows.UI.Xaml.Visibility.Visible)
                {
                    this.itemGridView.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
                else if (this.itemListView.Visibility == Windows.UI.Xaml.Visibility.Visible)
                {
                    this.itemListView.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
                else if (this.DeviceItemTreeView.Visibility == Windows.UI.Xaml.Visibility.Visible)
                {
                    this.DeviceItemTreeView.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }

                this.groupedItemsViewSource.View.MoveCurrentTo(selectedItem);
                this.itemGridView.ScrollIntoView(selectedItem);
                this.itemListView.ScrollIntoView(selectedItem);
                this.DeviceItemTreeView.ScrollIntoViewWithDeviceItemVisual(selectedItem as DeviceItemVisual);

                // タイミングの問題か↑だけではスクロールされないことがある...
                await System.Threading.Tasks.Task.Delay(1000);
                this.itemGridView.ScrollIntoView(selectedItem);
                this.itemListView.ScrollIntoView(selectedItem);
                this.DeviceItemTreeView.ScrollIntoViewWithDeviceItemVisual(selectedItem as DeviceItemVisual);

                this.itemListView.SelectionMode = ListViewSelectionMode.None;
            }
        }

        /// <summary>
        ///  GridView / ListView ItemClick event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DeviceItemVisual;
            NavigateToDeviceItemDetailPage(item);
        }

        /// <summary>
        ///  デバイスのプロパティページに移動する.
        /// </summary>
        /// <param name="item"></param>
        private void NavigateToDeviceItemDetailPage(DeviceItemVisual item)
        {
            DeviceItemGroup.Instance.SelectedItemId = item.DeviceItem.Data.Device.Id;

            // 適切な移動先のページに移動し、新しいページを構成します。
            // このとき、必要な情報をナビゲーション パラメーターとして渡します
            this.Frame.Navigate(typeof(DeviceItemDetailPage), item.DeviceItem.Data.Device.Id);
        }

        #region AppBar

        /// <summary>
        ///  ページ下のAppBarの<c>Loaded</c>イベントを処理する.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BottomAppBar_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = (sender as AppBar).Content as Panel;
            foreach (var ui in panel.Children)
            {
                base.StartLayoutUpdates(ui, new RoutedEventArgs());
            }
        }

        /// <summary>
        ///  ページ下のAppBarの<c>Unloaded</c>イベントを処理する.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BottomAppBar_Unloaded(object sender, RoutedEventArgs e)
        {
            var panel = (sender as AppBar).Content as Panel;
            foreach (var ui in panel.Children)
            {
                base.StopLayoutUpdates(ui, new RoutedEventArgs());
            }
        }

        /// <summary>
        ///  グループ分けの種類を選択するポップアップメニューを表示するAppBarボタンが押された.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarGroupTypeButton_Click(object sender, RoutedEventArgs e)
        {
            this.GroupTypePopupMenu.HorizontalOffset = 5;
            this.GroupTypePopupMenu.VerticalOffset = this.ActualHeight - this.BottomAppBar.ActualHeight - 5;
            this.GroupTypePopupMenu.IsOpen = true;
        }

        /// <summary>
        ///  グループ分けの種類を選択するポップアップメニューのコンテンツがロードされた.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupTypePopupMenu_Border_Loaded(object sender, RoutedEventArgs e)
        {
            // check radio
            var currentGroup = Enum.GetName(typeof(GroupType), DeviceItemGroup.Instance.GroupType);
            var objRadio = (((sender as Border).Child as Panel).Children).FirstOrDefault(
                x => currentGroup.Equals((x as RadioButton).Tag));
            if (objRadio != null)
            {
                (objRadio as RadioButton).IsChecked = true;
            }

            // adjust location
            var popup = (sender as Border).Parent as Popup;
            popup.VerticalOffset -= (sender as Border).ActualHeight;
        }

        /// <summary>
        ///  グループ分けの種類を選択するポップアップメニューのラジオボタンがチェックされた.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupTypePopupMenu_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var groupType = (GroupType)Enum.Parse(typeof(GroupType), (sender as RadioButton).Tag as string);
            if (groupType == DeviceItemGroup.Instance.GroupType)
            {
                return;
            }

            DeviceItemGroup.Instance.GroupType = groupType;
            UpdateItemsAsync(true);
        }

        /// <summary>
        ///  更新ボタンのClick event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemsAsync(true);
        }

        /// <summary>
        ///  「非表示のデバイスの表示」ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarShowHiddenDevicesButton_Click(object sender, RoutedEventArgs e)
        {
            // ToggleButtonをAppBarで使うときにはworkaroundが必要らしい
            // http://www.visuallylocated.com/post/2012/09/04/Fixing-the-VisualState-of-your-AppBar-ToggleButton.aspx

            var btn = sender as ToggleButton;
            VisualStateManager.GoToState(btn, btn.IsChecked.Value ? "Checked" : "Unchecked", false);

            UpdateItemsAsync(true);
        }

        #endregion

        #region IShareSourcePage

        /// <summary>
        ///  共有ソースとして<c>DataRequested</c>イベントを処理する.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (this.DeviceItemTreeView.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                this.DeviceItemTreeView.OnDataRequested(sender, args);
            }
            else
            {
                var requestData = args.Request.Data;
                var item = this.groupedItemsViewSource.View.CurrentItem as DeviceItemVisual;
                if (item != null)
                {
                    item.SetDataPackage(requestData, DataPackageTextSource.DisplayName);
                }
                else
                {
                    var res = Windows.UI.Xaml.Application.Current.Resources["MyStringResource"] as MyStringResources;
                    args.Request.FailWithDisplayText(res["ShareNoItemsSelectedText"]); 
                }
            }
        }

        #endregion

    }
}
