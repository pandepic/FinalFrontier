using ElementEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier
{
    public static class ClientUtility
    {
        private static List<Vector2I> _visibleSectors = new List<Vector2I>();

        public static List<Vector2I> GetVisibleSectors(Camera2D camera, Vector2I cameraSector)
        {
            var cameraView = camera.ScaledView;

            _visibleSectors.Clear();
            _visibleSectors.Add(cameraSector);

            var sectorTopLeft = cameraView.Location / Globals.GalaxySectorScale;
            var sectorBottomRight = cameraView.BottomRight / Globals.GalaxySectorScale;

            for (var x = sectorTopLeft.X; x <= sectorBottomRight.X; x++)
            {
                for (var y = sectorTopLeft.Y; y <= sectorBottomRight.Y; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    _visibleSectors.Add(cameraSector + new Vector2I(x, y));
                }
            }

            return _visibleSectors;

        } // GetVisibleSectors
    }
}
