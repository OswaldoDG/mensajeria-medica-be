using System.Security.Cryptography;
using System.Text;

namespace mensajeriamedica.api.comunicaciones.extensiones;

public static class ExtensionesProcesamiento
{
    /// <summary>
    /// Calcula el hash SHA256 de una cadena de texto.
    /// </summary>
    /// <param name="input">TExto.</param>
    /// <returns>Hash.</returns>
    public static string? GetSha256Hash(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return null;
        }

        SHA256 shaHash = SHA256.Create();

        // Convert the input string to a byte array and compute the hash.
        byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }
}
