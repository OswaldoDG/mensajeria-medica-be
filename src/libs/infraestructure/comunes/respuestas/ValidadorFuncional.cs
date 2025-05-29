using System.Diagnostics.CodeAnalysis;

namespace comunes.respuestas;

/// <summary>
/// Validador funciona de entidades.
/// </summary>
/// <typeparam name="T">Instancia a validar.</typeparam>
[ExcludeFromCodeCoverage]
public class ValidadorFuncional<T>(T instancia)
    where T : class
{
    public T Instancia { get; set; } = instancia;

    public ErrorProceso? Error { get; set; } = null;
}
