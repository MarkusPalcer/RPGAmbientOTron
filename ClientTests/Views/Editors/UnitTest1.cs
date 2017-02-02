using System;
using System.Reactive.Disposables;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientTests.Views.Editors
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestMethod1()
    {
      var uut = new SerialDisposable();
      
      var b1 = new BooleanDisposable();
      var b2 = new BooleanDisposable();

      uut.Disposable = b1;
      uut.Disposable = b2;
      b1.IsDisposed.Should().BeTrue();
      b2.IsDisposed.Should().BeFalse();
    }
  }
}
