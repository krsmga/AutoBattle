using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AutoBattle
{
    public static class EnumExtesions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            string displayName = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();

            if (String.IsNullOrEmpty(displayName))
            {
                displayName = enumValue.ToString();
            }

            return displayName;
        }
    }
}

