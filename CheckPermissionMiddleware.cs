using Microsoft.AspNetCore.Http;
using System.Numerics;
using Newtonsoft.Json;
using CheckPermissionsLib.models;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
namespace CheckPermissionsLib;


public class CheckPermissionMiddleware : IMiddleware
{

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configurations;

    public CheckPermissionMiddleware(IConfiguration configurations)
    {
        _configurations = configurations;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
       using (var _httpClient = new HttpClient())
       {
            StringValues tokenFullS;
            string tokenFull = tokenFullS.ToString();
            if(context.Request.Headers.TryGetValue("Authorization", out tokenFullS))
            {

                context.Response.StatusCode = 403;
                context.Response.WriteAsync("Acesso Negado");
            }
            var tokenList = tokenFull.Split(" ");
            if (tokenList.Length < 2)
            {
                context.Response.StatusCode = 403;
                context.Response.WriteAsync("Acesso Negado");
            }
            string token = tokenList[0];
            
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Bearer " + token);

            var respose = await _httpClient.SendAsync(request);
            if (!respose.IsSuccessStatusCode)
            {
                context.Response.StatusCode = 403;
                context.Response.WriteAsync("Acesso Negado");
            }
            var body = respose.Content.ReadAsStringAsync().Result;
            var permissionsUser = JsonConvert.DeserializeObject<PermissivosLevelDto>(body);

            var currentUri = context.Request.Path.ToString();

            var permissionsLevel =  _configurations[currentUri+"level"];
            var product = _configurations[currentUri+"product"];
            if (permissionsUser != null)
            {
                context.Response.StatusCode = 403;
                context.Response.WriteAsync("Acesso Negado");
            }

            foreach (var productLevelAcess in permissionsUser.LevelAcesses)
            {
                if (product.Equals(productLevelAcess.Name))
                {
                    if(productLevelAcess.Permissions.Contains(permissionsLevel)) await next(context);
                }

            }

            context.Response.StatusCode = 403;
            context.Response.WriteAsync("Acesso Negado");
        }
    }
}
