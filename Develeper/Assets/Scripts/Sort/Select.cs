using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Sort
{

    public class Select :MonoBehaviour
    {
        public static void Sort(int[] array)
        {

            if (array == null)
            {
                return;
            }

            string msg = "before";
            for (int i = 0; i < array.Length; i++)
            {
                msg += array[i] + " ";
            }
            Debug.Log(msg);

            int minIndex = 0;
            
            int startIndex = 0;

            while (startIndex < array.Length - 1)
            {
                minIndex = startIndex;

                for (int i = startIndex + 1; i < array.Length; i++)
                {
                    if (array[i] < array[minIndex])
                    {
                        minIndex = i;
                    }
                }

                int temp = array[startIndex];
                array[startIndex] = array[minIndex];
                array[minIndex] = temp;

                startIndex++;

                msg = string.Format(" 第{0}次排序: ",startIndex);
                for (int i = 0; i < array.Length; i++)
                {
                    msg += array[i] + " ";
                }
                Debug.Log(msg);
            }
        }

    }
}
