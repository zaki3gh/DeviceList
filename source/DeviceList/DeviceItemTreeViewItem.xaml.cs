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

// ユーザー コントロールのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace MyApps.DeviceList
{
    /// <summary>
    ///  接続別表示のツリービューのための<c>ListViewItem</c>拡張.
    /// </summary>
    public sealed partial class DeviceItemTreeViewItem : 
        ListViewItem, 
        System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        public DeviceItemTreeViewItem()
        {
            this.InitializeComponent();

            this.DataContext = this;
        }

        /// <summary>
        ///  子デバイスのDeviceItemTreeViewItem.
        /// </summary>
        public List<DeviceItemTreeViewItem> Children { get; set; }

        /// <summary>
        ///  表示されるデバイス.
        /// </summary>
        public DeviceItemVisual Device { get; set; }

        /// <summary>
        ///  子デバイスを表示するかどうか.
        /// </summary>
        public System.Boolean ChildrenVisible
        {
            get { return this.childrenVisible; }
            set
            {
                // Win8.1に再ターゲットしてからどうしても落ちる
                // なのでツリー表示は表示しっぱなし限定とする
                if (!value)
                {
                    return;
                }

                this.childrenVisible = value;
                if (this.Device.DeviceItem.HasChildren)
                {
                    foreach (var child in this.Children)
                    {
                        if (value)
                        {
                            child.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        }
                        else
                        {
                            child.ChildrenVisible = value;
                            child.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        }
                    }
                }

                FirePropertyChanged("ChildrenVisible");
            }
        }

        /// <summary>
        ///  ChildrenVisibleプロパティ用変数.
        /// </summary>
        private System.Boolean childrenVisible = false;

        /// <summary>
        ///  インデント幅.
        /// </summary>
        public double IndentWidth
        {
            get
            {
                return this.Device.DeviceItem.Depth * FontSize * 3;
            }
        }

        // INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChanged(string propertyName)
        {
            var evt = this.PropertyChanged;
            if (evt != null)
            {
                evt(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
