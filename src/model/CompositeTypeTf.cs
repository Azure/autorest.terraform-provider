using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Terraform
{
    class CompositeTypeTf
        : CompositeType
    {
        public override string ToString()
        {
            return Name;
        }

        public void AppendToDisplayString(IndentedStringBuilder builder)
        {
            var propertiesSet = new HashSet<Property>();
            foreach (var property in Properties.Cast<PropertyTf>())
            {
                propertiesSet.Add(property);
                property.AppendToDisplayString(builder);
            }

            foreach (var property in ComposedProperties.Cast<PropertyTf>())
            {
                if (!propertiesSet.Contains(property))
                {
                    property.AppendToDisplayString(builder, true);
                }
            }
        }
    }
}
