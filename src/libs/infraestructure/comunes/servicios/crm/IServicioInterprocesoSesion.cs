namespace comunes.servicios.crm;

public interface IServicioInterprocesoSesion
{
    /// <summary>
    /// Determina si la cuenta fiscal existe.
    /// </summary>
    /// <param name="id">ID a verificar.</param>
    /// <returns>VErdaero si existe el Id.</returns>
    Task<bool> ExisteCuentaFiscal(Guid id);

    /// <summary>
    /// VErifica si existe un usuario en una cuenta fiscal.
    /// </summary>
    /// <param name="cuentaFiscalId">ID de la cuenta.</param>
    /// <param name="usuarioId">Id del usaurio.</param>
    /// <param name="roles">Roles buscados en la asociacion.</param>
    /// <returns>Verdadero si existe en la cuenta.</returns>
    Task<bool> ExisteUsuarioCuentaFiscal(Guid cuentaFiscalId, Guid usuarioId, List<string>? roles);

    /// <summary>
    /// VErifica si el usuario es primario en una cuenta fiscal.
    /// </summary>
    /// <param name="cuentaFiscalId">Id de la cuenta fiscal</param>
    /// <param name="usuarioId">Id del usuario.</param>
    /// <returns>True si es primario false si no.</returns>
    Task<bool> EsPrimarioCuentaFiscal(Guid cuentaFiscalId, Guid usuarioId);
}
