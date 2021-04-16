using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using System.Diagnostics;
using OpenTK;

namespace OpenTKEditor
{
    class Mouse
    {
        public delegate void _MouseEventHandler();
        public event EventHandler MouseMoved;
        public bool _leftButtonPressed = false;
        public bool _rightButtonPressed = false;
        private float lastX;
        private float lastY;
        public float offsetX;        
        public float offsetY;
        MouseState _mouseState;

        public Mouse(GameWindow window)
        {
            //window.MouseDown += CalculateMouseVariables;
            _mouseState = OpenTK.Input.Mouse.GetState();
            lastX = _mouseState.X;
            lastY = _mouseState.Y;          
        }

        //public void CalculateMouseVariables(object sender, MouseEventArgs e)
        //{            
        //    //Debug.WriteLine(e.Mouse.LeftButton);
        //   // e.Mouse.IsButtonDown(e.Mouse.LeftButton);
        //}

        public void Refresh()
        {
            _rightButtonPressed = _mouseState[MouseButton.Right];
            _leftButtonPressed = _mouseState[MouseButton.Left];

            lastX = _mouseState.X;
            lastY = _mouseState.Y;       
            
            _mouseState = OpenTK.Input.Mouse.GetState();
            offsetX = _mouseState.X - lastX;

            // Negative mark to cancel mouse inversion
            offsetY = -(_mouseState.Y - lastY);

            // Seems to cause big buffer -> causes camera move after stopping mouse
            //if (_mouseState.X != lastX || _mouseState.Y != lastY)
            //{
            //    MouseMoved(this, EventArgs.Empty);
            //}
        }
    }
}