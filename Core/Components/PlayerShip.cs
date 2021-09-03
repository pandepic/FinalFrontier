using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct PlayerShip
    {
        public string Username;

        public static void WriteSync(NetworkPacket packet, Entity entity)
        {
            packet.Writer.Write((int)NetworkPacketDataType.SyncPlayerShip);
            packet.Writer.Write(entity.ID);

            ref var player = ref entity.GetComponent<PlayerShip>();

            packet.Writer.Write(player.Username);

            packet.DataCount += 1;
        }

        public static void ReadSync(Registry registry, BinaryReader reader, GameClient gameClient)
        {
            var entityID = reader.ReadInt32();

            var player = new PlayerShip();

            player.Username = reader.ReadString();

            var entity = registry.CreateEntity(entityID);
            entity.TryAddComponent(player);

            if (player.Username == ClientGlobals.Username)
            {
                ClientGlobals.PlayerShip = entity;

                if (entity.HasComponent<WorldIcon>())
                {
                    ref var worldIcon = ref entity.GetComponent<WorldIcon>();
                    worldIcon.Texture = "Markers/player_marker.png";
                    worldIcon.TextureReference = AssetManager.LoadTexture2D(worldIcon.Texture);
                    worldIcon.AtlasRect = new Rectangle(new Vector2I(0), worldIcon.TextureReference.Size);
                    worldIcon.Origin = worldIcon.AtlasRect.SizeF / 2;
                }
            }
        }
    }
}
