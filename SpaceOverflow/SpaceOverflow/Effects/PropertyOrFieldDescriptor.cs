using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SpaceOverflow.Effects
{
    public class PropertyOrFieldDescriptor 
    {
        public PropertyOrFieldDescriptor(MemberInfo member) {
            this.Member = member;
        }

        public MemberInfo Member { get; private set; }

        public void SetValue(object owner, object value) {
            if (this.Member is PropertyInfo) ((PropertyInfo)this.Member).SetValue(owner, value, new object[0]);
            else ((FieldInfo)this.Member).SetValue(owner, value);

        }

        public object GetValue(object owner) {
            if (this.Member is PropertyInfo) return ((PropertyInfo)this.Member).GetValue(owner, new object[0]);
            else return ((FieldInfo)this.Member).GetValue(owner);
        }

        public Type Type {
            get {
                if (this.Member is PropertyInfo) return ((PropertyInfo)this.Member).PropertyType;
                else return ((FieldInfo)this.Member).FieldType;
            }
        }
    }
}
