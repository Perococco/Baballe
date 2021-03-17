using Raylib_cs;

namespace Baballe
{
    public class BasicPlaygroundDrawer : IPlaygroundDrawer
    {
        public void Draw(Playground playground)
        {
            var cellSize = playground.CellSize;
            foreach (var wall in playground.Borders())
            {
                DrawBorder(cellSize,wall);
            }
            foreach (var wall in playground.Walls())
            {
                DrawWall(cellSize,wall);
            }
            foreach (var coin in playground.Coins())
            {
                DrawCoin(cellSize,coin);
            }
        }

        private void DrawBorder(int cellSize, Point2D wall)
        {
            Raylib.DrawRectangle(wall.X*cellSize,wall.Y*cellSize, cellSize,cellSize,Color.ORANGE);            
        }

        protected virtual void DrawWall(int cellSize, Point2D wall)
        {
            Raylib.DrawRectangle(wall.X*cellSize,wall.Y*cellSize, cellSize,cellSize,Color.RED);            
        }

        protected virtual void DrawCoin(int cellSize, Point2D coin)
        {
            Raylib.DrawCircle(coin.X*cellSize+cellSize/2,
                coin.Y*cellSize+cellSize/2,
                cellSize*0.5f,Color.YELLOW);            
        }
    }
}