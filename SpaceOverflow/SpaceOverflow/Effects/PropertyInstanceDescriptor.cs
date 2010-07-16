using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SpaceOverflow.Effects
{
    /// <summary>
    /// Represents a property or field described by a property path and tied to an instance of its owner.
    /// </summary>
    public class PropertyOrFieldInstanceDescriptor
    {
        /// <param name="owner">The instance of the property/field owner.</param>
        /// <param name="path">The path to the property or field - e.g. MyProperty.MyField.MyValue</param>
        public PropertyOrFieldInstanceDescriptor(object owner, string path) {
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

        /// <summary>
        /// Gets or sets the value of the described property.
        /// </summary>
        public object Value {
            get {
                return this.Steps.Last().GetValue(this.Owners.Last());
            }
            set {
                foreach (var copyPoint in this.CopyPoints) {
                    var step = this.Steps[copyPoint - 1];
                    var owner = this.Owners[copyPoint - 1];
                    var val = step.GetValue(owner);

                    this.Owners[copyPoint] = val;
                }

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

        /// <summary>
        /// List of owner/value objects in the property/value chain.
        /// </summary>
        protected List<object> Owners { get; private set; }

        /// <summary>
        /// List of properties or field of the owners that can be found at the same index.
        /// </summary>
        protected List<PropertyOrFieldDescriptor> Steps { get; private set; }

        /// <summary>
        /// Indices of owners that are value types. Those have to be copied when the value is updated.
        /// </summary>
        protected List<int> CopyPoints { get; private set; }
    }
}
