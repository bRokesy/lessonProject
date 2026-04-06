using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RaycastDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var results = new List<RaycastResult>();
            var data = new PointerEventData(EventSystem.current) 
            { 
                position = Input.mousePosition 
            };
            EventSystem.current.RaycastAll(data, results);
            
            foreach (var r in results)
                Debug.Log("HIT: " + r.gameObject.name);
        }
    }
}