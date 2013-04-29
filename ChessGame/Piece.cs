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
        public const int WHITE = 1;
        public const int BLACK = 2;

        public string type;
        public char pieceChar;
        public int player;
        public int moved = 0;

        //3-arg Piece constructor
        public Piece(int player)
        {
            this.player = player;
        }

        //return a list of all possible moves for a piece
        public abstract List<Square> getPossibleMoves(Board board, Square s);

        //check to make sure the piece stays on the board
        public bool inBounds(int file, int rank)
        {
            if (file >= 0 && rank >= 0 && file < 8 && rank < 8)
                return true;
            return false;
        }

        //determine if a move is legal
        public bool isLegalMove(Board board, int file, int rank, Square s)
        {
            var possibleMoves = getPossibleMoves(board, s);
            foreach (Square i in possibleMoves)
                if (file == i.file && rank == i.rank)
                    return true;
            return false;
        }

        //return the player
        public int getPlayer()
        {
            return player;
        }

        //return the piece type
        public String getPieceType()
        {
            return type;
        }

        public char getPieceChar()
        {
            return pieceChar;
        }
    }
}
