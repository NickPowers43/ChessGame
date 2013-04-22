
#define UNIQUE_UNIFORM_METHOD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Threading;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

//author: Nick Powers
//



namespace ChessGame
{
    sealed class GraphicsManager0
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
            //TEX_WIDTH = 512,
            //TEX_HEIGHT = 512,
            PIECE_TEX_SIZE = 1.0f / 3.0f;

        

        int vaohandle, 
            verticeshandle,
            currentIndiceshandle,
            pieceProgram,
            boardProgram,
            pieceFShader, pieceVShader,
            boardFShader, boardVShader,
            pieceInPositionLoc, pieceInTexCoordLoc,
            boardInPositionLoc;

        Vector2 bottomLeft = new Vector2(-1, -1);
        float pieceScale = 2.0f / 8.0f;

        int pieceTextureUniform, textureBuffer;

        #region Indices variables and methods

#if UNIQUE_UNIFORM_METHOD

        int uniformOffset, uniformScale;
        int[] indiceshandle = new int[6];

        private void bindIndices(int index)
        {
            GL.BindVertexArray(vaohandle);

            currentIndiceshandle = indiceshandle[index];

            GL.BindBuffer(BufferTarget.ArrayBuffer, currentIndiceshandle);
            //GenIndices(index);

            GL.BindVertexArray(0);
        }
        private void GenIndices(int index)
        {
            uint[] indices = new uint[4];

            switch (index)
            {
                case 0 :
                    indices[0] = 0;
                    indices[1] = 4;
                    indices[2] = 5;
                    indices[3] = 1;
                    break;
                case 1:
                    indices[0] = 1;
                    indices[1] = 5;
                    indices[2] = 6;
                    indices[3] = 2;
                    break;
                case 2:
                    indices[0] = 2;
                    indices[1] = 6;
                    indices[2] = 7;
                    indices[3] = 3;
                    break;
                case 3:
                    indices[0] = 4;
                    indices[1] = 8;
                    indices[2] = 9;
                    indices[3] = 5;
                    break;
                case 4:
                    indices[0] = 5;
                    indices[1] = 9;
                    indices[2] = 10;
                    indices[3] = 6;
                    break;
                case 5:
                    indices[0] = 6;
                    indices[1] = 10;
                    indices[2] = 11;
                    indices[3] = 7;
                    break;
                default:
                    break;
            }

            indices[1] += 12;
            indices[2] += 24;
            indices[3] += 36;
            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indiceshandle[index]);
            GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, (IntPtr)(4 * 4), indices, BufferUsageHint.StaticDraw);
        }
#else
        int indiceshandle;

        private static Vector2[] GenPositions()
        {
            const float increment = 0.25f;

            Vector2 offset = new Vector2(1.0f, 1.0f);

            Vector2[] output = new Vector2[81];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int index = (i * 9) + j;
                    output[index] = new Vector2((float)j * increment, (float)i * increment) - offset;
                }
            }

            return output;
        }
