using System;
using System.Windows;
using System.Windows.Input;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Resources;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using TimelineController.Components;

namespace ProducerAlpha
{
	public partial class App : Application
	{
        //--------------------------------------------------------------------------------------------
        // Internal Fields Section
        //--------------------------------------------------------------------------------------------
        internal const bool TRACE_MODE = true;
        
        internal const string ApplicationFolderName = "ProducerAlpha";
        internal readonly static string BaseProjectPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine(App.ApplicationFolderName, @"Projects"));
        internal readonly static string BaseSettingsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.Combine(App.ApplicationFolderName, @"Settings"));

        internal static double currentZoom = 1.0;

        public static MediaElement PreviewWindow;
        public static Storyboard VideoController;
        public static SLTimelineController TimelineController;

        //--------------------------------------------------------------------------------------------
        // Private Fields Section
        //--------------------------------------------------------------------------------------------
        private readonly static string RecentFilesFilePath = App.BaseSettingsPath + @"\" + "RecentProjects.xml";
        private const int NumberOfRecentFiles = 5;
        private static StringCollection recentProjects = new StringCollection();

        //--------------------------------------------------------------------------------------------
        // Overrides (Application Events) Section
        //--------------------------------------------------------------------------------------------
        protected override void OnStartup(StartupEventArgs e)
        {
            // Tests for Project Directory and if it is not there, create it
            if (!File.Exists(App.BaseProjectPath))
            {
                Directory.CreateDirectory(App.BaseProjectPath);
            }

            // Tests for Settings Directory and if it is not there, create it
            if (!File.Exists(App.BaseSettingsPath))
            {
                Directory.CreateDirectory(App.BaseSettingsPath);
            }


            // Load the collection of recent files.
            LoadRecentFiles();

            // Loads App.Settings
            Properties.Settings appSettings = ProducerAlpha.Properties.Settings.Default;

            // Add properties to XAML Application.Resources
            Type resType = typeof(ProducerAlpha.Properties.Resources);
            PropertyInfo[] properties = resType.GetProperties(BindingFlags.Static | BindingFlags.NonPublic);
            foreach( PropertyInfo property in properties )
            {
                if (property.PropertyType == typeof(string))
                {
                    this.Resources.Add(property.Name, property.GetValue(null, null));
                }
            }

            // Goes to Base Action
            base.OnStartup(e);
        }

        //--------------------------------------------------------------------------------------------
        protected override void OnExit(ExitEventArgs e)
        {
            SaveRecentFiles();
            base.OnExit(e);
        }

        //--------------------------------------------------------------------------------------------
        // Public Methods Section
        //--------------------------------------------------------------------------------------------
        public static StringCollection RecentProjects
        {
            get { return recentProjects; }
        }

        //--------------------------------------------------------------------------------------------
        public static void LoadRecentFiles()
        {
            if (File.Exists(RecentFilesFilePath))
            {
                // Load the Recent Files from disk
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(StringCollection));
                using (TextReader reader = new StreamReader(RecentFilesFilePath))
                {
                    recentProjects = (StringCollection)ser.Deserialize(reader);
                }

                // Remove files from the Recent Files list that no longer exists.
                for (int i = 0; i < recentProjects.Count; i++)
                {
                    if (!File.Exists(recentProjects[i]))
                        recentProjects.RemoveAt(i);
                }

                // Only keep the 5 most recent files, trim the rest.
                while (recentProjects.Count > NumberOfRecentFiles)
                    recentProjects.RemoveAt(NumberOfRecentFiles);
            }
        }

        //--------------------------------------------------------------------------------------------
        public static void SaveRecentFiles()
        {
            XmlSerializer ser = new XmlSerializer(typeof(StringCollection));
            if (!File.Exists(RecentFilesFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(RecentFilesFilePath));
            }
            using (TextWriter writer = new StreamWriter(RecentFilesFilePath))
            {
                ser.Serialize(writer, recentProjects);
            }
        }

	}
}
