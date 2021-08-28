using ElementEngine;
using ElementEngine.ECS;
using System;
using System.Collections.Generic;
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
    } // Transform
}