#endif

        #endregion

        public GraphicsManager0()
        {
            //Generates Objects on the graphics card and establishes a handle to work with them.

            #region Create and compile shaders

            pieceProgram = GL.CreateProgram();
            boardProgram = GL.CreateProgram();

            pieceVShader = GL.CreateShader(ShaderType.VertexShader);
            pieceFShader = GL.CreateShader(ShaderType.FragmentShader);
            boardVShader = GL.CreateShader(ShaderType.VertexShader);
            boardFShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(pieceVShader, PieceVSource);
            GL.ShaderSource(pieceFShader, PieceFSource);
            GL.ShaderSource(boardVShader, BoardVSource);
            GL.ShaderSource(boardFShader, BoardFSource);
            
            GL.CompileShader(pieceVShader);
            GL.CompileShader(pieceFShader);
            GL.CompileShader(boardVShader);
            GL.CompileShader(boardFShader);

            Console.WriteLine(GetShaderCompileResults(pieceVShader));
            Console.WriteLine(GetShaderCompileResults(pieceFShader));
            Console.WriteLine(GetShaderCompileResults(boardVShader));
            Console.WriteLine(GetShaderCompileResults(boardFShader));

            GL.AttachShader(pieceProgram, pieceVShader);
            GL.AttachShader(pieceProgram, pieceFShader);
            GL.AttachShader(boardProgram, boardVShader);
            GL.AttachShader(boardProgram, boardFShader);
            
            LinkPieceShaderProgram();
            LinkBoardShaderProgram();

            Console.WriteLine(GL.GetError());
            pieceTextureUniform = GL.GetUniformLocation(pieceProgram, "Texture");
            uniformOffset = GL.GetUniformLocation(pieceProgram, "Offset");
            uniformScale = GL.GetUniformLocation(pieceProgram, "Scale");
            pieceInPositionLoc = GL.GetAttribLocation(pieceProgram, "inPosition");
            pieceInTexCoordLoc = GL.GetAttribLocation(pieceProgram, "inTexCoord");

            boardInPositionLoc = GL.GetAttribLocation(boardProgram, "inPosition");

            PrintProgramInfoLog(pieceProgram);

            Console.WriteLine(GL.GetError());

            string output = GL.GetProgramInfoLog(pieceProgram);
            Console.WriteLine(output);

            UsePieceShaderProgram();

            string temp = GL.GetError().ToString();
            Console.WriteLine(temp);
            Console.WriteLine(temp);
            Console.WriteLine(temp);
            Console.WriteLine(temp);
            Console.WriteLine(temp);
            
            

            #endregion

            LoadTexture();

            GL.GenVertexArrays(1, out vaohandle);
            GL.GenBuffers(1, out verticeshandle);

            setVertices(GenVertices());

#if UNIQUE_UNIFORM_METHOD
            GL.GenBuffers(6, indiceshandle);

            GenIndices(0);
            GenIndices(1);
            GenIndices(2);
            GenIndices(3);
            GenIndices(4);
            GenIndices(5);

#else
            GL.GenBuffers(1, out indiceshandle);
#endif
            

            GL.BindVertexArray(vaohandle);
            GL.EnableVertexAttribArray(pieceInPositionLoc);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, true, 16, 0);
            GL.EnableVertexAttribArray(pieceInTexCoordLoc);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, true, 16, 0);
            GL.BindVertexArray(0);

            
        }

        public void renderBoard(Board board)
        {
            //render checkerboard pattern

            //render pieces
            UsePieceShaderProgram();
            LinkPieceShaderProgram();

            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    object temp = board.Pieces[rank, file];

                    if (temp != null)
                    {
                        int indicesElement = 0;

                        if (temp is Pawn)
                            indicesElement = 0;
                        else if (temp is Bishop)
                            indicesElement = 1;
                        else if (temp is King)
                            indicesElement = 2;
                        else if (temp is Rook)
                            indicesElement = 3;
                        else if (temp is Knight)
                            indicesElement = 4;
                        else if (temp is Queen)
                            indicesElement = 5;

                        bindIndices(indicesElement);

                        //setUniformOffset();
                        //setUniformOffset(bottomLeft + new Vector2(0, 0));
                        setUniformScale(pieceScale);

                        Drawelements(bottomLeft + new Vector2(file * pieceScale, rank * pieceScale), pieceScale);

                        Console.WriteLine(GL.GetError());
                    }
                }
            }
        }
        private void DrawQuad(Vector2 position, float scale)
        {
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(position);
            GL.Vertex2(Vector2.Zero);
            GL.Vertex2(position + new Vector2(0, 1));
            GL.Vertex2(new Vector2(0, 1));
            GL.Vertex2(position + new Vector2(1, 1));
            GL.Vertex2(new Vector2(1, 1));
            GL.Vertex2(position + new Vector2(1, 0));
            GL.Vertex2(new Vector2(1, 0));
            GL.End();
        }
        private void Drawelements(Vector2 position, float scale)
        {
            
            ////GL.Enable(EnableCap.VertexArray);
            //GL.BindVertexArray(vaohandle);
            //GL.EnableClientState(ArrayCap.VertexArray);
            //GL.EnableClientState(ArrayCap.IndexArray);
            ////GL.Enable(EnableCap.VertexArray);
            //GL.EnableVertexAttribArray(pieceInPositionLoc);
            //GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, true, 16, 0);
            //GL.EnableVertexAttribArray(pieceInTexCoordLoc);
            //GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, true, 16, 0);
            //GL.BindVertexArray(0);
            ////GL.VertexPointer(2, VertexPointerType.Float, 0, ref verticeshandle);
            ////GL.DrawElements(BeginMode.Quads, 4, DrawElementsType.UnsignedInt, IntPtr.Zero);
            ////GL.DrawRangeElements(BeginMode.Quads, 0, 4, 4, DrawElementsType.UnsignedInt, IntPtr.Zero);
            //GL.BindVertexArray(0);

            GL.Begin(BeginMode.Quads);
            GL.Vertex2(position);
            GL.Vertex2(Vector2.Zero);
            GL.Vertex2(position + new Vector2(0, 1));
            GL.Vertex2(new Vector2(0, 1));
            GL.Vertex2(position + new Vector2(1, 1));
            GL.Vertex2(new Vector2(1,1));
            GL.Vertex2(position + new Vector2(1, 0));
            GL.Vertex2(new Vector2(1,0));
            GL.End();
        }
        private ushort[] GenIndices(Board board)
        {
            ushort[] output = new ushort[0];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                }
            }

            return output;
        }
        private void setIndices(ushort[] indices)
        {
            //GL.BindVertexArray(vaohandle);

#if UNIQUE_UNIFORM_METHOD
            GL.GenBuffers(6, indiceshandle);
            for (int i = 0; i < 6; i++)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indiceshandle[i]);
                GL.BufferData<ushort>(BufferTarget.ElementArrayBuffer, (IntPtr)(2 * 4), indices, BufferUsageHint.StaticDraw);
            }
