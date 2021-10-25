using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralUtility
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

namespace PlayerData
{
    [System.Serializable]
    public class ConsuamblesData
    {
        public string Name;
        public int Quantity;

        public ConsuamblesData(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }
    }
}

namespace UIUtility
{
    public class StatText
    {
        public string TextFieldName;
        public GameObject TextObj;
        public StatText(string TextFieldName, GameObject TextObj)
        {
            this.TextFieldName = TextFieldName;
            this.TextObj = TextObj;
        }       
    }
}