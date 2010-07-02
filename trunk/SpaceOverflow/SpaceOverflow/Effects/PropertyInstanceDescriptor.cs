using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SpaceOverflow.Effects
{
    public class PropertyInstanceDescriptor
    {
        public PropertyInstanceDescriptor(object owner, string path) {
            this.Owner = owner;
            this.Path = path;

            var pathComponents = path.Split('.');
            this.Owners = new List<object>();
            this.Steps = new List<PropertyOrFieldDescriptor>();
            this.CopyPoints = new List<int>();

            PropertyOrFieldDescriptor step = null;

            for (var i = 0; i< pathComponents.Count(); ++i) {
                var component = pathComponents[i];
                
                this.Owners.Add(owner); //Add owner

                //Add copy point
                if (step != null && step.Type.IsValueType)
                    this.CopyPoints.Add(i);

                MemberInfo member;

                var fields = owner.GetType().GetFields();

                //Add property/value step
                if ((member = owner.GetType().GetProperty(component)) == null)
                    member = owner.GetType().GetField(component, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                
                step = new PropertyOrFieldDescriptor(member);

                //Next owner
                owner = step.GetValue(owner);

                this.Steps.Add(step);
            }
        }

        public object Value {
            get {
                return this.Steps.Last().GetValue(this.Owners.Last());
            }
            set {
                this.Steps.Last().SetValue(this.Owners.Last(), value);

                foreach (var copyPoint in this.CopyPoints.OrderByDescending(x => x)) {
                    var step = this.Steps[copyPoint - 1];
                    var owner = this.Owners[copyPoint - 1];
                    var val = this.Owners[copyPoint];

                    step.SetValue(owner, val);
                }
            }
        }

        public object Owner { get; protected set; }
        public string Path { get; protected set; }

        protected List<object> Owners { get; private set; }
        protected List<PropertyOrFieldDescriptor> Steps { get; private set; }
        protected List<int> CopyPoints { get; private set; }
    }
}
