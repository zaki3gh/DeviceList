using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;


namespace MyApps.DeviceList
{
    /// <summary>
    ///  共有元になるページが実装するインターフェース.
    /// </summary>
    interface IShareSourcePage
    {
        /// <summary>
        ///  共有ソースとして<c>DataRequested</c>イベントを処理する.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args);
    }

    /// <summary>
    ///  共有元としての実装.
    /// </summary>
    static class ShareSource
    {
        /// <summary>
        ///  DataPackageに共有するデータを設定する
        /// </summary>
        /// <param name="item">デバイス</param>
        /// <param name="data">DataPackage</param>
        /// <param name="textSource">共有するテキストの種類</param>
        internal static void SetDataPackage(this DeviceItemVisual item, DataPackage data, DataPackageTextSource textSource)
        {
            var res = Windows.UI.Xaml.Application.Current.Resources["MyStringResource"] as MyStringResources;

            if (item != null)
            {
                // title / description
                data.Properties.Title = res["ShareTitle"];
                data.Properties.Description = String.Format(res["ShareDescriptionFormat"], item.DisplayName);

                // text
                switch (textSource)
                {
                    case DataPackageTextSource.DisplayName:
                        data.SetText(item.DisplayName);
                        break;
                    case DataPackageTextSource.AllProperties:
                        data.SetText(String.Format("{0}\n\n{1}\n{2}\n\n{3}\n{4}",
                            item.DisplayName,
                            res["DetailColumnTitleDeviceProperty"],
                            item.DevicePropertiesText,
                            res["DetailColumnTitleDeviceContainerProperty"],
                            item.ContainerPropertiesText));
                        break;
                }

                // bitmap
                if (item.DeviceThumbnail != null)
                {
                    var strmThumbnail = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromStream(item.DeviceThumbnail);
                    data.SetBitmap(strmThumbnail);
                    data.Properties.Thumbnail = strmThumbnail;
                }
            }
            else
            {
            }
        }
    }

    /// <summary>
    ///  共有するテキスト.
    /// </summary>
    internal enum DataPackageTextSource
    {
        /// <summary>表示名だけ</summary>
        DisplayName,

        /// <summary>プロパティ全部</summary>
        AllProperties,
    }
}
