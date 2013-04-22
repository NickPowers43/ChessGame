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
        public Pawn(int player, int file, int rank)
            : base(player, file, rank)
        {
            type = "Pawn";
        }

        //check if the move is legal
        public override bool isLegal(Board board, int newFile, int newRank)
        {
            if (newFile == file & newRank == rank)
                return false;

            int tempFile;
            int tempRank;

            int[,] offsets = { { 0, 1 }, { 0, 2 }, { -1, 1 }, { 1, 1 } };

            Console.WriteLine("Seeking match at " + newFile + "," + newRank);
            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                Console.WriteLine("Testing offsets: " + offsets[i, 0] + "," + offsets[i, 1]);
                tempFile = file;
                tempRank = rank;

                if (player == WHITE)
                {
                    tempFile += offsets[i, 0];
                    tempRank += offsets[i, 1];
                }
                else if (player == BLACK)
                {
                    tempFile -= offsets[i, 0];
                    tempRank -= offsets[i, 1];
                }

                if (tempFile == newFile & tempRank == newRank)
                {
                    Console.WriteLine("Match found");
                    //capture enemy piece
                    if (newFile != file && newRank != rank && board.Pieces[newFile, newRank] != null)
                    {
                        Console.WriteLine("Testing capture:");
                        if (board.Pieces[newFile, newRank].getPlayer() != player)
                            return true;
                        else return false;
                    }

                    else if (newRank != rank & newFile == file)
                    {
                        Console.WriteLine("Moving forwards");
                        //forward 2 on first move only
                        if (newRank == rank + 2)
                            if (moved == 0 && board.Pieces[newFile, newRank] == null)
                                return true;
                            else return false;
                        if (board.Pieces[newFile, newRank] == null)
                            return true;
                        else return false;
                    }
                }
            }
            return false;
        }

        public static void Draw(Vector2 position, float scale)
        {
            GL.Begin(BeginMode.Lines);
            GL.Vertex2(position + new Vector2(0.5f, 0.1f) * scale);
            GL.Vertex2(position + new Vector2(0.5f, 0.9f) * scale);
            GL.End();
        }
    }
}
