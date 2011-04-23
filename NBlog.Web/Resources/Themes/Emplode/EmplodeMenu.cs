using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBlog.Web.Resources.Themes.Emplode
{
    public class EmplodeMenu
    {
        private static List<EmplodeMenuItem> allItems = new List<EmplodeMenuItem>
        {
            {new EmplodeMenuItem("Home", "Home", "Index")},
            {new EmplodeMenuItem("Contact", "Contact", "Index")},
            {new EmplodeMenuItem("Rss", "Feed", "Index")}
        };

        public static IEnumerable<EmplodeMenuItem> GetItems(string currentController, string currentAction)
        {
            foreach (var item in allItems)
            {
                if (item.Controller == currentController && item.Action == currentAction)
                    item.CssClass = "current_page_item";
                else
                    item.CssClass = "page_item";
                yield return item;
            }
        }
    }

    public class EmplodeMenuItem
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Text { get; set; }
        public string CssClass { get; set; }

        public EmplodeMenuItem(string text, string controller, string action)
        {
            Text = text;
            Controller = controller;
            Action = action;
        }
    }
}