#else
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indiceshandle);
            GL.BufferData<ushort>(BufferTarget.ElementArrayBuffer, (IntPtr)(2 * indices.Length), indices, BufferUsageHint.StaticDraw);
#endif



            //GL.BindVertexArray(0);
        }
        private void setVertices(Vertex[] vertices)
        {
            GL.BindVertexArray(vaohandle);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, verticeshandle);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(16 * vertices.Length), vertices, BufferUsageHint.StaticDraw);

            //GenIndices(0);

            GL.BindVertexArray(0);
        }
        private void setUniformOffset(Vector2 offset)
        {
            GL.Uniform2(uniformOffset, offset);
        }
        private void setUniformScale(float scale)
        {
            GL.Uniform1(uniformScale, scale);
        }
        private string GetShaderCompileResults(int handle)
        {
            string output = "Compilation successful!";

            int result;
            GL.GetShader(handle, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                Console.WriteLine("Failed to compile shader!");
                output = GL.GetShaderInfoLog(handle);
                
            }

            return output;
        }
        private void PrintProgramInfoLog(int handle)
        {
            string output;
            GL.GetProgramInfoLog(handle, out output);
            Console.WriteLine(output);
        }
        private void LinkPieceShaderProgram()
        {
            GL.LinkProgram(pieceProgram);
        }
        private void LinkBoardShaderProgram()
        {
            GL.LinkProgram(boardProgram);
        }
        private void UsePieceShaderProgram()
        {
            GL.UseProgram(pieceProgram);
        }
        private void UseBoardShaderProgram()
        {
            GL.UseProgram(boardProgram);
        }
        private void LoadTexture()
        {
            Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            Bitmap TextureBitmap = new Bitmap(thisExe.GetManifestResourceStream("ChessGame.Textures.ChessPieces" + ".png"));

            System.Drawing.Imaging.BitmapData TextureData = TextureBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, TextureBitmap.Width, TextureBitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            GL.GenTextures(1, out textureBuffer);
            GL.BindTexture(TextureTarget.Texture2D, textureBuffer);

            GL.TexEnv(TextureEnvTarget.TextureEnv,
            TextureEnvParameter.TextureEnvMode,
            (float)TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter,
            (float)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter,
            (float)TextureMagFilter.Linear);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            TextureBitmap.UnlockBits(TextureData);
        }

        private static Vertex[] GenVertices()
        {
            Vertex[] output;

#if UNIQUE_UNIFORM_METHOD
            Vector2[] positions = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0)};
            Vector2[] texCoords = GenTexCoords();

            output = new Vertex[48];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    int index = (i * 12) + j;
                    output[index] = new Vertex(positions[i], texCoords[j]);
                }
            }
