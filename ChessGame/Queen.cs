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
        public Queen(int player, int file, int rank)
            : base(player, file, rank) 
        {
            type = "Queen";
        }

        //check if the move is legal
        override public bool isLegal(Board board, int newRank, int newFile)
        {
            int tempFile = file;
            int tempRank = rank;

            while (true)
            {
                //northwest move
                if (newRank > rank && newFile < file)
                {
                    tempRank++;
                    tempFile--;
                }
                //southwest move
                else if (newRank < rank && newFile < file)
                {
                    tempRank--;
                    tempFile--;
                }
                //northeast move
                else if (newRank > rank && newFile > file)
                {
                    tempFile++;
                    tempRank++;
                }
                //southwest move
                else if (newRank < rank && newFile < file)
                {
                    tempFile--;
                    tempRank--;
                }
                //north move
                if (newRank > rank && newFile == file)
                {
                    tempRank++;
                }
                //south move
                else if (newRank < rank && newFile == file)
                {
                    tempRank--;
                }
                //east move
                else if (newRank == rank && newFile > file)
                {
                    tempFile++;
                }
                //west move
                else if (newRank == rank && newFile < file)
                {
                    tempFile--;
                }

                //square occupied
                if (!board.Pieces[tempFile, tempRank].isEmpty())
                {
                    if (board.Pieces[tempFile, tempRank].getPlayer() == player)
                        return false;
                    else if (board.Pieces[tempFile, tempRank].getPlayer() != player)
                    {
                        if (tempFile == newFile && tempRank == newRank)
                            return true;
                        else return false;
                    }
                }
            }
        }

        public static void Draw(Vector2 position, float scale)
        {
            GL.Begin(BeginMode.Lines);
            GL.Vertex2(position + new Vector2(0.5f, 0.1f) * scale);
            GL.Vertex2(position + new Vector2(0.5f, 0.9f) * scale);
            GL.Vertex2(position + new Vector2(0.1f, 0.5f) * scale);
            GL.Vertex2(position + new Vector2(0.9f, 0.5f) * scale);
            GL.Vertex2(position + new Vector2(0.1f, 0.9f) * scale);
            GL.Vertex2(position + new Vector2(0.9f, 0.1f) * scale);
            GL.Vertex2(position + new Vector2(0.1f, 0.1f) * scale);
            GL.Vertex2(position + new Vector2(0.9f, 0.9f) * scale);
            GL.End();
        }
    }
}
