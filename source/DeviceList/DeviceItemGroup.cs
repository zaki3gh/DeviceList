using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApps.DeviceList
{
    public class DeviceItemGroup : Common.BindableBase
    {
        private DeviceItemGroup()
        {
        }

        static public DeviceItemGroup Instance
        {
            get { return s_instance; }
        }
        private static DeviceItemGroup s_instance = new DeviceItemGroup();

        private DeviceItemMap DeviceItemMap { get; set; }

        public bool ShouldUpdate
        {
            get { return (this.DeviceItemMap == null) || this.m_shouldUpdate; }
            private set { SetProperty(ref m_shouldUpdate, value); }
        }
        private bool m_shouldUpdate = true;

        public bool UpdatingNow
        {
            get { return this.m_updatingNow; }
            set { SetProperty(ref m_updatingNow, value); }
        }
        private bool m_updatingNow = false;

        public GroupType GroupType
        {
            get { return m_groupType; }
            set { SetProperty(ref m_groupType, value); }
        }
        private GroupType m_groupType;

        public bool ShouldShowNotShownDevice
        {
            get { return m_shouldShowNotShownDevice; }
            set { SetProperty(ref m_shouldShowNotShownDevice, value); }
        }
        private bool m_shouldShowNotShownDevice = false;

        public string SelectedItemId
        {
            get { return m_selectedItemId; }
            set { SetProperty(ref m_selectedItemId, value); }
        }
        private string m_selectedItemId;

        public System.Collections.IDictionary ItemsMap
        {
            get { return m_itemsMap; }
            private set { SetProperty(ref m_itemsMap, value); }
        }
        private System.Collections.IDictionary m_itemsMap;

        public IEnumerable<DeviceItemVisual> Items
        {
            get
            {
                if (this.ItemsMap == null)
                {
                    yield break;
                }

                foreach (var items in this.ItemsMap.Values)
                {
                    foreach (var item in items as List<DeviceItemVisual>)
                    {
                        yield return item;
                    }
                }
            }
        }

        public Task Update()
        {
            switch (this.GroupType)
            {
                case MyApps.DeviceList.GroupType.ByDeviceClass:
                    return UpdateDeviceGroup(new DeviceGroupBuildParameter<string>()
                    {
                        KeyPropertyFunc = DeviceClassNameLocalizer.Instance.LocalizeDeviceClassName,
                        KeyComparer = StringComparer.CurrentCultureIgnoreCase,
                        ItemComparer = new DeviceItemVisualDisplayNameComparer()
                    });

                case MyApps.DeviceList.GroupType.ByManufacturer:
                    return UpdateDeviceGroup(new DeviceGroupBuildParameter<string>()
                    {
                        KeyPropertyFunc = new DeviceItemPropertyFunc() { PropertyName = DevicePropertyHelper.DEVPKEY_Device_Manufacturer }.GetProperty,
                        KeyComparer = StringComparer.CurrentCultureIgnoreCase,
                        ItemComparer = new DeviceItemVisualDisplayNameComparer()
                    });

                case MyApps.DeviceList.GroupType.ByDriverProvider:
                    return UpdateDeviceGroup(new DeviceGroupBuildParameter<string>()
                    {
                        KeyPropertyFunc = new DeviceItemPropertyFunc() { PropertyName = DevicePropertyHelper.DEVPKEY_Device_DriverProvider }.GetProperty,
                        KeyComparer = StringComparer.CurrentCultureIgnoreCase,
                        ItemComparer = new DeviceItemVisualDisplayNameComparer()
                    });

                case MyApps.DeviceList.GroupType.ByInstallDate:
                    return UpdateDeviceGroup(new DeviceGroupBuildParameter<DateTimeOffset>()
                    {
                        KeyPropertyFunc = new DeviceItemPropertyFunc() { PropertyName = DevicePropertyHelper.DEVPKEY_Device_InstallDate }.GetProperty,
                        KeyComparer = new DateTimeOffsetComparer() { Mode = DateTimeOffsetComparer.ComparisonMode.OldIsLarge },
                        ItemComparer = new DeviceItemVisualDisplayNameComparer()
                    });

                case MyApps.DeviceList.GroupType.ByLastArrivalDate:
                    return UpdateDeviceGroup(new DeviceGroupBuildParameter<DateTimeOffset>()
                    {
                        KeyPropertyFunc = new DeviceItemPropertyFunc() { PropertyName = DevicePropertyHelper.DEVPKEY_Device_LastArrivalDate }.GetProperty,
                        KeyComparer = new DateTimeOffsetComparer() { Mode = DateTimeOffsetComparer.ComparisonMode.OldIsLarge },
                        ItemComparer = new DeviceItemVisualDisplayNameComparer()
                    });

                case MyApps.DeviceList.GroupType.ByConnection:
                    return UpdateDeviceGroup(new DeviceGroupBuildParameter<string>()
                    {
                        KeyPropertyFunc = x=>String.Empty, 
                        KeyComparer = StringComparer.CurrentCultureIgnoreCase, 
                        ItemComparer = new DeviceItemVisualConnectionTreeComparer()
                    });

                default:
                    throw new InvalidOperationException();
            }
        }

        async Task UpdateDeviceGroup<T>(
            DeviceGroupBuildParameter<T> parameter)
        {
            this.UpdatingNow = true;

            if (this.DeviceItemMap == null)
            {
                this.DeviceItemMap = await DeviceItemMapBuilder.Build();
            }
            var deviceItemMap = this.DeviceItemMap;
            var propertyItemMap = await Task.Run(() =>
            {
                return CreatePropertyToItemMap<T>(
                    deviceItemMap.DeviceItemSet.Values,
                    parameter.KeyPropertyFunc,
                    parameter.KeyComparer,
                    parameter.ItemComparer);
            });

            this.ItemsMap = propertyItemMap;
            this.UpdatingNow = false;
            this.ShouldUpdate = false;
        }

        private SortedDictionary<T, List<DeviceItemVisual>> CreatePropertyToItemMap<T>(
            IEnumerable<TreeNode<DeviceItem>> deviceItems,
            Func<TreeNode<DeviceItem>, Object> keyPropertyFunc,
            IComparer<T> keyComparer,
            IComparer<DeviceItemVisual> itemComparer)
        {
            var propertyToItemMap = new SortedDictionary<T, List<DeviceItemVisual>>(keyComparer);

            foreach (var deviceItem in deviceItems)
            {
                deviceItem.Data.Visual = new DeviceItemVisual(deviceItem);
                if (!this.ShouldShowNotShownDevice && DeviceItemVisualHelper.IsNotShownDevice(deviceItem))
                {
                    continue;
                }

                var objProperty = keyPropertyFunc(deviceItem);
                if ((objProperty == null) || !(objProperty is T))
                {
                    continue;
                }
                T property = (T)objProperty;
                if (!propertyToItemMap.ContainsKey(property))
                {
                    propertyToItemMap.Add(property, new List<DeviceItemVisual>());
                }
                propertyToItemMap[property].Add(new DeviceItemVisual(deviceItem));
            }

            foreach (var items in propertyToItemMap.Values)
            {
                items.Sort(itemComparer);
            }

            return propertyToItemMap;
        }
    }

    class DeviceGroupBuildParameter<T>
    {
        public Func<TreeNode<DeviceItem>, Object> KeyPropertyFunc { get; set; }
        public IComparer<T> KeyComparer { get; set; }
        public IComparer<DeviceItemVisual> ItemComparer { get; set; }
    }

    /// <summary>
    ///  グループ分けの種類.
    /// </summary>
    public enum GroupType
    {
        /// <summary>デバイスクラス毎.</summary>
        ByDeviceClass,

        /// <summary>製造元.</summary>
        ByManufacturer,

        /// <summary>ドライバー提供元.</summary>
        ByDriverProvider,

        /// <summary>インストール日毎.</summary>
        ByInstallDate,

        /// <summary>最後に接続した日毎.</summary>
        ByLastArrivalDate,

        /// <summary>接続ツリー.</summary>
        ByConnection,
    }

    /// <summary>
    ///  特定の<c>GroupType</c>の値に対して<c>Visibility</c>を指定したいときに使う<c>ValueConverter</c>.
    /// </summary>
    public class GroupTypeVisibilityValueConverter : Windows.UI.Xaml.Data.IValueConverter
    {
        public Windows.UI.Xaml.Visibility VisibilityIfEqual { get; set; }
        public Windows.UI.Xaml.Visibility VisibilityIfNotEqual { get; set; }
        public GroupType GroupType { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var groupType = value as GroupType?;
            if ((!groupType.HasValue) || (groupType.Value != this.GroupType))
            {
                return this.VisibilityIfNotEqual;
            }
            else
            {
                return this.VisibilityIfEqual;
            }
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }

}
