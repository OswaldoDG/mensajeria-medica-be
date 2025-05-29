using System.Diagnostics.CodeAnalysis;

namespace contabee.model.identity.administracion;

/// <summary>
/// DTO para la creación de un empleado.
/// </summary>
[ExcludeFromCodeCoverage]
public class CrearEmpleado
{
    /// <summary>
    /// Correo de inicio de sesión.
    /// </summary>
    required public string Email { get; set; }

    /// <summary>
    /// Npombre del empleado.
    /// </summary>
    required public string Nombre { get; set; }

    /// <summary>
    /// Contraseña para inicio de sesión.
    /// </summary>
    required public string Password { get; set; }

    /// <summary>
    /// Teléfono del empleado.
    /// </summary>
    required public string? Telefono { get; set; }

    /// <summary>
    /// Identificador único de la cuenta fiscal a la que pertenece.
    /// </summary>
    public Guid? CuentaFiscalId { get; set; }
}