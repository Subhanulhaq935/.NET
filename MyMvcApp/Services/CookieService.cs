using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace MyMvcApp.Services;

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetCookie(string key, string value, int? expireDays = null)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        };

        if (expireDays.HasValue)
        {
            options.Expires = DateTimeOffset.UtcNow.AddDays(expireDays.Value);
        }

        _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, options);
    }

    public string? GetCookie(string key)
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[key];
    }

    public void RemoveCookie(string key)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
    }

    public void SetObjectCookie<T>(string key, T obj, int? expireDays = null)
    {
        var json = JsonSerializer.Serialize(obj);
        SetCookie(key, json, expireDays);
    }

    public T? GetObjectCookie<T>(string key)
    {
        var cookieValue = GetCookie(key);
        if (string.IsNullOrEmpty(cookieValue))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(cookieValue);
        }
        catch
        {
            return default;
        }
    }
}





