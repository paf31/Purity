using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=false, Inherited=false)]
    public class InfiniteAttribute : Attribute
    {
        public InfiniteAttribute(string constructorName, string destructorName, string unfoldName, Type destructorClass)
        {
            ConstructorName = constructorName;
            DestructorName = destructorName;
            UnfoldName = unfoldName;
            DestructorClass = destructorClass;
        }

        public string ConstructorName
        {
            get;
            set;
        }

        public string DestructorName
        {
            get;
            set;
        }

        public string UnfoldName
        {
            get;
            set;
        }

        public Type DestructorClass
        {
            get;
            set;
        }
    }
}
