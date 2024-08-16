using System.Threading;
using System.Threading.Tasks;
using PushNotifications.Firebase;

namespace PushNotifications.Interfaces;

public interface IFirebaseSender {
    Task<FirebaseResponse> SendAsync(object payload, FirebaseSettings settings, CancellationToken cancellationToken = default);
}