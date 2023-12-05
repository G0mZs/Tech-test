using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;

namespace CallDetailRecordAPI.Structure.Converters
{
    public class CustomDateTimeConverter : DateTimeConverter
    {
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
