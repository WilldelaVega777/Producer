﻿#pragma checksum "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E24E4EE16AD00DD0D667E3C1C477C716E5608FC15274D12D5D3DA1AF48A7B1E4"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ProducerAlpha {
    
    
    /// <summary>
    /// ErrorReporter
    /// </summary>
    public partial class ErrorReporter : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutContent;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid IconPanel;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image ErrorIcon;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MessagePanel;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtErrorDetails;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer StackTraceViewer;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtErrorStackTrace;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cmdSave;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button cmdClose;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ProducerAlpha;component/components/errorreporter/errorreporter.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.LayoutContent = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.IconPanel = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.ErrorIcon = ((System.Windows.Controls.Image)(target));
            return;
            case 4:
            this.MessagePanel = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.txtErrorDetails = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.StackTraceViewer = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 7:
            this.txtErrorStackTrace = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.cmdSave = ((System.Windows.Controls.Button)(target));
            
            #line 79 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
            this.cmdSave.Click += new System.Windows.RoutedEventHandler(this.cmdSave_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.cmdClose = ((System.Windows.Controls.Button)(target));
            
            #line 91 "..\..\..\..\Components\ErrorReporter\ErrorReporter.xaml"
            this.cmdClose.Click += new System.Windows.RoutedEventHandler(this.cmdClose_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
