namespace BearerAuthDemo;

public class Jwt
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; } // Seconds until the token expires
    public int ExpiresInMinutes => ExpiresIn / 60;
}