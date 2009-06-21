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
  public partial class KdTree<T> : ICollection<T> where T : IVector
  {
   
    
    /// <summary>
    /// Find all elements that are inside the given bounding volume
    /// </summary>
    public IEnumerable<T> FindInsideVolume(IBoundingVolume bv) {
     
      // Test against root node
      if (bv.Intersect(this.Root.InternalBounds)) {
        // Init search
        Stack<KdNode<T>> s = new Stack<KdNode<T>>();
        s.Push(this.Root);
        
        while (s.Count > 0) {
          KdNode<T> n = s.Pop();
          if (n.Leaf) {
            foreach (T t in n.Vectors) {
              if (bv.Inside(t))
                yield return t;
            }
          } else { // Intermediate node
            // Classify against split plane
            EPlanePosition pos = bv.ClassifyPlane(n.SplitDimension, n.SplitLocation);
            if (pos == EPlanePosition.LeftOfBV) {
              s.Push(n.Right);
            } else if (pos == EPlanePosition.RightOfBV) {
              s.Push(n.Left);
            } else {
              s.Push(n.Right);
              s.Push(n.Left);
            }
          }
        }
      }
    }

    /// <summary>
    /// Find all vectors in sorted order from the given query position.
    /// </summary>
    public IEnumerable<T> FindInSortedOrder(IVector query, double max_distance) {
      // We maintain two priority queues: one based on distances to nodes, one based on distances to elements.
      PriorityQueue<double, KdNode<T>> pqNodes = new PriorityQueue<double, KdNode<T>>();
      PriorityQueue<double, T> pqElements = new PriorityQueue<double, T>();

      IVector closest = new Vector(query.Dimensions);
      double dist2;
      
      // Make sure that we use maximum possible value for squared maximum distance
      double max_distance2 = max_distance;
      if (max_distance2 < Math.Sqrt(Double.MaxValue))
        max_distance2 *= max_distance2;
      else
        max_distance2 = Double.MaxValue;

      // Push root
      this.Root.InternalBounds.Closest(query, ref closest);
      dist2 = VectorReductions.SquaredL2NormDistance(query, closest);
      if (dist2 <= max_distance2)
        pqNodes.Push(dist2, this.Root);
      
      // While there is something to process
      while (pqNodes.Count > 0 || pqElements.Count > 0) {
        bool processNode = pqNodes.Count > 0 && (pqElements.Count == 0 || pqNodes.PeekPriority() < pqElements.PeekPriority());
        if (processNode) {
          KdNode<T> n = pqNodes.Pop();
          if (n.Leaf) {
            // For a leaf we insert all points if they are closer than allowed max distance.
            foreach (T t in n.Vectors) {
              dist2 = VectorReductions.SquaredL2NormDistance(query, t);
              if (dist2 < max_distance2)
                pqElements.Push(dist2, t);
            }
          } else {
            // For a node we insert its children if their closest point is less than the allowed maximum distance
            n.Left.InternalBounds.Closest(query, ref closest);
            dist2 = VectorReductions.SquaredL2NormDistance(query, closest);
            if (dist2 <= max_distance2)
              pqNodes.Push(dist2, n.Left);

            n.Right.InternalBounds.Closest(query, ref closest);
            dist2 = VectorReductions.SquaredL2NormDistance(query, closest);
            if (dist2 <= max_distance2)
              pqNodes.Push(dist2, n.Right);
          }
        } else { // process elements while possible
          double dist2closestNode = Double.MaxValue;
          if (pqNodes.Count > 0)
            dist2closestNode = pqNodes.PeekPriority();

          while ((pqElements.Count > 0) && (pqElements.PeekPriority() < dist2closestNode)) {
            yield return pqElements.Pop();
          }
        }
      }
    }
  }
}
