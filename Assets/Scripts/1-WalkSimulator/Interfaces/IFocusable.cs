using System.Diagnostics;
using UnityEngine;

public interface IFocusable {
    public void FocusManager(GameObject focusedObject);
    public void Focus();
    public void UnFocus();
}