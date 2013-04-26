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
        public int rank;
        public int file;
        public int player;
        public int moved = 0;

        //3-arg Piece constructor
        public Piece(int player, int file, int rank)
        {
            this.player = player;
            this.file = file;
            this.rank = rank;
        }

        //return a list of all possible moves for a piece
        public abstract List<Square> getPossibleMoves(Board board);

        //check to make sure the piece stays on the board
        public bool inBounds(int file, int rank)
        {
            if (file >= 0 && rank >= 0 && file < 8 && rank < 8)
                return true;
            return false;
        }

        //determine if a move is legal
        public bool isLegalMove(Board board, int newFile, int newRank)
        {
            var possibleMoves = getPossibleMoves(board);
            foreach (Square i in possibleMoves)
                if (newFile == i.file && newRank == i.rank)
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
    }
}
