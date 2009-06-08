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
using System.Collections.Generic;

namespace Accelerators
{
  
  /// <summary>
  /// Axis-aligned bounding box
  /// </summary>
  public class AABB : IBoundingVolume
  {
    
    /// <summary>
    /// Create an empty AABB in n-dimensions (min > max).
    /// </summary>
    public AABB(int dimensions)
    {
      _min = new Vector(dimensions, Double.MaxValue);
      _max = new Vector(dimensions, -Double.MaxValue);
    }
    
    /// <summary>
    /// Copy construct AABB
    /// </summary>
    public AABB(AABB other) {
      _min = new Vector(other._min);
      _max = new Vector(other._max);
    }
    
    /// <summary>
    /// Construct from two corner points.
    /// </summary>
    public AABB(IVector lower, IVector upper) {
      _min = new Vector(lower);
      _max = new Vector(upper);
    }
    
    /// <summary>
    /// Enlarge AABB to contain the given vectors.
    /// </summary>
    public void Enlarge<T>(IEnumerable<T> values) where T : IVector {
      foreach(IVector v in values) {
        this.Enlarge(v);
      }
    }
    
    /// <summary>
    /// Enlarge AABB to contain given vector.
    /// </summary>
    public void Enlarge<T>(T v) where T : IVector {
      for(int i = 0; i < this.Dimensions; ++i) {
        double vi = v[i];
        if (vi < _min[i]) _min[i] = vi;
        if (vi > _max[i]) _max[i] = vi;
      }
    }
    
    /// <summary>
    /// Split AABB into two parts using an axis aligned splitting plane. The plane
    /// is specified by the dimension it is orthogonal to and a position value on the
    /// axis.
    /// </summary>
    public void Split(int dimension, double position, out AABB left, out AABB right) {
      
      if (!Inside(dimension, position))
        throw new ArgumentException("Split plane is outside of AABB");
      
      // -> x
      // |           upperL     upperR
      // v  +--------+--------+
      // y  |        |        |
      //    | left   | right  |
      //    |        |        |
      //    |        |        |
      //    +--------+--------+
      //    lowerL     lowerR
      
      left = new AABB(this);
      right = new AABB(this);
      left.Upper[dimension] = position;
      right.Lower[dimension] = position;
    }
    
    /// <value>
    /// Test if AABB is empty. 
    /// </value>
    public bool Empty {
      get {
        return 
          VectorComparison.Equal(_min, new Vector(this.Dimensions, Double.MaxValue)) &&
          VectorComparison.Equal(_max, new Vector(this.Dimensions, -Double.MaxValue));
      }
    }
    
    /// <summary>
    /// Reset to empty state.
    /// </summary>
    public void Reset() {
      VectorOperations.Fill(_min, Double.MaxValue);
      VectorOperations.Fill(_max, -Double.MaxValue);
    }
    
    /// <value>
    /// Lower corner of AABB.
    /// Changes to the returned vector will affect the internal state of this ABBB.
    /// </value>
    public IVector Lower {
      get {
        return _min;
      }
    }
    
    
    /// <value>
    /// Upper corner of AABB.
    /// Changes to the returned vector will affect the internal state of this ABBB.
    /// </value>
    public IVector Upper {
      get {
        return _max;
      }
    }
    
    /// <value>
    /// Diagonal of AABB as vector from lower corner to upper corner.
    /// This vector is calculated and changes to it will not modify the internal state of this AABB.
    /// </value>
    public IVector Diagonal {
      get {
        return (_max - _min);
      }
    }
    
    /// <value>
    /// Access the center of the AABB.
    /// This vector is calculated and changes to it will not modify the internal state of this AABB.
    /// </value>
    public IVector Center {
      get {
        return (_min + (_max - _min)*0.5);
      }
    }
    
    /// <summary>
    /// Return the AABBs extension in the given dimension
    /// </summary>
    public double Extension(int dimension) {
      return _max[dimension] - _min[dimension];
    }
    
    /// <value>
    /// Access the number of dimensions 
    /// </value>
    public int Dimensions {
      get {
        return _min.Dimensions;
      }
    }
    
    /// <summary>
    /// Test if given vector is contained in AABB
    /// </summary>
    public bool Inside(IVector x) {
      for (int i = 0; i < this.Dimensions; ++i) {
        if (!Inside(i, x[i]))
          return false;
      }
      return true;
    }
    
    /// <summary>
    /// Test if AABB overlaps given AABB
    /// </summary>
    public bool Intersect(AABB aabb) {
      // Perform a per-dimension overlapping interval test and early exit if
      // a single interval is none overlapping
      for (int i = 0; i < this.Dimensions; ++i) {
        if (!OverlapInterval(this.Lower[i], this.Upper[i], aabb.Lower[i], aabb.Upper[i]))
          return false;
      }
      return true;
    }
    
    /// <summary>
    /// Determine the location of the given axis aligned plane relative to the position of the
    /// bounding volume.
    /// </summary>
    public EPlanePosition ClassifyPlane(int dimension, double position) {
      double li = Lower[dimension];
      double ui = Upper[dimension];
      
      if (position < li)
        return EPlanePosition.LeftOfBV;
      else if (position > ui)
        return EPlanePosition.RightOfBV;
      else
        return EPlanePosition.IntersectingBV;
    }

    /// <summary>
    /// Find the point on/in the AABB that is closest to the query.
    /// </summary>
    public void Closest(IVector x, ref IVector closest) {
      for (int i = 0; i < this.Dimensions; ++i) {
        // Closest is given by: lower[i] if x[i] < lower[i], upper[i] if x[i] > upper[i], else
        // it is x[i]
        double xi = x[i];
        if (xi < this.Lower[i])
          closest[i] = this.Lower[i];
        else if (xi > this.Upper[i])
          closest[i] = this.Upper[i];
        else
          closest[i] = xi;
      }
    }
    
    /// <summary>
    /// Calculate the closest point on/inside this AABB to the given query point.
    /// </summary>
    public IVector Closest(IVector x) {
      IVector closest = new Vector(this.Dimensions);
      this.Closest(x, ref closest);
      return closest;
    }
    
    /// <summary>
    /// Convert to string.
    /// </summary>
    public override string ToString ()
    {
      return string.Format("[AABB: Lower={0}, Upper={1}]", Lower, Upper);
    }

    
    /// <summary>
    /// Test if the given axis aligned plane crosses the AABB
    /// </summary>
    private bool Inside(int dimension, double position) {
      return OverlapInterval(this.Lower[dimension], this.Upper[dimension], position, position);
    }
    
    /// <summary>
    /// Test if two intervals overlap
    /// </summary>
    private bool OverlapInterval(double a_lower, double a_upper, double b_lower, double b_upper) {
      return a_lower <= b_upper && b_lower <= a_upper;
    }
      
    
    private Vector _min;
    private Vector _max;
  }
}
