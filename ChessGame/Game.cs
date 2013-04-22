using System;
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

        private bool prevMouseButton = false;
        private Board board;
        bool isHolding = false;

        private const MouseButton MOUSE0 = MouseButton.Left;

        public Game()
        {
            board = new Board();
        }

        public void Update(KeyboardDevice keyboard, MouseDevice mouse)
        {
            int file = 0, rank = 0;
            float temp = (float)mouse.X / (float)SCREEN_WIDTH;
            temp *= 8.0f;
            file = (int)temp;
            temp = (float)mouse.Y / (float)SCREEN_HEIGHT;
            temp = 1.0f - temp;
            temp *= 8.0f;
            rank = (int)temp;

            if (mouse[MOUSE0] & mouse[MOUSE0] != prevMouseButton)
            {
                if (isHolding)
                {
                    board.SetPiece(file, rank);
                    isHolding = false;
                }
                else
                {
                    board.PickupPiece(file, rank);
                    isHolding = true;
                }
            }
            else
            {
                if (isHolding)
                {
                    board.HoverPiece(file, rank);
                }
            }

            prevMouseButton = mouse[MOUSE0];
        }

        public void Render()
        {
            board.Draw();
        }
    }
}
