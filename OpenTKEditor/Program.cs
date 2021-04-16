using System;
using OpenTK;
using static System.Console;


namespace OpenTKEditor
{
    
    static class Program
    {
        private static readonly int _width = 500;
        private static readonly int _height = 500;
        private static GameWindow window;

        [STAThread]
        static void Main(string[] args)
        {
            window = new GameWindow(_width, _height);
            Window gm = new Window(window);      
        }
    }
}
