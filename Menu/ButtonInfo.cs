using System;
namespace LagMenu.Menu
{
    public class ButtonInfo
    {
        public string buttonText;
        public string toolTip;
        public bool isTogglable;
        public Action method;
        public Action enableMethod;
        public Action disableMethod;
        public bool enabled;
        public Func<bool> shouldShow;
        public string overlapText;

        public ButtonInfo() { }

        public ButtonInfo(string buttonText, string toolTip, bool isTogglable, Action method, Action enableMethod, Action disableMethod, bool enabled, Func<bool> shouldShow, string overlapText = null)
        {
            this.buttonText = buttonText;
            this.toolTip = toolTip;
            this.isTogglable = isTogglable;
            this.method = method;
            this.enableMethod = enableMethod;
            this.disableMethod = disableMethod;
            this.enabled = enabled;
            this.shouldShow = shouldShow;
            this.overlapText = overlapText;
        }

        public bool Is(string name)
        {
            return buttonText == name;
        }
        public static ButtonInfo Get(string name)
        {
            for (int i = 0; i < Buttons.buttons.Length; i++)
            {
                for (int j = 0; j < Buttons.buttons[i].Length; j++)
                {
                    if (Buttons.buttons[i][j].buttonText == name)
                        return Buttons.buttons[i][j];
                }
            }
            return null;
        }
        public static bool GetEnabled(string name)
        {
            var btn = Get(name);
            return btn != null && btn.enabled;
        }
    }
}
