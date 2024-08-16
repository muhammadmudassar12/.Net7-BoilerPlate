using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PushNotifications.Interfaces;
using PushNotifications.Serialization;
using PushNotifications.Utils;

namespace PushNotifications.Firebase
{
    /// <summary>
    /// Firebase message sender
    /// </summary>
    public class FirebaseSender : IFirebaseSender
    {
        private readonly HttpClient _httpClient;
        private readonly FirebaseSettings _settings;
        private readonly IJsonSerializer _serializer;

        private DateTime? _firebaseTokenExpiration;
        private FirebaseTokenResponse _firebaseToken;

        public FirebaseSender(FirebaseSettings settings, HttpClient httpClient, IJsonSerializer serializer)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            if (string.IsNullOrWhiteSpace(_settings.ClientEmail) ||
                string.IsNullOrWhiteSpace(_settings.PrivateKey) ||
                string.IsNullOrWhiteSpace(_settings.ProjectId) ||
                string.IsNullOrWhiteSpace(_settings.TokenUri))
            {
                throw new ArgumentException("Some settings are not defined", nameof(settings));
            }
        }

        public async Task<FirebaseResponse> SendAsync(object payload, FirebaseSettings settings, CancellationToken cancellationToken = default)
        {
            var json = _serializer.Serialize(payload);

            using var message = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://fcm.googleapis.com/v1/projects/{settings.ProjectId}/messages:send");

            var token = await GetJwtTokenAsync();

            message.Headers.Add("Authorization", $"Bearer {token}");
            message.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(message, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Firebase notification error: " + responseString);
            }

            return _serializer.Deserialize<FirebaseResponse>(responseString);
        }

        private async Task<string> GetJwtTokenAsync()
        {
            if (_firebaseToken != null && _firebaseTokenExpiration > DateTime.UtcNow)
            {
                return _firebaseToken.AccessToken;
            }

            using var message = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
            using var form = new MultipartFormDataContent();
            var authToken = GetMasterToken();
            form.Add(new StringContent(authToken), "assertion");
            form.Add(new StringContent("urn:ietf:params:oauth:grant-type:jwt-bearer"), "grant_type");
            message.Content = form;

            using var response = await _httpClient.SendAsync(message);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Firebase error when creating JWT token: " + content);
            }

            _firebaseToken = _serializer.Deserialize<FirebaseTokenResponse>(content);
            _firebaseTokenExpiration = DateTime.UtcNow.AddSeconds(_firebaseToken.ExpiresIn - 10);

            if (string.IsNullOrWhiteSpace(_firebaseToken.AccessToken) || _firebaseTokenExpiration < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Couldn't deserialize firebase token response");
            }

            return _firebaseToken.AccessToken;
        }

        private string GetMasterToken()
        {
            var header = _serializer.Serialize(new { alg = "RS256", typ = "JWT" });
            var payload = _serializer.Serialize(new
            {
                iss = _settings.ClientEmail,
                aud = _settings.TokenUri,
                scope = "https://www.googleapis.com/auth/firebase.messaging",
                iat = CryptoHelper.GetEpochTimestamp(),
                exp = CryptoHelper.GetEpochTimestamp() + 3600 /* has to be short lived */
            });

            var headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
            var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
            var unsignedJwtData = $"{headerBase64}.{payloadBase64}";
            var unsignedJwtBytes = Encoding.UTF8.GetBytes(unsignedJwtData);

            using var rsa = RSA.Create();
            rsa.ImportFromPem(_settings.PrivateKey.ToCharArray());

            var signature = rsa.SignData(unsignedJwtBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var signatureBase64 = Convert.ToBase64String(signature);

            return $"{unsignedJwtData}.{signatureBase64}";
        }
    }
}
