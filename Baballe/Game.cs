using System;
using System.Dynamic;
using System.Numerics;
using System.Security;
using Raylib_cs;

namespace Baballe
{
    class Game
    {
        public Playground Playground { get; }

        public Point2D PlayerPoint = new Point2D(0, 0);
        public Vector2 PlayerPosition = new Vector2(0, 0);
        public float TimePerCell = 0;
        public int DeltaX = 1;
        public int DeltaY = 0;

        public int Score = 0;
        public bool Paused = true;
        public bool GameOver { get; private set; } = false;
        public bool CrashedOnWall { get; private set; } = false;


        private IPlaygroundDrawer Drawer = new BasicPlaygroundDrawer();

        public Game(Playground playground, int nbCellsPerSecond)
        {
            Playground = playground;
            TimePerCell = 1f / nbCellsPerSecond;
            PlayerPoint = Playground.PickStartingPoint();
        }

        public void Reset()
        {
            GameOver = false;
            CrashedOnWall = false;
            Score = 0;
            DeltaX = 1;
            DeltaY = 0;
            Paused = true;
            Playground.Initialize();
            PlayerPoint = Playground.PickStartingPoint();
        }

        
        public Vector2 PlaygroundCenter()
        {
            return Playground.Center;
        }
        
        public void Resume()
        {
            Paused = false;
        }


        private float TimeOnCell = 0;

        public bool Update(float dt)
        {
            if (GameOver || Paused)
            {
                return false;
            }

            int posX;
            int posY;

            TimeOnCell += dt;
            if (TimeOnCell > TimePerCell)
            {
                posX = PlayerPoint.X + DeltaX;
                posY = PlayerPoint.Y + DeltaY;
                DeltaY = 0;
                TimeOnCell -= TimePerCell;
            }
            else
            {
                return false;
            }

            var bounced = false;
            do
            {
                var cellType = Playground.Cells[posX, posY];

                if (cellType == CellType.Empty)
                {
                    PlayerPoint = new Point2D(posX, posY);
                    return bounced;
                }

                if (cellType == CellType.Wall)
                {
                    GameOver = true;
                    CrashedOnWall = true;
                    return bounced;
                }

                bounced = true;

                if (cellType == CellType.Coin)
                {
                    GameOver = Playground.RemoveCoin(posX, posY);
                    Score += 1;
                }

                DeltaX = -DeltaX;
                posX = posX + 2 * DeltaX;
            } while (!GameOver);

            return true;
        }

        public void Draw()
        {
            DrawPlayground();
            DrawPlayer();
        }


        private void DrawPlayground()
        {
            Drawer.Draw(Playground);
        }

        private void DrawPlayer()
        {
            var cellSize = Playground.CellSize;
            PlayerPosition.X = (PlayerPoint.X + 0.5f) * cellSize;
            PlayerPosition.Y = (PlayerPoint.Y + 0.5f) * cellSize;
            Raylib.DrawCircleV(PlayerPosition, cellSize * 0.5f, Color.GREEN);
        }
        


        public void MoveUp()
        {
            DeltaY = -1;
        }

        public void MoveDown()
        {
            DeltaY = +1;
        }

        public float PlaygroundWidth()
        {
            return Playground.Width;
        }

        public float PlaygroundHeight()
        {
            return Playground.Height;
        }

    }
}