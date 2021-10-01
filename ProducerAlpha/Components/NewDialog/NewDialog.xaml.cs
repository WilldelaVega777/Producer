using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.IO;
using Ookii.Dialogs.Wpf;

namespace ProducerAlpha
{
	/// <summary>
	/// Interaction logic for NewDialog.xaml
	/// </summary>
	public partial class NewDialog
	{
        //---------------------------------------------------------------------------------
        // Private Fields Section
        //---------------------------------------------------------------------------------
        private bool bErrorMessageIsVisible;
        private string projectLocation;
        private string projectName;
        private string showTitle;
        private string showAuthor;


        //---------------------------------------------------------------------------------
        // Routed Events Section
        //---------------------------------------------------------------------------------
        public static readonly RoutedEvent CloseButtonClickEvent = EventManager.RegisterRoutedEvent(
            "CloseButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewDialog));

        public static readonly RoutedEvent BrowseButtonClickEvent = EventManager.RegisterRoutedEvent(
            "BrowseButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewDialog));

        public static readonly RoutedEvent OKButtonClickEvent = EventManager.RegisterRoutedEvent(
            "OKButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewDialog));

        public static readonly RoutedEvent CancelButtonClickEvent = EventManager.RegisterRoutedEvent(
            "CancelButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NewDialog));


        //---------------------------------------------------------------------------------
        public event RoutedEventHandler CloseButtonClick
        {
            add { AddHandler(CloseButtonClickEvent, value); }
            remove { RemoveHandler(CloseButtonClickEvent, value); }
        }

        //---------------------------------------------------------------------------------
        public event RoutedEventHandler BrowseButtonClick
        {
            add { AddHandler(BrowseButtonClickEvent, value); }
            remove { RemoveHandler(BrowseButtonClickEvent, value); }
        }

        //---------------------------------------------------------------------------------
        public event RoutedEventHandler OKButtonClick
        {
            add { AddHandler(OKButtonClickEvent, value); }
            remove { RemoveHandler(OKButtonClickEvent, value); }
        }


        //---------------------------------------------------------------------------------
        public event RoutedEventHandler CancelButtonClick
        {
            add { AddHandler(CancelButtonClickEvent, value); }
            remove { RemoveHandler(CancelButtonClickEvent, value); }
        }

        //---------------------------------------------------------------------------------
        // Public Properties Section
        //---------------------------------------------------------------------------------
        public string ProjectLocation
        {
            get
            {
                return this.projectLocation;
            }

            set
            {
                this.txtLocation.Text = value;
                try
                {
                    System.IO.Directory.SetCurrentDirectory(this.projectLocation);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //---------------------------------------------------------------------------------
        public string ProjectName
        {
            get
            {
                return this.projectName;
            }

            set
            {
                this.projectName = value;
            }
        }

        //---------------------------------------------------------------------------------
        public string ShowTitle
        {
            get
            {
                return this.showTitle;
            }

            set
            {
                this.showTitle = value;
            }
        }

        //---------------------------------------------------------------------------------
        public string ShowAuthor
        {
            get
            {
                return this.showAuthor;
            }

            set
            {
                this.showAuthor = value;
            }
        }

        //---------------------------------------------------------------------------------
        // Constructor Method Section
        //---------------------------------------------------------------------------------
		public NewDialog()
		{
			this.InitializeComponent();
            this.DataContext = this;
            this.RegExValidation.RegexText = "[a-z]+";
            this.projectLocation = App.BaseProjectPath;            
		}


        //---------------------------------------------------------------------------------
        // Event Handlers Section
        //---------------------------------------------------------------------------------
        private void NewDialogControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtAuthor.Text = "Carl Carpenter";
            Keyboard.Focus(this.txtProjectName);
        }

        //---------------------------------------------------------------------------------
        private void cmdClose_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CloseButtonClickEvent));
        }

        //---------------------------------------------------------------------------------
        private void cmdBrowse_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Select Project Folder";
            dialog.UseDescriptionForTitle = true;
            dialog.SelectedPath = this.ProjectLocation + @"\";

            // Process Select File DialogBox results
            if ((bool)dialog.ShowDialog(this.Parent as frmMain))
            {
                this.ProjectLocation = dialog.SelectedPath;
                this.txtLocation.Text = (this.ProjectLocation + @"\" + this.ProjectName);
            }

            RaiseEvent(new RoutedEventArgs(BrowseButtonClickEvent));
        }

        //---------------------------------------------------------------------------------
        private void cmdOk_Click(object sender, RoutedEventArgs e)
        {
            if (!WayvFS.CheckIfProjectFileExists(this.ProjectLocation + @"\" + this.ProjectName + @"\"))
            {
                RaiseEvent(new RoutedEventArgs(OKButtonClickEvent));
            }
            else
            {
                this.ShowErrorMessage(Properties.Resources.AnotherProjectInFolderMessage);
            }
        }


        //---------------------------------------------------------------------------------
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CancelButtonClickEvent));
        }

        //---------------------------------------------------------------------------------
        private void txtProjectName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.bErrorMessageIsVisible)
            {
                this.HideErrorMessage();
            }

            this.ProjectName = this.txtProjectName.Text;
            this.txtTitle.Text = this.ProjectName;
            this.txtLocation.Text = (this.ProjectLocation + @"\" + this.ProjectName);
            this.ValidateIfOk();
        }

        //---------------------------------------------------------------------------------
        private void txtAuthor_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.showAuthor = this.txtAuthor.Text;
            this.ValidateIfOk();
        }

        //---------------------------------------------------------------------------------
        private void txtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.showTitle = this.txtTitle.Text;
        }

        //---------------------------------------------------------------------------------
        // Public Methods Section
        //---------------------------------------------------------------------------------
        public void SetFocusOnProjectName()
        {
            Keyboard.Focus(this.txtProjectName);
        }

        //---------------------------------------------------------------------------------
        // Private Methods Section
        //---------------------------------------------------------------------------------
        private void ValidateIfOk()
        {
            ValidationResult OVR = this.RegExValidation.Validate(this.txtProjectName.Text, new CultureInfo("en-us"));

            if (OVR.IsValid)
            {
                if ((this.txtProjectName.Text != "") && (this.txtAuthor.Text != ""))
                {
                    this.cmdOk.IsEnabled = true;
                }
                else
                {
                    this.cmdOk.IsEnabled = false;
                }
            }
        }

        //---------------------------------------------------------------------------------
        private void ShowErrorMessage(string pMessage)
        {
            this.ErrorMessage.Text = pMessage;
            this.ErrorMessage.Visibility = Visibility.Visible;
            this.bErrorMessageIsVisible = true;
        }

        //---------------------------------------------------------------------------------
        private void HideErrorMessage()
        {
            this.ErrorMessage.Visibility = Visibility.Hidden;
            this.bErrorMessageIsVisible = false;
        }

	}
}