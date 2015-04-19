using CocosInjector.InjectorHost.QuickStart;
using CocosSharp;
using Splat;

namespace CocosInjector.DebugApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Locator.CurrentMutable.RegisterConstant(new ConsoleLogger(), typeof (ILogger));

            var app = new CCApplication(false, new CCSize(1024f, 768f))
            {
                ApplicationDelegate = new CocosInjectionAppDelgate()
            };

            app.StartGame();
        }
    }
}