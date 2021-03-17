using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Baballe
{
    public class Playground
    {
        public static Playground Create(int nbRows, int nbColumns, int cellSize)
        {
            var playGround = new Playground(nbRows, nbColumns, cellSize);
            playGround.Initialize();
            return playGround;
        }

        public readonly int NbRows;
        public readonly int NbColumns;
        public readonly int CellSize;
        public readonly int Width;
        public readonly int Height;

        public CellType[,] Cells { get; }

        public readonly ISet<Point2D> Borders = new HashSet<Point2D>();
        public readonly ISet<Point2D> Coins = new HashSet<Point2D>();
        public readonly ISet<Point2D> Walls = new HashSet<Point2D>();
        public Vector2 Center { get; private set; }

        private Playground(int nbRows, int nbColumns, int cellSize)
        {
            NbRows = nbRows;
            CellSize = cellSize;
            NbColumns = nbColumns;
            Cells = new CellType[nbColumns, nbRows];
            Height = nbRows * CellSize;
            Width = nbColumns * CellSize;
            Center = new Vector2(CellSize * NbColumns * 0.5f, CellSize * NbRows * 0.5f);
        }

        public void Initialize()
        {
            PlaygroundInitializer.Initialize(this);
        }

        public Point2D PickStartingPoint()
        {
            Point2D? result = null;
            int nbPicked = 0;
            for (int x = 1; x < NbColumns-1; x++)
            {
                for (int y = 1; y < NbRows-1; y++)
                {
                    if (IsNotWall(x - 1, y) && IsEmpty(x, y) && IsNotWall(x + 1, y))
                    {
                        var point = new Point2D(x, y);
                        var value = Raylib.GetRandomValue(0, nbPicked);
                        if (value == nbPicked) 
                        {
                            result = point;
                        }

                        nbPicked++;
                    }
                }
            }

            return result ?? throw new NoValidStartingPositionFound();
        }

        private bool IsNotWall(int x, int y)
        {
            return Cells[x, y] != CellType.Wall;
        }
        private bool IsEmpty(int x, int y)
        {
            return Cells[x, y] == CellType.Empty;
        }

        public void SetCellType(Point2D position, CellType cellType)
        {
            Func<Point2D, bool> action = cellType switch
            {
                CellType.Border => Borders.Add,
                CellType.Coin => Coins.Add,
                CellType.Wall => Walls.Add,
                _ => p => true,
            };
            action.Invoke(position);
            Cells[position.X, position.Y] = cellType;
        }


        public bool RemoveCoin(int gridX, int gridY)
        {
            Coins.Remove(new Point2D(gridX, gridY));
            Cells[gridX, gridY] = CellType.Empty;
            return Coins.Count == 0;
        }
    }
}