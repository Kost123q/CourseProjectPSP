using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NetworkLibrary
{
    /// <summary>
    /// Класс сетевого взаимодействия для клиента
    /// </summary>
    public class Client : INetworkHandler, IDisposable
    {
        private HttpClient _client;
        private string _connectUri;

        /// <summary>
        /// Событие получения данных по сети
        /// </summary>
        public event Action<object> OnDataGot;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="serverAddress">Адрес подклбчения к серверу</param>
        public Client(string serverAddress)
        {
            _client = new HttpClient(new HttpClientHandler());
            _connectUri = $"http://{serverAddress}:8000/";
        }
        
        /// <summary>
        /// Обновление данных
        /// </summary>
        /// <param name="obj">Объект для передачи по сети</param>
        /// <typeparam name="T">Тип объекта для передачи по сети</typeparam>
        public async Task UpdateData<T>(T obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                _client.DefaultRequestHeaders.Accept.Clear();
                var response = await _client.PostAsync(_connectUri, data);
                string resultText = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<T>(resultText);

                OnDataGot?.Invoke(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <typeparam name="T">Тип получаемых данных</typeparam>
        public async Task GetData<T>()
        {
            try
            {
                var response = await _client.GetAsync(_connectUri);
                var resultText = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(resultText);
                var result = JsonConvert.DeserializeObject<T>(resultText);

                OnDataGot?.Invoke(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Очистка случшителей событий
        /// </summary>
        public void ClearListeners()
        {
            OnDataGot = null;
        }
        
        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}