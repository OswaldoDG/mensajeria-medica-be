using System.Net;
using comunes.respuestas;

namespace contabee.model.identity.usuarios;

/// <summary>
/// Codigos de error de usuarios.
/// </summary>
public static class ErroresUsuario
{
    public static ErrorProceso DatosNulos(string instancia)
    {
        return new ErrorProceso() { Codigo = "USR-DATOS-NULOS", HttpCode = HttpStatusCode.BadRequest, Mensaje = $"Los datos para {instancia} son nulos" };
    }

    public static ErrorProceso CondicionDatos(string condicion)
    {
        return new ErrorProceso() { Codigo = "USR_CONDICION-DATOS", HttpCode = HttpStatusCode.BadRequest, Mensaje = $"Datos no válidos, {condicion}" };
    }


    public static ErrorProceso CuentaFiscalRequerida()
    {
        return new ErrorProceso() { Codigo = "GLOBAL-CUENTA-FISCAL-REQUERIDA", HttpCode = HttpStatusCode.BadRequest, Mensaje = "El id de la cuenta fiscal es requerido" };
    }

    public static ErrorProceso ContrasenaRequerida()
    {
        return new ErrorProceso() { Codigo = "USR-CONTRASENA-REQUERIDA", HttpCode = HttpStatusCode.BadRequest, Mensaje = "La constraseña es obligatoria" };
    }

    public static ErrorProceso EmailRequerido()
    {
        return new ErrorProceso() { Codigo = "USR-EMAIL-REQUERIDO", HttpCode = HttpStatusCode.BadRequest, Mensaje = "El email es obligatorio" };
    }

    public static ErrorProceso UsuarioExistente()
    {
        return new ErrorProceso() { Codigo = "USR-USUARIO-EXISTENTE", HttpCode = HttpStatusCode.Conflict, Mensaje = "El email utilizado ya existe registrado" };
    }

    public static ErrorProceso CracionDesconocido(string error)
    {
        return new ErrorProceso() { Codigo = "USR-CREAR-ERROR-DESCONOCIDO", HttpCode = HttpStatusCode.InternalServerError, Mensaje = $"Error desconocido al crear el usuario {error}" };
    }

    public static ErrorProceso DatoFiltroRequerido(string propiedad)
    {
        return new ErrorProceso() { Codigo = "URS-DATO-FILTRO-REQUERIDO", HttpCode = HttpStatusCode.BadRequest, Mensaje = $"El filtro debe contener un valor para la propiedad {propiedad}" };
    }

    public static ErrorProceso DatoFiltroValorNoValido(string propiedad, string valor)
    {
        return new ErrorProceso() { Codigo = "USR-DATO-FILTROVALOR-NO-VALIDO", HttpCode = HttpStatusCode.BadRequest, Mensaje = $"El valor para la propiedad {propiedad} no es válido {valor}" };
    }

    public static ErrorProceso DatoFiltroTipoUsuario()
    {
        return new ErrorProceso() { Codigo = "USR-TIPO-USUARIO-INCORRECTO", HttpCode = HttpStatusCode.BadRequest, Mensaje = $"El tipo para el usuario no es compatible con la cuenta." };
    }

    public static ErrorProceso UsuarioInexistente()
    {
        return new ErrorProceso() { Codigo = "USR-USUARIO-INEXISTENTE", HttpCode = HttpStatusCode.NotFound, Mensaje = "Usuario inexistente" };
    }

    public static ErrorProceso UsuarioSinRol()
    {
        return new ErrorProceso() { Codigo = "USR-USUARIO-SINROL", HttpCode = HttpStatusCode.Forbidden, Mensaje = "Usuario sin roles asignados" };
    }

    public static ErrorProceso ContrasenaActualNoValida()
    {
        return new ErrorProceso() { Codigo = "USR-CONTRASENA-ACTUAL-NO-VALIDA", HttpCode = HttpStatusCode.BadRequest, Mensaje = "La contraseña proporcionada no es válida" };
    }

    public static ErrorProceso CuentafiscalNoValida()
    {
        return new ErrorProceso() { Codigo = "USR-CUENTA-FISCAL-NO-VALIDA", HttpCode = HttpStatusCode.BadRequest, Mensaje = "El usuario no pertenece a la cuenta fiscal" };
    }

    public static ErrorProceso CambioPasswordDesconocido(string error)
    {
        return new ErrorProceso() { Codigo = "USR-ERROR-CAMBIO-PASSWORD-DESCONOCIDO", HttpCode = HttpStatusCode.InternalServerError, Mensaje = $"Error desconocido al cambiar la contraseña {error}" };
    }

    public static ErrorProceso EliminarDesconocido(string error)
    {
        return new ErrorProceso() { Codigo = "USR-ERROR-ELIMINAR-USAURIO-DESCONOCIDO", HttpCode = HttpStatusCode.InternalServerError, Mensaje = $"Error desconocido al eliminar el usuario {error}" };
    }

    public static ErrorProceso ActualizarDesconocido(string error)
    {
        return new ErrorProceso() { Codigo = "USR-ERROR-ACTUALZIAR-DESCONOCIDO", HttpCode = HttpStatusCode.InternalServerError, Mensaje = $"Error desconocido al actualizar el usuario {error}" };
    }

    public static ErrorProceso ActualizarRolesDesconocido(string error)
    {
        return new ErrorProceso() { Codigo = "USR-ERROR-ROL-UPDATE-DESCONOCIDO", HttpCode = HttpStatusCode.InternalServerError, Mensaje = $"Error desconocido al actualizar roles del usuario {error}" };
    }

    public static ErrorProceso Desconocido(string error, string metodo)
    {
        return new ErrorProceso() { Codigo = "USR-ERROR-DESCONOCIDO", HttpCode = HttpStatusCode.InternalServerError, Mensaje = $"Error desconocido en {metodo} el usuario {error}" };
    }

    public static ErrorProceso RolNoValido()
    {
        return new ErrorProceso() { Codigo = "USR-ROL-NO-VALIDO", HttpCode = HttpStatusCode.BadRequest, Mensaje = "Alguno de los roles proporcionados no es válido" };
    }

    public static ErrorProceso RecursoNoEncontrado(string recurso)
    {
        return new ErrorProceso() { Codigo = "RECURSO_NO_ENCONTRADO", HttpCode = HttpStatusCode.NotFound, Mensaje = $"{recurso} no encontrado" };
    }

    public static ErrorProceso TokenCaducado()
    {
        return new ErrorProceso() { Codigo = "TOKEN_CADUCADO", HttpCode = HttpStatusCode.Forbidden, Mensaje = "Este token esta caducado" };
    }
}
