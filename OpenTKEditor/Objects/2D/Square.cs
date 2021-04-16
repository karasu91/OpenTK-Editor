using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
namespace OpenTKEditor
{
    class Square : Object2D
    {
        public Square(float size) 
        {
            vertexBuffer = new Vector2[]
            {
                new Vector2(size * Globals.windowWidth, size * Globals.windowHeight),
                new Vector2(-size * Globals.windowWidth, size * Globals.windowHeight),
                new Vector2(-size * Globals.windowWidth, -size * Globals.windowHeight),
                new Vector2(size * Globals.windowWidth, -size * Globals.windowHeight)
            };
            _id = GL.GenBuffer(); // Generate buffer identifier
            RefreshVertices();
        }
        public override void Draw()
        {
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.Disable(EnableCap.DepthTest);
            GL.LoadIdentity();
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _id);            
            GL.VertexPointer(2, VertexPointerType.Float, Vector2.SizeInBytes, 0);
            GL.Color3(Color.Indigo);
            GL.DrawArrays(PrimitiveType.Quads, 0, vertexBuffer.Length);
            GL.Enable(EnableCap.DepthTest);
            GL.PopMatrix();
        }
    }
}
