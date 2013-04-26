using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class Rook : Piece
    {
        //3-arg Rook constructor
        public Rook(int player, int file, int rank) 
            : base(player,file,rank)
        {
            type = "Rook";
        }

        //get list of possible moves
        public override List<Square> getPossibleMoves(Board board)
        {
            var possibleMoves = new List<Square>();
            int[,] offsets = {{-1, 0},{0, 1},{1, 0},{0, -1}};

            int tempFile, tempRank;
            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                tempFile = file;
                tempRank = rank;
                for (int j = 0; j < 8; j++)
                {
                    tempFile += offsets[i, 0];
                    tempRank += offsets[i, 1];

                    if (!inBounds(tempFile, tempRank))
                        break;

                    if (board.Pieces[tempFile, tempRank] != null)
                    {
                        if (board.Pieces[tempFile, tempRank].getPlayer() != player)
                            possibleMoves.Add(new Square(tempFile, tempRank));
                        break;
                    }
                    else
                        possibleMoves.Add(new Square(tempFile, tempRank));
                }
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
            GL.End();
        }

    }
}
