namespace PushNotifications.Serialization;

public interface IJsonSerializer
{
    string Serialize(object obj);
    TObject Deserialize<TObject>(string json);
}