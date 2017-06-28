using System.Windows;
using System.Windows.Media;

namespace Mineral.Helper
{
    class ControlHelper
    {
        /// <summary>
        /// 获取模板中的子元素
        /// </summary>
        /// <typeparam name="ChildItem">基本为</typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ChildItem FindVisualChildItem<ChildItem>(DependencyObject obj, string name) where ChildItem : FrameworkElement
        {
            if (null != obj)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is ChildItem && (child as ChildItem).Name.Equals(name))
                    {
                        return (ChildItem)child;
                    }
                    else
                    {
                        ChildItem childOfChild = FindVisualChildItem<ChildItem>(child, name);
                        if (childOfChild != null && childOfChild is ChildItem && (childOfChild as ChildItem).Name.Equals(name))
                        {
                            return childOfChild;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ChildItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ChildItem FindVisualChildItem<ChildItem>(DependencyObject obj) where ChildItem : DependencyObject
        {
            if (null != obj)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is ChildItem)
                        return (ChildItem)child;
                    else
                    {
                        ChildItem childOfChild = FindVisualChildItem<ChildItem>(child);
                        if (childOfChild != null)
                            return childOfChild;
                    }
                }
            }
            return null;
        }
    }
}
