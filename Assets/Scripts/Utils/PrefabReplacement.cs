using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabReplacement : MonoBehaviour
{
    public GameObject prefabToReplaceWith;

    [ContextMenu("Replace With Prefab")]
    public void ReplaceWithPrefab()
    {
#if UNITY_EDITOR
        int option = EditorUtility.DisplayDialogComplex("Replace With Prefab",
                       "Are you sure you want to replace all children with the prefab?",
                                  "Yes", "No", "Cancel");
        if (option == 0)
        {
            int childCount = transform.childCount;

            // Instantiate prefabToReplaceWith at all childs of this GameObject
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);

                var obj = PrefabUtility.InstantiatePrefab(prefabToReplaceWith, transform) as GameObject;
                obj.transform.localPosition = child.localPosition;
                obj.transform.localRotation = child.localRotation;
            }
        }
#endif
    }
}
