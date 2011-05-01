using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=false, Inherited=false)]
    public class FiniteAttribute : Attribute
    {
        public FiniteAttribute(string constructorName, string destructorName, string foldName, Type destructorClass)
        {
            ConstructorName = constructorName;
            DestructorName = destructorName;
            FoldName = foldName;
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

        public string FoldName
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
