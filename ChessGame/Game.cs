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

        private Board board;
        bool isHolding = false;

        public Game()
        {
            board = new Board();
        }

        public void Update(KeyboardDevice keyboard, MouseDevice mouse)
        {
            int file = SCREEN_WIDTH / mouse.X;
            int rank = SCREEN_WIDTH / mouse.X;

            if (mouse[MouseButton.Button1] & !isHolding)
            {
                if (isHolding)
                {
                    

                    board.PickupPiece(file, rank);
                    isHolding = true;
                }
                else
                {

                }
            }

            if (isHolding)
            {

            }
            //GraphicsManager.Instance.Update(board);
        }

        public void Render()
        {
            GraphicsManager.Instance.renderBoard(board);
        }
    }
}
