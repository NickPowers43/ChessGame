using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ChessGame
{
    class Program : GameWindow
    {
        private Game game;

        public Program()
            : base(800, 600)
        {
            GL.ClearColor(Color4.Black);

            //game = new Game();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            //game.Update(keyboard, mouse);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Begin(BeginMode.Points);
            //GL.Vertex3(0, 0, 0);
            //GL.End();

            //game.Render();

            SwapBuffers();
        }

        static void Main(string[] args)
        {
            using (Program program = new Program())
            {
                program.Run(30.0f, 30.0f);
            }
        }
    }
}
