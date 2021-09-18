using System.Collections;
using System.Collections.Generic;

namespace Utility
{
    public class ListHelper<T>
    {
        public bool CheckSublistExists(List<T> listA, List<T> listB)
        {
            int count = 0;
            foreach(T element in listB)
            {
                if (listA.Contains(element))
                {
                    count++;
                }
            }

            return count == listB.Count;
        }
    }
}