﻿using System;
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

        public const float
            PIECE_TEX_SIZE = 1.0f / 3.0f,
            PIECE_HEIGHT = 2.0f / 8.0f,
            PIECE_WIDTH = 2.0f / 10.0f,
            TIME_BAR_WIDTH = PIECE_WIDTH,
            BOARD_WIDTH = PIECE_WIDTH * 8.0f;

        const int
            DEFAULT_TIME_LIMIT = 300;

        Color DARK_COLOR = Color.Brown;
        Color LIGHT_COLOR = Color.SandyBrown;

        public static Vector2 bottomLeft = new Vector2(PIECE_WIDTH - 1.0f, -1);
        public static Vector2 scale = new Vector2(PIECE_WIDTH, PIECE_HEIGHT);
        public static Vector2 boardSize = scale * 8.0f;

        int currentPlayer = 1;//Piece.WHITE;
        double 
            player1Time = DEFAULT_TIME_LIMIT,
            player2Time = DEFAULT_TIME_LIMIT;

        //last moved piece data
        private Piece lastEatenPiece;
        public Piece LastEatenPiece
        {
            get
            {
                return lastEatenPiece;
            }
            set
            {
                lastEatenPiece = value;
            }
        }
        private bool lastMoveCapture = false;
        public bool undoDone = true;
        public bool LastMoveCapture
        {
            get
            {
                return lastMoveCapture;
            }
            set
            {
                lastMoveCapture = value;
            }
        }
        public int lastFile, lastRank;
        public int currFile, currRank;

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

        public bool PickupPiece(int file, int rank)
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

                    return true;
                }
                else
                {
                    Console.Beep(600, 200); 
                    return false;
                } 
            }
            else return false;
        }
        public void SetPiece(int file, int rank)
        {
            //Console.WriteLine("Setting piece to: " + file + ", " + rank);
            //check if move is legal

            if (heldPiece != null)
            {
                if (heldPiece.isLegal(this, file, rank))
                {
                    heldPiece.moved++;
                    MovePiece(heldPiece.file, heldPiece.rank, file, rank);
                    
                    SwapCurrentPlayer();
                }
            }
        }
        public void HoverPiece(int file, int rank)
        {
            heldPieceHoverFile = file;
            heldPieceHoverRank = rank;
        }
        public void setBoard()
        {
            currentPlayer = WHITE;

            ResetTimes();
            heldPiece = null;

            //reset player times
            player1Time = DEFAULT_TIME_LIMIT;
            player2Time = DEFAULT_TIME_LIMIT;

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
            DrawTimeBar();

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
                Pawn.Draw(bottomLeft + new Vector2(scale.X * file, scale.Y * rank), scale);
            else if (piece is Bishop)
                Bishop.Draw(bottomLeft + new Vector2(scale.X * file, scale.Y * rank), scale);
            else if (piece is King)
                King.Draw(bottomLeft + new Vector2(scale.X * file, scale.Y * rank), scale);
            else if (piece is Rook)
                Rook.Draw(bottomLeft + new Vector2(scale.X * file, scale.Y * rank), scale);
            else if (piece is Knight)
                Knight.Draw(bottomLeft + new Vector2(scale.X * file, scale.Y * rank), scale);
            else if (piece is Queen)
                Queen.Draw(bottomLeft + new Vector2(scale.X * file, scale.Y * rank), scale);
        }
        public void LetGoOfPiece()
        {
            heldPiece = null;
        }
        private void DrawTimeBar()
        {
            float x = -1;
            float y = -1;


            double quotient = player1Time / DEFAULT_TIME_LIMIT;
            GL.Color4(new Color4(1, (float)quotient, (float)quotient, 1));

            GL.Begin(BeginMode.Quads);
            GL.Vertex2(x, y);
            y += (float)quotient * 2.0f;
            GL.Vertex2(x, y);
            x += TIME_BAR_WIDTH;
            GL.Vertex2(x, y);
            y = -1;
            GL.Vertex2(x, y);

            quotient = player2Time / DEFAULT_TIME_LIMIT;
            GL.Color4(new Color4(1, (float)quotient, (float)quotient, 1));
            x = 1;
            y = -1;
            GL.Vertex2(x, y);
            x -= TIME_BAR_WIDTH;
            GL.Vertex2(x, y);
            y += (float)quotient * 2.0f;
            GL.Vertex2(x, y);
            x += TIME_BAR_WIDTH;
            GL.Vertex2(x, y);

            GL.End();
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

                    DrawQuad(bottomLeft + new Vector2(scale.X * j, scale.Y * i), scale);
                }
                swap0 = !swap0;
            }
            GL.End();
        }
        private void DrawQuad(Vector2 position, Vector2 scale)
        {
            GL.Vertex2(position);
            GL.Vertex2(position + new Vector2(0, scale.Y));
            GL.Vertex2(position + scale);
            GL.Vertex2(position + new Vector2(scale.X, 0));
        }
        public void SwapCurrentPlayer()
        {
            currentPlayer = 3 - currentPlayer;
        }
        public void SubtractTime(double time)
        {
            if (currentPlayer == 1)
            {
                player1Time -= time;
            }
            else
            {
                player2Time -= time;
            }
        }
        public void ResetTimes()
        {
            player1Time = DEFAULT_TIME_LIMIT;
            player2Time = DEFAULT_TIME_LIMIT;
        }

        private void MovePiece(int file, int rank, int newFile, int newRank)
        {
            undoDone = false;
            lastEatenPiece = pieces[newFile, newRank];
            currFile = newFile;
            currRank = newRank;
            lastFile = file;
            lastRank = rank;
            pieces[newFile, newRank] = heldPiece;
            pieces[file, rank] = null;
            LetGoOfPiece();

            if (pieces[newFile, newRank] is King)
                setBoard();
        }
        private void SetHeldPiece(int file, int rank)
        {
            if (heldPiece != null)
            {
                SwapCurrentPlayer();
                undoDone = false;
                currFile = file;
                currRank = rank;
                lastFile = heldPiece.file;
                lastRank = heldPiece.rank;
                pieces[file, rank] = heldPiece;
                LetGoOfPiece();
                DisplayNotation(file, rank);
            }
            else
            {
                Console.WriteLine("there is no held piece to set");
            }
        }
        public void OnClick(int file, int rank)
        {
            if (heldPiece == null)
            {
                PickupPiece(file, rank);
            }
            else
            {
                Console.WriteLine("picking up piece");
                if (heldPiece.isLegal(this, file, rank))
                {
                    SetHeldPiece(file, rank);
                }
            }
        }
        public void OnHover(int file, int rank)
        {
            heldPieceHoverFile = file;
            heldPieceHoverRank = rank;
        }
        public void UndoLastMove()
        {
            if (!undoDone)
            {
                undoDone = true;
                Pieces[lastFile, lastRank] = Pieces[currFile, currRank];
                Pieces[currFile, currRank] = lastEatenPiece;
                SwapCurrentPlayer(); 
            }
        }

        public void DisplayNotation(int file, int rank)
        {
            if (pieces[file, rank] != null)
            {
                Console.WriteLine(pieces[file, rank].getType() + " to " + getFile(file) + rank);
            }
        }

        public static char getFile(int file)
        {
            return (char)(97 + file);
        }


    }
}
