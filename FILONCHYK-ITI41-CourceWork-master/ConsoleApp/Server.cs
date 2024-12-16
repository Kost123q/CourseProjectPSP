using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NetworkLibrary
{
    /// <summary>
    /// Класс сетевого взаимодействия для сервера
    /// </summary>
    public class Server : INetworkHandler, IDisposable
    {
        private HttpListener _listener;

        /// <summary>
        /// Событие получения данных по сети
        /// </summary>
        public event Action<object> OnDataGot;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public Server()
        {
            _listener = new HttpListener();
            
            _listener.Prefixes.Add($"http://*:{8000}/");
            _listener.Start();
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
                HttpListenerContext context = await _listener.GetContextAsync();
                
                var result = HandleRequest<T>(context.Request);
                SendResponse(context.Response, obj);

                OnDataGot?.Invoke(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Отправка ответа клиенту
        /// </summary>
        /// <param name="response">Объекта ответа</param>
        /// <param name="obj">Объект для передачи по сети</param>
        /// <typeparam name="T">Тип объекта для передачи по сети</typeparam>
        private void SendResponse<T>(HttpListenerResponse response, T obj)
        {
            Stream output = null;
            try
            {
                output = response.OutputStream;
                string responseStr = JsonConvert.SerializeObject(obj);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
                response.ContentLength64 = buffer.Length;
                output.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
            finally
            {
                output?.Close();
            }
        }
        
        /// <summary>
        /// Обработка ответа от клиента
        /// </summary>
        /// <param name="request">Объекта ответа</param>
        /// <typeparam name="T">Тип получаемого объекта по сети</typeparam>
        /// <returns>Полученный по сети объекта</returns>
        private T HandleRequest<T>(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return default;
            }

            Stream stream = null;
            StreamReader reader = null;
            try
            {
                stream = request.InputStream;
                reader = new StreamReader(stream, request.ContentEncoding);
                var resultText = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(resultText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
            finally
            {
                stream?.Close();
                reader?.Close();
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
            _listener.Close();
        }
    }
}