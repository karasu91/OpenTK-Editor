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
    class Triangle : Object2D
    {
        public Triangle(float size)
        {
            vertexBuffer = new Vector2[]
            {
                new Vector2(size * Globals.windowWidth, -size * Globals.windowHeight),
                new Vector2(-size * Globals.windowWidth, -size * Globals.windowHeight),
                new Vector2(0, Globals.windowHeight * size)
            };
            _id = GL.GenBuffer(); // Generate buffer identifier
            RefreshVertices();
        }
    }
}
