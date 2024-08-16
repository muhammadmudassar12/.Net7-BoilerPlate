using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EMS20.WebApi.Infrastructure.OpenApi;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SwaggerHeaderAttribute : Attribute
{
    public string HeaderName { get; }
    public string? Description { get; }
    public string? DefaultValue { get; }
    public bool IsRequired { get; }

    public SwaggerHeaderAttribute(string headerName, string? description = null, string? defaultValue = null, bool isRequired = false)
    {
        HeaderName = headerName;
        Description = description;
        DefaultValue = defaultValue;
        IsRequired = isRequired;
    }
}

public class SwaggerDefaultHeaderAtttribute : Attribute
{
    public SwaggerDefaultHeaderAtttribute(double? latitude, double? longitude, string? imei, string? os, string? deviceName)
    {
        Latitude = latitude;
        Longitude = longitude;
        Imei = imei;
        Os = os;
        DeviceName = deviceName;
    }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Imei { get; set; }
    public string? Os { get; set; }
    public string? DeviceName { get; set; }

}

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var defaultHeaderAttributes = context.MethodInfo.GetCustomAttributes(true)
            .OfType<SwaggerDefaultHeaderAtttribute>()
            .FirstOrDefault();

        if (defaultHeaderAttributes != null)
        {
            // Add or update the headers in the Swagger operation
            AddOrUpdateHeader(operation.Parameters, "Latitude", defaultHeaderAttributes.Latitude);
            AddOrUpdateHeader(operation.Parameters, "Longitude", defaultHeaderAttributes.Longitude);
            AddOrUpdateHeader(operation.Parameters, "Imei", defaultHeaderAttributes.Imei);
            AddOrUpdateHeader(operation.Parameters, "Os", defaultHeaderAttributes.Os);
            AddOrUpdateHeader(operation.Parameters, "DeviceName", defaultHeaderAttributes.DeviceName);
        }
    }

    private void AddOrUpdateHeader(IList<OpenApiParameter> parameters, string headerName, object? value)
    {
        var existingParameter = parameters.FirstOrDefault(p => p.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase));

        if (existingParameter != null)
        {
            // If the header exists, update its default value
            existingParameter.Schema.Default = new OpenApiString(Convert.ToString(value));
        }
        else
        {
            // If the header doesn't exist, add it with the default value
            parameters.Add(new OpenApiParameter
            {
                Name = headerName,
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema { Default = new OpenApiString(Convert.ToString(value)) },
                Required = false // You can set this to true if the header is required
            });
        }
    }
}