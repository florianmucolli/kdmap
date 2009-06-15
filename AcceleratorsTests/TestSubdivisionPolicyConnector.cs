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
using Accelerators.Subdivision;
using System.Collections.Generic;

namespace AcceleratorsTests
{
  
  
  [TestFixture()]
  public class TestSubdivisionPolicyConnector
  {
    
    [Test]
    [ExpectedException(typeof(BucketSizeException))]
    public void TestBucketSize() {
      SubdivisionPolicyConnector c = new SubdivisionPolicyConnector(10);

      KdNode<Vector> n = new KdNode<Vector>();
      n.Vectors = new List<Vector>(new Vector[] { new Vector(1.0, 1.0), new Vector(2.0, 3.0), new Vector(3.0, 1.0), new Vector(4.0, 1.0) });
      n.SplitBounds = new AABB(2);
      n.SplitBounds.Enlarge<Vector>(n.Vectors);
      n.InternalBounds = new AABB(n.SplitBounds);
      c.Split(n);
    }
    
    [Test]
    [ExpectedException(typeof(IntermediateNodeException))]
    public void TestIntermediateNode() {
      SubdivisionPolicyConnector c = new SubdivisionPolicyConnector(1);


      KdNode<Vector> n = new KdNode<Vector>();
      n.Vectors = new List<Vector>(new Vector[] { new Vector(1.0, 1.0), new Vector(2.0, 3.0), new Vector(3.0, 1.0), new Vector(4.0, 1.0) });
      n.SplitBounds = new AABB(2);
      n.SplitBounds.Enlarge<Vector>(n.Vectors);
      n.InternalBounds = new AABB(n.SplitBounds);
      c.Split(n);
      c.Split(n); // split again
    }
    
    [Test]
    public void TestSplitOneDimensional() {
      ISubdivisionPolicy p = SubdivisionPolicyConnector.CreatePolicy<AxisOfMaximumSpreadSelector, MidpointSelector, SlidingPlaneResolver>(1);
      
      KdNode<Vector> n = new KdNode<Vector>();
      n.Vectors = new List<Vector>(new Vector[] { new Vector(-1.0), new Vector(1.0), new Vector(3.0), new Vector(2.0) });
      n.SplitBounds = new AABB(1);
      n.SplitBounds.Enlarge<Vector>(n.Vectors);
      n.InternalBounds = new AABB(n.SplitBounds);
      p.Split(n);

      Assert.AreEqual(1.0, n.SplitLocation, FloatComparison.DefaultEps);
      Assert.AreEqual(0, n.SplitDimension);

      KdNode<Vector> left = n.Left;
      KdNode<Vector> right = n.Right;

      Assert.AreEqual(2, left.Vectors.Count);
      Assert.AreEqual(-1.0, left.Vectors[0][0], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0, left.Vectors[1][0], FloatComparison.DefaultEps);
      
      Assert.AreEqual(2, right.Vectors.Count);
      Assert.AreEqual(3.0, right.Vectors[0][0], FloatComparison.DefaultEps);
      Assert.AreEqual(2.0, right.Vectors[1][0], FloatComparison.DefaultEps);

      Assert.AreEqual(-1.0, left.SplitBounds.Lower[0], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0, left.SplitBounds.Upper[0], FloatComparison.DefaultEps);

      Assert.AreEqual(1.0, right.SplitBounds.Lower[0], FloatComparison.DefaultEps);
      Assert.AreEqual(3.0, right.SplitBounds.Upper[0], FloatComparison.DefaultEps);
    }
    
    [Test]
    public void TestSplitMultiDimensional() {
      ISubdivisionPolicy p = SubdivisionPolicyConnector.CreatePolicy<AxisOfMaximumSpreadSelector, MidpointSelector, SlidingPlaneResolver>(1);

      KdNode<Vector> n = new KdNode<Vector>();
      n.Vectors = new List<Vector>(new Vector[] { new Vector(1.0, 1.0), new Vector(1.0, -1.0), new Vector(1.0, 3.0), new Vector(1.0, 2.0) });
      n.SplitBounds = new AABB(2);
      n.SplitBounds.Enlarge<Vector>(n.Vectors);
      n.InternalBounds = new AABB(n.SplitBounds);
      p.Split(n);

      Assert.AreEqual(1.0, n.SplitLocation, FloatComparison.DefaultEps);
      Assert.AreEqual(1, n.SplitDimension);

      KdNode<Vector> left = n.Left;
      KdNode<Vector> right = n.Right;

      Assert.AreEqual(2, left.Vectors.Count);
      Assert.AreEqual(1.0, left.Vectors[0][0], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0, left.Vectors[0][1], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0, left.Vectors[1][0], FloatComparison.DefaultEps);
      Assert.AreEqual(-1.0, left.Vectors[1][1], FloatComparison.DefaultEps);

      Assert.AreEqual(2, right.Vectors.Count);
      Assert.AreEqual(1.0, right.Vectors[0][0], FloatComparison.DefaultEps);
      Assert.AreEqual(3.0, right.Vectors[0][1], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0, right.Vectors[1][0], FloatComparison.DefaultEps);
      Assert.AreEqual(2.0, right.Vectors[1][1], FloatComparison.DefaultEps);

      
      Assert.AreEqual(1.0, left.SplitBounds.Lower[0], FloatComparison.DefaultEps);
      Assert.AreEqual(-1.0, left.SplitBounds.Lower[1], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0, left.SplitBounds.Upper[0], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0, left.SplitBounds.Upper[1], FloatComparison.DefaultEps);

      Assert.AreEqual(1.0, right.SplitBounds.Lower[0], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0, right.SplitBounds.Lower[1], FloatComparison.DefaultEps);
      Assert.AreEqual(1.0, right.SplitBounds.Upper[0], FloatComparison.DefaultEps);
      Assert.AreEqual(3.0, right.SplitBounds.Upper[1], FloatComparison.DefaultEps);
    }
  }
}
