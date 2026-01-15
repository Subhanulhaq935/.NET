namespace MyMvcApp.Services;

public interface ICookieService
{
    void SetCookie(string key, string value, int? expireDays = null);
    string? GetCookie(string key);
    void RemoveCookie(string key);
    void SetObjectCookie<T>(string key, T obj, int? expireDays = null);
    T? GetObjectCookie<T>(string key);
}





