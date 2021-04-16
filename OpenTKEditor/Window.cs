using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Input;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace OpenTKEditor
{
    class Window
    {
        #region Variables
        private static readonly double fps = 144;
        private static readonly double ups = 144;
        private static float _timeElapsed = 0.0f;
        private static float _timeInterval = 0.1f * (1 / (float)fps);
        Camera _Camera;
        Mouse _Mouse;
        static System.Windows.Forms.Cursor _Cursor;

        private Matrix4 _projectionMatrix = Matrix4.Identity;
        private Matrix4 _viewMatrix = Matrix4.Identity;
        private Matrix4 _modelMatrix = Matrix4.Identity;
        private Matrix4 _rotationMatrix = Matrix4.Identity;
        private Matrix4 _transMatrix = Matrix4.Identity;

        private List<Object2D> _2dObjects = new List<Object2D>();
        private List<Object3D> _3DObjects = new List<Object3D>();

        float x_rot = 2;
        float y_rot = 2;
        float z_rot = 2;

        private static Triangle tri;
        private static Square sq;
        private static Cube cube;
        private int frames_tot = 0;
        private int timer = 0; // Based on frames.. -> compensate TODO

        private float currentTime = 0.0f; // For deltatime calculation
        private float prevTime = 0.0f;

        private struct PerspectiveVariables
        {
            public float zNear;
            public float zFar;
        }

        private struct Colors // Used for coloring objects
        {
            public float _red;
            public float _green;
            public float _blue;
        }

        Colors _Colors;
        PerspectiveVariables PerspVars; // zNear, zFar

        private bool _lockMouse = true;

        Process proc = Process.GetCurrentProcess();

        #endregion

        GameWindow window;

        public Window(GameWindow window)
        {
            this.window = window;
            Start();
        }

        private void Start()
        {
            #region GL Event initialisations
            window.Load += OnWindowStartup;
            window.Resize += OnResize;
            window.RenderFrame += OnRenderFrame;
            window.UpdateFrame += OnUpdateFrame;
            window.VSync = OpenTK.VSyncMode.Off;
            #endregion
            window.Run(ups, fps);
        }
        private void HandleMouse()
        {
            // Center mouse cursor every frame
            Point _gameWindowPoint = window.PointToScreen(new Point((int)Globals.windowWidth / 2,
                (int)Globals.windowHeight / 2));
            if (_lockMouse == true)
                OpenTK.Input.Mouse.SetPosition(_gameWindowPoint.X, _gameWindowPoint.Y);

            // Mouse
            float _mouseSensivityX = 0.03f;
            float _mouseSensivityY = 0.03f;
            float _mouseSensivityRoll = 0.001f;

            // Enable camera movement on left mouse hold
            if (_Mouse._leftButtonPressed)
                _Camera.SetYawPitchRoll(_Mouse.offsetX * _mouseSensivityX, _Mouse.offsetY * _mouseSensivityY, 0);
            // Move along Z-axis if both mouse buttons are pressed
            if (_Mouse._leftButtonPressed && _Mouse._rightButtonPressed)
                _Camera.MoveUpDown(_Mouse.offsetY * _mouseSensivityY);
            //_Camera.SetYawPitchRoll(0, 0, _Mouse.offsetX * _cameraMultiplier);
            // Move camera up/down when both buttons are held
            else if (_Mouse._rightButtonPressed && !_Mouse._leftButtonPressed)
                _Camera.SetYawPitchRoll(0, 0, _Mouse.offsetY * _mouseSensivityY);
            if (_Mouse._rightButtonPressed && !_Mouse._leftButtonPressed)
            {
                _Camera.Roll(_Mouse.offsetX * _mouseSensivityRoll, _Mouse.offsetY * _mouseSensivityRoll, 0);
            }
        }

        private void HandleKeyboard()
        {
            float resize_mult = 1.0f;
            float mult = 0.01f;

            float rot_mult_x = 0.005f;
            float rot_mult_y = 0.005f;
            float rot_mult_z = 0.01f;

            float _recolor_mult = 0.0001f;

            // Camera buttons
            bool Escape = ((Keyboard.GetKeyStates(Key.Escape) & KeyStates.Down) > 0);
            bool LeftKey = ((Keyboard.GetKeyStates(Key.Left) & KeyStates.Down) > 0);
            bool RightKey = ((Keyboard.GetKeyStates(Key.Right) & KeyStates.Down) > 0);
            bool UpKey = ((Keyboard.GetKeyStates(Key.Up) & KeyStates.Down) > 0);
            bool DownKey = ((Keyboard.GetKeyStates(Key.Down) & KeyStates.Down) > 0);
            bool HomeKey = ((Keyboard.GetKeyStates(Key.Home) & KeyStates.Down) > 0);
            bool EndKey = ((Keyboard.GetKeyStates(Key.End) & KeyStates.Down) > 0);
            // Object rescale key
            bool D9Key = ((Keyboard.GetKeyStates(Key.D9) & KeyStates.Down) > 0);
            bool D0Key = ((Keyboard.GetKeyStates(Key.D0) & KeyStates.Down) > 0);
            // Echo (debug) key
            bool F1Key = ((Keyboard.GetKeyStates(Key.F1) & KeyStates.Down) > 0);
            // Rotation keys
            bool AKey = ((Keyboard.GetKeyStates(Key.A) & KeyStates.Down) > 0);
            bool SKey = ((Keyboard.GetKeyStates(Key.S) & KeyStates.Down) > 0);
            bool DKey = ((Keyboard.GetKeyStates(Key.D) & KeyStates.Down) > 0);
            bool WKey = ((Keyboard.GetKeyStates(Key.W) & KeyStates.Down) > 0);
            bool QKey = ((Keyboard.GetKeyStates(Key.Q) & KeyStates.Down) > 0);
            bool EKey = ((Keyboard.GetKeyStates(Key.E) & KeyStates.Down) > 0);
            // Color keys
            bool D1Key = ((Keyboard.GetKeyStates(Key.D1) & KeyStates.Down) > 0);
            bool D2Key = ((Keyboard.GetKeyStates(Key.D2) & KeyStates.Down) > 0);
            bool D3Key = ((Keyboard.GetKeyStates(Key.D3) & KeyStates.Down) > 0);

            // Unassigned
            bool JKey = ((Keyboard.GetKeyStates(Key.J) & KeyStates.Down) > 0);
            bool KKey = ((Keyboard.GetKeyStates(Key.K) & KeyStates.Down) > 0);
            bool HKey = ((Keyboard.GetKeyStates(Key.H) & KeyStates.Down) > 0);
            bool UKey = ((Keyboard.GetKeyStates(Key.U) & KeyStates.Down) > 0);

            // Cursor lock/visible toggle
            bool Tab = ((Keyboard.GetKeyStates(Key.Tab) & KeyStates.Down) > 0);


            // Close graphics window if ESC is pressed

            if (Escape)
            {
                window.Exit();

            }
            if (Tab)
            {
                timer++;
                // Hold for 1 second -> lock / unlock mouse
                if (timer * 1 / fps > 1)
                {
                    window.CursorVisible = !window.CursorVisible;
                    _lockMouse = !_lockMouse;
                    timer = 0;
                }
            }
            if (AKey)
            {
                _Camera.MoveLeft();
            }
            if (DKey)
            {
                _Camera.MoveRight();
            }
            if (WKey)
            {
                _Camera.MoveUp(0.1f);
            }
            if (SKey)
            {
                _Camera.MoveDown(0.1f);
            }
            if (HomeKey)
            {
                PerspVars.zNear += 0.01f;
            }
            if (EndKey)
            {
                PerspVars.zNear -= 0.01f;
            }

            if (D9Key)
            {
                resize_mult -= 0.01f;
                cube.Resize(resize_mult);
            }

            if (D0Key)
            {
                resize_mult += 0.01f;

                cube.Resize(resize_mult);
            }
            if (F1Key)
            {
                cube.Echo();
            }

            if (LeftKey)
            {
                cube.Rotate3D(-rot_mult_x, 0, 0);
            }
            if (RightKey)
            {
                cube.Rotate3D(rot_mult_x, 0, 0);
            }
            if (UpKey)
            {
                cube.Rotate3D(0, -rot_mult_y, 0);
            }
            if (DownKey)
            {
                cube.Rotate3D(0, rot_mult_y, 0);
            }
            if (QKey)
            {
                cube.Rotate3D(0, 0, -rot_mult_z);
            }
            if (EKey)
            {
                cube.Rotate3D(0, 0, rot_mult_z);
            }
            if (D1Key) // Red
            {
                _Colors._red += _recolor_mult;
                cube.Recolor(_Colors._red, _Colors._green, _Colors._blue);
            }
            if (D2Key) // Green
            {
                _Colors._green += _recolor_mult;
                cube.Recolor(_Colors._red, _Colors._green, _Colors._blue);
            }
            if (D3Key) // Blue
            {
                _Colors._blue += _recolor_mult;
                cube.Recolor(_Colors._red, _Colors._green, _Colors._blue);
            }


            _rotationMatrix = Matrix4.CreateRotationX(x_rot) *
                Matrix4.CreateRotationY(y_rot) *
                Matrix4.CreateRotationZ(z_rot);
        }

        private void OnResize(object o, EventArgs e)
        {
            GL.Viewport(0, 0, window.Width, window.Height);
            GL.MatrixMode(MatrixMode.Projection);
            // Create perspective matrix
            _modelMatrix = Matrix4.CreatePerspectiveFieldOfView(
                1.5f, Globals.windowWidth / Globals.windowHeight,
                PerspVars.zNear, PerspVars.zFar);
            GL.LoadMatrix(ref _modelMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
            Globals.windowHeight = window.Height;
            Globals.windowWidth = window.Width;
        }


        // Called every frame update
        private void LoadPerspectiveMatrix(FrameEventArgs e)
        {
            prevTime = currentTime;
            currentTime = (float)e.Time;
            float deltaTime = 1 - (currentTime - prevTime);
            _Camera.SetCameraJitter(Math.Abs(deltaTime));

            _timeElapsed += _timeInterval;
            // Create perspective matrix
            _modelMatrix = Matrix4.CreatePerspectiveFieldOfView(
                (float)(Math.PI / 3), // 60 degrees
                Globals.windowWidth / Globals.windowHeight,
                PerspVars.zNear, PerspVars.zFar);

            // View matrix for controlling the camera

            Matrix3 mat3 = _Camera.GetViewMatrix();
            // mat3 = mat3 * Matrix3.CreateRotationZ(_Mouse.offsetX * 0.01f);

            _viewMatrix = Matrix4.LookAt(mat3.Row0,
                mat3.Row1,
                mat3.Row2);

            _viewMatrix = _viewMatrix;// * Matrix4.CreateRotationZ(_Mouse.offsetX*0.0000000000000001f);

            //_rotationMatrix * _transMatrix;

            // _transMatrix = _projectionMatrix * _viewMatrix * _modelMatrix;// * 

            //GL.LoadMatrix(ref _modelMatrix);
            GL.LoadMatrix(ref _viewMatrix);
        }

        // Called at every frame update
        private void OnUpdateFrame(object o, FrameEventArgs e)
        {
            LoadPerspectiveMatrix(e);
            _Mouse.Refresh();
            HandleMouse();
            HandleKeyboard();
        }

        private void OnRenderFrame(object o, FrameEventArgs e)
        {
            #region Console Output
            Console.SetCursorPosition(0,0);
            Console.WriteLine(_Mouse._leftButtonPressed.ToString(), "asd");
            Console.WriteLine($"Y: {Globals.windowHeight.ToString()} X: {Globals.windowWidth.ToString()}");
            Console.WriteLine($"OrthoX: {Globals.ortho_x}, OrthoY: {Globals.ortho_y}, OrthoZ: { Globals.ortho_z}");
            Console.WriteLine($"frames: {frames_tot}");
            Console.WriteLine($"memory : {proc.WorkingSet64 / 1000} kB");
            Console.WriteLine($"Viewmatrix: {_viewMatrix}");
            Console.WriteLine($"Leftbutton: { _Mouse._leftButtonPressed.ToString()}");
            Console.WriteLine($"Rightbutton: { _Mouse._rightButtonPressed.ToString()}");
            Console.WriteLine($"fps: {1 / e.Time}");
            Console.WriteLine($"Mouse lock: {_lockMouse}, Cursor visible: {window.CursorVisible}");
            Console.WriteLine($"trans: {_transMatrix}");
            #endregion

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //Draw2dObjects(); // Bugs the 3d camera
            Draw3dObjects();
            GL.Flush();
            window.SwapBuffers();
            frames_tot++;


        }

        private void Draw2dObjects()
        {
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.Disable(EnableCap.DepthTest);
            GL.LoadIdentity();
            foreach (Object2D obj in _2dObjects)
                obj.Draw();
            //GL.MatrixMode(MatrixMode.Modelview);
            GL.Enable(EnableCap.DepthTest);
            GL.LoadIdentity();
        }

        private void Draw3dObjects()
        {
            GL.PushMatrix();                        // Save current matrix state
            GL.CullFace(CullFaceMode.FrontAndBack);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.Enable(EnableCap.DepthTest);
            GL.Translate(Globals.CameraX, Globals.CameraY, Globals.CameraZ);
            for (int i = 0; i < _3DObjects.Count; i++)
                _3DObjects[i].Draw();
            GL.PopMatrix();
        }


        private void OnWindowStartup(object o, EventArgs e)
        {
            window.CursorVisible = false;
            GL.ClearColor(1.0f, 1.0f, 1.0f, 0.0f); // Color values for clearing the buffer 
            _Camera = new Camera();
            _Mouse = new Mouse(window);
            _Cursor = new System.Windows.Forms.Cursor(System.Windows.Forms.Cursor.Current.Handle);
            Globals.windowWidth = window.Width;
            Globals.windowHeight = window.Height;
            cube = new Cube(0.3f);
            PerspVars.zNear = 0.1f;
            PerspVars.zFar = 100000.0f;
            _Colors._red = 0.0f;
            _Colors._blue = 0.0f;
            _Colors._green = 0.0f;
            tri = new Triangle(0.001f);
            sq = new Square(0.0005f);
            //_2dObjects.Add(tri);
            //_2dObjects.Add(sq);
            _3DObjects.Add(cube);
        }
    }
}


