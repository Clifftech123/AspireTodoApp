using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace AspireTodoApp.Server.Extensions;

public static class OpenApiOptionsExtensions
{
    public static OpenApiOptions AddBearerTokenAuthentication(this OpenApiOptions options)
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter your JWT access token."
        };

        var schemeReference = new OpenApiSecuritySchemeReference("Bearer");

        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info.Title = "Aspire Todo API";
            document.Info.Version = "v1";

            document.Components ??= new OpenApiComponents();
            document.Components!.SecuritySchemes["Bearer"] = scheme;

            return Task.CompletedTask;
        });

        options.AddOperationTransformer((operation, context, cancellationToken) =>
        {
            if (context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            {
                operation.Security = [new OpenApiSecurityRequirement { [schemeReference] = [] }];
            }

            return Task.CompletedTask;
        });

        return options;
    }
}
