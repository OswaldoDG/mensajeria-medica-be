namespace mensajeriamedica.model.identity.tokenloginless;

public class TokenVinculacion
{
    public string DeviceId { get; set; }
    public string Token { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow.AddHours(24);
    public bool Activado { get; set; }
}
