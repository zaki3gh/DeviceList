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

// ユーザー コントロールのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace MyApps.DeviceList
{
    /// <summary>
    ///  接続別表示のツリービュー.
    /// </summary>
    public sealed partial class DeviceItemTreeView : UserControl
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public DeviceItemTreeView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///  ツリー表示を更新する.
        /// </summary>
        public void Update()
        {
            this.ItemsTreeView.Items.Clear();

            var rootItems = DeviceItemGroup.Instance.Items.Where(x => x.DeviceItem.IsRoot);
            foreach (var rootItem in rootItems.OrderBy(x => x.DeviceItem.Data.Name))
            {
                if (rootItem.DeviceItem.Data.Visual == null)
                {
                    continue;
                }
                AddItemsAsTree(rootItem, null);
            }
        }

        /// <summary>
        ///  ツリーとしてアイテムを追加していく.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="parentViewItem"></param>
        private void AddItemsAsTree(DeviceItemVisual item, DeviceItemTreeViewItem parentViewItem)
        {
            if (item == null)
            {
                return;
            }

            var devViewItem = new DeviceItemTreeViewItem()
            {
                Visibility = item.DeviceItem.IsRoot ? Visibility.Visible : Visibility.Collapsed,
                Device = item, 
            };

            this.ItemsTreeView.Items.Add(devViewItem);
            if (parentViewItem != null)
            {
                System.Diagnostics.Debug.Assert(parentViewItem.Children != null);
                parentViewItem.Children.Add(devViewItem);
            }

            if (!item.DeviceItem.HasChildren)
            {
                return;
            }

            devViewItem.Children = new List<DeviceItemTreeViewItem>(item.DeviceItem.Children.Count);
            foreach (var child in item.DeviceItem.Children)
            {
                AddItemsAsTree(child.Data.Visual, devViewItem);
            }
            devViewItem.ChildrenVisible = true;
        }

        /// <summary>
        ///  指定したアイテムが表示されるようにスクロールさせる.
        /// </summary>
        /// <param name="item"></param>
        public void ScrollIntoViewWithDeviceItemVisual(DeviceItemVisual item)
        {
            if (item == null)
            {
                return;
            }

            var selectedItem = this.ItemsTreeView.Items.FirstOrDefault(
                x => (x as DeviceItemTreeViewItem).Device.DeviceItem.Data.Device.Id.Equals(item.DeviceItem.Data.Device.Id));
            if (selectedItem != null)
            {
                this.ItemsTreeView.SelectedItem = selectedItem;
                this.ItemsTreeView.ScrollIntoView(selectedItem);
            }
        }

        /// <summary>
        ///  デバイス詳細ページに移動する<c>delegate</c>.
        /// </summary>
        public Action<DeviceItemVisual> NavigateToDeviceItemDetailPageHandler { get; set; }

        /// <summary>
        ///  デバイスのアイテムがクリックされた時の処理を行う.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemsTreeView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var control = e.ClickedItem as Panel;
            if (control == null)
            {
                return;
            }
            var item = control.Parent as DeviceItemTreeViewItem;
            if (item == null)
            {
                return;
            }

            if (this.NavigateToDeviceItemDetailPageHandler != null)
            {
                this.NavigateToDeviceItemDetailPageHandler(item.Device);
            }
        }

        /// <summary>
        ///  共有のデータが要求された時の処理を行う.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var requestData = args.Request.Data;
            var viewitem = this.ItemsTreeView.SelectedItem as DeviceItemTreeViewItem;
            if (viewitem != null)
            {
                viewitem.Device.SetDataPackage(requestData, DataPackageTextSource.DisplayName);
            }
            else
            {
                var res = Windows.UI.Xaml.Application.Current.Resources["MyStringResource"] as MyStringResources;
                args.Request.FailWithDisplayText(res["ShareNoItemsSelectedText"]); 
            }
        }

        /// <summary>
        ///  キーボード操作用.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            if (e.KeyStatus.IsMenuKeyDown)
            {
                base.OnKeyUp(e);
                return;
            }

            switch (e.Key)
            {
                // collapse
                //  子デバイスが表示中: 隠す
                //  子デバイス非表示: 親に移動
                case Windows.System.VirtualKey.Left:
                    {
                        var item = GetSelectedFocusedItem();
                        if (item != null)
                        {
                            // win8.1再ターゲット後、ツリーを開閉するとクラッシュするので
                            // 表示しっぱなしに変更した
                            //if (item.ChildrenVisible)
                            //{
                            //    item.ChildrenVisible = false;
                            //}
                            //else
                            {
                                if (item.Device.DeviceItem.HasParent)
                                {
                                    var newSelected = this.ItemsTreeView.Items.FirstOrDefault(
                                        x => ((x as DeviceItemTreeViewItem).Device == item.Device.DeviceItem.Parent.Data.Visual));
                                    this.ItemsTreeView.SelectedItem = newSelected;
                                }
                            }
                        }
                    }
                    e.Handled = true;
                    break;

                // expand
                //  子デバイスが表示中: 子に移動
                //  子デバイス非表示: 子を表示
                case Windows.System.VirtualKey.Right:
                    {
                        var item = GetSelectedFocusedItem();
                        if (item != null)
                        {
                            if (!item.ChildrenVisible)
                            {
                                if (item.Device.DeviceItem.HasChildren)
                                {
                                    item.ChildrenVisible = true;
                                }
                            }
                            else
                            {
                                if (item.Device.DeviceItem.HasChildren)
                                {
                                    var child = item.Device.DeviceItem.Children.FirstOrDefault();
                                    if (child != null)
                                    {
                                        var newSelected = this.ItemsTreeView.Items.FirstOrDefault(
                                            x => ((x as DeviceItemTreeViewItem).Device == child.Data.Visual));
                                        if (newSelected != null)
                                        {
                                            this.ItemsTreeView.SelectedItem = newSelected;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    e.Handled = true;
                    break;
            }

            base.OnKeyUp(e);
        }

        private DeviceItemTreeViewItem GetSelectedFocusedItem()
        {
            if (this.ItemsTreeView.Visibility != Windows.UI.Xaml.Visibility.Visible)
            {
                return null;
            }

            var item = this.ItemsTreeView.SelectedItem as DeviceItemTreeViewItem;
            if (item == null)
            {
                return null;
            }

            if (item.FocusState == Windows.UI.Xaml.FocusState.Unfocused)
            {
                return null;
            }

            return item;
        }
    }
}
