using System;

namespace Pocket.Common.ObjectTree
{
    public class PrimitiveNode : Node
    {
        public static Node Of(Type type, object value) =>
            type.IsPrimitive ||
            type.IsEnum ||
            type.Is<string>()
                ? new PrimitiveNode(type, value) : null;
        
        private PrimitiveNode(Type type, object value) : base(type, value) { }
    }
}