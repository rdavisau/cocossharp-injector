using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CocosInjector.InjectorHost.Messaging;
using CocosSharp;

namespace CocosInjector.InjectorHost.Extensions
{
    internal static class CocosInjectorExtensions
    {
        public static List<Tuple<TypeInfo, TAttribute>> GetTypesWithAttribute<TAttribute>(this Assembly assembly)
            where TAttribute : Attribute
        {
            return assembly
                .DefinedTypes
                .Select(
                    t =>
                        new Tuple<TypeInfo, TAttribute>(t,
                            CustomAttributeExtensions.GetCustomAttribute<TAttribute>((MemberInfo) t)))
                .Where(t => t.Item2 != null)
                .ToList();
        }

        public static CocosAssemblyLoadErrorMessage ToErrorMessage(this Exception e, object sceneContext = null,
            object nodeContext = null)
        {
            return new CocosAssemblyLoadErrorMessage(e, sceneContext, nodeContext);
        }

        public static void SetActiveInjectionLayer(this CCLayer layer)
        {
            CocosInjectorHost.Current.SetActiveLayer(layer);
        }
    }
}