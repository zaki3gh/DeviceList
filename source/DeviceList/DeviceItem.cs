
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;

namespace MyApps.DeviceList
{
    public class DeviceItem
    {
        public PnpObject Container { get; set; }
        public PnpObject Device { get; set; }
        public List<DeviceInformation> Information { get; set; }
        public DeviceItemVisual Visual { get; set; }

        /// <summary>
        ///  デバイスの名前.
        /// </summary>
        public System.String Name
        {
            get
            {
                var ret = this.Device.Properties[DevicePropertyHelper.SystemItemNameDisplay] as string;
                if (ret == null)
                {
                    return String.Empty;
                }
                return ret;
            }
        }

        public System.Boolean NoSystemItemNameDisplay
        {
            get { return this.Device.Properties[DevicePropertyHelper.SystemItemNameDisplay] == null; }
        }

        public System.String ContainerName
        {
            get { 
                if (this.Container != null) {
                    var ret = this.Container.Properties[DevicePropertyHelper.SystemDevicesFriendlyName] as string;
                    if (ret != null)
                    {
                        return ret;
                    }
                }
                return String.Empty;
            }
        }
    }


    class DeviceItemMap
    {
        public DeviceItemMap(Dictionary<string, TreeNode<DeviceItem>> deviceItemSet)
        {
            this.deviceItemSet = deviceItemSet;
        }

        public Dictionary<string, TreeNode<DeviceItem>> DeviceItemSet
        {
            get
            {
                return this.deviceItemSet;
            }
        }
        private readonly Dictionary<string, TreeNode<DeviceItem>> deviceItemSet;
    }

    class DeviceItemMapBuilder
    {
        static public async Task<DeviceItemMap> Build()
        {
            var devProps = new string[]{
                DevicePropertyHelper.SystemDevicesChildren, 
                DevicePropertyHelper.SystemDevicesContainerId, 
                DevicePropertyHelper.SystemDevicesDeviceHasProblem, 
                DevicePropertyHelper.SystemDevicesDeviceInstanceId, 
                DevicePropertyHelper.SystemItemNameDisplay, 
                DevicePropertyHelper.SystemDevicesHardwareIds, 

                DevicePropertyHelper.DEVPKEY_Device_Class, 
                DevicePropertyHelper.DEVPKEY_Device_ClassGuid, 
                DevicePropertyHelper.DEVPKEY_Device_DevNodeStatus, 
                DevicePropertyHelper.DEVPKEY_Device_ProblemCode, 

                DevicePropertyHelper.DEVPKEY_Device_Driver, 
                DevicePropertyHelper.DEVPKEY_Device_Manufacturer, 
                DevicePropertyHelper.DEVPKEY_Device_DriverDate, 
                DevicePropertyHelper.DEVPKEY_Device_DriverVersion, 
                DevicePropertyHelper.DEVPKEY_Device_DriverProvider, 

                DevicePropertyHelper.DEVPKEY_Device_InstallDate, 
                DevicePropertyHelper.DEVPKEY_Device_FirstInstallDate, 
                DevicePropertyHelper.DEVPKEY_Device_LastArrivalDate, 
                DevicePropertyHelper.DEVPKEY_Device_LastRemovalDate, 
            };

            var pnpObjDevs = await Windows.Devices.Enumeration.Pnp.PnpObject.FindAllAsync(
                Windows.Devices.Enumeration.Pnp.PnpObjectType.Device, devProps);

            var devDict = new Dictionary<string, TreeNode<DeviceItem>>(pnpObjDevs.Count);
            foreach (var obj in pnpObjDevs)
            {
                var devItem = RegisterDeviceToDeviceItemDictionary(obj, devDict);
                RegisterDeviceToParentOfChildren(devItem, devDict);
            }

            var devInfoSet = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync();
            AssociateDeviceToInformation(devInfoSet, devDict);

            RemoveNullData(devDict);
            foreach (var dev in devDict)
            {
                // Sort
                if (dev.Value.HasChildren)
                {
                    dev.Value.Children.Sort((x, y) => x.Data.Name.CompareTo(y.Data.Name));
                }
            }

            // DevContainer
            var devContainersDict = await GetDeviceContainers();
            AssociateDeviceToContainer(devContainersDict, devDict);

            return new DeviceItemMap(devDict);
        }

        /// <summary>
        ///  デバイスのPnpObjectをツリーの情報を保持するDeviceItemとしてDictionaryに登録する.
        /// </summary>
        /// <param name="device">デバイスのPnpObject</param>
        /// <param name="dict">登録先</param>
        /// <returns>登録されたデバイスのDeviceItemを返す</returns>
        private static TreeNode<DeviceItem> RegisterDeviceToDeviceItemDictionary(PnpObject device, Dictionary<string, TreeNode<DeviceItem>> dict)
        {
            System.Diagnostics.Debug.Assert(device != null);
            System.Diagnostics.Debug.Assert(dict != null);

            TreeNode<DeviceItem> devItem;
            if (dict.ContainsKey(device.Id))
            {
                System.Diagnostics.Debug.Assert(dict[device.Id].Data == null);
                System.Diagnostics.Debug.Assert(dict[device.Id].Parent != null);

                devItem = dict[device.Id];
                devItem.Data = new DeviceItem() { Device = device };
            }
            else
            {
                devItem = new TreeNode<DeviceItem> { Data = new DeviceItem() { Device = device } };
                dict.Add(device.Id, devItem);
            }

            return devItem;
        }

