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
        public Knight(int player, int file, int rank)
            : base(player, file, rank)
        {
            type = "Knight";
        }

        //check if the move is legal
        public override bool isLegal(Board board, int newFile, int newRank)
        {
            if (newFile == file & newRank == rank)
                return false;

            int tempFile;
            int tempRank;

            int[,] offsets = {{-2, 1},{-1, 2},{1, 2},{2, 1},
                         {2, -1},{1, -2},{-1, -2},{-2, -1}};

            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                tempFile = file;
                tempRank = rank;
                tempFile += offsets[i, 0];
                tempRank += offsets[i, 1];
                if(tempFile == newFile && tempRank == newRank)
                {
                    if(board.Pieces[tempFile,tempRank] == null)
                            return true;
                    else if(board.Pieces[tempFile,tempRank] != null)
                    {
                        if (board.Pieces[tempFile, tempRank].getPlayer() == player)
                            return false;
                        else
                            return true;
                    }
                }
            }
            return false;
        }

        public static void Draw(Vector2 position, float scale)
        {
            GL.Begin(BeginMode.LineStrip);
            GL.Vertex2(position + new Vector2(0.5f, 0.9f) * scale);
            GL.Vertex2(position + new Vector2(0.5f, 0.1f) * scale);
            GL.Vertex2(position + new Vector2(0.9f, 0.1f) * scale);
            GL.End();
        }

    }
}
