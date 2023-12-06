using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;

namespace CallDetailRecordAPI.Structure.Converters
{
    /// <summary>Represents a custom date time converter.</summary>
    public class CustomDateTimeConverter : DateTimeConverter
    {
        /// <summary>Converts a date time to the format "dd/MM/yyyy".</summary>
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (DateTime.TryParseExact(text, "dd/MM/yyyy", null, DateTimeStyles.None, out var result))
            {
                return result;
            }

            return base.ConvertFromString(text, row, memberMapData);
        }
    }
}
