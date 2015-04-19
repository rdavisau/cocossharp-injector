using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CocosInjector.InjectorHost.Attributes;
using CocosInjector.InjectorHost.Extensions;
using CocosInjector.InjectorHost.Messaging;
using CocosSharp;
using Injector.DTO.Messaging;
using Injector.InjectorHost;
using Splat;

namespace CocosInjector.InjectorHost
{
    public class CocosInjectorHost : InjectorHostBase<CocosInjectorProxy, Message>, IEnableLogger
    {
        private static Lazy<CocosInjectorHost> _implementation =
            new Lazy<CocosInjectorHost>(() => new CocosInjectorHost(), LazyThreadSafetyMode.PublicationOnly);

        public static CocosInjectorHost Current
        {
            get { return _implementation.Value; }
        }

        private CCLayer _activeLayer;
        private bool _cleaningScene;
        private readonly Dictionary<string, CCNode> _sceneItems = new Dictionary<string, CCNode>();

        public void SetActiveLayer(CCLayer layer)
        {
            CleanupScene();
            _activeLayer = layer;

            if (!IsListening)
                Task.Run(() => StartHostingAsync()).Wait();
        }

        private void CleanupScene()
        {
            // ha ha mutable state
            _cleaningScene = true;

            if (_activeLayer != null && _sceneItems.Any())
                _sceneItems.ToList()
                    .ForEach((guid, node) =>
                    {
                        if (node.Parent != null)
                            node.RemoveFromParent();
                        else
                            node.Dispose();

                        _sceneItems.Remove(guid);
                    });

            _cleaningScene = false;
        }

        protected override bool ShouldLoadAssembly(IMessageWithAssemblyBytes msg)
        {
            return _activeLayer != null && !_cleaningScene;
        }

        protected override void ProcessNewAssembly(Assembly newAssembly, CocosInjectorProxy sender)
        {
            if (_activeLayer == null)
            {
                var err = "No active layer, can't do anything with new assembly";
                this.Log().Debug(err);

                var msg = new InvalidOperationException(err).ToErrorMessage();
                Task.Run(() => SendFeedback(msg, sender)).Wait();

                return;
            }

            var injectables = newAssembly.GetTypesWithAttribute<InjectAttribute>();

            foreach (var tuple in injectables)
            {
                var type = tuple.Item1;
                var injectAttribute = tuple.Item2;

                if (!type.IsSubclassOf(typeof (CCNode)))
                {
                    var err = "Injectables must be CCNode subclasses";
                    this.Log().Error(err);

                    var msg = new InvalidOperationException(err).ToErrorMessage(_activeLayer);
                    msg.CocosNodeContext = type.FullName;

                    Task.Run(() => SendFeedback(msg, sender)).Wait();

                    continue;
                }

                var node = Activator.CreateInstance(type.AsType()) as CCNode;
                var guid = injectAttribute.Guid ?? type.FullName;
                var shouldReplace = injectAttribute.ShouldReplaceExisting;

                CCNode existingNode;

                if (_sceneItems.TryGetValue(guid, out existingNode) && shouldReplace)
                {
                    this.Log()
                        .Debug("Node {0} [{1}] being replaced by new {2}", guid, existingNode.GetType().Name, type.Name);

                    if (existingNode.Parent != null)
                        existingNode.RemoveFromParent();

                    _sceneItems.Remove(guid);
                    _activeLayer.AddChild(node);
                    _sceneItems.Add(guid, node);
                }
                else
                {
                    try
                    {
                        _activeLayer.AddChild(node);
                        _sceneItems.Add(guid, node);
                    }
                    catch (Exception e)
                    {
                        this.Log().Error(e.Message);
                        var msg = e.ToErrorMessage(_activeLayer, node);
                        Task.Run(() => SendFeedback(msg, sender)).Wait();
                    }
                }
            }
        }
    }
}