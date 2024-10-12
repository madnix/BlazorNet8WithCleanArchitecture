using System.Text.Json;
using System.Text.Json.Serialization;
using Application.DTOs.Request.Account;
using NetcodeHub.Packages.Extensions.LocalStorage;

namespace Application.Extensions;

public class LocalStorageService (ILocalStorageService localStorageService)
{
    #region Private Methods
    
    /// <summary>
    /// 로컬 스토리지 상수 키 반환
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetBrowserLocalStorage()
    {
        var tokenModel = await localStorageService.GetEncryptedItemAsStringAsync(Constant.BrowserStorageKey);
        return tokenModel;
    }

    /// <summary>
    /// 모델 객체 직렬화
    /// </summary>
    /// <param name="modelObject"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static string SerializeObj<T>(T modelObject)
        => JsonSerializer.Serialize(modelObject, JsonOptions());

    /// <summary>
    /// Json 문자열 역직렬화
    /// </summary>
    /// <param name="jsonString"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static T? DeserializeJsonString<T>(string jsonString)
        => JsonSerializer.Deserialize<T>(jsonString, JsonOptions());

    /// <summary>
    /// Json 직렬화 처리 옵션
    /// </summary>
    /// <returns></returns>
    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
        };
    }
    
    #endregion

    #region Public Methods
    
    /// <summary>
    /// 현재 Token으로 LocalStorageDto 모델을 반환
    /// </summary>
    /// <returns></returns>
    public async Task<LocalStorageDto?> GetModelFromToken()
    {
        try
        {
            var token = await GetBrowserLocalStorage();
            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
                return new LocalStorageDto();

            return DeserializeJsonString<LocalStorageDto>(token);
        }
        catch
        {
            return new LocalStorageDto();
        }
    }

    /// <summary>
    /// 로컬 스토리지에 Token을 암호화하여 저장
    /// </summary>
    /// <param name="localStorageDto"></param>
    public async Task SetBrowserLocalStorage(LocalStorageDto localStorageDto)
    {
        try
        {
            var token = SerializeObj(localStorageDto);
            await localStorageService.SaveAsEncryptedStringAsync(Constant.BrowserStorageKey, token);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// 로컬 스토리지의 Token 삭제
    /// </summary>
    public async Task RemoveTokenFromBrowserLocalStorage()
        => await localStorageService.DeleteItemAsync(Constant.BrowserStorageKey);

    #endregion
}