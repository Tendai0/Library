using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryServices
{
    public class DataHelpers
    {
        public static List<string> HumanizeHours(IEnumerable<BranchHours> branchHours)
        {
            var hours = new List<string>();

            foreach(var time in branchHours)
            {
                var day = HumanizeDay(time.DayOfWeek);
                var openTime = HumanizeTime(time.OpenTime);
                var closeTime = HumanizeTime(time.CloseTime);

                var timeEntry = $"{day}{openTime} to {closeTime}";
                hours.Add(timeEntry);
            }

            return hours;

        }

        private static object HumanizeTime(int Time)
        {
            return TimeSpan.FromHours(Time);
        }

        private static object HumanizeDay(int number)
        {
            return Enum.GetName(typeof(DayOfWeek), number);
        }
    }
}
