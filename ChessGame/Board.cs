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

        private bool gameOver = false;
        public bool GameOver
        {
            get
            {
                return gameOver;
            }
            set
            {
                gameOver = value;
            }
        }

        public bool[] CastlingRights = {true, true};

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

        int currentPlayer = WHITE;

        //clock time variables
        double player1Time = DEFAULT_TIME_LIMIT, player2Time = DEFAULT_TIME_LIMIT;

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

        //variables for undoLastMove
        public bool undoDone = true;
        public int lastFile, lastRank;
        public int currFile, currRank;

        //variables for heldPiece
        private int heldPieceFile, heldPieceRank;
        private int heldPieceHoverFile, heldPieceHoverRank;

        //heldPiece and property
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

        //Piece array and property
        private Piece[,] pieces = new Piece[SIZE, SIZE];
        public Piece[,] Pieces
        {
            get
            {
                return pieces;
            }
        }
        //board no-arg constructor
        public Board()
        {
            setBoard();
        }
        //pick up a piece to heldPiece from the board
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
        //(re)set the game
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
        //draw method
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

            if (heldPiece != null)
                DrawPossibleMoves(heldPieceFile, heldPieceRank, heldPiece);
        }
        //draw pieces
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
        //highlight squares the heldPiece can move to
        private void DrawPossibleMoves(int file, int rank, Piece p)
        {
            GL.Color4(Color.Green);
            var possibleMoves = p.getPossibleMoves(this);
            foreach (Square i in possibleMoves)
            {
                GL.Begin(BeginMode.Quads);
                DrawQuad(bottomLeft + new Vector2(scale.X * i.file, scale.Y * i.rank), scale/8);
                GL.End();
            }
        }
        //set heldPiece back to null
        public void ClearHeldPiece()
        {
            heldPiece = null;
        }
        //return a piece to its original square
        public void ReleasePiece()
        {
            pieces[heldPieceFile, heldPieceRank] = heldPiece;
        }
        //draw the clocks
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
        //draw the whole board
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
        //draw a board square
        private void DrawQuad(Vector2 position, Vector2 scale)
        {
            GL.Vertex2(position);
            GL.Vertex2(position + new Vector2(0, scale.Y));
            GL.Vertex2(position + scale);
            GL.Vertex2(position + new Vector2(scale.X, 0));
        }
        //change the player at turn
        public void SwapCurrentPlayer()
        {
            currentPlayer = 3 - currentPlayer;
        }
        //decrement the clock
        public void SubtractTime(double time)
        {
            if (!gameOver)
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
        }
        //reset clock times
        public void ResetTimes()
        {
            player1Time = DEFAULT_TIME_LIMIT;
            player2Time = DEFAULT_TIME_LIMIT;
        }
        //promote pawns
        public void PromotePawns()
        {
            for(int file = 0; file < SIZE; file++)
            {
                if (Pieces[file, 0] is Pawn)
                    Pieces[file, 0] = new Queen(2, file, 0);
                if (Pieces[file, 7] is Pawn)
                    Pieces[file, 7] = new Queen(1, file, 7);
            }
        }
        //determine if a player is in check
        public void checkState()
        {
            for (int file = 0; file < SIZE; file++)
            {
                for (int rank = 0; rank < SIZE; rank++)
                {
                    Piece p = pieces[file, rank];
                    if (p != null)
                    {
                        if (p.getPlayer() == currentPlayer)
                        {
                            var possibleMoves = p.getPossibleMoves(this);
                            foreach (Square i in possibleMoves)
                            {
                                Piece p2 = pieces[i.file, i.rank];
                                if (p2 != null)
                                    if (p2.getPlayer() != currentPlayer && p2 is King)
                                        Console.WriteLine("Check!");
                            }
                        }
                    }
                }
            }
        }
        //execute legal move
        private void SetHeldPiece(int file, int rank)
        {
            if (heldPiece != null)
            {
                undoDone = false;
                currFile = file;
                currRank = rank;
                lastFile = heldPieceFile;
                lastRank = heldPieceRank;
                lastEatenPiece = pieces[file, rank];
                pieces[file, rank] = heldPiece;
                pieces[file, rank].file = file;
                pieces[file, rank].rank = rank;
                if (heldPiece is King)
                {
                    if (CastlingRights[currentPlayer - 1])
                    {
                        {
                            if (file == 2) //queenside
                            {
                                pieces[3, rank] = pieces[0, rank];
                                pieces[3, rank].file = 3;
                                pieces[0, rank] = null;
                            }
                            if (file == 6) //kingside
                            {
                                pieces[5, rank] = pieces[7, rank];
                                pieces[5, rank].file = 5;
                                pieces[7, rank] = null;
                            }
                        }
                    }
                    else CastlingRights[currentPlayer - 1] = false;
                }

                ClearHeldPiece();
                checkState();
                PromotePawns();
                SwapCurrentPlayer();
                if (lastEatenPiece is King)
                {
                    Console.Beep(600, 500);
                    gameOver = true;
                }
            }
            else
            {
                Console.WriteLine("there is no held piece to set");
            }
        }
        //determine what happens on a mouse click
        public void OnClick(int file, int rank)
        {
            if (!gameOver)
            {
                if (heldPiece == null)
                {
                    PickupPiece(file, rank);

                }
                else
                {
                    if (heldPiece.isLegalMove(this, file, rank))
                    {
                        SetHeldPiece(file, rank);
                        pieces[file, rank].moved++;
                        Console.WriteLine(DisplayNotation(file, rank));
                    }
                    else
                    {
                        ReleasePiece();
                        ClearHeldPiece();
                        Console.Beep();
                    }
                }
            }
        }
        //set heldPiece rank and file values
        public void OnHover(int file, int rank)
        {
            heldPieceHoverFile = file;
            heldPieceHoverRank = rank;
        }
        //undo the previous turn
        public void UndoLastMove()
        {
            if (!undoDone)
            {
                undoDone = true;
                Pieces[lastFile, lastRank] = Pieces[currFile, currRank];
                Pieces[lastFile, lastRank].file = lastFile;
                Pieces[lastFile, lastRank].rank = lastRank;
                Pieces[lastFile, lastRank].moved--;
                Pieces[currFile, currRank] = lastEatenPiece;
                if (lastEatenPiece != null)
                {
                    lastEatenPiece.file = currFile;
                    lastEatenPiece.rank = currRank;
                }
                SwapCurrentPlayer(); 
            }
        }
        //return the current move as a string
        public string DisplayNotation(int file, int rank)
        {
            string Notation = "";
            if (pieces[file, rank] != null)
            {
                Notation += (pieces[file, rank].getPieceType() + " to " + getFile(file) + (rank+1));
            }
            return Notation;
        }
        //convert file index into a-h notation
        public static char getFile(int file)
        {
            return (char)(97 + file);
        }
    }
}
