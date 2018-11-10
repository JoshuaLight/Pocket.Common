using System.Reflection;

namespace Pocket.Common
{
  /// <summary>
  ///     Represents extension-methods for <see cref="MethodInfo"/>.
  /// </summary>
  public static class MethodInfoExtensions
  {
    /// <summary>
    ///   Gets arguments of specified method.
    /// </summary>
    /// <param name="self"><code>this</code> object.</param>
    /// <returns>An array of <see cref="ParameterInfo"/> that represent arguments of method.</returns>
    public static ParameterInfo[] Arguments(this MethodInfo self) =>
      self.GetParameters();
  }
}