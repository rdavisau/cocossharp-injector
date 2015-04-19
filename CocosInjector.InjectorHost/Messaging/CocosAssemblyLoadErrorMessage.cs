using System;
using Injector.DTO.Messaging;

namespace CocosInjector.InjectorHost.Messaging
{
    public class CocosAssemblyLoadErrorMessage : ErrorMessage
    {
        public string CocosSceneContext { get; set; }
        public string CocosNodeContext { get; set; }

        public CocosAssemblyLoadErrorMessage(Exception e, object sceneContext = null, object nodeContext = null)
            : base(e)
        {
            CocosSceneContext = sceneContext != null ? sceneContext.GetType().Name : "NULL";
            CocosNodeContext = nodeContext != null ? nodeContext.GetType().Name : "NULL";
        }
    }
}