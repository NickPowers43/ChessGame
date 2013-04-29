using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class Knight : Piece
    {
        //3-arg Knight constructor
        public Knight(int player) : base(player)
        {
            type = "Knight";
            pieceChar = 'n';
        }

        //get list of possible moves
        public override List<Square> getPossibleMoves(Board board, Square s)
        {
            var possibleMoves = new List<Square>();
            int[,] offsets = {{-2, 1},{-1, 2},{1, 2},{2, 1},
                         {2, -1},{1, -2},{-1, -2},{-2, -1}};

            int tempFile, tempRank;
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
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex2(position + new Vector2(scale.X * 0.5f, scale.Y * 0.9f));
            GL.Vertex2(position + new Vector2(scale.X * 0.5f, scale.Y * 0.1f));
            GL.Vertex2(position + new Vector2(scale.X * 0.9f, scale.Y * 0.1f));
            GL.End();
        }

    }
}
