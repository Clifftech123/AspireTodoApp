using System.Reflection;
using MiniValidation;

namespace AspireTodoApp.Server.Features.Validation;

public static class ValidationFilterExtensions
{
    private static readonly ProducesResponseTypeMetadata ValidationErrorResponseMetadata =
        new(400, typeof(HttpValidationProblemDetails), ["application/problem+json"]);

    public static TBuilder WithParameterValidation<TBuilder>(this TBuilder builder, params Type[] typesToValidate)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.Add(eb =>
        {
            var methodInfo = eb.Metadata.OfType<MethodInfo>().FirstOrDefault();

            if (methodInfo is null) return;

            List<int>? parameterIndexesToValidate = null;
            foreach (var p in methodInfo.GetParameters())
            {
                if (typesToValidate.Contains(p.ParameterType))
                {
                    parameterIndexesToValidate ??= [];
                    parameterIndexesToValidate.Add(p.Position);
                }
            }

            if (parameterIndexesToValidate is null) return;

            eb.Metadata.Add(ValidationErrorResponseMetadata);

            eb.FilterFactories.Add((context, next) =>
            {
                return efic =>
                {
                    foreach (var index in parameterIndexesToValidate)
                    {
                        if (efic.Arguments[index] is { } arg && !MiniValidator.TryValidate(arg, out var errors))
                        {
                            return new ValueTask<object?>(Results.ValidationProblem(errors));
                        }
                    }

                    return next(efic);
                };
            });
        });

        return builder;
    }
}
