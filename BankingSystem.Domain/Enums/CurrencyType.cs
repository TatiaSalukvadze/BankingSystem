using System.Text.Json.Serialization;

namespace BankingSystem.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CurrencyType { GEL = 1, USD, EUR };
}
