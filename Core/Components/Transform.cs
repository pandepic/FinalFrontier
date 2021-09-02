using ElementEngine;
using ElementEngine.ECS;
using FinalFrontier.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Components
{
    public struct Transform
    {
        public Entity Parent;
        public float Rotation;
        public Vector2 Position;
        public Vector2I SectorPosition;

        public Vector2 TransformedPosition
        {
            get
            {
                if (!Parent.IsAlive)
                    return Position;
                else
                {
                    ref var parentTransform = ref Parent.GetComponent<Transform>();
                    var transformMatrix =
                        Matrix3x2.CreateRotation(parentTransform.Rotation.ToRadians()) *
                        Matrix3x2.CreateTranslation(parentTransform.TransformedPosition);

                    return Vector2.Transform(Position, transformMatrix);
                }
            }
        }

        public Vector2I TransformedSectorPosition
        {
            get
            {
                if (!Parent.IsAlive)
                    return SectorPosition;
                else
                {
                    ref var parentTransform = ref Parent.GetComponent<Transform>();
                    return parentTransform.TransformedSectorPosition;
                }
            }
        }

        public static void WriteSync(NetworkPacket packet, Entity entity)
        {
            packet.Writer.Write((int)NetworkPacketDataType.SyncTransform);
            packet.Writer.Write(entity.ID);

            ref var transform = ref entity.GetComponent<Transform>();

            packet.Writer.Write(transform.Parent.IsAlive ? transform.Parent.ID : -1);
            packet.Writer.Write(transform.Rotation);
            packet.Writer.Write(ref transform.SectorPosition);
            packet.Writer.Write(ref transform.Position);

            packet.DataCount += 1;
        }
        
        public static void ReadSync(Registry registry, BinaryReader reader, GameClient gameClient)
        {
            var entityID = reader.ReadInt32();
            var parentID = reader.ReadInt32();

            var transform = new Transform();

            transform.Rotation = reader.ReadSingle();
            transform.SectorPosition = reader.ReadVector2I();
            transform.Position = reader.ReadVector2();

            if (parentID > -1)
                transform.Parent = registry.CreateEntity(parentID);

            var entity = registry.CreateEntity(entityID);
            entity.TryAddComponent(transform);
        }
    } // Transform
}
