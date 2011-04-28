using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Purity.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=false, Inherited=false)]
    public class ExportAttribute : Attribute
    {
    }
}
