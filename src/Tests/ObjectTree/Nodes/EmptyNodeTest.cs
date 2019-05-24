using Pocket.Common.ObjectTree;
using Shouldly;
using Xunit;

namespace Pocket.Common.Tests.ObjectTree.Nodes
{
    public class EmptyNodeTest
    {
        [Fact] public void Of_ShouldReturnNull_IfObjectIsNotOfTypeObject() =>
            Node<int>(of: 1).ShouldBeNull();
        
        [Fact] public void Of_ShouldReturnEmptyNode_IfObjectIsDefaultObject() =>
            Node<object>(of: new object()).ShouldBeNull();
        
        private static Node Node<T>(object of) =>
            EmptyNode.Of(typeof(T), of);
    }
}