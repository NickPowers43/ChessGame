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

        public int rank;
        public int file;
        public int player;
        public int moved = 0;

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
            }
            //update the graphics display
            else //some kind of error must be flagged, noise would be ideal
                Console.WriteLine("Illegal move");
        }
    

        //check to make sure the move is legal
        abstract public bool isLegal(Board board, int newFile, int newRank);

        public int getPlayer()
        {
            return player;
        }

        public bool isEmpty()
        {
            if (this == null)
                return true;
            else return false;
        }

        public Piece(int color)
        {


        }

        //move logic for the extended piece
        public void move();

        //check to make sure the move is legal
        public bool isLegal();
       
    }
}
