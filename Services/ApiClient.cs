using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TaskSyncApp.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "https://your-app-name.herokuapp.com/api"; // Замените на реальный адрес API

        public ApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        // Получение всех задач с сервера
        public async Task<List<T>> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<T>>(jsonResponse);
        }

        // Получение одной задачи по id
        public async Task<T> GetAsync<T>(string endpoint, int id)
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/{endpoint}/{id}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        }

        // Отправка задачи на сервер
        public async Task<bool> PostAsync<T>(string endpoint, T data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{ApiBaseUrl}/{endpoint}", content);
            return response.IsSuccessStatusCode;
        }

        // Обновление задачи на сервере
        public async Task<bool> PutAsync<T>(string endpoint, int id, T data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{ApiBaseUrl}/{endpoint}/{id}", content);
            return response.IsSuccessStatusCode;
        }

        // Удаление задачи с сервера
        public async Task<bool> DeleteAsync(string endpoint, int id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/{endpoint}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
