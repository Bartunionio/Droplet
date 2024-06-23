using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Droplet.Helpers
{
    public static class EnumHelper
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
            where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enum");
            }

            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new
                         {
                             ID = e,
                             Name = GetEnumDescription(e as Enum)
                         };

            return new SelectList(values, "ID", "Name");
        }

        public static SelectList ToSelectList<T>() where T : Enum
        {
            var values = Enum.GetValues(typeof(T)).Cast<T>().Select(e => new
            {
                Value = e,
                Text = GetEnumDescription(e)
            }).ToList();

            return new SelectList(values, "Value", "Text");
        }

        private static string GetEnumDescription(Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return descriptionAttributes?.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
        }
    }
}