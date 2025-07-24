using Agenda.Dominio.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Agenda.Aplicacao.Factory
{
    public class TokenJwtFactory

    {
        public string JWT_TOKEN { get; set; }

        private TokenJwtFactory(string jwtToken)
        {
            JWT_TOKEN = jwtToken;
        }

        public static ResultadoGenerico<TokenJwtFactory> Criar(
            string issuer,
            string audience,
            string secretKey,
            long id,
            string email,
            bool administrador)
        {
            if (string.IsNullOrWhiteSpace(issuer))
                return new ResultadoGenerico<TokenJwtFactory>(false, "Issuer não pode ser nulo ou vazio", null);

            if (string.IsNullOrWhiteSpace(audience))
                return new ResultadoGenerico<TokenJwtFactory>(false, "Audience não pode ser nulo ou vazio", null);

            if (string.IsNullOrWhiteSpace(secretKey))
                return new ResultadoGenerico<TokenJwtFactory>(false, "SecretKey não pode ser nulo ou vazio", null);

            if (id <= 0)
                return new ResultadoGenerico<TokenJwtFactory>(false, "Id não pode ser menor ou igual a zero", null);

            if (string.IsNullOrWhiteSpace(email))
                return new ResultadoGenerico<TokenJwtFactory>(false, "Email não pode ser nulo ou vazio", null);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Aud, audience),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.PrimarySid, id.ToString()),
                new Claim(ClaimTypes.Email, email)
            };

            if (administrador)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrador"));
            }

            var token = GerarJwtToken(secretKey, claims);

            return new ResultadoGenerico<TokenJwtFactory>(true, "Token gerado com sucesso", new TokenJwtFactory(token));
        }

        private static string GerarJwtToken(string key, IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
