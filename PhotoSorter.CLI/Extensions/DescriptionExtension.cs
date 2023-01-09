using System.ComponentModel;
using System.Reflection;

namespace PhotoSorter.CLI.Extensions
{
    public static class DescriptionExtension
    {
        public static string GetDescription<T>(this T value) where T : struct
        {
            var valueName = value.ToString();
            var memberInfo = value.GetType().GetMember(valueName!);
            var attributes = memberInfo[0].GetCustomAttributes<DescriptionAttribute>(false).ToArray();
            return (attributes.Any()) ? attributes[0].Description : valueName!;
        }
    }
}
