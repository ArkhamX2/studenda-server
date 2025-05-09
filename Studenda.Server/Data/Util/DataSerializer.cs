using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Studenda.Server.Data.Util;

/// <summary>
///     Класс-обертка для сериализации данных в формат JSON.
/// </summary>
public static class DataSerializer
{
    /// <summary>
    ///     Сериализовать объект в строку JSON.
    /// </summary>
    /// <param name="rawData">Объект.</param>
    /// <returns>Строка JSON.</returns>
    public static object Serialize(object? rawData)
    {
        var json = JsonConvert.SerializeObject(rawData, Configuration);

        return System.Text.Json.JsonSerializer.Deserialize<object>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to deserialize JSON.");
    }

    /// <summary>
    ///     Десериализовать строку JSON в объект указанного типа данных.
    /// </summary>
    /// <param name="serializedData">Строка JSON.</param>
    /// <typeparam name="T">Выходной тип данных.</typeparam>
    /// <returns>Объект указанного выходного типа данных.</returns>
    public static T? Deserialize<T>([StringSyntax("Json")] string serializedData)
    {
        return JsonConvert.DeserializeObject<T>(serializedData, Configuration);
    }

    /// <summary>
    ///     Конфигурация.
    /// </summary>
    private static JsonSerializerSettings Configuration { get; } = new()
    {
        PreserveReferencesHandling = PreserveReferencesHandling.All,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };
}