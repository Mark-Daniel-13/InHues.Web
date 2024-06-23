using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using InHues.Web.Implementation;
using InHues.StateMngmt;
using System.Text;
using InHues.StateMngmt.Storage;
using System.Reflection;

namespace InHues.Bridge
{
    public class Bridge : IBridge
    {
        HttpClient HttpClient;
        JsonSerializerOptions JsonSerializerOptions;
        AppKeys DermtricsKeys;
        IStorageMngmt StoreMngmt;
        AppState AppState;
        public Bridge(AppKeys dermtricsKeys, AppState appState, IStorageMngmt storeMngmt)
        {
            HttpClient = new();
            JsonSerializerOptions = new()
            {
                NumberHandling = JsonNumberHandling.Strict,
                PropertyNameCaseInsensitive = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            DermtricsKeys = dermtricsKeys;
            AppState = appState;
            StoreMngmt = storeMngmt;
        }
        public async Task<HttpResponse<TResponse>> GetJsonAsync<TResponse>(string url, OdataRequestBase requestParams)
        {
            try
            {
                var queryParams = new Dictionary<string, string>{};
                var _url = string.Empty;

                if (!string.IsNullOrEmpty(requestParams.Select)) queryParams.Add("$select", requestParams.Select);
                if (!string.IsNullOrEmpty(requestParams.Expand)) queryParams.Add("$expand", requestParams.Expand);

                if (string.IsNullOrEmpty(requestParams.Id))
                {
                    if (!string.IsNullOrEmpty(requestParams.SearchString)) queryParams.Add("$filter", requestParams.SearchString);
                    if (!string.IsNullOrEmpty(requestParams.Apply)) queryParams.Add("$apply", requestParams.Apply);
                    if (requestParams.Top != 0) queryParams.Add("$top", requestParams.Top.ToString());
                    if (requestParams.Skip != 0) queryParams.Add("$skip", requestParams.Skip.ToString());
                    if (!string.IsNullOrEmpty(requestParams.OrderBy)) queryParams.Add("$orderby", requestParams.OrderBy);
                    queryParams.Add("$count", requestParams.Count.ToString().ToLower());

                    var stringBuilder = new StringBuilder();
                    foreach (var queryParam in queryParams)
                    {
                        stringBuilder.Append($"&{queryParam.Key}={queryParam.Value}");
                    }
                    var queryString = queryParams.Any() ? stringBuilder.ToString().Substring(1) : string.Empty;
                    _url = $"{DermtricsKeys.BackendOrigin}{url}?{queryString}";
                }
                else {
                    var stringBuilder = new StringBuilder();
                    foreach (var queryParam in queryParams)
                    {
                        stringBuilder.Append($"&{queryParam.Key}={queryParam.Value}");
                    }
                    var queryString = queryParams.Any() ? stringBuilder.ToString().Substring(1) : string.Empty;

                    _url = $"{DermtricsKeys.BackendOrigin}{url}";

                    var index = _url.LastIndexOf('/');
                    _url = $"{_url.Substring(0, index + 1)}{requestParams.Id}?{queryString}";
                    
                }
                
               
                var request = new HttpRequestMessage
                {
                    Method = new HttpMethod("GET"),
                    RequestUri = new Uri(_url),
                };
                request.Headers.Add("Accept", "*/*");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await StoreMngmt.GetValueAsync(DermtricsKeys.AccessToken));

                var response = await HttpClient.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) {
                    return new HttpResponse<TResponse> { 
                        IsSuccess = false,
                        HttpStatusCode = response.StatusCode,
                        Errors = JsonSerializer.Deserialize<string[]>(json, JsonSerializerOptions)
                    };
                }

                if (string.IsNullOrEmpty(json))
                {
                    return new HttpResponse<TResponse>
                    {
                        IsSuccess = true,
                        HttpStatusCode = response.StatusCode
                    };
                }


                var data = new HttpResponse<TResponse>
                {
                    IsSuccess = true,
                    HttpStatusCode = response.StatusCode,
                    Response = JsonSerializer.Deserialize<TResponse>(json, JsonSerializerOptions)
                };
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new() { HttpStatusCode = HttpStatusCode.InternalServerError};
            }
        }

        public async Task<HttpResponse<TResponse>> GetJsonAsync<TResponse>(string url)
        {
            try
            {
                var _url = string.Empty;


                _url = $"{DermtricsKeys.BackendOrigin}{url}";

                var request = new HttpRequestMessage
                {
                    Method = new HttpMethod("GET"),
                    RequestUri = new Uri(_url),
                };
                request.Headers.Add("Accept", "*/*");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await StoreMngmt.GetValueAsync(DermtricsKeys.AccessToken));

                var response = await HttpClient.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResponse<TResponse>
                    {
                        IsSuccess = false,
                        HttpStatusCode = response.StatusCode,
                        Errors = JsonSerializer.Deserialize<string[]>(json, JsonSerializerOptions)
                    };
                }

                if (string.IsNullOrEmpty(json))
                {
                    return new HttpResponse<TResponse>
                    {
                        IsSuccess = true,
                        HttpStatusCode = response.StatusCode
                    };
                }


                var data = new HttpResponse<TResponse>
                {
                    IsSuccess = true,
                    HttpStatusCode = response.StatusCode,
                    Response = JsonSerializer.Deserialize<TResponse>(json, JsonSerializerOptions)
                };
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new() { HttpStatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<HttpResponse<TResponse>> CommandJsonAsync<TResponse, TRequest>(string url, TRequest request, RequestType requesType)
        {
            try
            {
                var _url = DermtricsKeys.BackendOrigin + url;
                if (requesType == RequestType.DELETE) {
                    var index = _url.LastIndexOf('/');
                    _url = _url.Substring(0, index + 1) + request;
                }
                var httpRequest = new HttpRequestMessage
                {
                    Method = new HttpMethod(requesType.ToString()),
                    RequestUri = new Uri($"{_url}"),
                    Content = JsonContent.Create(request, null, JsonSerializerOptions)
                };

                httpRequest.Headers.Add("Accept", "*/*");
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await StoreMngmt.GetValueAsync(DermtricsKeys.AccessToken));

                //if (AppState.CurrentUser?.TenantId is not null) {
                //    Type model = typeof(TRequest);
                //    PropertyInfo? prop = model.GetProperty("TenantId");
                //    if (prop is not null) {
                //        prop.SetValue(request,AppState.CurrentUser.TenantId);
                //    }
                //}

                var response = await HttpClient.SendAsync(httpRequest);
                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResponse<TResponse>
                    {
                        IsSuccess = false,
                        HttpStatusCode = response.StatusCode,
                        Errors = JsonSerializer.Deserialize<string[]>(json, JsonSerializerOptions)
                    };
                }

                if (string.IsNullOrEmpty(json)) {
                    return new HttpResponse<TResponse>
                    {
                        IsSuccess = true,
                        HttpStatusCode = response.StatusCode
                    };
                }

                var data = new HttpResponse<TResponse>
                {
                    IsSuccess = true,
                    HttpStatusCode = response.StatusCode,
                    Response = JsonSerializer.Deserialize<TResponse>(json, JsonSerializerOptions)
                };
                return data;
               
            }
            catch (Exception e) {
                return new() { HttpStatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}
