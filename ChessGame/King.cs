using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class King : Piece
    {
        //3 arg constructor for King
        public King(int player) : base(player)
        {
            type = "King";
            pieceChar = 'k';
        }

        //get list of possible moves
        public override List<Square> getPossibleMoves(Board board, Square s)
        {
            var possibleMoves = new List<Square>();

            int[,] offsets = { { -1, 1 }, { 0, 1 }, { 1, 1 }, 
                             { -1, 0 }, { 1, 0 }, { -1, -1 }, 
                             { 0, -1 }, { 1, -1 } };

            int tempFile, tempRank;
            if (moved == 0)
            {
                //queenside castle
                if (!board.Square[0, s.rank].isEmpty())
                {
                    if (board.Square[0, s.rank].Piece.moved == 0)
                    {
                        for (int i = 4; i > 0; --i)
                        {
                            if (!board.Square[i, s.rank].isEmpty())
                                break;
                            if (i == 1)
                                possibleMoves.Add(new Square(2, s.rank));
                        }
                    }
                }
                //kingside castle
                if (!board.Square[7, s.rank].isEmpty())
                {
                    if (board.Square[7, s.rank].Piece.moved == 0)
                    {
                        for (int i = 4; i < 7; ++i)
                        {
                            if (!board.Square[i, s.rank].isEmpty())
                                break;
                            if (i == 6)
                                possibleMoves.Add(new Square(6, s.rank));
                        }
                    }
                }
            }
            //normal moves
            for (int i = 0; i < offsets.GetLength(0); i++)
            {

                tempFile = s.file;
                tempRank = s.rank;

                tempFile += offsets[i, 0];
                tempRank += offsets[i, 1];

                if (!inBounds(tempFile, tempRank))
                    continue;

                if (!board.Square[tempFile, tempRank].isEmpty())
                {
                    if (board.Square[tempFile, tempRank].Piece.getPlayer() != player)
                        possibleMoves.Add(new Square(tempFile, tempRank));
                    else continue;
                }
                else
                    possibleMoves.Add(new Square(tempFile, tempRank));
            }
            return possibleMoves;
        }

        public static void Draw(Vector2 position, Vector2 scale)
        {
            GL.Begin(BeginMode.Lines);
            GL.Vertex2(position + new Vector2(scale.X * 0.5f, scale.Y * 0.1f));
            GL.Vertex2(position + new Vector2(scale.X * 0.5f, scale.Y * 0.9f));
            GL.Vertex2(position + new Vector2(scale.X * 0.1f, scale.Y * 0.5f));
            GL.Vertex2(position + new Vector2(scale.X * 0.9f, scale.Y * 0.5f));
            GL.Vertex2(position + new Vector2(scale.X * 0.1f, scale.Y * 0.9f));
            GL.Vertex2(position + new Vector2(scale.X * 0.9f, scale.Y * 0.1f));
            GL.Vertex2(position + new Vector2(scale.X * 0.1f, scale.Y * 0.1f));
            GL.Vertex2(position + new Vector2(scale.X * 0.9f, scale.Y * 0.9f));

            GL.Vertex2(position + new Vector2(scale.X * 0.35f, scale.Y * 0.75f));
            GL.Vertex2(position + new Vector2(scale.X * 0.65f, scale.Y * 0.75f));

            GL.End();
        }

    }
}
