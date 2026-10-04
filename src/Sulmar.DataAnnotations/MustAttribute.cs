using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Sulmar.DataAnnotations;

// Wywołuje statyczną metodę bool (obiekt, wartość) — odpowiednik Must((obiekt, wartość) => ...) z FluentValidation.
[AttributeUsage(AttributeTargets.Property)]
public sealed class MustAttribute(string methodName) : ValidationAttribute("{0} nie spełnia warunku.")
{
    private string MethodName { get; } = methodName;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var valueType = ResolveValueType(value, validationContext);
        var method = FindMethod(validationContext.ObjectType, valueType);

        // Brak wartości obsługuje osobny atrybut [Required].
        if (value is null)
            return ValidationResult.Success;

        if (Invoke(method, validationContext.ObjectInstance, value))
            return ValidationResult.Success;

        var message = string.Format(ErrorMessageString, validationContext.DisplayName);
        var memberNames = validationContext.MemberName is { } memberName
            ? new[] { memberName }
            : Array.Empty<string>();

        return new ValidationResult(message, memberNames);
    }

    private Type ResolveValueType(object? value, ValidationContext validationContext)
    {
        if (value is not null)
            return value.GetType();

        if (validationContext.MemberName is not { } memberName)
        {
            throw new InvalidOperationException(
                $"Nie można ustalić typu wartości dla metody '{MethodName}'.");
        }

        var property = validationContext.ObjectType.GetProperty(
            memberName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property is null || property.GetIndexParameters().Length != 0)
        {
            throw new InvalidOperationException(
                $"Nie znaleziono właściwości '{memberName}'.");
        }

        return Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
    }

    private MethodInfo FindMethod(Type objectType, Type valueType)
    {
        const BindingFlags flags =
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        MethodInfo? match = null;

        for (var type = objectType; type is not null && type != typeof(object); type = type.BaseType)
        {
            foreach (var method in type.GetMethods(flags))
            {
                if (method.Name != MethodName || method.ContainsGenericParameters || method.ReturnType != typeof(bool))
                    continue;

                var parameters = method.GetParameters();
                if (parameters.Length != 2)
                    continue;

                if (!parameters[0].ParameterType.IsAssignableFrom(objectType) ||
                    !Accepts(parameters[1].ParameterType, valueType))
                    continue;

                if (match is not null)
                {
                    throw new InvalidOperationException(
                        $"Metoda '{MethodName}' jest niejednoznaczna.");
                }

                match = method;
            }
        }

        if (match is null)
        {
            throw new InvalidOperationException(
                $"Nie znaleziono statycznej metody '{MethodName}' przyjmującej walidowany obiekt i wartość właściwości.");
        }

        return match;
    }

    private static bool Accepts(Type parameterType, Type valueType)
    {
        if (parameterType.IsAssignableFrom(valueType))
            return true;

        var underlying = Nullable.GetUnderlyingType(parameterType);
        return underlying is not null && underlying.IsAssignableFrom(valueType);
    }

    private static bool Invoke(MethodInfo method, object instance, object value)
    {
        try
        {
            return (bool)method.Invoke(null, [instance, value])!;
        }
        catch (TargetInvocationException exception) when (exception.InnerException is { } inner)
        {
            ExceptionDispatchInfo.Capture(inner).Throw();
            throw;
        }
    }
}
