using System.Collections.Generic;
using Raylib_cs;

namespace Baballe
{
    public class PlaygroundInitializer
    {

        public static void Initialize(Playground playground)
        {
            new PlaygroundInitializer(playground).Initialize();            
        }
        
        private readonly Playground _playground;

        public PlaygroundInitializer(Playground playground)
        {
            _playground = playground;
        }


        private readonly IList<Point2D> _freePoints = new List<Point2D>();
        
        private void Initialize()
        {
            ClearPlayground();
            SetupBorders();
            var nbItems = (_playground.NbColumns - 2) * (_playground.NbRows - 2) / 20;

            AddAtRandomPosition(CellType.Coin,nbItems);
            AddAtRandomPosition(CellType.Wall,nbItems);
        }
        
        
        private void ClearPlayground()
        {
            for (var y = 0; y < _playground.NbRows; y++)
            {
                for (var x = 0; x < _playground.NbColumns; x++)
                {
                    var position = new Point2D(x, y);
                    _playground.SetCellType(position,CellType.Empty);
                    _freePoints.Add(position);
                }
            }
            _playground.Borders.Clear();
            _playground.Walls.Clear();
            _playground.Coins.Clear();
        }

        private void SetupBorders()
        {
            for (var y = 0; y < _playground.NbRows; y++)
            {
                var leftBorder = new Point2D(0, y);
                var rightBorder = new Point2D(_playground.NbColumns-1, y);
                _playground.SetCellType(leftBorder,CellType.Border);
                _playground.SetCellType(rightBorder,CellType.Border);
                _freePoints.Remove(leftBorder);
                _freePoints.Remove(rightBorder);
            }

            for (var x = 0; x < _playground.NbColumns; x++)
            {
                var upperBorder = new Point2D(x, 0);
                var lowerBorder = new Point2D(x,_playground.NbRows-1);
                _playground.SetCellType(upperBorder,CellType.Border);
                _playground.SetCellType(lowerBorder,CellType.Border);
                _freePoints.Remove(upperBorder);
                _freePoints.Remove(lowerBorder);
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
            var point = PickRandomEmptyPosition();
            _playground.SetCellType(point, cellType);
        }
        
        private Point2D PickRandomEmptyPosition()
        {
            if (_freePoints.Count == 0)
            {
                throw new NoMoreFreePosition();
            }

            var index = Raylib.GetRandomValue(0, _freePoints.Count - 1);

            var point = _freePoints[index];
            _freePoints.RemoveAt(index);
            return point;
        }

    }
}