using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessGame
{
    class Square
    {
        public int file, rank;
        //public int owner; //0=null 1=white 2=black
        private Piece piece = null;
        public Piece Piece
        {
            get
            {
                return piece;
            }
            set
            {
                piece = value;
            }
        }

        public Square(int file, int rank)
        {
            this.file = file;
            this.rank = rank;
        }

        public bool isEmpty()
        {
            if (piece == null)
                return true;
            return false;
        }

        public void setPiece(char p, int player)
        {
            if (p == 'p')
                piece = new Pawn(player);
            else if (p == 'r')
                piece = new Rook(player);
            else if (p == 'n')
                piece = new Knight(player);
            else if (p == 'b')
                piece = new Bishop(player);
            else if (p == 'q')
                piece = new Queen(player);
            else if (p == 'k')
                piece = new King(player);
        }
    }
}
