using System.Text.Json.Serialization;

namespace CorePush.Firebase;

public record FirebaseSettings(
    [property: JsonPropertyName("ProjectId")] string ProjectId,
    [property: JsonPropertyName("PrivateKey")] string PrivateKey,
    [property: JsonPropertyName("ClientEmail")] string ClientEmail,
    [property: JsonPropertyName("TokenUri")] string TokenUri
);