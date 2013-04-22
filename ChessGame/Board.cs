using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

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

        const float
            PIECE_TEX_SIZE = 1.0f / 3.0f,
            PIECE_SCALE = 2.0f / 8.0f;

        Color DARK_COLOR = Color.Brown;
        Color LIGHT_COLOR = Color.SandyBrown;

        static Vector2 bottomLeft = new Vector2(-1, -1);

        int currentPlayer = 1;//Piece.WHITE;

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
            Piece piece = pieces[file, rank];

            if (piece != null)
            {
                if (piece.getPlayer() == currentPlayer)
                {
                    heldPieceFile = file;
                    heldPieceRank = rank;

                    heldPiece = piece;

                    pieces[file, rank] = null;
                }
                else
                    Console.Beep(600, 200); 
            }
        }
        public void SetPiece(int file, int rank)
        {
            Console.WriteLine("Setting piece to: " + file + ", " + rank);
            //check if move is legal


            if (heldPiece != null)
            {
                //bool result = heldPiece.isLegal(this, file, rank);

                heldPiece.move(this, file, rank);
            }
            //pieces[file, rank] = heldPiece;
            //heldPiece = null;
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
        public void Draw()
        {
            DrawCheckerBoard();

            GL.ClearDepth(0);

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    if (file == heldPieceHoverFile & rank == heldPieceHoverRank & heldPiece != null)
                        DrawPiece(file, rank, heldPiece);
                    else
                    {
                        Piece temp = pieces[file, rank];

                        if (temp != null)
                            DrawPiece(file, rank, temp);
                    }


                }
            }
        }
        private void DrawPiece(int file, int rank, Piece piece)
        {
            if (piece.getPlayer() == 1)
                GL.Color4(Color.White);
            else
                GL.Color4(Color.Black);

            if (piece is Pawn)
                Pawn.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
            else if (piece is Bishop)
                Bishop.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
            else if (piece is King)
                King.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
            else if (piece is Rook)
                Rook.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
            else if (piece is Knight)
                Knight.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
            else if (piece is Queen)
                Queen.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
        }
        public void DropHeldPiece()
        {
            heldPiece = null;

        }
        private void DrawCheckerBoard()
        {
            GL.Begin(BeginMode.Quads);
            bool swap0 = false;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    swap0 = !swap0;
                    if (swap0)
                        GL.Color4(DARK_COLOR);
                    else
                        GL.Color4(LIGHT_COLOR);

                    DrawQuad(bottomLeft + new Vector2(PIECE_SCALE * j, PIECE_SCALE * i), PIECE_SCALE);
                }
                swap0 = !swap0;
            }
            GL.End();
        }
        private void DrawQuad(Vector2 position, float scale)
        {
            GL.Vertex2(position);
            GL.Vertex2(position + new Vector2(0, 1) * PIECE_SCALE);
            GL.Vertex2(position + new Vector2(1, 1) * PIECE_SCALE);
            GL.Vertex2(position + new Vector2(1, 0) * PIECE_SCALE);
        }
        public void SwapCurrentPlayer()
        {
            if (currentPlayer == 1)
            {
                currentPlayer = 2;
            }
            else
            {
                currentPlayer = 1;
            }
        }
    }
}
