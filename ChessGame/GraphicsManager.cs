using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class GraphicsManager
    {
        int vaohandle;
        int verticeshandle, indiceshandle;

        public GraphicsManager()
        {
            //Generates Objects on the graphics card and establishes a handle to work with them.
            GL.GenVertexArrays(1, out vaohandle);
            GL.GenBuffers(1, out verticeshandle);
            GL.GenBuffers(1, out indiceshandle);

            GL.BindVertexArray(vaohandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, verticeshandle);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer, (IntPtr)(15552), GenVertices(), BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        //unfinished
        private ushort[] GenIndices()
        {
            ushort[] output = new ushort[0];


            return output;
        }

        //finished
        private void setIndices(ushort[] indices)
        {
            GL.BindVertexArray(vaohandle);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indiceshandle);
            GL.BufferData<ushort>(BufferTarget.ElementArrayBuffer, (IntPtr)(2 * indices.Length), indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        private static Vertex[] GenVertices()
        {
            Vector2[] positions = new Vector2[81];
            Vector2[] texCoords = new Vector2[12];

            Vertex[] output = new Vertex[972];

            for (int i = 0; i < 81; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    int index = (i * 12) + j;
                    output[index] = new Vertex(positions[i], texCoords[j]);
                }
            }

            return output;
        }
        private static Vector2[] GenTexCoords()
        {
            const float increment = (float)(512.0d / 3.0d);

            Vector2 offset = new Vector2(0.0f, increment);

            Vector2[] output = new Vector2[12];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    output[i + j] = new Vector2((float)j * increment, (float)i * increment) + offset;
                }
            }

            return output;
        }
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
