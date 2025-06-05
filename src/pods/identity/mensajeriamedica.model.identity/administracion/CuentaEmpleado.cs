using System.Diagnostics.CodeAnalysis;

namespace mensajeriamedica.model.identity.administracion;

/// <summary>
/// Informacin de la cuenta de empleados.
/// </summary>
[ExcludeFromCodeCoverage]
public class CuentaEmpleado : CuentaBase
{
    /// <summary>
    /// Lista de roles de identificacion asociados al usuario.
    /// </summary>
    public List<string> RolesId { get; set; } = [];
}
