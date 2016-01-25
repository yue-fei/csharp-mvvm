﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace WP.MVVM.Core
{
    public static class VisualTreeEnumeration
    {
        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root, int depth)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                if (depth > 0)
                {
                    foreach (var descendent in Descendents(child, --depth))
                        yield return descendent;
                }
            }
        }

        //Child对象集合
        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
        {
            return Descendents(root, Int32.MaxValue);
        }

        //PARNET 对象集合
        public static IEnumerable<DependencyObject> Ancestors(this DependencyObject root)
        {
            DependencyObject current = VisualTreeHelper.GetParent(root);
            while (current != null)
            {
                yield return current;
                current = VisualTreeHelper.GetParent(current);
            }
        }
    }
}
