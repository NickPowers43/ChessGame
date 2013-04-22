using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class GraphicsManager
    {
        public static GraphicsManager instance;// = new GraphicsManager();
        public static GraphicsManager Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        const float 
            PIECE_TEX_SIZE = 1.0f / 3.0f,
            PIECE_SCALE = 2.0f / 8.0f;

        Color DARK_COLOR = Color.Brown;
        Color LIGHT_COLOR = Color.SandyBrown;

        static Vector2 bottomLeft = new Vector2(-1, -1);
        //static int[][] indices = GenerateIndiceGroups();

        //Vector2[] TexCoords = GenerateTexCoords();
        int
            texture,
            textureAlpha;

        public GraphicsManager()
        {
            //LoadTexture();
        }

        public void renderBoard(Board board)
        {
            DrawCheckerBoard();

            GL.ClearDepth(0);

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    object temp = board.Pieces[file, rank];

                    if (temp != null)
                    {
                        GL.Color4(Color.White);
                        if (temp is Pawn)
                            Pawn.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
                        //else if (temp is Bishop)
                        //    Bishop.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
                        //else if (temp is King)
                        //    King.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
                        //else if (temp is Rook)
                        //    Rook.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
                        //else if (temp is Knight)
                        //    Knight.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);
                        //else if (temp is Queen)
                        //    Queen.Draw(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), PIECE_SCALE);


                        //DrawTexturedQuad(bottomLeft + new Vector2(file * PIECE_SCALE, rank * PIECE_SCALE), 3.0f, indicesElement);
                    }
                }
            }
        }
        private void DrawCheckerBoard()
        {
            GL.Begin(BeginMode.Quads);
            bool swap0 = false;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    swap0 = !swap0;
                    if (swap0)
                        GL.Color4(DARK_COLOR);
                    else
                        GL.Color4(LIGHT_COLOR);

                    DrawQuad(bottomLeft + new Vector2(PIECE_SCALE * j, PIECE_SCALE * i), PIECE_SCALE);
                }
                swap0 = !swap0;
            }
            GL.End();
        }
        private void DrawQuad(Vector2 position, float scale)
        {
            GL.Vertex2(position);
            GL.Vertex2(position + new Vector2(0, 1) * PIECE_SCALE);
            GL.Vertex2(position + new Vector2(1, 1) * PIECE_SCALE);
            GL.Vertex2(position + new Vector2(1, 0) * PIECE_SCALE);
        }
        //private void DrawTexturedQuad(Vector2 position, float scale, int indicesGroup)
        //{
        //    //GL.Enable(EnableCap.Blend);
        //    GL.Enable(EnableCap.Texture2D);

        //    GL.BindTexture(TextureTarget.Texture2D, textureAlpha);
        //    //GL.BlendFunc(BlendingFactorSrc.Zero, BlendingFactorDest.SrcColor);
        //    //GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

        //    GL.Begin(BeginMode.Quads);
        //    GL.Vertex2(position);
        //    GL.TexCoord2(TexCoords[indices[indicesGroup][3]]);
        //    GL.Vertex2(position + new Vector2(0, 1) * PIECE_SCALE);
        //    GL.TexCoord2(TexCoords[indices[indicesGroup][0]]);
        //    GL.Vertex2(position + new Vector2(1, 1) * PIECE_SCALE);
        //    GL.TexCoord2(TexCoords[indices[indicesGroup][1]]);
        //    GL.Vertex2(position + new Vector2(1, 0) * PIECE_SCALE);
        //    GL.TexCoord2(TexCoords[indices[indicesGroup][2]]);
        //    GL.End();

        //    //GL.BindTexture(TextureTarget.Texture2D, texture);
        //    //GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

        //    //GL.Begin(BeginMode.Quads);
        //    //GL.Vertex2(position);
        //    //GL.TexCoord2(TexCoords[indices[indicesGroup][0]]);
        //    //GL.Vertex2(position + new Vector2(0, 1) * PIECE_SCALE);
        //    //GL.TexCoord2(TexCoords[indices[indicesGroup][1]]);
        //    //GL.Vertex2(position + new Vector2(1, 1) * PIECE_SCALE);
        //    //GL.TexCoord2(TexCoords[indices[indicesGroup][2]]);
        //    //GL.Vertex2(position + new Vector2(1, 0) * PIECE_SCALE);
        //    //GL.TexCoord2(TexCoords[indices[indicesGroup][3]]);
        //    //GL.End();

        //    //GL.Disable(EnableCap.Texture2D);
        //}
        //private void LoadTexture()
        //{
        //    Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();

        //    Bitmap textureBitmap = new Bitmap(thisExe.GetManifestResourceStream("ChessGame.Textures.ChessPiecesAlphaless" + ".png"));
        //    textureAlpha = TexUtil.CreateTextureFromBitmap(textureBitmap);
        //    //GL.GenTextures(1, out texture);

        //    //System.Drawing.Imaging.BitmapData textureData = textureBitmap.LockBits(
        //    //    new System.Drawing.Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height),
        //    //    System.Drawing.Imaging.ImageLockMode.ReadOnly,
        //    //    System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            

        //    //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        //    //textureBitmap.UnlockBits(textureData);

        //    //GL.BindTexture(TextureTarget.Texture2D, texture);
        //    //GL.TexEnv(TextureEnvTarget.TextureEnv,
        //    //TextureEnvParameter.TextureEnvMode,
        //    //(float)TextureEnvMode.Modulate);
        //    //GL.TexParameter(TextureTarget.Texture2D,
        //    //TextureParameterName.TextureMinFilter,
        //    //(float)TextureMinFilter.LinearMipmapLinear);
        //    //GL.TexParameter(TextureTarget.Texture2D,
        //    //TextureParameterName.TextureMagFilter,
        //    //(float)TextureMagFilter.Linear);

        //    //Bitmap alphaTextureBitmap = new Bitmap(thisExe.GetManifestResourceStream("ChessGame.Textures.ChessPiecesAlpha" + ".png"));
        //    //GL.GenTextures(1, out textureAlpha);

        //    //System.Drawing.Imaging.BitmapData alphaTextureData = alphaTextureBitmap.LockBits(
        //    //    new System.Drawing.Rectangle(0, 0, alphaTextureBitmap.Width, alphaTextureBitmap.Height),
        //    //    System.Drawing.Imaging.ImageLockMode.ReadOnly,
        //    //    System.Drawing.Imaging.PixelFormat.Format32bppRgb);

        //    //GL.BindTexture(TextureTarget.Texture2D, textureAlpha);
        //    //GL.TexEnv(TextureEnvTarget.TextureEnv,
        //    //TextureEnvParameter.TextureEnvMode,
        //    //(float)TextureEnvMode.Modulate);
        //    //GL.TexParameter(TextureTarget.Texture2D,
        //    //TextureParameterName.TextureMinFilter,
        //    //(float)TextureMinFilter.LinearMipmapLinear);
        //    //GL.TexParameter(TextureTarget.Texture2D,
        //    //TextureParameterName.TextureMagFilter,
        //    //(float)TextureMagFilter.Linear);

        //    //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        //    //alphaTextureBitmap.UnlockBits(alphaTextureData);
        //}
        //private static int[][] GenerateIndiceGroups()
        //{
        //    int[][] output = new int[6][];

        //    for (int i = 0; i < 6; i++)
        //    {
        //        int[] indices = new int[4];

        //        switch (i)
        //        {
        //            case 0:
        //                indices[0] = 0;
        //                indices[1] = 4;
        //                indices[2] = 5;
        //                indices[3] = 1;
        //                break;
        //            case 1:
        //                indices[0] = 1;
        //                indices[1] = 5;
        //                indices[2] = 6;
        //                indices[3] = 2;
        //                break;
        //            case 2:
        //                indices[0] = 2;
        //                indices[1] = 6;
        //                indices[2] = 7;
        //                indices[3] = 3;
        //                break;
        //            case 3:
        //                indices[0] = 4;
        //                indices[1] = 8;
        //                indices[2] = 9;
        //                indices[3] = 5;
        //                break;
        //            case 4:
        //                indices[0] = 5;
        //                indices[1] = 9;
        //                indices[2] = 10;
        //                indices[3] = 6;
        //                break;
        //            case 5:
        //                indices[0] = 6;
        //                indices[1] = 10;
        //                indices[2] = 11;
        //                indices[3] = 7;
        //                break;
        //            default:
        //                break;
        //        }

        //        output[i] = indices;
        //    }

        //    return output;
        //}
        //private static Vector2[] GenerateTexCoords()
        //{
        //    Vector2 offset = new Vector2(0.0f, PIECE_TEX_SIZE);

        //    Vector2[] output = new Vector2[12];

        //    for (int i = 0; i < 3; i++)
        //    {
        //        for (int j = 0; j < 4; j++)
        //        {
        //            output[(i * 4) + j] = new Vector2((float)j * PIECE_TEX_SIZE, (float)i * PIECE_TEX_SIZE) + offset;
        //        }
        //    }

        //    return output;
        //}
    }
}
