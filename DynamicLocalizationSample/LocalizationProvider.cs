using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

namespace DynamicLocalizationSample
{
    public static class LocalizationProvider
    {
        private static List<DependencyObject> _contentObjects;
        private static List<DependencyObject> _toolTipObjects;
        private static List<DependencyObject> _titleObjects;
        private static List<DependencyObject> _waterMarkObjects;

        static LocalizationProvider()
        {
            _contentObjects = new List<DependencyObject>();
            _toolTipObjects = new List<DependencyObject>();
            _titleObjects = new List<DependencyObject>();
            _waterMarkObjects = new List<DependencyObject>();
        }

        class ResourceAndCulture
        {
            public ResourceAndCulture(string baseName = "", string cultureName = "")
            {
                if (baseName == string.Empty || cultureName == string.Empty)
                {
                    string newBaseName = string.Empty;
                    string newCultureName = string.Empty;

                    switch (AppSettings.AppLanguage)
                    {
                        case "Spanish":
                            {
                                newBaseName = "DynamicLocalizationSample.Resources.es";
                                newCultureName = "es";
                                break;
                            }
                        default:
                            {
                                newBaseName = "DynamicLocalizationSample.Resources.en-US";
                                newCultureName = "en";
                                break;
                            }
                    }

                    if (baseName == string.Empty)
                    {
                        baseName = newBaseName;
                    }

                    if (cultureName == string.Empty)
                    {
                        cultureName = newCultureName;
                    }
                }

                Res = new ResourceManager(baseName, typeof(LocalizationProvider).Assembly);
                Cul = CultureInfo.CreateSpecificCulture(cultureName);
            }

            public ResourceManager Res
            {
                private set;
                get;
            }

            public CultureInfo Cul
            {
                private set;
                get;
            }
        }

        private static string _GetLocalizedString(string key, ResourceAndCulture rc = null)
        {
            if (rc == null)
            {
                rc = new ResourceAndCulture();
            }

            return rc.Res.GetString(key, rc.Cul) ?? key;
        }

        public static string GetLocalizedString(string key)
        {
            return _GetLocalizedString(key);
        }

        public static Dictionary<string, string> GetLocalizedStrings(List<string> keys)
        {
            Dictionary<string, string> localizedStrings = new Dictionary<string, string>();
            ResourceAndCulture rc = new ResourceAndCulture();

            foreach (string key in keys)
            {
                localizedStrings[key] = _GetLocalizedString(key, rc);
            }

            return localizedStrings;
        }

        public static void UpdateAllObjects()
        {
            ResourceAndCulture rc = null;
            string key = string.Empty;
            string resourceValue = string.Empty;

            foreach (DependencyObject obj in _contentObjects)
            {
                DependencyObject cctrl = obj;

                if (cctrl == null)
                {
                    throw new InvalidCastException(string.Format("Type '{0}' does not derive from type 'ContentControl'.", obj.GetType().FullName));
                }

                try
                {
                    key = (string)obj.GetValue(ContentIDProperty);
                    rc = new ResourceAndCulture();
                    resourceValue = rc.Res.GetString(key, rc.Cul);
                }
                catch (Exception) { }

                if (resourceValue != null)
                {
                    if (cctrl.GetType() == typeof(GroupBox) || cctrl.GetType() == typeof(TreeViewItem))
                    {
                        cctrl.SetValue(HeaderedContentControl.HeaderProperty, resourceValue);
                    }
                    else if (cctrl.GetType() == typeof(GridViewColumn))
                    {
                        cctrl.SetValue(GridViewColumn.HeaderProperty, resourceValue);
                    }
                    else if (cctrl.GetType() == typeof(DataGridTextColumn))
                    {
                        cctrl.SetValue(DataGridTextColumn.HeaderProperty, resourceValue);
                    }
                    else if (cctrl.GetType() == typeof(DataGridTemplateColumn))
                    {
                        cctrl.SetValue(DataGridTemplateColumn.HeaderProperty, resourceValue);
                    }
                    else if (cctrl.GetType() == typeof(MenuItem))
                    {
                        cctrl.SetValue(HeaderedItemsControl.HeaderProperty, resourceValue);
                    }
                    else if (cctrl.GetType() == typeof(Window))
                    {
                        cctrl.SetValue(Window.TitleProperty, resourceValue);
                    }
                    else if (cctrl.GetType() == typeof(TextBlock))
                    {
                        cctrl.SetValue(TextBlock.TextProperty, resourceValue);
                    }
                    else
                    {
                        cctrl.SetValue(ContentControl.ContentProperty, resourceValue);
                    }
                }
            }

            foreach (DependencyObject obj in _toolTipObjects)
            {
                DependencyObject cctrl = obj;

                if (cctrl == null)
                {
                    throw new InvalidCastException(string.Format("Type '{0}' does not derive from type 'ContentControl'.", obj.GetType().FullName));
                }

                try
                {
                    key = (string)obj.GetValue(ToolTipIDProperty);
                }
                catch (Exception) { }

                if (key != null)
                {
                    resourceValue = rc.Res.GetString(key, rc.Cul);

                    if (resourceValue != null)
                    {
                        cctrl.SetValue(FrameworkElement.ToolTipProperty, resourceValue);
                    }
                }
            }

            foreach (DependencyObject obj in _titleObjects)
            {
                DependencyObject cctrl = obj;

                if (cctrl == null)
                {
                    throw new InvalidCastException(string.Format("Type '{0}' does not derive from type 'ContentControl'.", obj.GetType().FullName));
                }

                try
                {
                    key = (string)obj.GetValue(TitleIDProperty);
                }
                catch (Exception) { }

                if (key != null)
                {
                    resourceValue = rc.Res.GetString(key, rc.Cul);

                    if (resourceValue != null)
                    {
                        cctrl.SetValue(Window.TitleProperty, resourceValue);
                    }
                }
            }
        }

