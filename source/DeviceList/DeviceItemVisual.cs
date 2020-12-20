using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Data;
using System.ComponentModel;


namespace MyApps.DeviceList
{
    /// <summary>
    ///  デバイス(画面表示用のプロパティを追加).
    /// </summary>
    public class DeviceItemVisual :
        System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="DeviceItem">デバイス</param>
        public DeviceItemVisual(TreeNode<DeviceItem> deviceItem)
        {
            this.deviceItem = deviceItem;
            deviceItem.Data.Visual = this;
        }

        /// <summary>
        ///  デバイス.
        /// </summary>
        public TreeNode<DeviceItem> DeviceItem { get { return this.deviceItem; } }

        /// <summary>
        ///  DeviceItemプロパティ用のフィールド.
        /// </summary>
        private TreeNode<DeviceItem> deviceItem;

        /// <summary>
        ///  画面表示用の名前.
        /// </summary>
        public System.String DisplayName
        {
            get
            {
                if (this.DeviceItem == null)
                {
                    return String.Empty;
                }

                var name = this.DeviceItem.Data.Name;
                if (!String.IsNullOrEmpty(name))
                {
                    return name;
                }
                if (this.DeviceItem.IsRoot)
                {
                    name = DeviceItem.Data.ContainerName;
                }

                return name;
            }
        }

        /// <summary>
        ///  サムネイル画像.
        /// </summary>
        public BitmapImage ThumbnailBitmapImage
        {
            get
            {
                if (this.thumbnailBitmapImage != null)
                {
                    return this.thumbnailBitmapImage;
                }
                else
                {
                    GetThumbnailAsync();
                    return null;
                }
            }
        }

        /// <summary>
        ///  ThumbnailBitmapImageプロパティ用のフィールド.
        /// </summary>
        private BitmapImage thumbnailBitmapImage;

        public Windows.Devices.Enumeration.DeviceThumbnail DeviceThumbnail
        {
            get { return this.m_deviceThumbnail; }
        }

        private Windows.Devices.Enumeration.DeviceThumbnail m_deviceThumbnail;

        /// <summary>
        ///  サムネイル画像を取得しプロパティ変更通知を行う.
        /// </summary>
        private async void GetThumbnailAsync()
        {
            if (this.DeviceItem == null)
            {
                return;
            }
            if (this.DeviceItem.Data.Information == null)
            {
                return;
            }

            var thumbnail = await this.DeviceItem.Data.Information.First().GetThumbnailAsync();
            if (thumbnail == null)
            {
                return;
            }

            this.m_deviceThumbnail = thumbnail;

            var bmpImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
            bmpImage.SetSource(thumbnail);

            this.thumbnailBitmapImage = bmpImage;
            OnPropertyChanged("ThumbnailBitmapImage");
        }

