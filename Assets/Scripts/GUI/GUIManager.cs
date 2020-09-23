using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : SingletonComponent<GUIManager>
{
    private List<GUIMonoBehaviour> _guis = new List<GUIMonoBehaviour>();

    private void Update()
    {
        if (GUIHasCursor())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void AddGUI(GUIMonoBehaviour gui)
    {
        _guis.Add(gui);
    }

    public void RemoveGUI(GUIMonoBehaviour gui)
    {
        _guis.Remove(gui);
    }

    public bool GUIHasCursor()
    {
        foreach(var gui in _guis)
        {
            if(gui.IsOpen && gui.HasCursor())
            {
                return true;
            }
        }
        return false;
    }
}
