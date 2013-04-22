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
            if (isLegal(board, newRank, newFile))
            {
                board.Pieces[newFile, newRank] = board.Pieces[file, rank];
                board.Pieces[file, rank] = null;
                file = newFile;
                rank = newRank;
                moved++;
                //promote pawns
                if(board.Pieces[file,rank].getType().Equals("Pawn") && (rank == 0 || rank == 8))
                    board.Pieces[file,rank] = new Queen(getPlayer(),file,rank);
            }
            //update the graphics display
            else //some kind of error must be flagged, noise would be ideal
                Console.WriteLine("Illegal move");
        }

        //check to make sure the move is legal
        public abstract bool isLegal(Board board, int newFile, int newRank);

        //return the player
        public int getPlayer()
        {
            return player;
        }

        //check if a square is empty
        public bool isEmpty()
        {
            if (this == null)
                return true;
            else return false;
        }

        public String getType()
        {
            return type;
        }
    }
}