        /// <summary>
        ///  デバイスのプロパティ.
        /// </summary>
        public IEnumerable<Tuple<string, object>> DeviceProperties
        {
            get
            {
                if (this.DeviceItem == null)
                {
                    yield break;
                }

                if (m_propertyNameLocalizer == null)
                {
                    m_propertyNameLocalizer = new DevicePropertyNameLocalizer();
                }

                // device
                foreach (var key in new string[] {
//                    DevicePropertyHelper.SystemItemNameDisplay, 
//                    DevicePropertyHelper.SystemDevicesDeviceHasProblem, 
                    DevicePropertyHelper.DEVPKEY_Device_ProblemCode, 
                    DevicePropertyHelper.DEVPKEY_Device_Class, 
                    DevicePropertyHelper.SystemDevicesHardwareIds,

//                    DevicePropertyHelper.DEVPKEY_Device_Driver, 
                    DevicePropertyHelper.DEVPKEY_Device_Manufacturer, 
                    DevicePropertyHelper.DEVPKEY_Device_DriverDate, 
                    DevicePropertyHelper.DEVPKEY_Device_DriverVersion, 
                    DevicePropertyHelper.DEVPKEY_Device_DriverProvider, 

                    DevicePropertyHelper.DEVPKEY_Device_InstallDate, 
                    DevicePropertyHelper.DEVPKEY_Device_FirstInstallDate, 
                    DevicePropertyHelper.DEVPKEY_Device_LastArrivalDate, 
                    DevicePropertyHelper.DEVPKEY_Device_LastRemovalDate, 
                    })
                {
                    object value;
                    if (this.DeviceItem.Data.Device.Properties.TryGetValue(key, out value))
                    {
                        if (value != null)
                        {
                            if (key.Equals(DevicePropertyHelper.DEVPKEY_Device_Class))
                            {
                                value = DeviceClassNameLocalizer.Instance.Localize(value as string);
                            }
                            else if (value is DateTimeOffset?)
                            {
                                value = Windows.Globalization.DateTimeFormatting.DateTimeFormatter.LongDate.Format((DateTimeOffset)value);
                            }
                            else if (value is string[])
                            {
                                value = (value as string[]).Aggregate((x, y) => x + "\n" + y);
                            }

                            yield return new Tuple<string, object>(m_propertyNameLocalizer.Localize(key), value);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  コンテナーのプロパティ.
        /// </summary>
        public IEnumerable<Tuple<string, object>> ContainerProperties
        {
            get
            {
                if (this.DeviceItem == null)
                {
                    yield break;
                }
                if (this.DeviceItem.Data.Container == null)
                {
                    yield break;
                }

                // container properties
                foreach (var key in new string[] {
                    DevicePropertyHelper.SystemDevicesFriendlyName, 
                    DevicePropertyHelper.SystemDevicesModelName, 
                    DevicePropertyHelper.SystemDevicesManufacturer, 
                    DevicePropertyHelper.SystemDevicesInLocalMachineContainer, 
                    DevicePropertyHelper.SystemDevicesDiscoveryMethod, 
                    DevicePropertyHelper.DEVPKEY_DeviceContainer_Category, 
                    //DevicePropertyHelper.DEVPKEY_DeviceContainer_PrimaryCategory, 
                    })
                {
                    object value;
                    if (this.DeviceItem.Data.Container.Properties.TryGetValue(key, out value))
                    {
                        if (value != null)
                        {
                            if (value is string[])
                            {
                                value = (value as string[]).Aggregate((x, y) => x + "\n" + y);
                            }
                            yield return new Tuple<string, object>(m_propertyNameLocalizer.Localize(key), value);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  デバイスのプロパティの一覧の文字列.
        /// </summary>
        public string DevicePropertiesText
        {
            get { return MakePropertiesText(this.DeviceProperties); }
        }

        /// <summary>
        ///  デバイスコンテナーのプロパティの一覧の文字列.
        /// </summary>
        public string ContainerPropertiesText
        {
            get { return MakePropertiesText(this.ContainerProperties); }
        }

        /// <summary>
        /// プロパティの一覧の文字列を作成する.
        /// </summary>
        /// <param name="properties">プロパティ</param>
        /// <returns>プロパティの一覧の文字列</returns>
        private static string MakePropertiesText(IEnumerable<Tuple<string, object>> properties)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var prop in properties)
            {
                if ((prop.Item1 == null) || (prop.Item2 == null))
                {
                    continue;
                }

                sb.AppendFormat("<{0}>\n{1}\n\n", prop.Item1, prop.Item2);
            }

            return sb.ToString();
        }


        /// <summary>
        ///  プロパティ名のローカライズ.
        /// </summary>
        private DevicePropertyNameLocalizer m_propertyNameLocalizer;

        /// <summary>
        ///  非表示のデバイスかどうか.
        /// </summary>
        public System.Boolean NoShow
        {
            get
            {
                return DeviceItemVisualHelper.IsNotShownDevice(this.DeviceItem);
            }
        }

        #region INotifyPropertyChanged

        /// <summary>
        ///  INotifyPropertyChanged.PropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///  PropertyChangedを発生させる.
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }


    /// <summary>
    ///  DeviceItemVisualを表示名で並び替えるためのComparer.
    /// </summary>
    class DeviceItemVisualDisplayNameComparer :
        Comparer<DeviceItemVisual>
    {
        public override int Compare(DeviceItemVisual x, DeviceItemVisual y)
        {
            return String.Compare(x.DisplayName, y.DisplayName, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    /// <summary>
    ///  DeviceItemVisualをプロパティで並び替えるためのComparer.
    /// </summary>
    class DeviceItemVisualPropertyComparer<T> :
        Comparer<DeviceItemVisual>
    {
        public DeviceItemVisualPropertyComparer(string propertyName, IComparer<T> comparer)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("propertyName");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            this.propertyName = propertyName;
            this.comparer = comparer;
        }

        private string propertyName;
        private IComparer<T> comparer;

        public override int Compare(DeviceItemVisual x, DeviceItemVisual y)
        {
            object valX;
            object valY;
            x.DeviceItem.Data.Device.Properties.TryGetValue(this.propertyName, out valX);
            y.DeviceItem.Data.Device.Properties.TryGetValue(this.propertyName, out valY);

            if ((valX == null) && (valY == null))
            {
                return 0;
            }
            if ((valX != null) && (valY == null))
            {
                return 1;
            }
            if ((valX == null) && (valY != null))
            {
                return -1;
            }

            if (!(valX is T) || !(valY is T))
            {
                throw new InvalidOperationException();
            }

            return this.comparer.Compare((T)valX, (T)valY);
        }
    }


    static class DeviceItemVisualHelper
    {
        public static System.Boolean IsNotShownDevice(TreeNode<DeviceItem> item)
        {
            if (!item.IsRoot)
            {
                // Windows 8.1 (か8.1 update)で仕様が変わったのか
                // ルート(HTREE\0)はここで非表示デバイスになってしまう
                // このためIsRootでその可能性を除外しておく
                if (DevicePropertyHelper.IsNotShowInDeviceManager(item.Data.Device))
                {
                    return true;
                }
            }

            return IsNotShownDevice(item.Data);
        }

        private static System.Boolean IsNotShownDevice(DeviceItem item)
        {
            if (item == null)
            {
                return true;
            }

            if (item.Container == null)
            {
                return true;
            }
            if (!(item.Container.Properties[DevicePropertyHelper.SystemDevicesConnected] as bool?).GetValueOrDefault(false))
            {
                return true;
            }
            if (item.Information != null)
            {
                if (item.Information.All(x => !x.IsEnabled))
                {
                    return true;
                }
            }
            if (!(item.Device.Properties[DevicePropertyHelper.DEVPKEY_Device_ProblemCode] as UInt32?).HasValue)
            {
                if (!item.NoSystemItemNameDisplay)
                {
                    return true;
                }
            }

            return false;
        }

    }




    /// <summary>
    ///  日付だけをつかってDateTimeを比較するComparer実装.
    /// </summary>
    class DateTimeComparer :
        Comparer<DateTime>
    {
        public override int Compare(DateTime x, DateTime y)
        {
            return x.Date.CompareTo(y.Date);
        }
    }

    /// <summary>
    ///  日付だけをつかってDateTimeを比較するComparer実装.
    /// </summary>
    class DateTimeOffsetComparer :
        Comparer<DateTimeOffset>
    {
        public DateTimeOffsetComparer()
        {
            this.Mode = DateTimeOffsetComparer.ComparisonMode.OldIsLarge;
        }

        public override int Compare(DateTimeOffset x, DateTimeOffset y)
        {
            return x.Date.CompareTo(y.Date) * (int)this.Mode;
        }

        /// <summary>
        ///  比較の種類.
        /// </summary>
        public enum ComparisonMode
        {
            NewIsLarge = 1, 
            OldIsLarge = -1, 
        }

        public ComparisonMode Mode { get; set; }
    }

    public class DateTimeOffsetValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset)
            {
                return Windows.Globalization.DateTimeFormatting.DateTimeFormatter.LongDate.Format((DateTimeOffset)value);
            }
            else
            {
                return value;
            }
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    ///  デバイスクラスの名前をローカライズする.
    /// </summary>
    /// <remarks>
    ///  APIではうまくいかないので、デバイスクラス名のresourceファイルを用意して
    ///  変換している。このresourceファイルはdevconのclassesオプションで作成した。
    /// </remarks>
    class DeviceClassNameLocalizer
    {
        private DeviceClassNameLocalizer()
        {
        }

        public static DeviceClassNameLocalizer Instance
        {
            get { return s_instance; }
        }
        static readonly DeviceClassNameLocalizer s_instance = new DeviceClassNameLocalizer();

        /// <summary>
        ///  デバイスクラス名をローカライズする.
        /// </summary>
        /// <param name="propertyName">デバイスクラス名</param>
        /// <returns>ローカライズされたデバイスクラス名</returns>
        public System.String Localize(String deviceClassName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(deviceClassName));

            var localized = this.resourceLoader.GetString(deviceClassName);
            if (String.IsNullOrEmpty(localized))
            {
                localized = deviceClassName;
            }
            return localized;
        }

        /// <summary>
        ///  デバイスのプロパティのデバイスクラス名をローカライズする.
        /// </summary>
        /// <param name="deviceItem">デバイス</param>
        /// <returns>ローカライズされたデバイスクラス名</returns>
        public System.String LocalizeDeviceClassName(TreeNode<DeviceItem> deviceItem)
        {
            System.Diagnostics.Debug.Assert(deviceItem != null);
            System.Diagnostics.Debug.Assert(deviceItem.Data != null);
            System.Diagnostics.Debug.Assert(deviceItem.Data.Device != null);

            var name = deviceItem.Data.Device.Properties[DevicePropertyHelper.DEVPKEY_Device_Class] as string;
            if (String.IsNullOrEmpty(name))
            {
                return null;
            }

            return Localize(name);
        }

        /// <summary>
        ///  リソース.
        /// </summary>
        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader = 
            new Windows.ApplicationModel.Resources.ResourceLoader("DeviceClassName");
    }


    class DeviceItemPropertyFunc
    {
        public string PropertyName { get; set; }
        public object GetProperty(TreeNode<DeviceItem> deviceItem)
        {
            return deviceItem.Data.Device.Properties[this.PropertyName];
        }
    }


    /// <summary>
    ///  デバイスのプロパティの名前をローカライズする.
    /// </summary>
    class DevicePropertyNameLocalizer
    {
        /// <summary>
        ///  デバイスのプロパティ名をローカライズする.
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>ローカライズされたプロパティ名</returns>
        public System.String Localize(String propertyName)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(propertyName));

            var localized = this.resourceLoader.GetString(propertyName.Replace('.', '_'));
            if (String.IsNullOrEmpty(localized))
            {
                localized = propertyName;
            }
            return localized;
        }

        /// <summary>
        ///  リソース.
        /// </summary>
        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader =
            new Windows.ApplicationModel.Resources.ResourceLoader("DeviceProperty");
    }

    /// <summary>
    ///  DeviceItemVisualを接続別ツリー表示用に並び替えるためのComparer.
    /// </summary>
    class DeviceItemVisualConnectionTreeComparer :
        Comparer<DeviceItemVisual>
    {

        public override int Compare(DeviceItemVisual x, DeviceItemVisual y)
        {
            if (x.DeviceItem.Parent == y.DeviceItem.Parent)
            {
                return this.m_nameComparer.Compare(x, y);
            }

            var xDepth = x.DeviceItem.Depth;
            var yDepth = y.DeviceItem.Depth;

            DeviceItemVisual x2 = x;
            DeviceItemVisual y2 = y;
            if (xDepth < yDepth)
            {
                var yAncestor = y.DeviceItem.GetAncestor(yDepth - xDepth);
                if (yAncestor == null)
                {
                    return -1;
                }

                y2 = yAncestor.Data.Visual;
                if (y2 == x)
                {
                    return -1;
                }
            }
            else if (xDepth > yDepth)
            {
                var xAncestor = x.DeviceItem.GetAncestor(xDepth - yDepth);
                if (xAncestor == null)
                {
                    return 1;
                }

                x2 = xAncestor.Data.Visual;
                if (x2 == y)
                {
                    return 1;
                }
            }

            while (x2.DeviceItem.Parent != y2.DeviceItem.Parent)
            {
                x2 = x2.DeviceItem.Parent.Data.Visual;
                y2 = y2.DeviceItem.Parent.Data.Visual;
            }

            return this.m_nameComparer.Compare(x2, y2);
        }

        private DeviceItemVisualDisplayNameComparer m_nameComparer = new DeviceItemVisualDisplayNameComparer();
    }

}
