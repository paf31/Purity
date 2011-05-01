using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=false, Inherited=false)]
    public class SynonymAttribute : Attribute
    {
        public SynonymAttribute(string constructorName, string destructorName, Type destructorClass)
        {
            ConstructorName = constructorName;
            DestructorName = destructorName;
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

        public Type DestructorClass
        {
            get;
            set;
        }
    }
}
