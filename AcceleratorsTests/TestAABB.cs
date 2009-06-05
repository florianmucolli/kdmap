//  
//  Copyright (C) 2009 Christoph Heindl
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using NUnit.Framework;
using Accelerators;

namespace AcceleratorsTests
{
  
  
  [TestFixture()]
  public class TestAABB
  {
    [Test]
    public void TestConstructor() {
      AABB a = new AABB(2);
      Assert.AreEqual(a.Lower.Dimensions, 2);
      Assert.AreEqual(a.Upper.Dimensions, 2);
      Assert.AreEqual(Single.MaxValue, a.Lower[0]);
      Assert.AreEqual(Single.MaxValue, a.Lower[1]);
      Assert.AreEqual(-Single.MaxValue, a.Upper[0]);
      Assert.AreEqual(-Single.MaxValue, a.Upper[1]);
      Assert.IsTrue(a.Empty);
    }
    
    [Test]
    public void TestEmpty() {
      AABB a = new AABB(20);
      Assert.IsTrue(a.Empty);
      a.Lower[10] = 1.0f;
      Assert.IsFalse(a.Empty);
    }
    
    [Test]
    public void TestReset() {
      AABB a = new AABB(20);
      a.Lower[10] = 1.0f;
      a.Upper[10] = 1.0f;
      Assert.IsFalse(a.Empty);
      a.Reset();
      Assert.IsTrue(a.Empty);
    }
    
    [Test]
    public void TestEnlargeSingle() {
      AABB box = new AABB(2);
      Vector a = new Vector(1.0f, 2.0f);
      box.Enlarge(a);
      Assert.IsFalse(box.Empty);
      Assert.IsTrue(VectorComparison.Close(a, box.Lower, FloatComparison.DefaultEps));
      Assert.IsTrue(VectorComparison.Close(a, box.Upper, FloatComparison.DefaultEps));
      
      Vector b = new Vector(-1.0f, 4.0f);
      box.Enlarge(b);
      Assert.AreEqual(box.Lower[0], -1.0f, FloatComparison.DefaultEps);
      Assert.AreEqual(box.Lower[1], 2.0f, FloatComparison.DefaultEps);
      Assert.AreEqual(box.Upper[0], 1.0f, FloatComparison.DefaultEps);
      Assert.AreEqual(box.Upper[1], 4.0f, FloatComparison.DefaultEps);
    }
    
    [Test]
    public void TestEnlargeCollection() {
      AABB box = new AABB(2);
      box.Enlarge(VectorSampling.InAABB(1000, 2, -1.0f, 1.0f, 10));
      Assert.LessOrEqual(-1.0f , box.Lower[0]);
      Assert.LessOrEqual(-1.0f , box.Lower[1]);
      Assert.GreaterOrEqual(1.0f , box.Upper[0]);
      Assert.GreaterOrEqual(1.0f , box.Upper[1]);
    }
    
    [Test]
    public void TestDiagonal() {
      AABB box = new AABB(2);
      box.Enlarge(new Vector(1.0f, -2.0f));
      box.Enlarge(new Vector(-1.0f, 5.0f));
      Vector diag = (Vector)box.Diagonal;
      Assert.AreEqual(diag[0], 2.0f, FloatComparison.DefaultEps);
      Assert.AreEqual(diag[1], 7.0f, FloatComparison.DefaultEps);
    }
    
    [Test]
    public void TestExtension() {
      AABB box = new AABB(2);
      box.Enlarge(new Vector(1.0f, -2.0f));
      box.Enlarge(new Vector(-1.0f, 5.0f));
      Assert.AreEqual(box.Extension(0), 2.0f, FloatComparison.DefaultEps);
      Assert.AreEqual(box.Extension(1), 7.0f, FloatComparison.DefaultEps);
    }
    
    [Test]
    public void TestSplit() {
      AABB box = new AABB(new Vector(-1.0f, -1.0f), new Vector(1.0f, 1.0f));
      AABB left, right;
      box.Split(1, 0.5f, out left, out right);
      
      Assert.AreEqual(-1.0f, left.Lower[0], FloatComparison.DefaultEps);
      Assert.AreEqual(-1.0f, left.Lower[1], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0f, left.Upper[0], FloatComparison.DefaultEps);
      Assert.AreEqual(0.5f, left.Upper[1], FloatComparison.DefaultEps);
      
      Assert.AreEqual(-1.0f, right.Lower[0], FloatComparison.DefaultEps);
      Assert.AreEqual(0.5f, right.Lower[1], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0f, right.Upper[0], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0f, right.Upper[1], FloatComparison.DefaultEps);
    }
    
    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void TestSplitOutsidePlane() {
      AABB box = new AABB(new Vector(-1.0f, -1.0f), new Vector(1.0f, 1.0f));
      AABB left, right;
      box.Split(1, -1.1f, out left, out right);
    }
    
  }
}