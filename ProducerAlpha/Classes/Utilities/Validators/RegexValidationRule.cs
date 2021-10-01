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
	public class RegexValidationRule : ValidationRule
	{

		private string errorMessage;
		private RegexOptions regexOptions = RegexOptions.None;
		private string regexText;


		public RegexValidationRule()
		{
		}

		public RegexValidationRule( string regexText )
		{
			this.RegexText = regexText;
		}

		public RegexValidationRule( string regexText, string errorMessage )
			: this( regexText )
		{
			this.RegexOptions = regexOptions;
		}

		public RegexValidationRule( string regexText, string errorMessage, RegexOptions regexOptions )
			: this( regexText )
		{
			this.RegexOptions = regexOptions;
		}

		public string ErrorMessage
		{
			get { return this.errorMessage; }
			set { this.errorMessage = value; }
		}

		public RegexOptions RegexOptions
		{
			get { return regexOptions; }
			set { regexOptions = value; }
		}

		public string RegexText
		{
			get { return regexText; }
			set { regexText = value; }
		}


		public override ValidationResult Validate( object value, CultureInfo cultureInfo )
		{
			ValidationResult result = ValidationResult.ValidResult;

			// If there is no regular expression to evaluate,
			// then the data is considered to be valid.
			if( ! String.IsNullOrEmpty( this.RegexText ) )
			{
				// Cast the input value to a string (null becomes empty string).
				string text = value as string ?? String.Empty;

				// If the string does not match the regex, return a value
				// which indicates failure and provide an error mesasge.
				if( ! Regex.IsMatch( text, this.RegexText, this.RegexOptions ) )
					result = new ValidationResult( false, this.ErrorMessage );
			}

			return result;
		}

	}
}