using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CompanyAndContactManagement.HttpApi.Services;

public interface IFilters
{
    public IEnumerable<(string Key, List<FilterMetadata> Value)> ToKeyValuePairs() =>
        GetType()
            .GetProperties()
            .Select(p =>
            {
                var name = p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? p.Name;
                var rawValue = p.GetValue(this);

                // Use the 'as' operator to safely cast the value to List<FilterMetadata>
                
                var value = new List<FilterMetadata>();

                if (p.GetValue(this) is Dictionary<string,object>)
                {
                    value =new List<FilterMetadata>();
                }
                else
                {
                    value = (List<FilterMetadata>) p.GetValue(this);
                }

                

                // If 'value' is null, handle the case where the value is not a List<FilterMetadata>
                if (value == null)
                {
                    // Handle the case where 'rawValue' is not a List<FilterMetadata>
                    // You can log an error or take appropriate action here.
                }

                return (name, value);
                
            });
    
    public object this[string propertyName]
    {
        get => GetType().GetProperties()
            .FirstOrDefault(p => (p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? p.Name).Equals(propertyName))?.GetValue(this, null);
        set => GetType().GetProperties()
            .FirstOrDefault(p => (p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? p.Name).Equals(propertyName))?.SetValue(this, value, null);
    }
}
public interface IFiltersWithPlaceFilter : IFilters
{
    public List<FilterMetadata> PlaceFilter { get; set; }
}

public class FiltersConverter<T> : TypeConverter where T : IFilters, new()
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string casted)
        {
            var result = new T();
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(casted);
            foreach (var key in dictionary.Keys)
            {
                    result[key] = dictionary[key].ValueKind switch
                    {
                        JsonValueKind.Array => dictionary[key].Deserialize<List<FilterMetadata>>(),
                        _ => new List<FilterMetadata> {dictionary[key].Deserialize<FilterMetadata>()}
                    };
            }
            return result;
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
        Type destinationType)
    {
        return destinationType == typeof(string) && value is T casted
            ? string.Join("", casted.ToString())
            : base.ConvertTo(context, culture, value, destinationType);
    }
}

[JsonConverter(typeof(FilterMetadataJsonConverter))]
public class FilterMetadata
{
    public List<dynamic> Value { get; set; } = new();
    public FilterMatchMode MatchMode { get; set; } = FilterMatchMode.Equals;
    public FilterOperator Operator { get; set; } = FilterOperator.And;
}

public class FilterMetadataJsonConverter : JsonConverter<FilterMetadata>
{
    public override FilterMetadata Read(ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
        }

