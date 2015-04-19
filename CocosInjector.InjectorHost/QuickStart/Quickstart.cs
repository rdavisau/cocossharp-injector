using CocosInjector.InjectorHost.Extensions;
using CocosSharp;

namespace CocosInjector.InjectorHost.QuickStart
{
    // TODO: make it quicker than this
    public class CocosInjectionAppDelgate : CCApplicationDelegate
    {
        public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
        {
            base.ApplicationDidFinishLaunching(application, mainWindow);

            mainWindow.RunWithScene(CocosInjectionLayer.InScene(mainWindow));
        }
    }

    internal class CocosInjectionLayer : CCLayer
    {
        public static CCScene InScene(CCWindow window)
        {
            var scene = new CCScene(window);
            var layer = new CocosInjectionLayer();

            scene.AddChild(layer);

            return scene;
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            this.SetActiveInjectionLayer();
        }
    }
}