﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class Game
    {
        public const int SCREEN_WIDTH = 800, SCREEN_HEIGHT = 600;

        private bool paused = true, prevSpace = false, prevZ = false;
        
        private bool prevMouseButton = false;
        private Board board;

        private const MouseButton MOUSE0 = MouseButton.Left;

        public Game()
        {
            board = new Board();
        }

        public void Update(KeyboardDevice keyboard, MouseDevice mouse, double time)
        {
            int file = 0, rank = 0;
            Vector2 coords;
            //= Board.bottomLeft;

            coords.X = (float)mouse.X / (float)SCREEN_WIDTH;
            //temp *= 8.0f;
            //file = (int)temp;
            coords.Y = (float)(SCREEN_HEIGHT - mouse.Y) / (float)SCREEN_HEIGHT;

            coords *= 2.0f;
            coords -= new Vector2(1.0f, 1.0f);
            //temp = 1.0f - temp;
            //temp *= 8.0f;
            //rank = (int)temp;
          
            coords -= Board.bottomLeft;
            coords.X /= 1.6f;
            coords.Y *= 0.5f;
            
            file = (int)(coords.X * 8.0f);
            rank = (int)(coords.Y * 8.0f);

            //Console.WriteLine(file  + ", " + rank);

            if(!paused)
                board.SubtractTime(time);

            if (keyboard[Key.Z] & keyboard[Key.Z] != prevZ)
            {
                board.UndoLastMove();
            }
            if (keyboard[Key.R])
            {
                board.SetBoard();
                board.GameOver = false;
            }
            if (keyboard[Key.Space] & keyboard[Key.Space] != prevSpace)
            {
                paused = !paused;
            }

            if (mouse[MOUSE0] & mouse[MOUSE0] != prevMouseButton)
            {
                board.OnClick(file, rank);
            }
            else
            {
                board.OnHover(file, rank);
            }

            prevMouseButton = mouse[MOUSE0];
            prevSpace = keyboard[Key.Space];
            prevZ = keyboard[Key.Z];
        }

        public void Render()
        {
            board.Draw();
        }
    }
}