        var result = new FilterMetadata();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return result;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("JsonTokenType was not PropertyName");
            }

            var propertyName = reader.GetString();

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new JsonException("Failed to get property name");
            }

            reader.Read();
            switch (propertyName.ToPascalCase())
            {
                case nameof(FilterMetadata.MatchMode):
                    result.MatchMode = (FilterMatchMode)ExtractValue(ref reader, options)!;
                    break;
                case nameof(FilterMetadata.Operator):
                    result.Operator = (FilterOperator)ExtractValue(ref reader, options)!;
                    break;
                case nameof(FilterMetadata.Value):
                    var rawValue = ExtractValue(ref reader, options);
                    result.Value = rawValue != null && rawValue.GetType().IsGenericList()
                        ? (List<dynamic>)rawValue
                        : new List<dynamic> { rawValue };
                    break;
            }
        }

        return result;
    }

    private object ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                

                if (reader.TryGetGuid(out var guid))
                {
                    return guid;
                }

                

                return reader.GetString();
            case JsonTokenType.False:
                return false;
            case JsonTokenType.True:
                return true;
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out var int64))
                {
                    return int64;
                }
                return reader.GetDecimal();
            case JsonTokenType.StartObject:
                return Read(ref reader, null!, options);
            case JsonTokenType.StartArray:
                var list = new List<dynamic>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    list.Add(ExtractValue(ref reader, options));
                }

                return list;
            default:
                throw new JsonException($"'{reader.TokenType}' is not supported");
        }
    }
    
    private static void HandleValue(Utf8JsonWriter writer, string key, object objectValue)
    {
        if (key != null)
        {
            writer.WritePropertyName(key);
        }

        switch (objectValue)
        {
            case string stringValue:
                writer.WriteStringValue(stringValue);
                break;
            case DateTime dateTime:
                writer.WriteStringValue(dateTime);
                break;
            case long longValue:
                writer.WriteNumberValue(longValue);
                break;
            case int intValue:
                writer.WriteNumberValue(intValue);
                break;
            case float floatValue:
                writer.WriteNumberValue(floatValue);
                break;
            case double doubleValue:
                writer.WriteNumberValue(doubleValue);
                break;
            case decimal decimalValue:
                writer.WriteNumberValue(decimalValue);
                break;
            case bool boolValue:
                writer.WriteBooleanValue(boolValue);
                break;
            case Dictionary<string, object> dict:
                writer.WriteStartObject();
                foreach (var item in dict)
                {
                    HandleValue(writer, item.Key, item.Value);
                }
                writer.WriteEndObject();
                break;
            case object[] array:
                writer.WriteStartArray();
                foreach (var item in array)
                {
                    HandleValue(writer, null, item);
                }
                writer.WriteEndArray();
                break;
            case List<object> list:
                writer.WriteStartArray();
                foreach (var item in list)
                {
                    HandleValue(writer, null, item);
                }
                writer.WriteEndArray();
                break;
            default:
                writer.WriteNullValue();
                break;
        }
    }

    public override void Write(Utf8JsonWriter writer,
        FilterMetadata value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var property in value.GetType().GetProperties())
        {
            HandleValue(writer, property.Name.ToCamelCase(), property.GetValue(value));
        }
        writer.WriteEndObject();
    }
}

public enum FilterMagicValue
{
    [Description("\0")] Null,
}
public enum FilterOperator
{
    And,
    Or
}

public enum FilterMatchMode
{
    [Description("startsWith")] StartsWith,
    [Description("contains")] Contains,
    [Description("notContains")] NotContains,
    [Description("endsWith")] EndsWith,
    [Description("equals")] Equals,
    [Description("notEquals")] NotEquals,
    [Description("in")] In,
    [Description("lt")] LessThan,
    [Description("lte")] LessThanOrEqualTo,
    [Description("gt")] GreaterThan,
    [Description("gte")] GreaterThanOrEqualTo,
    [Description("between")] Between,
    [Description("is")] Is,
    [Description("isNot")] IsNot,
    [Description("before")] Before,
    [Description("after")] After,
    [Description("dateIs")] DateIs,
    [Description("dateIsNot")] DateIsNot,
    [Description("dateBefore")] DateBefore,
    [Description("dateAfter")] DateAfter
}
public static class FilterExtensions
{
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Remove any non-alphanumeric characters and split by whitespace
        string[] words = Regex.Split(input, @"\W+");

        // Combine the words, capitalizing the first letter of each word except the first one
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        for (int i = 0; i < words.Length; i++)
        {
            if (i == 0)
            {
                words[i] = words[i].ToLower();
            }
            else
            {
                words[i] = textInfo.ToTitleCase(words[i].ToLower());
            }
        }

        // Join the words without spaces
        return string.Join("", words);
    }
    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Remove any non-alphanumeric characters and split by whitespace
        string[] words = Regex.Split(input, @"\W+");

        // Combine the words, capitalizing the first letter of each word
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = textInfo.ToTitleCase(words[i].ToLower());
        }

        // Join the words without spaces
        return string.Join("", words);
    }
    public static bool IsGenericList(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }
    public static bool GetPlaceFilter<T>(this T filters, out T result) where T:IFiltersWithPlaceFilter
    {
        result = filters;
        var placeFilter = true;
        if (!filters!.PlaceFilter.IsNullOrEmpty() && filters!.PlaceFilter.First().Value.Count > 0)
        {
            placeFilter = filters!.PlaceFilter.First().Value.First();
            result!.PlaceFilter = new List<FilterMetadata>();
        }

        return placeFilter;
    }
    public static bool IsNullOrEmpty<T>(this List<T> list)
    {
        return list == null || list.Count == 0;
    }
}