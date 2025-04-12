using System.Text.Json.Serialization;

namespace BankingSystem.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CurrencyType { GEL, USD, EUR };
}
