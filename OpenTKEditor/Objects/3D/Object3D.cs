using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTKEditor
{
    class Object3D
    {
        protected Vector3[] _vertexBuffer;
        protected Vector3[] _colorBuffer;
        protected int[] _indexBuffer;
        protected int _vbo; // vertice buffer object
        protected int _ibo; // index buffer object
        protected int _cbo; // color buffer object

        /* Rotate object using a rotation matrix
         * https://en.wikipedia.org/wiki/Rotation_matrix
         * https://math.stackexchange.com/questions/1882276/combining-all-three-rotation-matrices
         * */

        // TODO: Add framerate compensation TODO: Add framerate compensation TODO: Add framerate compensation
        // TODO: Add framerate compensation TODO: Add framerate compensation TODO: Add framerate compensation
        // TODO: Add framerate compensation TODO: Add framerate compensation TODO: Add framerate compensation
        public void Rotate3D(float theta_x, float theta_y, float theta_z)
        { 
            Vector3[] vertexBuffer_t = new Vector3[this._vertexBuffer.Length];

            float _cosalph = (float)Math.Cos(theta_x);
            float _sinalph = (float)Math.Sin(theta_x);
            float _cosbeta = (float)Math.Cos(theta_y);
            float _sinbeta = (float)Math.Sin(theta_y);
            float _cosgam = (float)Math.Cos(theta_z);
            float _singam = (float)Math.Sin(theta_z);

            /* These are the multipliers acquired from combining all of the
             * X, Y & Z matrices together, written in a nicer, more efficient
             * format
             * */
            float _term1X = _cosbeta * _cosgam;
            float _term2X = _cosbeta * _singam;

            float _term1Y = (_sinalph * _sinbeta * _cosgam - _cosalph * _singam);
            float _term2Y = (_sinalph * _sinbeta * _singam + _cosalph * _cosgam);
            float _term3Y = _sinalph * _cosbeta;

            float _term1Z = (_cosalph * _sinbeta * _cosgam + _sinalph * _singam);
            float _term2Z = (_cosalph * _sinbeta * _singam - _sinalph * _cosgam);
            float _term3Z = _cosalph * _cosbeta;

            int i = 0;
            foreach (Vector3 vert in this._vertexBuffer)
            {
                vertexBuffer_t[i] = new Vector3(_term1X * vert.X +  _term2X * vert.Y - _sinbeta * vert.Z,
                     _term1Y * vert.X + _term2Y * vert.Y + _term3Y * vert.Z,
                     _term1Z * vert.X + _term2Z * vert.Y + _term3Z * vert.Z);
                i++;
            }
            _vertexBuffer = vertexBuffer_t;
            RefreshBuffers();
        }

        public void Echo()
        {
            foreach (var val in this._colorBuffer)
            {
                Debug.WriteLine(val.ToString());                
            }
            //Debug.WriteLine(_vertexBuffer.Length);
        }

        public void Resize(float mult)
        {
            Vector3[] vertexBuffer_t = new Vector3[this._vertexBuffer.Length];

            int i = 0;
            foreach (Vector3 vect in this._vertexBuffer)
            {   
                vertexBuffer_t[i] = new Vector3(vect.X * mult, vect.Y * mult, vect.Z * mult);
                ++i;
            }
            this._vertexBuffer = vertexBuffer_t;

            RefreshBuffers();
        }

        public void Recolor(float red, float green, float blue)
        {
            Vector3[] colorBuffer_t = new Vector3[this._colorBuffer.Length];

            int i = 0;
            foreach (Vector3 vect in this._colorBuffer)
            {
                colorBuffer_t[i] = new Vector3(red, green, blue);
                ++i;
            }
            this._colorBuffer = colorBuffer_t;

            RefreshBuffers();
        }
        protected void InitializeBuffers()
        {
            GL.GenBuffers(1, out _vbo); // Generate buffer identifier
            GL.GenBuffers(1, out _cbo); // Generate buffer identifier
            GL.GenBuffers(1, out _ibo); // Generate buffer identifier
            RefreshBuffers();
        }
        
        /*  Reloads the vertex buffer with new values
         * */
        protected void RefreshBuffers()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(_vertexBuffer.Length * Vector3.SizeInBytes), _vertexBuffer, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _cbo);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(_colorBuffer.Length * Vector3.SizeInBytes), _colorBuffer, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_indexBuffer.Length * sizeof(int)), _indexBuffer, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public virtual void Draw()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);            
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GL.VertexPointer(3, VertexPointerType.Float, BlittableValueType.StrideOf(_vertexBuffer), new IntPtr(0));
            GL.BindBuffer(BufferTarget.ArrayBuffer, _cbo);
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, BlittableValueType.StrideOf(_vertexBuffer), new IntPtr(12));
            GL.DrawElements(PrimitiveType.Triangles, _indexBuffer.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}
