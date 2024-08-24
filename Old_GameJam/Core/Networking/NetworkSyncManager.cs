using ElementEngine.ECS;
using FinalFrontier.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Networking
{
    public static class NetworkSyncManager
    {
        public class NetworkSyncType
        {
            public NetworkPacketDataType NetworkEnum;
            public Action<NetworkPacket, Entity> WriteFunction;
            public Action<Registry, BinaryReader, GameClient> ReadFunction;

            public NetworkSyncType(NetworkPacketDataType networkEnum, Action<NetworkPacket, Entity> writeFunction, Action<Registry, BinaryReader, GameClient> readFunction)
            {
                NetworkEnum = networkEnum;
                WriteFunction = writeFunction;
                ReadFunction = readFunction;
            }
        } // NetworkSyncType

        public static class EveryFrameSyncGroups<T> where T : struct
        {
            public static Group Group;
        }
        
        public static class TempSyncGroups<T> where T : struct
        {
            public static Group Group;
        }

        public static class PlayedJoinedSyncGroups<T> where T : struct
        {
            public static Group Group;
        }

        public static Dictionary<Type, NetworkSyncType> TempNetworkSyncTypes = new Dictionary<Type, NetworkSyncType>();
        public static Dictionary<Type, NetworkSyncType> EveryFrameNetworkSyncTypes = new Dictionary<Type, NetworkSyncType>();
        public static Dictionary<NetworkPacketDataType, Action<Registry, BinaryReader, GameClient>> NetworkSyncReadFunctions
            = new Dictionary<NetworkPacketDataType, Action<Registry, BinaryReader, GameClient>>();
        
        public static List<Action<NetworkPacket>> ServerTempSyncLoops = new List<Action<NetworkPacket>>();
        public static List<Action<NetworkPacket>> ServerPlayerJoinedSyncLoops = new List<Action<NetworkPacket>>();
        public static List<Action<NetworkPacket>> ServerEveryFrameSyncLoops = new List<Action<NetworkPacket>>();
        
        public static Registry Registry;

        public static void LoadShared()
        {
            TempNetworkSyncTypes.Clear();
            EveryFrameNetworkSyncTypes.Clear();
            NetworkSyncReadFunctions.Clear();
            ServerTempSyncLoops.Clear();
            ServerPlayerJoinedSyncLoops.Clear();
            ServerEveryFrameSyncLoops.Clear();

            RegisterTempSyncType<Transform>(NetworkPacketDataType.SyncTransform, Transform.WriteSync, Transform.ReadSync);
            RegisterTempSyncType<Drawable>(NetworkPacketDataType.SyncDrawable, Drawable.WriteSync, Drawable.ReadSync);
            RegisterTempSyncType<Colony>(NetworkPacketDataType.SyncColony, Colony.WriteSync, Colony.ReadSync);
            RegisterTempSyncType<WorldIcon>(NetworkPacketDataType.SyncWorldIcon, WorldIcon.WriteSync, WorldIcon.ReadSync);
            RegisterTempSyncType<Ship>(NetworkPacketDataType.SyncShip, Ship.WriteSync, Ship.ReadSync);
            RegisterTempSyncType<PlayerShip>(NetworkPacketDataType.SyncPlayerShip, PlayerShip.WriteSync, PlayerShip.ReadSync);
            RegisterTempSyncType<WorldSpaceLabel>(NetworkPacketDataType.SyncWorldSpaceLabel, WorldSpaceLabel.WriteSync, WorldSpaceLabel.ReadSync);
            RegisterTempSyncType<Shield>(NetworkPacketDataType.SyncShield, Shield.WriteSync, Shield.ReadSync);
            RegisterTempSyncType<Armour>(NetworkPacketDataType.SyncArmour, Armour.WriteSync, Armour.ReadSync);
            RegisterTempSyncType<Inventory>(NetworkPacketDataType.SyncInventory, Inventory.WriteSync, Inventory.ReadSync);

            RegisterEveryFrameSyncGroup<Transform>(NetworkPacketDataType.SyncTransform, Transform.WriteSync, Transform.ReadSync);
        }

        public static void LoadServer()
        {
            var registerMethod = typeof(NetworkSyncManager).GetMethod("RegisterTempAndPlayerJoinedSyncGroups");

            foreach (var (type, sync) in TempNetworkSyncTypes)
            {
                var genericMethod = registerMethod.MakeGenericMethod(type);
                genericMethod.Invoke(null, new object[] { sync.WriteFunction });
            }
        }

        public static void LoadClient()
        {
            foreach (var (type, sync) in TempNetworkSyncTypes)
            {
                NetworkSyncReadFunctions.Add(sync.NetworkEnum, sync.ReadFunction);
            }

            foreach (var (type, sync) in EveryFrameNetworkSyncTypes)
            {
                if (NetworkSyncReadFunctions.ContainsKey(sync.NetworkEnum))
                    continue;

                NetworkSyncReadFunctions.Add(sync.NetworkEnum, sync.ReadFunction);
            }
        }

        public static void RegisterTempSyncType<T>(
            NetworkPacketDataType packetType,
            Action<NetworkPacket, Entity> writeFunction,
            Action<Registry, BinaryReader, GameClient> readFunction) where T : struct
        {
            TempNetworkSyncTypes.Add(typeof(T), new NetworkSyncType(packetType, writeFunction, readFunction));
        }

        public static void RegisterTempAndPlayerJoinedSyncGroups<T>(Action<NetworkPacket, Entity> packetWriteFunction) where T : struct
        {
            RegisterTempSyncGroup<T>(packetWriteFunction);
            RegisterPlayerJoinedSyncGroup<T>(packetWriteFunction);
        }

        public static void RegisterTempSyncGroup<T>(Action<NetworkPacket, Entity> packetWriteFunction) where T : struct
        {
            var group = Registry.RegisterGroup<T, TempSync<T>>();
            TempSyncGroups<T>.Group = group;

            ServerTempSyncLoops.Add((packet) =>
            {
                foreach (var entity in GetTempSyncGroup<T>().Entities)
                {
                    packetWriteFunction(packet, entity);
                    entity.RemoveComponent<TempSync<T>>();
                }
            });
        }

        public static void RegisterPlayerJoinedSyncGroup<T>(Action<NetworkPacket, Entity> packetWriteFunction) where T : struct
        {
            var group = Registry.RegisterGroup<T, PlayerJoinedSync<T>>();
            PlayedJoinedSyncGroups<T>.Group = group;

            ServerPlayerJoinedSyncLoops.Add((packet) =>
            {
                foreach (var entity in GetPlayerJoinedSyncGroup<T>().Entities)
                    packetWriteFunction(packet, entity);
            });
        }

        public static void RegisterEveryFrameSyncGroup<T>(
            NetworkPacketDataType packetType,
            Action<NetworkPacket, Entity> writeFunction,
            Action<Registry, BinaryReader, GameClient> readFunction) where T : struct
        {
            var group = Registry.RegisterGroup<T, EveryFrameSync<T>>();
            EveryFrameSyncGroups<T>.Group = group;

            ServerEveryFrameSyncLoops.Add((packet) =>
            {
                foreach (var entity in GetEveryFrameSyncGroup<T>().Entities)
                    writeFunction(packet, entity);
            });

            EveryFrameNetworkSyncTypes.Add(typeof(T), new NetworkSyncType(packetType, writeFunction, readFunction));
        }

        public static Group GetTempSyncGroup<T>() where T : struct
        {
            return TempSyncGroups<T>.Group;
        }

        public static Group GetPlayerJoinedSyncGroup<T>() where T : struct
        {
            return PlayedJoinedSyncGroups<T>.Group;
        }

        public static Group GetEveryFrameSyncGroup<T>() where T : struct
        {
            return EveryFrameSyncGroups<T>.Group;
        }

    } // ServerSyncManager
}
