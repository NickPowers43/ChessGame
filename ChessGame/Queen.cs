using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class Queen : Piece
    {
        //3-arg Queen constructor
        public Queen(int player): base(player) 
        {
            type = "Queen";
            pieceChar = 'q';
        }

        //get list of possible moves
        public override List<Square> getPossibleMoves(Board board, Square s)
        {
            var possibleMoves = new List<Square>();
            int[,] offsets = { { -1, 1 }, { 0, 1 }, { 1, 1 }, 
                             { -1, 0 }, { 1, 0 }, { -1, -1 }, 
                             { 0, -1 }, { 1, -1 } };

            int tempFile, tempRank;
            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                tempFile = s.file;
                tempRank = s.rank;
                for (int j = 0; j < 8; j++)
                {
                    tempFile += offsets[i, 0];
                    tempRank += offsets[i, 1];

                    if (!inBounds(tempFile, tempRank))
                        break;

                    if (!board.Square[tempFile, tempRank].isEmpty())
                    {
                        if (board.Square[tempFile, tempRank].Piece.getPlayer() != player)
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
            GL.Vertex2(position + new Vector2(scale.X * 0.1f, scale.Y * 0.9f));
            GL.Vertex2(position + new Vector2(scale.X * 0.9f, scale.Y * 0.1f));
            GL.Vertex2(position + new Vector2(scale.X * 0.1f, scale.Y * 0.1f));
            GL.Vertex2(position + new Vector2(scale.X * 0.9f, scale.Y * 0.9f));
            GL.End();
        }
    }
}
