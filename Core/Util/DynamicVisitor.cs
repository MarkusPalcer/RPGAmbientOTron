using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Util
{
  public class DynamicVisitor<TBase>
  {
    readonly Dictionary<Type, Action<TBase>> handlers = new Dictionary<Type, Action<TBase>>();

    public void Register<T>(Action<T> handler) where T : TBase
    {
      Action<TBase> baseTypeHandler = value => handler((T) value);
      handlers[typeof(T)] = baseTypeHandler;
    }

    public void Visit(TBase value)
    {
      var actualType = value.GetType();

      handlers[actualType](value);
    }
  }
}