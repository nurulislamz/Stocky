using System.Text.Json;
using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// Shared base
// ------------------------------------------------------------
public abstract class YahooFinanceDto
{
    // Capture any fields yahoo-finance2 returns that we haven't explicitly modeled.
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}

public sealed class UnixSecondsDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());
        }

        if (reader.TokenType == JsonTokenType.String && long.TryParse(reader.GetString(), out var seconds))
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        }

        throw new JsonException("Expected unix timestamp in seconds.");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}

public sealed class UnixSecondsNullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());
        }

        if (reader.TokenType == JsonTokenType.String && long.TryParse(reader.GetString(), out var seconds))
        {
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        }

        throw new JsonException("Expected unix timestamp in seconds or null.");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteNumberValue(value.Value.ToUnixTimeSeconds());
    }
}

public sealed class UnixSecondsDateTimeOffsetListConverter : JsonConverter<List<DateTimeOffset>>
{
    public override List<DateTimeOffset> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return [];
        }

        var values = JsonSerializer.Deserialize<List<long>>(ref reader, options) ?? [];
        return values.Select(DateTimeOffset.FromUnixTimeSeconds).ToList();
    }

    public override void Write(Utf8JsonWriter writer, List<DateTimeOffset> value, JsonSerializerOptions options)
    {
        var seconds = value.Select(item => item.ToUnixTimeSeconds()).ToList();
        JsonSerializer.Serialize(writer, seconds, options);
    }
}

// Optional helper if you want to keep "raw/fmt" style values later.
// yahoo-finance2 often already normalizes values, but quoteSummary may include these.
public sealed class YahooValue<T> : YahooFinanceDto
{
    [JsonPropertyName("raw")]
    public T? Raw { get; set; }

    [JsonPropertyName("fmt")]
    public string? Fmt { get; set; }
}
