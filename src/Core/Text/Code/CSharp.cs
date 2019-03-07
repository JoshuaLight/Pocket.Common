using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pocket.Common
{
  public struct CSharp
  {
    private readonly Code _code;
    private readonly int _indent;

    public CSharp(Code code, int indent)
    {
      _code = code;
      _indent = indent;
    }

    public override string ToString() => _code.ToString();

    public CSharp Text(string text) =>
      With(_code.Text(text));
    public CSharp Text(string text, bool when) =>
      With(_code.Text(text, when));
    
    public CSharp NewLine() =>
      With(_code.NewLine());
    public CSharp NewLine(bool when) =>
      With(_code.NewLine(when));

    private CSharp With(Code _) => this;

    public Code.Scope Scope(bool endsWithNewLine = true) => new Code.Scope(_code,
      x => x.Text("{").NewLine(),
      x => x.Text("}").NewLine(when: endsWithNewLine)).With(_code.Indent(_indent));

    public Code.Scope Scope(string header, bool endsWithNewLine = true) =>
      Text(header).NewLine().Scope(endsWithNewLine);
    
    public Code.Scope Region(string name) => new Code.Scope(_code,
      x => x.Text($"#region {name}").NewLine(),
      x => x.Text($"#endregion").NewLine());

    public Code.Scope Namespace(string name) =>
      Scope(header: $"namespace {name}", endsWithNewLine: false);

    public CSharp Using(Type namespaceOf) =>
      Using(namespaceOf.Namespace);
    public CSharp Using(string @namespace) =>
      Text($"using {@namespace};");

    public CSharp Field(FieldInfo field)
    {
      return Text($"{Attributes(field)}{Modifier()} {field.FieldType.PrettyName()} {field.Name};");
      
      string Modifier() =>
        field.IsPublic ? "public" : field.IsPrivate ? "private" : "protected";
    }

    public CSharp Property(PropertyInfo property)
    {
      return Text($"{Attributes(property)}{Modifier()} " +
                  $"{property.PropertyType.PrettyName()} {property.Name} " +
                  $"{{ {Get()}{Set()}}}");

      string Modifier()
      {        
        var getMethod = property.GetMethod;
        var setMethod = property.SetMethod;
        
        return
          (getMethod?.IsPublic).Or(false) || (setMethod?.IsPublic).Or(false) ? "public" :
          (getMethod?.IsPrivate).Or(true) && (setMethod?.IsPrivate).Or(true) ? "private" : "protected";
      }

      string Get()
      {
        if (property.GetMethod == null)
          return "";
        
        var modifier = property.GetMethod.IsPublic ? "public" : property.GetMethod.IsPrivate ? "private" : "protected";

        return modifier == Modifier() ? "get; " : $"{modifier} get; ";
      }

      string Set()
      {
        if (property.SetMethod == null)
          return "";
        
        var modifier = property.SetMethod.IsPublic ? "public" : property.SetMethod.IsPrivate ? "private" : "protected";

        return modifier == Modifier() ? "set; " : $"{modifier} set; ";
      }
    }
    
    public Code.Scope Declaration(Type type)
    {
      return Text($"{Modifier()} {Kind()} {type.PrettyName()}{Parent()}").NewLine().Scope();
      
      string Modifier() =>
        (type.IsNested ? type.IsNestedPublic : type.IsPublic)
          ? "public"
          : "private";

      string Kind() =>
        type.IsValueType ? type.IsEnum ? "enum" : "struct" : "class";

      string Parent()
      {
        if (type.IsEnum)
        {
          var underlying = type.GetEnumUnderlyingType();
          if (underlying != typeof(int))
            return $" : {underlying.PrettyName()}";

          return "";
        }
        
        if (type.IsValueType)
          return "";
        if (type.BaseType == null || type.BaseType == typeof(object))
          return "";

        var name = type.BaseType.PrettyName();

        if (type.IsNested && type.BaseType.IsNested)
        {
          var declaring = DeclaringTypes(type.BaseType)
            .Except(DeclaringTypes(type))
            .Select(x => x.PrettyName())
            .Reverse()
            .Separate(".");

          name = $"{declaring}.{name}";
        }

        return $" : {name}";
      }

      IEnumerable<Type> DeclaringTypes(Type x)
      {
        while (x.DeclaringType != null)
          yield return x = x.DeclaringType;
      }

      string WithoutCommonStart(string source, string other)
      {
        var start = 0;
        var length = source.Length.Or(other.Length).IfGreater();
        
        for (var i = 0; i < length; i++)
        {
          if (source[i] != other[i])
            break;
          
          start = i;
        }

        return source.Substring(start + 2);
      }
    }
    
    public CSharp Enum(Type type)
    {
      using (Declaration(type))
      {
        var mappings = Mappings();

        foreach (var mapping in mappings)
          Text($"{mapping.Name} = {mapping.Value}")
            .Text(",", when: mapping != mappings.Last())
            .NewLine();
      }
      
      List<(string Name, object Value)> Mappings()
      {
        var underlying = type.GetEnumUnderlyingType();

        return type.GetEnumValues()
          .Cast<object>()
          .Select(x => (x.ToString(), Convert.ChangeType(x, underlying)))
          .ToList();
      }
      
      return this;
    }

    private static string Attributes(MemberInfo member)
    {
      var joined = member
        .GetCustomAttributesData()
        .Select(Attribute)
        .Separate(with: " ");
      if (joined.IsEmpty())
        return "";

      return $"{joined} ";
    }

    private static string Attribute(CustomAttributeData attribute)
    {
      var name = attribute.AttributeType.PrettyName().Without("Attribute").AtEnd;

      if (attribute.NamedArguments.IsEmpty() && attribute.ConstructorArguments.IsEmpty())
        return $"[{name}]";

      var arguments = attribute.ConstructorArguments.Select(x => x.ToString())
        .Concat(attribute.NamedArguments.Reverse().Select(x => x.ToString()))
        .Separate(", ");
      
      return $"[{name}({arguments})]";
    }
  }
}