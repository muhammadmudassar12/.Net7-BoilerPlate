
using IronBarCode;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ZXing;

namespace EMS20.WebApi.Application.Common.Auxillary;
public static class AuxillaryService
{
    public static byte[] GenerateQrCode(DefaultIdType Id, string qrcodeString)
    {

        QRCodeWriter.CreateQrCode(qrcodeString, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium).SaveAsPng($"{Id}.png");
        byte[] qrCodeBytes = File.ReadAllBytes($"{Id}.png");
        File.Delete($"{Id}.png");
        return qrCodeBytes;
    }

    public static CommonRequest GetHeaders(IHeaderDictionary headers)
    {
        var result = new CommonRequest();

        if (headers.ContainsKey("latitude") && double.TryParse(headers["latitude"], out var latitudeValue))
        {
            result.Latitude = latitudeValue;
        }

        if (headers.ContainsKey("longitude") && double.TryParse(headers["longitude"], out var longitudeValue))
        {
            result.Longitude = longitudeValue;
        }

        if (headers.ContainsKey("imei"))
        {
            result.Imei = headers["imei"];
        }

        if (headers.ContainsKey("os"))
        {
            result.Os = headers["os"];
        }

        if (headers.ContainsKey("device-name"))
        {
            result.DeviceName = headers["device-name"];
        }

        return result;
    }

    public static double CalculateDistance(GeoCoordinate source, GeoCoordinate destination)
    {
        double earthRadius = 6371;
        double lat1 = (double)(source.Latitude * Math.PI / 180.0);
        double lon1 = (double)source.Longitude * Math.PI / 180.0;
        double lat2 = (double)destination.Latitude * Math.PI / 180.0;
        double lon2 = (double)destination.Longitude * Math.PI / 180.0;

        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;

        double a = (Math.Sin(dLat / 2) * Math.Sin(dLat / 2)) +
                   (Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2));

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadius * c;
    }

}
