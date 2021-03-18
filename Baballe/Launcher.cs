using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Resources;
using Raylib_cs;

namespace Baballe
{
    public class Launcher
    {
        static void Main(string[] args)
        {
            var launcher = new Launcher();
            launcher.Initialize();
            launcher.Loop();
        }

        private Font _font;
        private Game _game;
        private Playground _playground;
        private Header _header;
        private Camera2D _camera = new Camera2D();

        public Launcher()
        {
        }
    
        /// <summary>
        /// Hack till I understand how to embed resource
        /// </summary>
        /// <returns>The path to the embedded resource (if you are lucky)</returns>
        public string GuessFontPath()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var idx = currentDir.IndexOf("Baballe/Baballe");
            if (idx < 0)
            {
                return "";
            }

            return currentDir.Substring(0, idx + "Baballe/Baballe".Length) + "/Resources/Roboto-Regular.ttf";

        }
        
        public void Initialize()
        {
            Raylib.SetConfigFlags(ConfigFlag.FLAG_MSAA_4X_HINT); 
            Raylib.InitWindow(800, 480, "Baballe");
            Raylib.SetWindowState(ConfigFlag.FLAG_WINDOW_RESIZABLE);
            Raylib.SetTargetFPS(60); // Set our game to run at 60 frames-per-second

            Console.WriteLine(GuessFontPath());
            
            _font = Raylib.LoadFont(GuessFontPath());

            _game = new Game(Playground.Create(40, 64, 20), 16);
            _header = new Header(_font);
        }

        public void Loop()
        {
            double elapsed = 0;
            double lastTime = -1;
            while (!Raylib.WindowShouldClose())
            {
                var bounced = false;
                double currentTime = Raylib.GetTime();
                if (lastTime < 0)
                {
                    lastTime = currentTime;
                }
                else
                {
                    float dt = (float) (currentTime - lastTime);
                    lastTime = currentTime;
                    bounced = _game.Update(dt);
                    if (!_game.Paused && !_game.GameOver)
                    {
                        elapsed += dt;
                    }
                }

                
                _header.Update(_game.Score, elapsed);


                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                {
                    if (_game.GameOver)
                    {
                        _game.Reset();
                        _header.Reset();
                    }
                    else
                    {
                        _game.Resume();
                    }
                }
                if (Raylib.IsKeyReleased(KeyboardKey.KEY_UP)) _game.MoveUp();
                else if (Raylib.IsKeyReleased(KeyboardKey.KEY_DOWN)) _game.MoveDown();


                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                PrepareCameraForHeader();
                Raylib.BeginMode2D(_camera);
                _header.Draw();
                Raylib.EndMode2D();
                
                
                PrepareCameraForGame();
                Raylib.BeginMode2D(_camera);
                _game.Draw();
                Raylib.EndMode2D();

                if (_game.Paused || _game.GameOver)
                {
                    _camera.zoom = 1;
                    Raylib.BeginMode2D(_camera);
                    DisplayHelpText();
                    Raylib.EndMode2D();
                }


                
                
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();

        }

        private void DisplayHelpText()
        {
            if (_game.GameOver)
            {
                DisplayHelpText(new List<string> {"SPACE to restart"});
            }
            else
            {
                DisplayHelpText(new List<string> {"SPACE to Start", "UP/DOWN to move"});
                
            }

        }

        private void DisplayHelpText(IList<string> texts)
        {
            var fontSize = 24;
            var sizes =texts.Select(t => Raylib.MeasureTextEx(_font, t, fontSize, 0.0f)).ToList();

            var height = sizes.Select(s => s.Y).Sum();
            var width = sizes.Select(s => s.X).Max();

            var XCenter = _camera.target.X;
            var YCenter = _camera.target.Y;
            var margin = 10;
            var boxWidth = width + 2 * margin;
            var boxHeight = height + 2 * margin;

            Raylib.DrawRectangle(
                (int)(XCenter-boxWidth*0.5f),
                (int)(YCenter-boxHeight*0.5),
                (int)boxWidth,
                (int)boxHeight,
                new Color(128,127,128,128));


            
            var position = new Vector2(0, YCenter - boxHeight*0.5f + margin);
            for (int i = 0; i < texts.Count; i++)
            {
                position.X = XCenter - sizes[i].X * 0.5f;
                Raylib.DrawTextEx(_font, texts[i], position, fontSize, 0.0f, Color.BLACK);
                position.Y += sizes[i].Y;
            }
        }
        
        private int Spacing = 0;
        
        private void PrepareCameraForGame()
        {
            var width = Raylib.GetScreenWidth();
            var height = Raylib.GetScreenHeight();
            var gameHeight = height - (_header.RequiredHeight + Spacing);
            _camera.offset.X = width * 0.5f;
            _camera.offset.Y = _header.RequiredHeight + Spacing + gameHeight * 0.5f;
            _camera.target = _game.PlaygroundCenter();
            _camera.zoom = Math.Min(width/_game.PlaygroundWidth(), gameHeight/_game.PlaygroundHeight());
        }
        private void PrepareCameraForHeader()
        {
            var width = Raylib.GetScreenWidth();
            var height = Raylib.GetScreenHeight();
            _camera.offset.X = width * 0.5f;
            _camera.offset.Y = _header.RequiredHeight * 0.5f;
            _camera.target = _camera.offset;
            _camera.zoom = 1;

        }


        
    }
}