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
    class Object2D
    {
        public Vector2[] vertexBuffer;
        protected int _id;

        /* Rotate object using a rotation matrix
         * https://en.wikipedia.org/wiki/Rotation_matrix
         * */
        public void Rotate2d(float theta)
        {
            /* Simple 2D rotation matrix [cos t, -sin t,
             *                            sin t, cos t]
             * */
        float[,] rot_arr = {
                { (float)Math.Cos(theta), (float)Math.Sin(theta) },
                { (float)-Math.Sin(theta), (float)Math.Cos(theta) }
            };

            Vector2[] vertexBuffer_t = new Vector2[this.vertexBuffer.Length];

            int i = 0;
            foreach (Vector2 vert in this.vertexBuffer)
            {
                vertexBuffer_t[i] = new Vector2(rot_arr[0, 0] * vertexBuffer[i].X + rot_arr[0, 1] * vertexBuffer[i].Y,
                    rot_arr[1, 0] * vertexBuffer[i].X + rot_arr[1, 1] * vertexBuffer[i].Y);
                ++i;
            }
            //vertexBuffer = null;
            this.vertexBuffer = vertexBuffer_t;
            RefreshVertices();
        }

        /*  Reloads the vertex buffer with new values
         * */
        public void RefreshVertices()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._id);
            // Place the vertexBuffer object into the graphics buffer
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vector2.SizeInBytes * this.vertexBuffer.Length), this.vertexBuffer,
                BufferUsageHint.DynamicDraw);
            // Do not do anything yet
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        public virtual void Draw()
        {
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.Disable(EnableCap.DepthTest);
            GL.LoadIdentity();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this._id);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(2, VertexPointerType.Float, Vector2.SizeInBytes, 0);
            GL.Color3(Color.Aqua);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexBuffer.Length);
            GL.Enable(EnableCap.DepthTest);
            GL.PopMatrix();
        }
    }
}
