using JikanDotNet.Exceptions;
using System;
using System.Text.RegularExpressions;

namespace JikanDotNet.Helpers
{
	internal static class Guard
	{
		private static readonly Regex DateRegex = new(@"^\d{4}(-\d{2}){0,2}$",
			RegexOptions.Compiled | RegexOptions.CultureInvariant);
		
		internal static void IsDefaultEndpoint(string endpoint, string methodName)
		{
			if (endpoint.Equals(DefaultHttpClientProvider.DefaultEndpoint))
			{
				throw new NotSupportedException($"Operation {methodName} is not available on the default endpoint.");
			}
		}

		internal static void IsNotNullOrWhiteSpace(string arg, string argumentName)
		{
			if (string.IsNullOrWhiteSpace(arg))
			{
				throw new JikanValidationException("Argument can't be null or whitespace.", argumentName);
			}
		}

		internal static void IsNotNull(object arg, string argumentName)
		{
			if (arg == null)
			{
				throw new JikanValidationException("Argument can't be a null.", argumentName);
			}
		}

		internal static void IsLongerThan2Characters(string arg, string argumentName)
		{
			if (string.IsNullOrWhiteSpace(arg) || arg.Length < 3)
			{
				throw new JikanValidationException("Argument must be at least 3 characters long", argumentName);
			}
		}

		internal static void IsGreaterThanZero(long arg, string argumentName)
		{
			if (arg < 1)
			{
				throw new JikanValidationException("Argument must be a natural number greater than 0.", argumentName);
			}
		}

		public static void IsGreaterThanOrEqual(long arg, long min, string argumentName)
		{
			if (arg < min)
			{
				throw new JikanValidationException($"Argument must not be less than {min}.", argumentName);
			}
		}
		
		internal static void IsLessThanOrEqual(long arg, long max, string argumentName)
		{
			if (arg > max)
			{
				throw new JikanValidationException($"Argument must not be greater than {max}.", argumentName);
			}
		}

		internal static void IsValid<T>(Func<T, bool> isValidFunc, T arg, string argumentName, string message = null)
		{
			if (isValidFunc(arg))
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(message))
			{
				message = "Argument is not valid.";
			}

			throw new JikanValidationException(message, argumentName);
		}

		internal static void IsValidEnum<TEnum>(TEnum arg, string argumentName) where TEnum : struct, Enum
		{
			if (!Enum.IsDefined(typeof(TEnum), arg))
			{
				throw new JikanValidationException("Enum value must be valid", argumentName);
			}
		}
		
		internal static void IsLetter(char character, string argumentName)
		{
			if (!char.IsLetter(character))
			{
				throw new JikanValidationException("Character must be a letter", argumentName);
			}
		}

		public static void IsValidDate(string date, string argumentName)
		{
			if (!DateRegex.IsMatch(date))
			{
				throw new JikanValidationException("Date must be represented in one of the available formats", argumentName);
			}
		}
	}
}