        /// <summary>
        ///  子デバイスに自分が親と登録する.
        /// </summary>
        /// <param name="devItem">自分自身を示すデバイス</param>
        /// <param name="dict">登録先</param>
        private static void RegisterDeviceToParentOfChildren(TreeNode<DeviceItem> devItem, Dictionary<string, TreeNode<DeviceItem>> dict)
        {
            System.Diagnostics.Debug.Assert(devItem != null);
            System.Diagnostics.Debug.Assert(dict != null);

            var children = devItem.Data.Device.Properties[DevicePropertyHelper.SystemDevicesChildren] as string[];
            if (children == null)
            {
                return;
            }

            devItem.Children = new List<TreeNode<DeviceItem>>(children.Length);
            foreach (var child in children)
            {
                if (dict.ContainsKey(child))
                {
                    System.Diagnostics.Debug.Assert(dict[child].Data != null);
                    System.Diagnostics.Debug.Assert(dict[child].Parent == null);

                    dict[child].Parent = devItem;
                    devItem.Children.Add(dict[child]);
                }
                else
                {
                    var childItem = new TreeNode<DeviceItem> { Parent = devItem };
                    dict.Add(child, childItem);
                    devItem.Children.Add(childItem);
                }
            }
        }

        /// <summary>
        ///  デバイスとDeviceInformation (デバイスインターフェイス) とを関連付ける.
        /// </summary>
        /// <param name="devInfoSet">DeviceInformation (デバイスインターフェイス)</param>
        /// <param name="devDict">デバイス登録情報</param>
        private static void AssociateDeviceToInformation(DeviceInformationCollection devInfoSet, Dictionary<string, TreeNode<DeviceItem>> devDict)
        {
            foreach (var devInfo in devInfoSet)
            {
                var devInstId = devInfo.Properties[DevicePropertyHelper.SystemDevicesDeviceInstanceId] as string;
                if (devDict[devInstId].Data.Information == null)
                {
                    devDict[devInstId].Data.Information = new List<DeviceInformation>();
                }
                devDict[devInstId].Data.Information.Add(devInfo);
            }
        }

        /// <summary>
        ///  無効なデータを取り除く.
        /// </summary>
        /// <param name="dict">デバイス登録情報</param>
        private static void RemoveNullData(Dictionary<string, TreeNode<DeviceItem>> dict)
        {
            foreach (var item in dict)
            {
                if (item.Value.HasChildren)
                {
                    item.Value.Children.RemoveAll(x => x.Data == null);
                    if (item.Value.Children.Count == 0)
                    {
                        item.Value.Children = null;
                    }
                }
            }

            for (; ; )
            {
                var nullData = dict.FirstOrDefault(x => x.Value.Data == null);
                if (nullData.Key == null)
                {
                    break;
                }
                dict.Remove(nullData.Key);
            }
        }

        /// <summary>
        ///  Device Containerを列挙する.
        /// </summary>
        /// <returns>列挙されたDevice Containerを返す</returns>
        private static async Task<Dictionary<Guid, PnpObject>> GetDeviceContainers()
        {
            var props = new string[] {
                DevicePropertyHelper.SystemItemNameDisplay, 
                DevicePropertyHelper.SystemDevicesFriendlyName, 
                DevicePropertyHelper.SystemDevicesModelName, 
                DevicePropertyHelper.SystemDevicesManufacturer, 
                DevicePropertyHelper.SystemDevicesInLocalMachineContainer, 
                DevicePropertyHelper.SystemDevicesDiscoveryMethod, 
                DevicePropertyHelper.SystemDevicesConnected, 
                DevicePropertyHelper.SystemDevicesIsNetworkConnected, 
                DevicePropertyHelper.DEVPKEY_DeviceContainer_Category, 
                DevicePropertyHelper.DEVPKEY_DeviceContainer_PrimaryCategory, 
            };
            var devContainers = await PnpObject.FindAllAsync(PnpObjectType.DeviceContainer, props);

            var idContainerDict = new Dictionary<Guid, PnpObject>(devContainers.Count);
            foreach (var container in devContainers) 
            {
                var guid = Guid.Parse(container.Id);
                idContainerDict.Add(guid, container);
            }

            return idContainerDict;
        }

        /// <summary>
        ///  デバイスとDeviceContainerとを関連付ける.
        /// </summary>
        /// <param name="containerDict">Device Container</param>
        /// <param name="deviceDict">デバイス登録情報</param>
        private static void AssociateDeviceToContainer(
            Dictionary<Guid, PnpObject> containerDict,
            Dictionary<string, TreeNode<DeviceItem>> deviceDict)
        {
            foreach (var device in deviceDict)
            {
                var containerId = device.Value.Data.Device.Properties[DevicePropertyHelper.SystemDevicesContainerId] as Guid?;
                if (containerId.HasValue)
                {
                    device.Value.Data.Container = containerDict[containerId.Value];
                }
            }
        }
    }
}
