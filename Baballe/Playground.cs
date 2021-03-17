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

        public Vector2 Position { get; set; } = Vector2.Zero;

        public int NbRows { get; }
        public int NbColumns { get; }

        public int CellSize { get; }

        public int Width { get; }
        public int Height { get; }

        public CellType[,] Cells { get; }

        private readonly ISet<Point2D> _borders = new HashSet<Point2D>();
        private readonly ISet<Point2D> _coins  = new HashSet<Point2D>();
        private readonly ISet<Point2D> _walls = new HashSet<Point2D>();
        public Vector2 Center { get; private set; }

        private Playground(int nbRows, int nbColumns, int cellSize)
        {
            NbRows = nbRows;
            CellSize = cellSize;
            NbColumns = nbColumns;
            Cells = new CellType[nbColumns, nbRows];
            Height = nbRows * CellSize;
            Width = nbColumns * CellSize;
        }

        public IEnumerable<Point2D> Borders()
        {
            return _borders;
        }

        public IEnumerable<Point2D> Coins()
        {
            return _coins;
        }

        public IEnumerable<Point2D> Walls()
        {
            return _walls;
        }
        
        public void Initialize()
        {
            Center = new Vector2(CellSize * NbColumns * 0.5f, CellSize * NbRows * 0.5f);
            var nbItems = (NbColumns - 2) * (NbRows - 2) / 20;

            ClearPlayground();
            SetupBorders();
            AddAtRandomPosition(CellType.Coin, nbItems);
            AddAtRandomPosition(CellType.Wall, nbItems);
        }

        private void ClearPlayground()
        {
            for (var y = 0; y < NbRows; y++)
            {
                for (var x = 0; x < NbColumns; x++)
                {
                    Cells[x, y] = CellType.Empty;
                }
            }
        }

        private void SetupBorders()
        {
            for (var y = 0; y < NbRows; y++)
            {
                AddCell(0, y, CellType.Border);
                AddCell(NbColumns - 1, y, CellType.Border);
            }

            for (var x = 0; x < NbColumns; x++)
            {
                AddCell(x, 0, CellType.Border);
                AddCell(x, NbRows - 1, CellType.Border);
            }
        }

        private void AddAtRandomPosition(CellType cellType, int nb)
        {
            for (int i = 0; i < nb; i++)
            {
                AddAtRandomPosition(cellType);
            }
        }

        private void AddAtRandomPosition(CellType cellType)
        {
            int x;
            int y;
            PickRandomEmptyPosition(out x, out y);
            AddCell(x, y, cellType);
        }


        private void SetCellType(Point2D position, CellType cellType)
        {
            Func<Point2D,bool> action = cellType switch
            {
                CellType.Border => _borders.Add,
                CellType.Coin => _coins.Add,
                CellType.Wall => _walls.Add,
                _ => p => true,
            };
            action.Invoke(position);
        }

        private void AddCell(int x, int y, CellType cellType)
        {
            var point = new Point2D(x, y);
            SetCellType(point, cellType);
            Cells[x, y] = cellType;
        }

        public void PickRandomEmptyPosition(out int x, out int y)
        {
            do
            {
                x = Raylib.GetRandomValue(1, NbColumns - 2);
                y = Raylib.GetRandomValue(1, NbRows - 2);
            } while (Cells[x, y] != CellType.Empty);
        }

        public bool RemoveCoin(int gridX, int gridY)
        {
            _coins.Remove(new Point2D(gridX, gridY));
            Cells[gridX, gridY] = CellType.Empty;
            return _coins.Count == 0;
        }
    }
}