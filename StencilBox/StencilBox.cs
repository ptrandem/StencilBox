namespace StencilBox
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Text.RegularExpressions;

	[Flags]
	public enum ProcessFlags
	{
		None = 0,
		
		/// <summary>
		/// Force the template processor to case-sensitive token matching.
		/// Default processing is case-insensitive. 
		/// </summary>
		CaseSensitive = 1,
		
		/// <summary>
		/// Force the template processor to remove un-replaced tokens at the end of processing.
		/// Default processing is to leave unprocessed tokens.
		/// </summary>
		CleanUnprocessed = 2
	}

	/// <summary>
	/// Uses simple template syntax:
	///		[~NameOfProperty]
	///		[~NameOfProperty:optionalmodifier]
	/// 
	/// For manual replacements (key token is replaced with value):
	///		[m~key]
	/// </summary>
	public static class Stencil
	{
		// TODO: document this thing
        public static string Apply<T>(string template, T source, Dictionary<string, string> manualReplacements = null, ProcessFlags flags = ProcessFlags.None) where T : class
		{
			var output = template;

			if (source != null)
			{
				IEnumerable<PropertyInfo> properties =
					typeof(T).GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance);

				foreach (var info in properties)
				{
					var token = string.Format(@"\[\~({0}):?(.*?)\]", info.Name);
					var value = info.GetValue(source, null);
				    if (value == null)
				    {
				        continue;
				    }

					output = Regex.Replace(
						output,
						token,
						(match) =>
							{
								var style = match.Groups[2];
								if (style.Success && style.Captures.Count > 0)
								{
                                    return GetModifiedValue(value, style.Captures[0].ToString());
								}
                                return value.ToString();
							},
							((flags & ProcessFlags.CaseSensitive) > 0) ? RegexOptions.None : RegexOptions.IgnoreCase);
				}
			}

			if (manualReplacements != null)
			{
				foreach (var key in manualReplacements.Keys)
				{
					//TODO: unviolate DRY
					var token = string.Format(@"\[\~({0}):?(.*?)\]", key);
					var value = manualReplacements[key];
					output = Regex.Replace(
						output,
						token,
						(match) =>
							{
								Group style = match.Groups[2];
								if (style.Success && style.Captures.Count > 0)
								{
									return GetModifiedValue(value, style.Captures[0].ToString());
								}
								return value;
							},
						((flags & ProcessFlags.CaseSensitive) > 0) ? RegexOptions.None : RegexOptions.IgnoreCase);
				}
			}

			if ((flags & ProcessFlags.CleanUnprocessed) > 0)
			{
				output = Regex.Replace(output, @"\[\~(.*?):?(.*?)\]", "");
			}

			return output;
		}

		internal static string GetModifiedValue(object value, string tokenModKey)
		{
		    if (value == null || (value is DateTime && ((DateTime)value) == DateTime.MinValue))
		    {
		       return string.Empty;
		    }

			switch (tokenModKey.ToLower())
			{
				case "codify":
					// Remove spaces
					var tempValue = value.ToString().Replace(" ", "");
					
					// Remove any characters not alpha-numeric
					tempValue = Regex.Replace(tempValue, "[^a-zA-Z0-9].*?", "");

					// Remove any numbers at the beginning of the string
					tempValue = Regex.Replace(tempValue, "^[0-9]+?", "", RegexOptions.Multiline);

					// Uppercase the first letter
					tempValue = Regex.Replace(tempValue, "^.", m => m.ToString().ToUpper(), RegexOptions.Multiline);
					return tempValue;
					break;

				case "c-comment":
					return string.Format("// {0}", value);
					break;

				case "lowercase":
					return value.ToString().ToLower();
					break;

				case "uppercase":
					return value.ToString().ToUpper();
					break;

                default:
			        var toStringMethod = value.GetType().GetMethod("ToString", new[] { typeof(string) });
			        if (toStringMethod != null)
			        {
			            return (string)toStringMethod.Invoke(value, new[] { tokenModKey });
			        }
			        break;
			}

			return value.ToString();
		}
	}
}
