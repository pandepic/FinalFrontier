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
        public int Money;
        public int Exp;
        public RankType Rank;

        public static void WriteSync(NetworkPacket packet, Entity entity)
        {
            packet.Writer.Write((int)NetworkPacketDataType.SyncPlayerShip);
            packet.Writer.Write(entity.ID);

            ref var player = ref entity.GetComponent<PlayerShip>();

            packet.Writer.Write(player.Username);
            packet.Writer.Write(player.Money);
            packet.Writer.Write(player.Exp);
            packet.Writer.Write((int)player.Rank);

            packet.DataCount += 1;
        }

        public static void ReadSync(Registry registry, BinaryReader reader, GameClient gameClient)
        {
            var entityID = reader.ReadInt32();

            var player = new PlayerShip();

            player.Username = reader.ReadString();
            player.Money = reader.ReadInt32();
            player.Exp = reader.ReadInt32();
            player.Rank = (RankType)reader.ReadInt32();

            var entity = registry.CreateEntity(entityID);
            entity.TryAddComponent(player);

            if (player.Username == ClientGlobals.Username)
            {
                ClientGlobals.PlayerShip = entity;
                UIBuilderIngame.UpdateBuyShip();
                UIBuilderIngame.UpdateCredits(player.Money);
                UIBuilderIngame.UpdateTopbarLabel();

                if (entity.HasComponent<WorldIcon>())
                {
                    ref var worldIcon = ref entity.GetComponent<WorldIcon>();
                    worldIcon.Texture = "Markers/player_marker.png";
                    worldIcon.TextureReference = AssetManager.Instance.LoadTexture2D(worldIcon.Texture);
                    worldIcon.AtlasRect = new Rectangle(new Vector2I(0), worldIcon.TextureReference.Size);
                    worldIcon.Origin = worldIcon.AtlasRect.SizeF / 2;
                }
            }
        }

        public static Dictionary<RankType, int> RankExpRequirements = new Dictionary<RankType, int>()
        {
            {RankType.Ensign, 0 },
            {RankType.Lieutenant, 2000 },
            {RankType.LieutenantCommander, 10000 },
            {RankType.Commander, 25000 },
            {RankType.Captain, 75000 },
            {RankType.ViceAdmiral, 150000 },
            {RankType.Admiral, 500000},
        };

        public void CheckRankUp()
        {
            foreach (var (rank, exp) in RankExpRequirements)
            {
                if (Exp >= exp)
                    Rank = rank;
            }
        }

    } // PlayerShip
}
