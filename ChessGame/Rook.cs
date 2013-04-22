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

        //check if the move is legal
        public override bool isLegal(Board board, int newRank, int newFile)
        {
            int tempFile = file;
            int tempRank = rank;

            while (true)
            {
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
            GL.End();
        }

    }
}
