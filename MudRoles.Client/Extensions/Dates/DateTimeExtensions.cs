using System.Globalization;

namespace MudRoles.Client.Extensions.Dates
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a DateTime object to a formatted string.
        /// </summary>
        /// <param name="dateTime">The DateTime object to format.</param>
        /// <param name="format">The desired format of the output string. Default is "MMMM dd, yyyy".</param>
        /// <returns>A formatted date and/or time string.</returns>
        public static string ToFormattedString(this DateTime dateTime, string format = "MMMM dd, yyyy")
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Calculates the time span between two DateTime objects and formats it as months, days, and hours.
        /// </summary>
        /// <param name="endDate">The end DateTime object.</param>
        /// <param name="startDate">The start DateTime object.</param>
        /// <returns>A formatted string representing the time span in months, days, and hours.</returns>
        public static string ToTimeSpanString(this DateTime endDate, DateTime startDate)
        {
            TimeSpan timeSpan = endDate - startDate;

            int totalDays = (int)timeSpan.TotalDays;
            int months = totalDays / 30;
            int days = totalDays % 30;
            int hours = timeSpan.Hours;

            return $"{months} months, {days} days, {hours} hours";
        }
    }

}
