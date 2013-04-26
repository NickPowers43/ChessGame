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
        public King(int player, int file, int rank)
            : base(player, file, rank)
        {
            type = "King";
        }

        //get list of possible moves
        public override List<Square> getPossibleMoves(Board board)
        {
            var possibleMoves = new List<Square>();

            int[,] offsets = { { -1, 1 }, { 0, 1 }, { 1, 1 }, 
                             { -1, 0 }, { 1, 0 }, { -1, -1 }, 
                             { 0, -1 }, { 1, -1 } };

            int tempFile, tempRank;
            if (moved == 0)
            {
                //queenside castle
                if (board.Pieces[0, rank] != null)
                {
                    if (board.Pieces[0, rank].moved == 0)
                    {
                        for (int i = file; i > 0; --i)
                        {
                            if (board.Pieces[i, rank] != null)
                                break;
                            if (i == 1)
                                possibleMoves.Add(new Square(2, rank));
                        }
                    }
                }
                //kingside castle
                if (board.Pieces[7, rank] != null)
                {
                    if (board.Pieces[7, rank].moved == 0)
                    {
                        for (int i = file; i < 7; ++i)
                        {
                            if (board.Pieces[i, rank] != null)
                                break;
                            if (i == 6)
                                possibleMoves.Add(new Square(6, rank));
                        }
                    }
                }
            }
            //normal moves
            for (int i = 0; i < offsets.GetLength(0); i++)
            {

                tempFile = file;
                tempRank = rank;

                tempFile += offsets[i, 0];
                tempRank += offsets[i, 1];

                if (!inBounds(tempFile, tempRank))
                    continue;

                if (board.Pieces[tempFile, tempRank] != null)
                {
                    if (board.Pieces[tempFile, tempRank].getPlayer() != player)
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
