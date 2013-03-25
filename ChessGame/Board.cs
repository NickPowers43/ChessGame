using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class Board
    {
        const int SIZE = 8;

        private Piece[,] pieces = new Piece[SIZE, SIZE];

        public Board()
        {

        }
    }
}
