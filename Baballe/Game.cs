using System;
using System.Dynamic;
using System.Numerics;
using Raylib_cs;

namespace Baballe
{
    class Game
    {
        public Playground Playground { get; }

        public Vector2 Player = new Vector2(0, 0);
        public float SpeedX = 0;
        public int DeltaY = 0;

        public int Score = 0;
        public bool Paused = true;
        public bool GameOver { get; private set; } = false;
        public bool CrashedOnWall { get; private set; } = false;


        private IPlaygroundDrawer Drawer = new BasicPlaygroundDrawer();

        public Game(Playground playground, int SpeedFactor)
        {
            Playground = playground;
            SpeedX = playground.CellSize*SpeedFactor;
            int gridX;
            int gridY;
            Playground.PickRandomEmptyPosition(out gridX, out gridY);
            Player.X = (gridX + 0.5f) * Playground.CellSize;
            Player.Y = (gridY + 0.5f) * Playground.CellSize;
        }

        public void Resume()
        {
            Paused = false;
        }

        public void Update(float dt)
        {
            if (GameOver || Paused)
            {
                return;
            }

            int cellSize = Playground.CellSize;
            var oldX = Player.X;
            var newX = oldX + SpeedX * dt;
            var oldGridX = (int) (oldX / cellSize);
            var newGridY = (int) (newX / cellSize);
            
            
            var center = (oldGridX + 0.5f) * cellSize;                
            float newY;
            if (oldGridX == newGridY && (Math.Sign(center-oldX) == Math.Sign(newX - center)))
            {
                newY = Player.Y + DeltaY * cellSize;
                DeltaY = 0;
            }
            else
            {
                newY = Player.Y;
            }


            int gridX;
            if (SpeedX > 0)
            {
                gridX = (int) Math.Floor(Player.X / cellSize + 0.5);
            }
            else
            {
                gridX = (int) Math.Floor(Player.X / cellSize - 0.5);
            }

            var gridY = (int) Player.Y / cellSize;

            var cellType = Playground.Cells[gridX, gridY];

            if (cellType == CellType.Empty)
            {
                Player.X = newX;
                Player.Y = newY;
            }
            else if (cellType == CellType.Border)
            {
                SpeedX = -SpeedX;
            }
            else if (cellType == CellType.Wall)
            {
                CrashedOnWall = true;
                GameOver = true;
            }
            else if (cellType == CellType.Coin)
            {
                SpeedX = -SpeedX;
                GameOver = Playground.RemoveCoin(gridX, gridY);
                Score += 1;
            }
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
            Raylib.DrawCircleV(Player, cellSize * 0.5f, Color.GREEN);
        }


        public void SetupCamera(Camera2D camera)
        {
            var width = Raylib.GetScreenWidth();
            var height = Raylib.GetScreenHeight();
            camera.offset.X = width * 0.5f;
            camera.offset.Y = height * 0.5f;
            camera.target = Playground.Center;
            camera.zoom = Math.Min(width / Playground.Width, height / Playground.Height);
        }


        static void Main(string[] args)
        {
            Raylib.InitWindow(800, 480, "Baballe");
            Raylib.SetWindowState(ConfigFlag.FLAG_WINDOW_RESIZABLE);

            var game = new Game(Playground.Create(20, 32, 20),8);

            var camera = new Camera2D();


            Raylib.SetTargetFPS(60); // Set our game to run at 60 frames-per-second

            camera.zoom = 1;
            double lastTime = -1;
            while (!Raylib.WindowShouldClose())
            {
                if (lastTime < 0)
                {
                    lastTime = Raylib.GetTime();
                }
                else
                {
                    double currentTime = Raylib.GetTime();
                    float dt = (float) (currentTime - lastTime);
                    lastTime = currentTime;
                    game.Update(dt);
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) game.Resume();
                if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) game.MoveUp();
                else if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) game.MoveDown();


                game.SetupCamera(camera);
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                
                Raylib.BeginMode2D(camera);

                game.Draw();

                Raylib.EndMode2D();
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        private void MoveUp()
        {
            DeltaY = -1;
        }

        private void MoveDown()
        {
            DeltaY = +1;
        }
    }
}