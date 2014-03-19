using System;
using System.Collections;
using System.Collections.Generic;

namespace ZewnetrzneBiblioteki.FortuneVoronoi
{
   /// <summary>
   ///    Set für effizienten Zugriff auf Objekte. Objete werden als Key abgelegt, value ist nur ein dummy-Objekt.
   /// </summary>
   [Serializable]
   public class FortuneHashSet<T> : IEnumerable<T>, ICollection<T>
   {
      private static readonly object Dummy = new object();
      private Dictionary<T, object> Core;

      public FortuneHashSet(IEnumerable<T> source)
         : this()
      {
         AddRange(source);
      }

      public FortuneHashSet(IEqualityComparer<T> eqComp)
      {
         Core = new Dictionary<T, object>(eqComp);
      }

      public FortuneHashSet()
      {
         Core = new Dictionary<T, object>();
      }

      public bool Contains(T o)
      {
         return Core.ContainsKey(o);
      }

      public bool Remove(T o)
      {
         return Core.Remove(o);
      }

      public void Clear()
      {
         Core.Clear();
      }

      void ICollection<T>.Add(T item)
      {
         Add(item);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return Core.Keys.GetEnumerator();
      }

      #region IEnumerable<T> Members

      public IEnumerator<T> GetEnumerator()
      {
         return Core.Keys.GetEnumerator();
      }

      #endregion

      #region ICollection<T> Members

      public bool IsSynchronized
      {
         get { return false; }
      }

      public int Count
      {
         get { return Core.Count; }
      }

      public void CopyTo(T[] array, int index)
      {
         Core.Keys.CopyTo(array, index);
      }

      public bool IsReadOnly
      {
         get { return false; }
      }

      #endregion

      public bool Add(T o)
      {
         int count = Core.Count;
         Core[o] = Dummy;
         if (count == Core.Count)
            return false;
         return true;
      }

      [Obsolete]
      public void AddRange(IEnumerable List)
      {
         foreach (T O in List)
            Add(O);
      }

      public void AddRange(IEnumerable<T> List)
      {
         foreach (T O in List)
            Add(O);
      }
   }
}