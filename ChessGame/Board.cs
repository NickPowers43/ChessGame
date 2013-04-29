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

        Stack<string> moves = new Stack<String>();

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

        //Square array and property
        private Square[,] square = new Square[SIZE, SIZE];
        public Square[,] Square
        {
            get
            {
                return square;
            }
            set 
            {
                square = value;
            }
        }

        //board no-arg constructor
        public Board()
        {
            InitializeSquares();
            SetBoard();
        }
        //pick up a piece to heldPiece from the board. doesn't need to be a bool, can be void?
        public bool PickupPiece(int file, int rank)
        {
            

            if (!square[file, rank].isEmpty()) //&& owner = currentPlayer
            {
                Piece piece = square[file, rank].Piece;
                if (piece.getPlayer() == currentPlayer)
                {
                    heldPieceFile = file;
                    heldPieceRank = rank;
                    heldPiece = piece;
                    square[file, rank].Piece = null;
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

        //initialize square array
        public void InitializeSquares()
        {
            for (int file = 0;  file < 8 ; file++)
                for (int rank = 0; rank < 8; rank++)
                    Square[file,rank] = new Square(file,rank);
        }
        //(re)set the board
        public void SetBoard()
        {
            currentPlayer = WHITE;

            ResetTimes();
            heldPiece = null;

            //reset player times
            player1Time = DEFAULT_TIME_LIMIT;
            player2Time = DEFAULT_TIME_LIMIT;

            CastlingRights[0] = true;
            CastlingRights[1] = true;

            //set all squares to null
            for (int i = 2; i < square.GetLength(0); i++)
                for (int j = 2; j < 6; j++)
                  square[i,j].Piece = null;

            for (int i = 0; i < 8; i++)
            {
                square[i, 1].Piece = new Pawn(WHITE);
                square[i, 6].Piece = new Pawn(BLACK);
            }
            //white pieces
            square[0, 0].Piece = new Rook(WHITE);
            square[1, 0].Piece = new Knight(WHITE);
            square[2, 0].Piece = new Bishop(WHITE);
            square[3, 0].Piece = new Queen(WHITE);
            square[4, 0].Piece = new King(WHITE);
            square[5, 0].Piece = new Bishop(WHITE);
            square[6, 0].Piece = new Knight(WHITE);
            square[7, 0].Piece = new Rook(WHITE);
            //black square
            square[0, 7].Piece = new Rook(BLACK);
            square[1, 7].Piece = new Knight(BLACK);
            square[2, 7].Piece = new Bishop(BLACK);
            square[3, 7].Piece = new Queen(BLACK);
            square[4, 7].Piece = new King(BLACK);
            square[5, 7].Piece = new Bishop(BLACK);
            square[6, 7].Piece = new Knight(BLACK);
            square[7, 7].Piece = new Rook(BLACK);
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
                        Piece temp = Square[file, rank].Piece;

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
            var possibleMoves = p.getPossibleMoves(this, Square[file, rank]);
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
            Square[heldPieceFile, heldPieceRank].Piece = heldPiece;
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
                if (Square[file, 0].Piece is Pawn)
                    Square[file, 0].Piece = new Queen(BLACK);
                if (Square[file, 7].Piece is Pawn)
                    Square[file, 7].Piece = new Queen(WHITE);
            }
        }
        //determine if a player is in check
        public bool CheckState()
        {
            for (int file = 0; file < SIZE; file++)
            {
                for (int rank = 0; rank < SIZE; rank++)
                {
                    if (!square[file, rank].isEmpty())
                    {
                    Piece p = square[file, rank].Piece;
                    
                        if (p.getPlayer() == currentPlayer)
                        {
                            var possibleMoves = p.getPossibleMoves(this, square[file, rank]);
                            foreach (Square i in possibleMoves)
                            {

                                if (!square[i.file, i.rank].isEmpty())
                                {
                                    Piece p2 = square[i.file, i.rank].Piece;
                                    if (p2.getPlayer() != currentPlayer && p2 is King)
                                    {
                                        Console.WriteLine("Check!");
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        //execute legal move
        private void SetHeldPiece(int file, int rank)
        {
            if (heldPiece != null)
            {
                Console.WriteLine(DisplayNotation(file, rank));
                undoDone = false;
                currFile = file;
                currRank = rank;
                lastFile = heldPieceFile;
                lastRank = heldPieceRank;
                lastEatenPiece = square[file, rank].Piece;
                square[file, rank].Piece = heldPiece;
                if (heldPiece is King)
                {
                    if (CastlingRights[currentPlayer - 1])
                    {
                        {
                            if (file == 2) //queenside
                            {
                                square[3, rank].Piece = square[0, rank].Piece;
                                square[0, rank].Piece = null;
                            }
                            if (file == 6) //kingside
                            {
                                square[5, rank].Piece = square[7, rank].Piece;
                                square[7, rank].Piece = null;
                            }
                        }
                    }
                    else CastlingRights[currentPlayer - 1] = false;
                }
                ClearHeldPiece();
                PromotePawns();                 
                square[file, rank].Piece.moved++;
                if (lastEatenPiece is King)
                {
                    Console.Beep(600, 500);
                    Console.WriteLine(GetCurrentPlayer() + " wins!");
                    gameOver = true;
                }
                SwapCurrentPlayer();
                if (CheckState())
                    UndoLastMove();
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
                    moves.Push(boardToFen());
                    PickupPiece(file, rank);

                }
                else
                {
                    if (heldPiece.isLegalMove(this, file, rank, square[heldPieceFile, heldPieceRank]))
                    {
                        SetHeldPiece(file, rank);
                    }
                    else
                    {
                        moves.Pop();
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
            if (moves.Count > 0)
            {
                fenToBoard(moves.Pop());
                SwapCurrentPlayer();
            }

            /*if (!undoDone)
            {
                undoDone = true;
                square[lastFile, lastRank].Piece = square[currFile, currRank].Piece;
                square[lastFile, lastRank].Piece.moved--;
                square[currFile, currRank].Piece = lastEatenPiece;
                SwapCurrentPlayer(); 
            }*/
        }
        //return the current move as a string
        public string DisplayNotation(int file, int rank)
        {
            string Notation = heldPiece.getPieceType();
            Notation += ((Square[file, rank].isEmpty()) ? " to " : " takes ");
            Notation += getFile(file) + "" + (rank+1);
            return Notation;
        }
        //convert file index into a-h notation
        public static char getFile(int file)
        {
            return (char)(97 + file);
        }
        //return the current player as a string
        public string GetCurrentPlayer()
        {
            if (currentPlayer == 1)
                return "White";
            return "Black";
        }

        public void ClearBoard()
        {
            for (int file = 0; file < SIZE; file++)
                for (int rank = 0; rank < SIZE; rank++)
                    square[file, rank].Piece = null;
        }

        public string boardToFen()
        {
            string fen = "";
            int blank;
            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 0; file < SIZE; file++)
                {
                    blank = 0;
                    if (square[file, rank].isEmpty())
                    {
                        blank++;
                        while (file < 7)
                        {
                            file++;
                            if (!square[file, rank].isEmpty())
                            {
                                file--;
                                break;
                            }
                            blank++;
                        }
                        fen += blank;
                        continue;
                    }
                    char piece = square[file, rank].Piece.getPieceChar();
                    fen += (square[file, rank].Piece.getPlayer() == 1) ? Char.ToUpper(piece) : Char.ToLower(piece);
                }
            }
            Console.WriteLine(fen);
            return fen;
        }

        public void fenToBoard(string fen)
        {
            ClearBoard();
            char[] fenArray = fen.ToCharArray();
            for (int i = 0, file = 0, rank = 7; i < fenArray.Length; i++)
            {
                Console.WriteLine(i + ": " + fenArray[i]);
                if(file > 7) 
                {
                    file = 0;
                    rank--;
                }
                if(Char.IsDigit(fenArray[i]))
                    for(int j=0; j<Char.GetNumericValue(fenArray[i]);j++)
                    {
                        if (file > 7)
                            break;
                        Console.WriteLine(file + "," + rank + " = " + getPiece(fenArray[i]));
                        square[file,rank].Piece = null;
                        file++;
                    }
                else if (Char.IsLetter(fenArray[i]))
                {
                    square[file, rank].Piece = getPiece(fenArray[i]);
                    Console.WriteLine(file + "," + rank + " = " + getPiece(fenArray[i]));
                    file++;
                }
            }

        }

        public Piece getPiece(char p)
        {
            switch (p)
            {
                case 'p':
                    return new Pawn(BLACK);
                case 'r':
                    return new Rook(BLACK);
                case 'n':
                    return new Knight(BLACK);   
                case 'b':
                    return new Bishop(BLACK);                    
                case 'q':
                    return new Queen(BLACK);                    
                case 'k':
                    return new King(BLACK);                    
                case 'P':
                    return new Pawn(WHITE);                   
                case 'R':
                    return new Rook(WHITE);                    
                case 'N':
                    return new Knight(WHITE);                    
                case 'B':
                    return new Bishop(WHITE);                    
                case 'Q':
                    return new Queen(WHITE);                    
                case 'K':
                    return new King(WHITE);
                default:
                    return null;
            }
        }
    }
}
