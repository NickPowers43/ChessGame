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

        //move logic for the extended piece
        public void move(Board board, int newFile, int newRank) 
        {
            if (isLegal(board, newFile, newRank))
            {
                //board.Pieces[newFile, newRank] = board.Pieces[file, rank];
                //board.Pieces[file, rank] = null;
                file = newFile;
                rank = newRank;
                moved++;
                if (board.Pieces[newFile, newRank] is King)
                    board.setBoard();
                else
                {
                    board.Pieces[newFile, newRank] = this;

                    //promote pawns
                    if (this is Pawn && (rank == 0 || rank == 7))
                        board.Pieces[newFile, newRank] = new Queen(player, file, rank);
                }
                board.DropHeldPiece();
                board.SwapCurrentPlayer();
            }
            //update the graphics display
            else
            {
                //some kind of error must be flagged, noise would be ideal
                Console.Beep();
                board.Pieces[file, rank] = this;
                board.DropHeldPiece();
            } 
        }

        //check to make sure the move is legal
        public abstract bool isLegal(Board board, int newFile, int newRank);

        //return the player
        public int getPlayer()
        {
            return player;
        }

        public String getType()
        {
            return type;
        }
    }
}