#else
            Vector2[] positions = GenPositions();
            Vector2[] texCoords = GenTexCoords();

            output = new Vertex[972];

            for (int i = 0; i < 81; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    int index = (i * 12) + j;
                    output[index] = new Vertex(positions[i], texCoords[j]);
                }
            }
#endif

            return output;
        }
        private static Vector2[] GenTexCoords()
        {
            //const float increment = (float)(512.0d / 3.0d);

            Vector2 offset = new Vector2(0.0f, PIECE_TEX_SIZE);

            Vector2[] output = new Vector2[12];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    output[(i * 4) + j] = new Vector2((float)j * PIECE_TEX_SIZE, (float)i * PIECE_TEX_SIZE) + offset;
                }
            }

            return output;
        }

        static string pieceVSource;
        static string PieceVSource
        {
            get
            {
                if (pieceVSource == null)
                {
                    Assembly _assembly = Assembly.GetExecutingAssembly();
                    StreamReader _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("ChessGame.Shaders.PieceVShader.txt"));

                    pieceVSource = _textStreamReader.ReadToEnd();

                    return pieceVSource;
                }
                else
                {
                    return pieceVSource;
                }
            }
        }
        static string pieceFSource;
        static string PieceFSource
        {
            get
            {
                if (pieceFSource == null)
                {
                    //string directory = Assembly.GetEntryAssembly().CodeBase;

                    //directory = directory.Remove(directory.Length - 13);

                    //directory += directory + "\\FragmentSource.txt";

                    //directory = new Uri(directory).LocalPath;

                    //fragmentSourceContainer = System.IO.File.ReadAllText(directory);

                    //fragmentSourceContainer = System.IO.File.ReadAllText(System.Windows.Forms.Application.StartupPath + @"\FShader.txt");

                    Assembly _assembly = Assembly.GetExecutingAssembly();
                    StreamReader _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("ChessGame.Shaders.PieceFShader.txt"));

                    pieceFSource = _textStreamReader.ReadToEnd();

                    return pieceFSource;
                }
                else
                {
                    return pieceFSource;
                }
            }
        }

        static string boardVSource;
        static string BoardVSource
        {
            get
            {
                if (boardVSource == null)
                {
                    Assembly _assembly = Assembly.GetExecutingAssembly();
                    StreamReader _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("ChessGame.Shaders.BoardVShader.txt"));

                    boardVSource = _textStreamReader.ReadToEnd();

                    return boardVSource;
                }
                else
                {
                    return boardVSource;
                }
            }
        }
        static string boardFSource;
        static string BoardFSource
        {
            get
            {
                if (boardFSource == null)
                {
                    Assembly _assembly = Assembly.GetExecutingAssembly();
                    StreamReader _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("ChessGame.Shaders.BoardFShader.txt"));

                    boardFSource = _textStreamReader.ReadToEnd();

                    return boardFSource;
                }
                else
                {
                    return boardFSource;
                }
            }
        }

        public struct Vertex
        {
            Vector2 position, texCoord;

            public Vertex(Vector2 position, Vector2 texCoord)
            {
                this.position = position;
                this.texCoord = texCoord;
            }
        }
    }
}
