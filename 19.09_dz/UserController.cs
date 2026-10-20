using ASP_P42.Data;
using ASP_P42.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ASP_P42.Controllers
{
    /* Д.З. 19.09.2026 - Перенести логіку реєстрації та автентифікації
     * користувача з UserController у DataAccessor.
     *
     * UserController тепер є тонким шаром (thin controller):
     * - отримує дані від клієнта
     * - делегує бізнес-логіку до DataAccessor
     * - повертає відповідь клієнту
     */
    public class UserController(
        DataAccessor dataAccessor
    ) : Controller
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;

        // Реєстрація — логіка перенесена до DataAccessor.RegisterUser()
        public IActionResult SignUp([FromBody] UserSignupFormModel formModel)
        {
            try
            {
                _dataAccessor.RegisterUser(formModel);
                return Json(formModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Basic Auth — логіка перенесена до DataAccessor.AuthenticateBasic()
        public IActionResult BasicAuth()
        {
            try
            {
                String authHeader = HttpContext.Request.Headers.Authorization.ToString();
                var userAccess = _dataAccessor.AuthenticateBasic(authHeader);

                if (userAccess == null)
                    return Unauthorized("Credentials rejected: check login and password");

                HttpContext.Session.SetString("userAccessId", userAccess.Id.ToString());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Basic Auth з JWT — автентифікація через DataAccessor, токен формується тут
        public IActionResult BasicAuthJwt()
        {
            try
            {
                String authHeader = HttpContext.Request.Headers.Authorization.ToString();
                var userAccess = _dataAccessor.AuthenticateBasic(authHeader);

                if (userAccess == null)
                    return Unauthorized("Credentials rejected: check login and password");

                // Формуємо JWT токен
                var header = new { alg = "HS256", typ = "JWT" };
                long time = (DateTime.Now.Ticks - DateTime.UnixEpoch.Ticks) / 100000;
                var payload = new
                {
                    sub  = userAccess.Login,
                    iat  = time,
                    exp  = time + 1000000,
                    name = userAccess.UserData.FullName,
                    email= userAccess.UserData.Email
                };

                String Encode(Object obj) => Microsoft.AspNetCore.Authentication.Base64UrlTextEncoder.Encode(
                    System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj)));

                String body = Encode(header) + "." + Encode(payload);
                String signature = Microsoft.AspNetCore.Authentication.Base64UrlTextEncoder.Encode(
                    System.Security.Cryptography.HMACSHA256.HashData(
                        System.Text.Encoding.UTF8.GetBytes("secret"),
                        System.Text.Encoding.UTF8.GetBytes(body)));

                return Ok(body + "." + signature);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
