using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ProducerAlpha
{

	public static class RegexValidator
	{


		static RegexValidator()
		{
			RegexTextProperty = DependencyProperty.RegisterAttached(
				"RegexText",
				typeof( string ),
				typeof( RegexValidator ),
				new UIPropertyMetadata( null, OnAttachedPropertyChanged ) );

			ErrorMessageProperty = DependencyProperty.RegisterAttached(
				"ErrorMessage",
				typeof( string ),
				typeof( RegexValidator ),
				new UIPropertyMetadata( null, OnAttachedPropertyChanged ) );
		}

		public static readonly DependencyProperty ErrorMessageProperty;

		public static string GetErrorMessage( TextBox textBox )
		{
			return textBox.GetValue( ErrorMessageProperty ) as string;
		}

		public static void SetErrorMessage( TextBox textBox, string value )
		{
			textBox.SetValue( ErrorMessageProperty, value );
		}

		public static readonly DependencyProperty RegexTextProperty;

		public static string GetRegexText( TextBox textBox )
		{
			return textBox.GetValue( RegexTextProperty ) as string;
		}

		public static void SetRegexText( TextBox textBox, string value )
		{
			textBox.SetValue( RegexTextProperty, value );
		}

		static void OnAttachedPropertyChanged( DependencyObject depObj, DependencyPropertyChangedEventArgs e )
		{
			TextBox textBox = depObj as TextBox;
			if( textBox == null )
				throw new InvalidOperationException(
					"The RegexValidator can only be used with a TextBox." );

			VerifyRegexValidationRule( textBox );
		}


		static RegexValidationRule GetRegexValidationRuleForTextBox( TextBox textBox )
		{
			if( ! textBox.IsInitialized )
			{
				// If the TextBox.Text property is bound, but the TextBox is not yet
				// initialized, the property's binding can be null.  In that situation,
				// hook its Initialized event and verify the validation rule again when 
				// that event fires.  At that point in time, the Text property's binding
				// will be non-null.
				EventHandler callback = null;
				callback = delegate
				{
					textBox.Initialized -= callback;
					VerifyRegexValidationRule( textBox );
				};
				textBox.Initialized += callback;
				return null;
			}

			// Get the binding expression associated with the TextBox's Text property.
			BindingExpression expression = textBox.GetBindingExpression( TextBox.TextProperty );
			if( expression == null )
				throw new InvalidOperationException(
					"The TextBox's Text property must be bound for the RegexValidator to validate it." );

			// Get the binding which owns the binding expression.
			Binding binding = expression.ParentBinding;
			if( binding == null )
				throw new ApplicationException(
					"Unexpected situation: the TextBox.Text binding expression has no parent binding." );

			// Look for an existing instance of the RegexValidationRule class in the
			// binding.  If there is more than one instance in the ValidationRules
			// then throw an exception because we don't know which one to modify.
			RegexValidationRule regexRule = null;
			foreach( ValidationRule rule in binding.ValidationRules )
			{
				if( rule is RegexValidationRule )
				{
					if( regexRule == null )
						regexRule = rule as RegexValidationRule;
					else
						throw new InvalidOperationException(
							"There should not be more than one RegexValidationRule in a Binding's ValidationRules." );
				}
			}

			// If the TextBox.Text property's binding does not yet have an 
			// instance of RegexValidationRule in its ValidationRules,
			// add an instance now and return it.
			if( regexRule == null )
			{
				regexRule = new RegexValidationRule();
				binding.ValidationRules.Add( regexRule );
			}

			return regexRule;
		}

		static void VerifyRegexValidationRule( TextBox textBox )
		{
			RegexValidationRule regexRule = GetRegexValidationRuleForTextBox( textBox );
			if( regexRule != null )
			{
				regexRule.RegexText = 
					textBox.GetValue( RegexValidator.RegexTextProperty ) as string;

				regexRule.ErrorMessage = 
					textBox.GetValue( RegexValidator.ErrorMessageProperty ) as string;
			}
		}

	}
}