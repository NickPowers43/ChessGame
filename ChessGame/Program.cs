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

            game = new Game();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            game.Update(Keyboard, Mouse, e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            game.Render();

            SwapBuffers();
        }

        static void Main(string[] args)
        {
            using (Program program = new Program())
            {
                Console.WriteLine("Chess Game by Michael Corallo and Nicholas Powers");
                Console.WriteLine("+ : Rook");
                Console.WriteLine("L : Knight");
                Console.WriteLine("X : Bishop");
                Console.WriteLine("* : Queen");
                Console.WriteLine("* with line through it : King");
                Console.WriteLine("| : Pawn");
                Console.WriteLine("White starts. Capture the enemy king.");

                program.Run(20.0f, 20.0f);
            }
        }
    }
}
