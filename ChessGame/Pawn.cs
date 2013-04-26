﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;

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

        //this method is for eventually determining checks
        public override List<Square> getPossibleMoves(Board board){
            var possibleMoves = new List<Square>();
            int[,] offsets = { { 0, 1 }, { 0, 2 }, { -1, 1 }, { 1, 1 } };
            int tempFile, tempRank;
            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                if (offsets[i, 1] == 2 && moved > 0)
                    continue;

                tempFile = file;
                tempRank = rank;

                if (player == WHITE){
                    tempFile += offsets[i, 0];
                    tempRank += offsets[i, 1];
                }else{
                    tempFile -= offsets[i, 0];
                    tempRank -= offsets[i, 1];
                }

                Console.WriteLine("Seeking match at " + tempFile + "," + tempRank);
                if (tempFile != file)
                {
                    if (board.Pieces[tempFile, tempRank] != null)
                    {
                        if (board.Pieces[tempFile, tempRank].getPlayer() != player)
                        {
                            Console.WriteLine("adding " + tempFile + "," + tempRank);
                            possibleMoves.Add(new Square(tempFile, tempRank));
                        }
                    }
                }
                else
                    if (board.Pieces[tempFile, tempRank] == null)
                    {
                        Console.WriteLine("adding " + tempFile + "," + tempRank);
                        possibleMoves.Add(new Square(tempFile, tempRank));
                    }
            }
            return possibleMoves;
        }

        //check if the move is legal
        public override bool isLegal(Board board, int newFile, int newRank)
        {
            Console.WriteLine(player);
            if (newFile == file & newRank == rank)
                return false;

            int tempFile;
            int tempRank;

            int[,] offsets = { { 0, 1 }, { 0, 2 }, { -1, 1 }, { 1, 1 } };

            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                if (offsets[i, 1] == 2 && moved > 0)
                    continue;
                //Console.WriteLine("Testing offsets: " + offsets[i, 0] + "," + offsets[i, 1]);
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

                Console.WriteLine((char)(tempFile + 97) + "" + (tempRank+1));

                if (tempFile == newFile & tempRank == newRank)
                {
                    //capture enemy piece
                    if (newFile != file && newRank != rank && board.Pieces[newFile, newRank] != null)
                    {
                        if (board.Pieces[newFile, newRank].getPlayer() != player)
                            return true;
                    }

                    else if (newRank != rank & newFile == file)
                    {
                        //Console.WriteLine("Moving forwards");
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

        public static void Draw(Vector2 position, Vector2 scale)
        {
            GL.Begin(BeginMode.Lines);
            GL.Vertex2(position + new Vector2(scale.X * 0.5f, scale.Y * 0.1f));
            GL.Vertex2(position + new Vector2(scale.X * 0.5f, scale.Y * 0.9f));
            GL.End();
        }
    }
}
