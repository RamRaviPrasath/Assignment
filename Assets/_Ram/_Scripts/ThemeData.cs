using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThemeData", menuName = "Theme/ThemeData", order = 1)]
public class ThemeData : ScriptableObject
{
    public Theme Normal;
    public Theme Dark;

    [System.Serializable]
    public class Theme
    {
        public Color PanelBGColor;
        public Color backgroundColor;
        public Color textColor;
        public Color buttonColor;
        public Color buttonTextColor;
        public Color IconColor;
    }

    public Theme GetTheme(int _index)
    {
        return _index == 0 ? Normal : Dark;
    }
}
