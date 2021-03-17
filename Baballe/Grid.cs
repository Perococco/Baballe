using System;
using System.Numerics;
using Raylib_cs;

namespace Baballe
{
    public class Grid
    {
        public int NbColumns { get; }
        public int NbRows { get; }

        public int CellSize { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2 Position { get; set; }
        
        public Vector2 Center { get; private set; }
        
        public Grid(int nbColumns, int nbRows)
        {
            NbColumns = nbColumns;
            NbRows = nbRows;
        }
        
        public void SetCellSize(int cellSize)
        {
            CellSize = cellSize;
            Recompute();
        }

        private void Recompute()
        {
            Width = (CellSize * NbColumns)+1;
            Height = (CellSize * NbRows)+1;
            Center = Position;
            Center = new Vector2(Width * 0.5f, Height*0.5f);
        }


        public void draw(Camera2D camera)
        {
            // var matrix4X4 = Raylib.GetCameraMatrix2D(camera);
            // var offset = new Vector3(OffsetX, OffsetY, 0);
            // matrix4X4.Translation += offset;

            var start = new Vector3();
            var end = new Vector3();
            for (int i = 0; i <= NbColumns; i++)
            {
                var y = (int)(CellSize * i + Position.Y);
                Raylib.DrawLine((int)Position.X, y, (int)Position.X+Width, y, Color.RED);
            }
                
            for (int i = 0; i <= NbRows; i++)
            {
                var x = (int)(CellSize * i + Position.X);
                Raylib.DrawLine(x, (int)Position.Y, x, (int)(Position.Y+Height), Color.RED);
            }


        }
    }
}