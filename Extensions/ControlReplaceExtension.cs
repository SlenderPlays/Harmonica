using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Harmonica.Extensions
{
	public static class ControlReplaceExtension
	{
		public static void ReplaceClass(this IControl target, string originalClass, string newClass)
		{
			if (target.Classes.Any(x => x == originalClass))
			{
				target.Classes.Remove(originalClass);
				target.Classes.Add(newClass);
			}
		}

		public static bool TryReplaceClass(this IControl target, string originalClass, string newClass)
		{
			bool anyMatch = false;
			if (target.Classes.Any(x => x == originalClass))
			{
				target.Classes.Remove(originalClass);
				target.Classes.Add(newClass);

				anyMatch = true;
			}

			return anyMatch;
		}
	}
}
