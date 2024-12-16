using System;
using System.Diagnostics;
using System.Threading.Tasks;
using GameLibrary.Bullets;
using GameLibrary.Bullets.BulletFactories;
using NetworkLibrary;

namespace GameLibrary.Maze.Network
{
    /// <summary>
    /// Менеджер сетевого взаимодействия
    /// </summary>
    public class NetworkManager
    {
        /// <summary>
        /// Событие записи данных
        /// </summary>
        public event Action<NetworkManager> OnWriteData;
        /// <summary>
        /// Событие обновления данных
        /// </summary>
        public event Action<NetworkManager> OnUpdateData;
        
        private INetworkHandler networkHandler;
        /// <summary>
        /// Данные текущего игрока
        /// </summary>
        public NetworkData CurrentPlayerNetworkData { get; private set; }
        /// <summary>
        /// Данные игрока по сети
        /// </summary>
        public NetworkData NetworkPlayerNetworkData { get; private set; }
        
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="networkHandler">Обработчик сетевого взаимодействия</param>
        public NetworkManager(INetworkHandler networkHandler)
        {
            this.networkHandler = networkHandler;
            CurrentPlayerNetworkData = new NetworkData();
            NetworkPlayerNetworkData = new NetworkData();
            networkHandler.OnDataGot += OnDataGot;
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public void UpdateData()
        {
            OnWriteData?.Invoke(this);
            networkHandler.UpdateData(CurrentPlayerNetworkData);
        }

        /// <summary>
        /// Событие, когда данные по сети получены
        /// </summary>
        /// <param name="data">Данные</param>
        private void OnDataGot(object data)
        {
            NetworkPlayerNetworkData = (NetworkData)data;
            var bulletData = NetworkPlayerNetworkData.BulletData;
            if (bulletData != null)
            {
                MazeScene.instance.AddObjectOnScene(
                    BulletFactory.CreateBullet((BulletType) bulletData.TypeId, bulletData.SpawnPosition, bulletData.Direction, bulletData.Tag));
            }

            CurrentPlayerNetworkData.BulletData = null;
            NetworkPlayerNetworkData.BulletData = null;
            OnUpdateData?.Invoke(this);
        }
    }
}