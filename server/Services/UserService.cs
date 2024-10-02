namespace TutorMioAPI1.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TutorMioAPI1.Domain;
using TutorMioAPI1.Interfaces;
using System.Data;
using TutorMioAPI1.Extensions;
using TutorMioAPI1.Requests;

public class UserService : IUserService
{
    IDataProvider _data = null;
    protected readonly IConfiguration _configuration;


    public UserService(IDataProvider provider, IConfiguration configuration)
    {
        _data = provider;
        _configuration = configuration;
    }

    public async Task<User> Authenticate(string username, string password)
    {
        string procName = "[dbo].[Users_Select_AuthData_v2]";
        User user = null;

        _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
        {
            paramCollection.AddWithValue("@Username", username);
        }, delegate (IDataReader reader, short set)
        {
            var passwordHash = reader["PasswordHash"].ToString(); // Ensure this is safe to access
            if (BCrypt.Net.BCrypt.Verify(password, passwordHash))
            {
            int startingIndex = 0;
            user = MapSingleUser(reader, ref startingIndex);
            }
        });
        return user;
    }
  
    public async Task<string> GenerateJwtToken(User user)
    {
        var jwtIssuer = _configuration["JwtSettings_Issuer"];
        var jwtAudience = _configuration["JwtSettings_Audience"];
        var jwtsecretKey = _configuration["JwtSettings_SecretKey"];
        var jwtexpirationInMinutes = _configuration["JwtExpirationInMinutes"];
       // var jwtSettings = _configuration.GetSection("JwtSettings");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtsecretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtexpirationInMinutes)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtIssuer,
            Audience = jwtAudience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public int Register(RegisterRequest request)
    {
        int id = 0;
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        string procName = "[dbo].[Users_Insert_v2]";
        _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
        {
            AddCommonParams(request, col, passwordHash);
            SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
            idOut.Direction = ParameterDirection.Output;
            col.Add(idOut);
        }, returnParameters: delegate (SqlParameterCollection returnCollection)
        {
            object oId = returnCollection["@Id"].Value;
            int.TryParse(oId.ToString(), out id);

        });
        return id;
    }
  
    private static void AddCommonParams(RegisterRequest request, SqlParameterCollection col, string passwordHash)
    {
        col.AddWithValue("@Username", request.Username);
        col.AddWithValue("@PasswordHash", passwordHash);
        col.AddWithValue("@Role", request.Role);
    }
    private static User MapSingleUser(IDataReader reader, ref int startingIndex)
    {
        User user;
        user = new User();
        user.Id = reader.GetSafeInt32(startingIndex++);
        user.Username = reader.GetString(startingIndex++);
        user.Role = reader["Role"].ToString();
        return user;
    }
}


