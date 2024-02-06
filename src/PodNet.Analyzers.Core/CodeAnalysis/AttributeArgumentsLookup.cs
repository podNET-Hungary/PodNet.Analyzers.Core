using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace PodNet.Analyzers.CodeAnalysis;

/// <summary>A more convenient lookup for <see cref="AttributeData.NamedArguments"/>.</summary>
/// <param name="Values">The named arguments provided to the attribute.</param>
public record AttributeArgumentsLookup(ImmutableDictionary<string, TypedConstant> Values)
{
    public static AttributeArgumentsLookup FromAttributeData(AttributeData attributeData)
    {
        if (attributeData.NamedArguments.Length is 0)
            return Empty;
        var result = new Dictionary<string, TypedConstant>(attributeData.NamedArguments.Length);
        foreach (var kv in attributeData.NamedArguments)
            result[kv.Key] = kv.Value;
        return new(result.ToImmutableDictionary());
    }

    public static AttributeArgumentsLookup Empty { get; } = new(ImmutableDictionary<string, TypedConstant>.Empty);

    /// <summary>Gets the named argument for the given <paramref name="key"/>, or an empty <see cref="TypedConstant"/> if it's not present.</summary>
    /// <param name="key">The key to look up among the attribute's <see cref="AttributeData.NamedArguments"/>.</param>
    /// <returns>The typed constant value for the given key, or a default <see cref="TypedConstant"/> with its kind being <see cref="TypedConstantKind.Error"/>.</returns>
    public TypedConstant this[string key] => Values.TryGetValue(key, out var value) ? value : default;

    /// <summary>Gets the named argument <paramref name="key"/> for the attribute typed as <typeparamref name="T"/>, or <paramref name="defaultValue"/> if it's not found or was <see langword="null"/>. Throws if the value kind is <see cref="TypedConstantKind.Error"/> or <see cref="TypedConstantKind.Array"/>, or the value can not be cast to <typeparamref name="T"/>. In the latter case, you have to use <see cref="GetValuesOrDefault"/>.</summary>
    /// <typeparam name="T">If the value is <see cref="TypedConstantKind.Primitive" /> or <see cref="TypedConstantKind.Enum" /> or <see cref="TypedConstantKind.Type"/> and <paramref name="key"/> is found among the named arguments for the attribute, the value will be cast to this type.</typeparam>
    /// <param name="key">The argument name to find in the attribute's <see cref="AttributeData.NamedArguments"/>.</param>
    /// <param name="defaultValue">If the key was not found or represents <see langword="null"/>, this value is returned.</param>
    /// <returns>The value cast to the target type, if found. <paramref name="defaultValue"/> otherwise.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the value has <see cref="TypedConstantKind.Error"/>.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value cannot be converted to the target type <typeparamref name="T"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the value represents an <see cref="TypedConstantKind.Array"/>. Use <see cref="GetValuesOrDefault"/> instead.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value represents an unknown kind.</exception>
    public T? GetValueOrDefault<T>(string key, T? defaultValue = default!)
    {
        return Values.TryGetValue(key, out var value) && !value.IsNull
            ? value.Kind switch
            {
                TypedConstantKind.Error => throw new InvalidOperationException($"The given key '{key}' was bound from the attribute, but does not represent a valid value."),
                TypedConstantKind.Primitive or TypedConstantKind.Enum or TypedConstantKind.Type => (T?)value.Value,
                TypedConstantKind.Array => throw new ArgumentException($"The given key represents an array type in the provided attribute argument list. Use '{nameof(GetValuesOrDefault)}' instead.", key),
                _ => throw new ArgumentOutOfRangeException($"Unknown {nameof(TypedConstantKind)}: {value.Kind}")
            }
            : defaultValue;
    }

    /// <summary>Gets the named argument <paramref name="key"/> for the attribute typed as an <see cref="ImmutableArray{T}"/> of type <typeparamref name="T"/>. Throws if the value kind is not <see cref="TypedConstantKind.Array"/>, or the individual values can not be cast to <typeparamref name="T"/>. In the latter case, you have to use <see cref="GetValueOrDefault"/>.</summary>
    /// <typeparam name="T">If the value is <see cref="TypedConstantKind.Array" /> and <paramref name="key"/> is found among the named arguments for the attribute, the values will be cast to this type.</typeparam>
    /// <param name="key">The argument name to find in the attribute's <see cref="AttributeData.NamedArguments"/>.</param>
    /// <returns>The values cast to the target type, if found. The <see cref="ImmutableArray{T}.Empty"> array otherwise.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the value has <see cref="TypedConstantKind.Error"/>.</exception>
    /// <exception cref="InvalidCastException">Thrown when the values cannot be converted to the target type <typeparamref name="T"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the value does not represent an <see cref="TypedConstantKind.Array"/>. Use <see cref="GetValueOrDefault"/> instead.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value represents an unknown kind.</exception>
    public ImmutableArray<T>? GetValuesOrDefault<T>(string key)
    {
        return Values.TryGetValue(key, out var value) && !value.IsNull
            ? value.Kind switch
            {
                TypedConstantKind.Error => throw new InvalidOperationException($"The given key '{key}' was bound from the attribute, but does not represent a valid value."),
                TypedConstantKind.Primitive or TypedConstantKind.Enum or TypedConstantKind.Type => throw new ArgumentException($"The given key represents a(n) {value.Kind} kind and not an array type in the provided attribute argument list. Use '{nameof(GetValueOrDefault)}' instead.", key),
                TypedConstantKind.Array => ToArray(value),
                _ => throw new ArgumentOutOfRangeException($"Unknown {nameof(TypedConstantKind)}: {value.Kind}")
            }
            : default;

        static ImmutableArray<T> ToArray(TypedConstant value)
        {
            if (value.Values.Length is 0)
                return ImmutableArray<T>.Empty;
            var result = new T[value.Values.Length];
            for (var i = 0; i < value.Values.Length; i++)
                result[i] = (T)value.Values[i].Value!;
            return ImmutableArray.Create(result);
        }
    }
}
