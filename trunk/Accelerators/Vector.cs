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

namespace Accelerators
{
  
  /// <summary>
  /// N-Dimensional vector implementation
  /// </summary>
  public class Vector : IVector
  {
    /// <summary>
    /// Create a vector that can hold n-dimensions.
    /// </summary>
    public Vector(int dimensions) {
      _coordinates = new double[dimensions];
    }
    
    /// <summary>
    /// Create vector of n-dimensions with equal coordinates
    /// </summary>
    public Vector(int dimensions, double val) {
      _coordinates = new double[dimensions];
      VectorOperations.Fill(this, val);
    }

    /// <summary>
    /// Create a two-dimensional vector with coordinates explicitly set.
    /// </summary>
    public static Vector Create(double x, double y) {
      Vector v = new Vector(2);
      v[0] = x;
      v[1] = y;
      return v;
    }

    /// <summary>
    /// Create a three-dimesional vector with coordinates explicitly set.
    /// </summary>
    public static Vector Create(double x, double y, double z) {
      Vector v = new Vector(3);
      v[0] = x;
      v[1] = y;
      v[2] = z;
      return v;
    }

    /// <summary>
    /// Create a one-dimensional vector with coordinates explicitly set.
    /// </summary>
    public static Vector Create(double x) {
      Vector v = new Vector(1);
      v[0] = x;
      return v;
    }
    
    /// <summary>
    /// Copy construct from the given vector
    /// </summary>
    public Vector(Vector other) {
      _coordinates = (double[])other._coordinates.Clone();
    }
    
    /// <summary>
    /// Copy construct from a vector implementing the IVector interface
    /// </summary>
    public Vector(IVector other) {
      _coordinates = new double[other.Dimensions];
      for (int i = 0; i < Dimensions ; ++i )
        _coordinates[i] = other[i];
    }

    #region IVector implementation
    public int Dimensions {
      get {
        return _coordinates.Length;
      }
    }
    
    public double this[int index] {
      get {
        return _coordinates[index];
      }
      set {
        _coordinates[index] = value;
      }
    }
    #endregion
    
    public static Vector operator+(Vector lhs, IVector rhs) {
      Vector res = new Vector(lhs.Dimensions);
      VectorOperations.Add(lhs, rhs, res);
      return res;
    }
    
    public static Vector operator-(Vector lhs, IVector rhs) {
      Vector res = new Vector(lhs.Dimensions);
      VectorOperations.Sub(lhs, rhs, res);
      return res;
    }
    
    public static Vector operator*(Vector lhs, double s) {
      Vector res = new Vector(lhs.Dimensions);
      VectorOperations.ScalarMul(lhs, s, res);
      return res;
    }
    
    public double L2Norm {
      get {
        return VectorReductions.L2Norm(this);
      }
    }
    
    public override string ToString ()
    {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      sb.Append('[');
      for(int i = 0; i < Dimensions; ++i) {
        sb.Append(_coordinates[i]);
        if (i < Dimensions - 1)
          sb.Append(',');
      }
      sb.Append(']');
      return sb.ToString();
    }

    
    private double[] _coordinates;
  }
}