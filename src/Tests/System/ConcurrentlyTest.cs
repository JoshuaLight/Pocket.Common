using Shouldly;
using Xunit;

namespace Pocket.Common.Tests.System
{
  public class ConcurrentlyTest
  {
    [Theory]
    [InlineData(1, 1, 2, 2)]
    [InlineData(2, 2, 3, 3)]
    [InlineData(3, 3, 4, 4)]
    public void Change_ShouldReplaceValueWithTo_IfItIsEqualToFrom(int actual, int from, int to, int expected)
    {
      Concurrently.Change(ref actual, from, to);
      
      actual.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData(1, 2, 3, 1)]
    [InlineData(2, 3, 4, 2)]
    [InlineData(3, 4, 5, 3)]
    public void Change_ShouldNotReplaceValueWithTo_IfItIsNotEqualToFrom(int actual, int from, int to, int expected)
    {
      Concurrently.Change(ref actual, from, to);
      
      actual.ShouldBe(expected);
    }
    
    [Theory]
    [InlineData(1, 1, 2, 1)]
    [InlineData(2, 2, 3, 2)]
    [InlineData(3, 3, 4, 3)]
    [InlineData(1, 2, 3, 1)]
    [InlineData(2, 3, 4, 2)]
    [InlineData(3, 4, 5, 3)]
    public void Change_ShouldReturnOldValueOfSelf(int actual, int from, int to, int expected) =>
      Concurrently.Change(ref actual, from, to).ShouldBe(expected);
  }
}