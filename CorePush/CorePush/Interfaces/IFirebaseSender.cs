using System.Threading;
using System.Threading.Tasks;

using CorePush.Firebase;

namespace CorePush.Interfaces;

public interface IFirebaseSender
{
    Task<FirebaseResponse> SendAsync(object payload, FirebaseSettings settings, CancellationToken cancellationToken = default);
}