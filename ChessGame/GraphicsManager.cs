
#define UNIQUE_UNIFORM_METHOD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

//author: Nick Powers
//



namespace ChessGame
{
    sealed class GraphicsManager
    {
        public static GraphicsManager instance = new GraphicsManager();
        public GraphicsManager Instance
        {
            get
            {
                return instance;
            }
        }

        const float 
            //TEX_WIDTH = 512,
            //TEX_HEIGHT = 512,
            PIECE_TEX_SIZE = 1.0f / 3.0f;

        int vaohandle, verticeshandle;
        int pieceProgram;
        int pieceFShader, pieceVShader;
        int boardProgram;
        int boardFShader, boardVShader;

        Vector2 bottomLeft = new Vector2(-1, -1);
        float pieceScale = 2.0f / 8.0f;

        int pieceTextureUniform;

        #region Indices variables and methods

#if UNIQUE_UNIFORM_METHOD

        int uniformOffset, uniformScale;
        int[] indiceshandle = new int[6];

        private void bindIndices(int index)
        {
            GL.BindVertexArray(vaohandle);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indiceshandle[index]);

            GL.BindVertexArray(0);
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

        public GraphicsManager()
        {
            //Generates Objects on the graphics card and establishes a handle to work with them.

            pieceProgram = GL.CreateProgram();

            int result;

            pieceVShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(pieceVShader, pieceVSource);
            GL.CompileShader(pieceVShader);
            Console.WriteLine(GetShaderCompileResults(pieceVShader));

            pieceFShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pieceVShader, pieceFSource);
            GL.CompileShader(pieceFShader);
            Console.WriteLine(GetShaderCompileResults(pieceFShader));

            boardVShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(boardVShader, boardVSource);
            GL.CompileShader(boardVShader);
            Console.WriteLine(GetShaderCompileResults(boardVShader));

            boardFShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(boardFShader, boardFSource);
            GL.CompileShader(boardFShader);
            Console.WriteLine(GetShaderCompileResults(boardFShader));


#if UNIQUE_UNIFORM_METHOD
            GL.GenBuffers(6, indiceshandle);
#else
            GL.GenBuffers(1, out indiceshandle);
#endif
            GL.GenVertexArrays(1, out vaohandle);
            GL.GenBuffers(1, out verticeshandle);

            setVertices(GenVertices());

        }

        public void renderBoard(Board board)
        {
            //render checkerboard pattern

            //render pieces
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    object temp = board.Pieces[rank, file].GetType();

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

                        setUniformOffset(bottomLeft + new Vector2(file * pieceScale, rank * pieceScale));
                    }
                }
            }
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
            GL.GetShader(pieceVShader, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                Console.WriteLine("Failed to compile shader!");
                output = GL.GetShaderInfoLog(pieceVShader);
                
            }

            return output;
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
                    output[i + j] = new Vector2((float)j * PIECE_TEX_SIZE, (float)i * PIECE_TEX_SIZE) + offset;
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
                    StreamReader _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("ChessGame.PieceVShader.txt"));

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
                    StreamReader _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("ChessGame.PieceFShader.txt"));

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
                    StreamReader _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("ChessGame.BoardVShader.txt"));

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
                    StreamReader _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("ChessGame.BoardFShader.txt"));

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
