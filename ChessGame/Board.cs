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

        //TODO: reset board method
        //TODO: remove piece
        //TODO: field to hold currently held piece

        public Board()
        {

        }
    }
}
