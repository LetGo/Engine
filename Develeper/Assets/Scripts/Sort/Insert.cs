using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Sort
{
    class Insert
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
            
            for (int i = 1; i < array.Length; i++)
            {
                int currIndex = i;
                int currNum = array[currIndex];
                
                while (currIndex - 1 >= 0)
                {
                    if (currNum < array[currIndex - 1] )
                    {
                        array[currIndex] = array[currIndex - 1];
                    }
                    else
                    {
                        array[currIndex] = currNum;
                        break;
                    }
                    currIndex--;
                    if (currIndex ==  0)
                    {
                        array[currIndex] = currNum;
                    }
                }

                msg = string.Format(" 第{0}次排序 当前数字 {1}: ", i,currNum);
                
                for (int n = 0; n < array.Length; n++)
                {
                    msg += array[n] + " ";
                }
                
                Debug.Log(msg);
            }
        }
    }
}
