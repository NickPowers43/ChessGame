﻿using System;
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

        //check if the move is legal
        public override bool isLegal(Board board, int newFile, int newRank)
        {
            if (newFile == file & newRank == rank)
                return false;

            int tempFile;
            int tempRank;

            int[,] offsets = { { 0, 1 }, { 0, 2 }, { -1, 1 }, { 1, 1 } };

            //normal king move
            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                tempFile = file;
                tempRank = rank;
                tempFile += offsets[i, 0];
                tempRank += offsets[i, 1];
                if (tempFile == newFile && tempRank == newRank)
                {
                    if (board.Pieces[newFile, newRank] == null)
                        return true;
                    else
                        if (board.Pieces[newFile, newRank].getPlayer() != player)
                            return true;
                        else 
                            return false;
                }
            }

            //castling for white
            if (player == 1 && newRank == rank && (newFile == 2 || newFile == 6)) 
            {
                if (newFile == 2 && board.Pieces[1, 0] == null && board.Pieces[2, 0] == null &&
                    board.Pieces[3, 0] == null && moved == 0 && board.Pieces[0, 0].moved == 0)
                {
                    board.Pieces[0, 0].move(board,3,0);
                    return true;
                }
                if (newFile == 6 && board.Pieces[5, 0] == null && board.Pieces[6, 0] == null &&
                    moved == 0 && board.Pieces[7, 0].moved == 0)
                {
                    board.Pieces[0, 0].move(board, 6, 0);
                    return true;
                }
            }

            //castling for black
            if (player == 2 && newRank == rank && (newFile == 2 || newFile == 6))
            {
                if (newFile == 2 && board.Pieces[1, 7] == null && board.Pieces[2, 7] == null &&
                    board.Pieces[3, 7] == null && moved == 0 && board.Pieces[0, 7].moved == 0)
                {
                    board.Pieces[0, 7].move(board, 3, 7);
                    return true;
                }
                if (newFile == 6 && board.Pieces[5, 7] == null && board.Pieces[6, 7] == null &&
                    moved == 0 && board.Pieces[7, 7].moved == 0)
                {
                    board.Pieces[0, 7].move(board, 6, 7);
                    return true;
                }
            }

            return false;
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

            GL.Vertex2(position + new Vector2(0.35f, 0.75f) * scale);
            GL.Vertex2(position + new Vector2(0.65f, 0.75f) * scale);

            GL.End();
        }

    }
}
