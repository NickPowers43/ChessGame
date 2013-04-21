using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class Game
    {
        private Board board;

        public Game()
        {
            board = new Board();
        }

        public void Update()
        {
            //GraphicsManager.Instance.Update(board);
        }

        public void Render()
        {
            GraphicsManager.Instance.renderBoard(board);
        }
    }
}
