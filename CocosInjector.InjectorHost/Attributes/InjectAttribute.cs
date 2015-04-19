using System;
using Injector.DTO.Attributes;

namespace CocosInjector.InjectorHost.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectAttribute : InjectorAttribute
    {
        public string Guid { get; set; }
        public bool ShouldReplaceExisting { get; set; }

        public InjectAttribute()
        {
            ShouldReplaceExisting = true;
        }
    }
}