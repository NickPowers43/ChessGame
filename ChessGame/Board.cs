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
        const int WHITE = 1;
        const int BLACK = 2;

        private int heldPieceFile, heldPieceRank;
        private int heldPieceHoverFile, heldPieceHoverRank;

        private Piece heldPiece;
        public Piece HeldPiece
        {
            get
            {
                return heldPiece;
            }
            set
            {
                heldPiece = value;
            }
        }

        private Piece[,] pieces = new Piece[SIZE, SIZE];
        public Piece[,] Pieces
        {
            get
            {
                return pieces;
            }
        }

        //TODO: reset board method
        //TODO: remove piece
        //TODO: field to hold currently held piece

        public Board()
        {
            setBoard();
        }

        public void PickupPiece(int file, int rank)
        {
            heldPieceFile = file;
            heldPieceRank = rank;

            heldPiece = pieces[file, rank];
        }
        public void SetPiece(int file, int rank)
        {
            heldPiece.move(this, file, rank);

            pieces[file, rank] = heldPiece;
            heldPiece = null;
        }
        public void HoverPiece(int file, int rank)
        {
            heldPieceHoverFile = file;
            heldPieceHoverRank = rank;
        }
        public void setBoard()
        {
            //set all squares to null
            for (int i = 0; i < pieces.GetLength(0); i++)
                for (int j = 0; j < pieces.GetLength(1); j++)
                    pieces[i, j] = null;

            for (int i = 0; i < 8; i++)
            {
                pieces[i, 1] = new Pawn(WHITE, i, 1);
                pieces[i, 6] = new Pawn(BLACK, i, 6);
            }
            //white pieces
            pieces[0, 0] = new Rook(WHITE, 0, 0);
            pieces[1, 0] = new Knight(WHITE, 1, 0);
            pieces[2, 0] = new Bishop(WHITE, 2, 0);
            pieces[3, 0] = new Queen(WHITE, 3, 0);
            pieces[4, 0] = new King(WHITE, 4, 0);
            pieces[5, 0] = new Bishop(WHITE, 5, 0);
            pieces[6, 0] = new Knight(WHITE, 6, 0);
            pieces[7, 0] = new Rook(WHITE, 7, 0);
            //black pieces
            pieces[0, 7] = new Rook(BLACK, 0, 7);
            pieces[1, 7] = new Knight(BLACK, 1, 7);
            pieces[2, 7] = new Bishop(BLACK, 2, 7);
            pieces[3, 7] = new Queen(BLACK, 3, 7);
            pieces[4, 7] = new King(BLACK, 4, 7);
            pieces[5, 7] = new Bishop(BLACK, 5, 7);
            pieces[6, 7] = new Knight(BLACK, 6, 7);
            pieces[7, 7] = new Rook(BLACK, 7, 7);
        }

    }
}
