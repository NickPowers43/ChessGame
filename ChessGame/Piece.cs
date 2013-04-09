using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    abstract class Piece
    {
        const int WHITE = 1;
        const int BLACK = 2;

        public Piece(int color)
        {


        }

        //move logic for the extended piece
        public void move();

        //check to make sure the move is legal
        public bool isLegal();
       
    }
}
