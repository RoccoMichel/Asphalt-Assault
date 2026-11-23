using System;
using UnityEngine;

namespace BoltsTools
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BoltsStaticViewerAttribute : PropertyAttribute
    {
        public readonly string MemberName;
        public readonly Type DeclaringType;
        
        public bool Editable { get; set; }
        
        public BoltsStaticViewerAttribute() {}

        public BoltsStaticViewerAttribute(string memberName, Type declaringType)
        {
            MemberName = memberName;
            DeclaringType = declaringType;
        }
    }
}
