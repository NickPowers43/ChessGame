using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class Pawn : Piece
    {
        //3 arg Pawn constructor
        public Pawn(int player) : base(player)
        {
            type = "Pawn";
            pieceChar = 'p';
        }

        //get list of possible moves
        public override List<Square> getPossibleMoves(Board board, Square s){
            var possibleMoves = new List<Square>();
            int[,] offsets = { { -1, 1 }, { 1, 1 }, { 0, 1 }, { 0, 2 } };
            int tempFile, tempRank;
            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                if (offsets[i, 1] == 2 && moved > 0)
                    continue;

                tempFile = s.file;
                tempRank = s.rank;

                if (player == WHITE){
                    tempFile += offsets[i, 0];
                    tempRank += offsets[i, 1];
                }else{
                    tempFile -= offsets[i, 0];
                    tempRank -= offsets[i, 1];
                }

                if (!inBounds(tempFile, tempRank))
                    continue;

                if (tempFile == s.file)
                {
                    if (board.Square[tempFile, tempRank].isEmpty())
                        possibleMoves.Add(new Square(tempFile, tempRank));
                    else break;
                }
                else if(tempFile != s.file && !board.Square[tempFile,tempRank].isEmpty())
                {
                     if(board.Square[tempFile,tempRank].Piece.getPlayer() != player)
                        possibleMoves.Add(new Square(tempFile, tempRank));
                     else continue;
                }
            }
            return possibleMoves;
        }

        public static void Draw(Vector2 position, Vector2 scale)
        {
            GL.Begin(BeginMode.Lines);
            GL.Vertex2(position + new Vector2(scale.X * 0.5f, scale.Y * 0.1f));
            GL.Vertex2(position + new Vector2(scale.X * 0.5f, scale.Y * 0.9f));
            GL.End();
        }
    }
}