        public static string GetContentID(DependencyObject obj)
        {
            return (string)obj.GetValue(ContentIDProperty);
        }

        public static void SetContentID(DependencyObject obj, string value)
        {
            obj.SetValue(ContentIDProperty, value);
        }

        public static string GetToolTipID(DependencyObject obj)
        {
            return (string)obj.GetValue(ToolTipIDProperty);
        }

        public static void SetToolTipID(DependencyObject obj, string value)
        {
            obj.SetValue(ToolTipIDProperty, value);
        }

        public static string GetTitleID(DependencyObject obj)
        {
            return (string)obj.GetValue(TitleIDProperty);
        }

        public static void SetTitleID(DependencyObject obj, string value)
        {
            obj.SetValue(TitleIDProperty, value);
        }

        private static void OnContentIDChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ResourceAndCulture rc = null;
            string key = (string)e.NewValue;

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            DependencyObject cctrl = obj;

            if (cctrl == null)
            {
                throw new InvalidCastException(string.Format("Type '{0}' does not derive from type 'ContentControl'.", obj.GetType().FullName));
            }

            string resourceValue = null;

            try
            {
                rc = new ResourceAndCulture();
                resourceValue = rc.Res.GetString(key, rc.Cul);
            }
            catch (Exception) { }

            if (resourceValue != null)
            {
                if (cctrl.GetType() == typeof(GroupBox) || cctrl.GetType() == typeof(TreeViewItem))
                {
                    cctrl.SetValue(HeaderedContentControl.HeaderProperty, resourceValue);
                }
                else if (cctrl.GetType() == typeof(GridViewColumn))
                {
                    cctrl.SetValue(GridViewColumn.HeaderProperty, resourceValue);
                }
                else if (cctrl.GetType() == typeof(DataGridTextColumn))
                {
                    cctrl.SetValue(DataGridTextColumn.HeaderProperty, resourceValue);
                }
                else if (cctrl.GetType() == typeof(DataGridTemplateColumn))
                {
                    cctrl.SetValue(DataGridTemplateColumn.HeaderProperty, resourceValue);
                }
                else if (cctrl.GetType() == typeof(MenuItem))
                {
                    cctrl.SetValue(HeaderedItemsControl.HeaderProperty, resourceValue);
                }
                else if (cctrl.GetType() == typeof(Window))
                {
                    cctrl.SetValue(Window.TitleProperty, resourceValue);
                }
                else if (cctrl.GetType() == typeof(TextBlock))
                {
                    cctrl.SetValue(TextBlock.TextProperty, resourceValue);
                }
                else
                {
                    cctrl.SetValue(ContentControl.ContentProperty, resourceValue);
                }

                if (!_contentObjects.Contains(obj))
                {
                    _contentObjects.Add(obj);
                }
            }
        }

        private static void OnToolTipIDChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            DependencyObject cctrl = obj;

            if (cctrl == null)
            {
                throw new InvalidCastException(string.Format("Type '{0}' does not derive from type 'DependencyObject'.", obj.GetType().FullName));
            }

            string resourceValue = null;

            try
            {

                ResourceAndCulture rc = new ResourceAndCulture();
                resourceValue = rc.Res.GetString((string)e.NewValue, rc.Cul);
            }
            catch (Exception /*ex*/) { }

            if (resourceValue != null)
            {
                cctrl.SetValue(FrameworkElement.ToolTipProperty, resourceValue);

                if (!_toolTipObjects.Contains(obj))
                {
                    _toolTipObjects.Add(obj);
                }
            }
        }

        private static void OnTitleIDChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            DependencyObject cctrl = obj;

            string resourceValue = null;

            try
            {
                ResourceAndCulture rc = new ResourceAndCulture();
                resourceValue = rc.Res.GetString((string)e.NewValue, rc.Cul);
            }
            catch (Exception /*ex*/) { }

            if (resourceValue != null)
            {
                cctrl.SetValue(Window.TitleProperty, resourceValue);

                if (!_titleObjects.Contains(obj))
                {
                    _titleObjects.Add(obj);
                }
            }
        }

        private static void OnWatermarkIDChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            DependencyObject cctrl = obj;

            string resourceValue = null;

            try
            {
                ResourceAndCulture rc = new ResourceAndCulture();
                resourceValue = rc.Res.GetString((string)e.NewValue, rc.Cul);
            }
            catch (Exception /*ex*/) { }

            if (resourceValue != null)
            {
                if (!_waterMarkObjects.Contains(obj))
                {
                    _waterMarkObjects.Add(obj);
                }
            }
        }

        public static object GetValue(DependencyObject d)
        {
            return d.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject d, object value)
        {
            d.SetValue(ValueProperty, value);
        }

        public static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is DataTrigger trigger)
            {
                trigger.Value = args.NewValue;
            }
        }

        public static DependencyProperty ContentIDProperty = DependencyProperty.RegisterAttached("ContentID",
            typeof(string), typeof(LocalizationProvider), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange,
                new PropertyChangedCallback(OnContentIDChanged)));

        public static DependencyProperty ToolTipIDProperty = DependencyProperty.RegisterAttached("ToolTipID",
            typeof(string), typeof(LocalizationProvider), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange,
                new PropertyChangedCallback(OnToolTipIDChanged)));

        public static DependencyProperty TitleIDProperty = DependencyProperty.RegisterAttached("TitleID",
            typeof(string), typeof(LocalizationProvider), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange,
                new PropertyChangedCallback(OnTitleIDChanged)));

        public static DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value",
            typeof(object), typeof(LocalizationProvider), new FrameworkPropertyMetadata(null, OnValueChanged));
    }
}
