using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProducerAlpha
{
    public static class LinqExtensionMethods
    {
        public static string AsStringEnumeration<TEntity, TProperty>(
          this IEnumerable<TEntity> allItems,
          Func<TEntity, TProperty> property)
          where TEntity : class
        {
            string result = "";

            foreach (var item in allItems)
            {
                result += property(item).ToString() + ", ";
            }

            if (!string.IsNullOrEmpty(result))
            {
                result = result.Remove(result.Length - 2);
            }

            return result;
        }
    }
}
