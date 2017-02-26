using UnityEngine;
using System.Collections;

public class Bubble : MonoBehaviour {

    public static void Sort(int[] array)
    {
        if (array == null)
        {
            Debug.LogError("array is null");
            return;
        }
        string msg = "";
        for (int i = 0; i < array.Length; i++)
        {
            msg += array[i] + "  ";
        }
        Debug.Log(msg);
        bool swap = false;

        int lastUnsortIndex = array.Length - 1;
        int index = 1;
        do
        {
            swap = false;
            for (int i = 0; i < lastUnsortIndex; i++)
			{
                if (array[i] > array[i + 1])
                {
                    int temp = array[i];
                    array[i] = array[i + 1];
                    array[i + 1] = temp;
                    swap = true;
                }
			}

            lastUnsortIndex-- ;


            msg = "index : " + index + "--->";
            index++;
            for (int i = 0; i < array.Length; i++)
            {
                msg += array[i] + "  " ;
            }
            Debug.Log(msg);
        } while (swap);

    }

}
