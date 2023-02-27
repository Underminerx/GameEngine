using OpenTK.Windowing.Desktop;

namespace Underminer_Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Window window = new Window(800, 600, "Underminer"))
            {
                window.RenderFrequency = 60;
                window.Run();
            }
        }


    }
}