using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Sort
{
    class MainSort :MonoBehaviour
    {

        public int[] array;

        void OnGUI()
        {

            if (GUILayout.Button("START Bubble"))
            {
                Bubble.Sort(array);
            }

            if (GUILayout.Button("START Insert"))
            {
                Insert.Sort(array);
            }
        }
    }
}
