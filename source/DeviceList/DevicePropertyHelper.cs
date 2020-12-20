using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApps.DeviceList
{
//    enum DeviceInterfacePropertyNameIndex
//    {
//        SystemDevicesContainerId,
//        SystemDevicesDeviceIntanceId,
//        SystemDevicesInterfaceClassGuid,
//        SystemDevicesInterfaceEnabled,
//        SystemItemNameDisplay,
//        SystemStorageIsMediaRemovable,
//        SystemStorageIsPortable,
//        DEVPKEY_DeviceInterfaceClass_Name, 
//    }

//    enum DevicePropertyNameIndex
//    {
//        SystemDevicesChildren,
//        SystemDevicesCompatibleIds,
//        SystemDevicesContainerId,
//        SystemDevicesDeviceCapabilities,
//        SystemDevicesDeviceCharacteristics,
//        SystemDevicesDeviceHasProblem,
//        SystemDevicesDeviceInstanceId,
//        SystemDevicesHardwareIds,
//        SystemDevicesInLocalMachineContainer,
//        SystemItemNameDisplay,

//        DEVPKEY_Device_DevNodeStatus,
//        DEVPKEY_Device_ProblemCode,

//        PKEY_Devices_BatteryLife,
//        PKEY_Devices_BatteryPlusCharging,
//        PKEY_Devices_BatteryPlusChargingText,
//        PKEY_Devices_Status1,
//        PKEY_Devices_Status2,

//        DEVPKEY_Device_DriverDate,
//        DEVPKEY_Device_DriverVersion,
//        DEVPKEY_Device_DriverDesc,
//        DEVPKEY_Device_DriverProvider,

//        DEVPKEY_DrvPkg_Model,
//        DEVPKEY_DrvPkg_VendorWebSite,

//        DEVPKEY_Device_DriverProblemDesc,

//        DEVPKEY_Device_Class,

//        DEVPKEY_DeviceClass_Name, 
//        DEVPKEY_DeviceClass_ClassName,  
//}

//    enum DeviceContainerPropertyNameIndex
//    {
//        SystemItemNameDisplay,
//        SystemDevicesDiscoveryMethod,
//        SystemDevicesConnected,
//        SystemDevicesPaired,
//        SystemDevicesIcon,
//        SystemDevicesLocalMachine,
//        SystemDevicesMetadataPath,
//        SystemDevicesLaunchDeviceStageFromExplorer,
//        SystemDevicesDeviceDescription1,
//        SystemDevicesDeviceDescription2,
//        SystemDevicesNotWorkingProperly,
//        SystemDevicesIsShared,
//        SystemDevicesIsNetworkConnected,
//        SystemDevicesIsDefault,
//        SystemDevicesCategory,
//        SystemDevicesCategoryPlural,
//        SystemDevicesCategoryGroup,
//        SystemDevicesFriendlyName,
//        SystemDevicesManufacturer,
//        SystemDevicesModelName,
//        SystemDevicesModelNumber,
//    }


    class DevicePropertyHelper
    {
        //public static IEnumerable<System.String> GetPropertyNames(params DeviceInterfacePropertyNameIndex[] nameIndexes)
        //{
        //    foreach (var index in nameIndexes)
        //    {
        //        yield return GetPropertyName(index);
        //    }
        //}

        //public static System.String GetPropertyName(DeviceInterfacePropertyNameIndex index)
        //{
        //    return DevicePropertyHelper.devicePropertyNames[(int)index];
        //}


        //public static IEnumerable<System.String> GetPropertyNames(params DevicePropertyNameIndex[] nameIndexes)
        //{
        //    foreach (var index in nameIndexes)
        //    {
        //        yield return GetPropertyName(index);
        //    }
        //}

        //public static System.String GetPropertyName(DevicePropertyNameIndex index)
        //{
        //    int baseIndex = Enum.GetNames(typeof(DeviceInterfacePropertyNameIndex)).Length;
        //    return DevicePropertyHelper.devicePropertyNames[baseIndex + (int)index];
        //}


        //public static IEnumerable<System.String> GetPropertyNames(params DeviceContainerPropertyNameIndex[] nameIndexes)
        //{
        //    foreach (var index in nameIndexes)
        //    {
        //        yield return GetPropertyName(index);
        //    }
        //}

        //public static System.String GetPropertyName(DeviceContainerPropertyNameIndex index)
        //{
        //    int baseIndex =
        //        Enum.GetNames(typeof(DeviceInterfacePropertyNameIndex)).Length +
        //        Enum.GetNames(typeof(DevicePropertyNameIndex)).Length;
        //    return DevicePropertyHelper.devicePropertyNames[baseIndex + (int)index];
        //}


        //private static readonly System.String[] devicePropertyNames = new System.String[] {
        //    // Device Interface
        //    "System.Devices.ContainerId", 
        //    "System.Devices.DeviceInstanceId", 
        //    "System.Devices.InterfaceClassGuid", 
        //    "System.Devices.InterfaceEnabled", 
        //    "System.ItemNameDisplay", 
        //    "System.Storage.IsMediaRemovable", 
        //    "System.Storage.IsPortable", 

        //   /*DEVPKEY_DeviceInterfaceClass_Name*/ "{14c83a99-0b3f-44b7-be4c-a178d3990564} 3", 

        //    // Device 
        //    "System.Devices.Children", 
        //    "System.Devices.CompatibleIds", 
        //    "System.Devices.ContainerId", 
        //    "System.Devices.DeviceCapabilities", 
        //    "System.Devices.DeviceCharacteristics", 
        //    "System.Devices.DeviceHasProblem", 
        //    "System.Devices.DeviceInstanceId", 
        //    "System.Devices.HardwareIds", 
        //    "System.Devices.InLocalMachineContainer", 
        //    "System.ItemNameDisplay", 

        //    /*DEVPKEY_Device_DevNodeStatus*/ "{4340a6c5-93fa-4706-972c-7b648008a5a7} 2", 
        //    /*DEVPKEY_Device_ProblemCode*/   "{4340a6c5-93fa-4706-972c-7b648008a5a7} 3", 
        //    /*PKEY_Devices_BatteryLife*/ "System.Devices.BatteryLife", 
        //    /*PKEY_Devices_BatteryPlusCharging*/ "System.Devices.BatteryPlusCharging", 
        //    /*PKEY_Devices_BatteryPlusChargingText*/ "System.Devices.BatteryPlusChargingText", 
        //    /*PKEY_Devices_Status1*/ "System.Devices.Status1", 
        //    /*PKEY_Devices_Status2*/ "System.Devices.Status2", 

        //    /*DEVPKEY_Device_DriverDate*/ "{a8b865dd-2e3d-4094-ad97-e593a70c75d6} 2", 
        //    /*DEVPKEY_Device_DriverVersion*/ "{a8b865dd-2e3d-4094-ad97-e593a70c75d6} 3", 
        //    /*DEVPKEY_Device_DriverDesc*/ "{a8b865dd-2e3d-4094-ad97-e593a70c75d6} 4", 
        //    /*DEVPKEY_Device_DriverProvider*/ "{a8b865dd-2e3d-4094-ad97-e593a70c75d6} 9", 

        //    /*DEVPKEY_DrvPkg_Model*/ "{cf73bb51-3abf-44a2-85e0-9a3dc7a12132} 2", 
        //    /*DEVPKEY_DrvPkg_VendorWebSite*/     "{cf73bb51-3abf-44a2-85e0-9a3dc7a12132} 3", 

        //    /*DEVPKEY_Device_DriverProblemDesc*/ "{540b947e-8b40-45bc-a8a2-6a0b894cbda2} 11", 

        //    /*DEVPKEY_Device_Class*/             "{a45c254e-df1c-4efd-8020-67d146a850e0} 9", 
        //    /*DEVPKEY_Device_ClassGuid*/             "{a45c254e-df1c-4efd-8020-67d146a850e0} 10", 
             
        //    /*DEVPKEY_DeviceClass_Name*/      "{259abffc-50a7-47ce-af08-68c9a7d73366} 2", 
        //    /*DEVPKEY_DeviceClass_ClassName*/ "{259abffc-50a7-47ce-af08-68c9a7d73366} 3",  

        //    // Device Container
        //    "System.ItemNameDisplay", 
        //    "System.Devices.DiscoveryMethod", 
        //    "System.Devices.Connected", 
        //    "System.Devices.Paired", 
        //    "System.Devices.Icon", 
        //    "System.Devices.LocalMachine", 
        //    "System.Devices.MetadataPath", 
        //    "System.Devices.LaunchDeviceStageFromExplorer", 
        //    "System.Devices.DeviceDescription1", 
        //    "System.Devices.DeviceDescription2", 
        //    "System.Devices.NotWorkingProperly", 
        //    "System.Devices.IsShared", 
        //    "System.Devices.IsNetworkConnected", 
        //    "System.Devices.IsDefault", 
        //    "System.Devices.Category", 
        //    "System.Devices.CategoryPlural", 
        //    "System.Devices.CategoryGroup", 
        //    "System.Devices.FriendlyName", 
        //    "System.Devices.Manufacturer", 
        //    "System.Devices.ModelName", 
        //    "System.Devices.ModelNumber", 
        //};


        // Device
        public const string SystemDevicesChildren = "System.Devices.Children";
        public const string SystemDevicesCompatibleIds = "System.Devices.CompatibleIds";
        public const string SystemDevicesContainerId = "System.Devices.ContainerId";
        public const string SystemDevicesDeviceCapabilities = "System.Devices.DeviceCapabilities";
        public const string SystemDevicesDeviceCharacteristics = "System.Devices.DeviceCharacteristics";
        public const string SystemDevicesDeviceHasProblem = "System.Devices.DeviceHasProblem";
        public const string SystemDevicesDeviceInstanceId = "System.Devices.DeviceInstanceId";
        public const string SystemDevicesHardwareIds = "System.Devices.HardwareIds";
        public const string SystemDevicesInLocalMachineContainer = "System.Devices.InLocalMachineContainer";
        public const string SystemItemNameDisplay = "System.ItemNameDisplay";

        public const string DEVPKEY_Device_DeviceDesc = "{a45c254e-df1c-4efd-8020-67d146a850e0} 2";
        public const string DEVPKEY_Device_Class = "{a45c254e-df1c-4efd-8020-67d146a850e0} 9";
        public const string DEVPKEY_Device_ClassGuid = "{a45c254e-df1c-4efd-8020-67d146a850e0} 10";
        public const string DEVPKEY_Device_Driver = "{a45c254e-df1c-4efd-8020-67d146a850e0} 11";    // DEVPROP_TYPE_STRING
        public const string DEVPKEY_Device_Manufacturer = "{a45c254e-df1c-4efd-8020-67d146a850e0} 13";    // DEVPROP_TYPE_STRING


        public const string DEVPKEY_Device_DevNodeStatus = "{4340a6c5-93fa-4706-972c-7b648008a5a7} 2";
        public const string DEVPKEY_Device_ProblemCode = "{4340a6c5-93fa-4706-972c-7b648008a5a7} 3";
        public const string DEVPKEY_Device_ProblemStatus = "{4340a6c5-93fa-4706-972c-7b648008a5a7} 12";

        public const string DEVPKEY_Device_DriverDate = "{a8b865dd-2e3d-4094-ad97-e593a70c75d6} 2";
        public const string DEVPKEY_Device_DriverVersion = "{a8b865dd-2e3d-4094-ad97-e593a70c75d6} 3";
        public const string DEVPKEY_Device_DriverDesc = "{a8b865dd-2e3d-4094-ad97-e593a70c75d6} 4";
        public const string DEVPKEY_Device_DriverProvider = "{a8b865dd-2e3d-4094-ad97-e593a70c75d6} 9";


        public const string DEVPKEY_Device_InstallDate = "{83da6326-97a6-4088-9453-a1923f573b29} 100";      // DEVPROP_TYPE_FILETIME
        public const string DEVPKEY_Device_FirstInstallDate = "{83da6326-97a6-4088-9453-a1923f573b29} 101"; // DEVPROP_TYPE_FILETIME
        public const string DEVPKEY_Device_LastArrivalDate = "{83da6326-97a6-4088-9453-a1923f573b29} 102";  // DEVPROP_TYPE_FILETIME
        public const string DEVPKEY_Device_LastRemovalDate = "{83da6326-97a6-4088-9453-a1923f573b29} 103";  // DEVPROP_TYPE_FILETIME


        // Device Container
        //public const string SystemItemNameDisplay = "System.ItemNameDisplay";
        public const string SystemDevicesDiscoveryMethod = "System.Devices.DiscoveryMethod"; 
        public const string SystemDevicesConnected = "System.Devices.Connected"; 
        public const string SystemDevicesPaired = "System.Devices.Paired"; 
        public const string SystemDevicesIcon = "System.Devices.Icon"; 
        public const string SystemDevicesLocalMachine = "System.Devices.LocalMachine"; 
        public const string SystemDevicesMetadataPath = "System.Devices.MetadataPath"; 
        public const string SystemDevicesLaunchDeviceStageFromExplorer = "System.Devices.LaunchDeviceStageFromExplorer"; 
        public const string SystemDevicesDeviceDescription1 = "System.Devices.DeviceDescription1"; 
        public const string SystemDevicesDeviceDescription2 = "System.Devices.DeviceDescription2"; 
        public const string SystemDevicesNotWorkingProperly = "System.Devices.NotWorkingProperly"; 
        public const string SystemDevicesIsShared = "System.Devices.IsShared"; 
        public const string SystemDevicesIsNetworkConnected = "System.Devices.IsNetworkConnected"; 
        public const string SystemDevicesIsDefault = "System.Devices.IsDefault"; 
        public const string SystemDevicesCategory = "System.Devices.Category"; 
        public const string SystemDevicesCategoryPlural = "System.Devices.CategoryPlural"; 
        public const string SystemDevicesCategoryGroup = "System.Devices.CategoryGroup"; 
        public const string SystemDevicesFriendlyName = "System.Devices.FriendlyName"; 
        public const string SystemDevicesManufacturer = "System.Devices.Manufacturer"; 
        public const string SystemDevicesModelName = "System.Devices.ModelName";
        public const string SystemDevicesModelNumber = "System.Devices.ModelNumber";

        public const string DEVPKEY_DeviceContainer_Category = "{78c34fc8-104a-4aca-9ea4-524d52996e57} 90";
        public const string DEVPKEY_DeviceContainer_PrimaryCategory = "{78c34fc8-104a-4aca-9ea4-524d52996e57} 97";


        public static bool IsNotShowInDeviceManager(Windows.Devices.Enumeration.Pnp.PnpObject obj)
        {
            var statusUInt32 = obj.Properties[DEVPKEY_Device_DevNodeStatus] as UInt32?;
            var status = (DevNodeStatus)statusUInt32.GetValueOrDefault(0);
            return status.HasFlag(DevNodeStatus.DN_NO_SHOW_IN_DM);
        }
    }


    [Flags]
    enum DevNodeStatus
    {
        DN_NO_SHOW_IN_DM = 0x40000000, 
    }
}
