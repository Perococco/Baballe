using System;
using System.Globalization;
using System.Numerics;
using Raylib_cs;

namespace Baballe
{
    public class Header
    {
        private Font _font;
        public int RequiredHeight { get; }= 30;


        private string ScoreText = "";
        private string TimeText = "";

        private int Score = -1;
        private int TenthOfSecond = -1;
        private int FontSize = 24;

        public Header(Font font)
        {
            _font = font;
        }

        public void Draw()
        {
            var position = new Vector2(0, 0);
            var width = Raylib.MeasureTextEx(_font, ScoreText, FontSize, 0.0f);
            Raylib.DrawTextEx(_font, ScoreText,position, FontSize,0.0f,Color.BLACK);
            position.X += width.X + 10;
            Raylib.DrawTextEx(_font,TimeText,position,FontSize,0.0f,Color.BLACK);
        }

        public void Update(int gameScore, double elapsedTime)
        {
            int newTime = (int) (elapsedTime * 10);
            if (Score != gameScore)
            {
                Score = gameScore;
                ScoreText = $"Score: {gameScore,3:D}";
            }

            if (TenthOfSecond != newTime)
            {
                TenthOfSecond = newTime;
                TimeText = $"Time: {newTime * 0.1f,5:F1}";
            }
        }

        public void Reset()
        {
            Score = -1;
            ScoreText = "";
            TimeText = "";
            TenthOfSecond = 0;
        }
    }
}