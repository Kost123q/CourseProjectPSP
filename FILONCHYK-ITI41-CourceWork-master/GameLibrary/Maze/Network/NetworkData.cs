using SharpDX;

namespace GameLibrary.Maze.Network
{
    /// <summary>
    /// Игровые данные, отправляемые по сети
    /// </summary>
    public class NetworkData
    {
        /// <summary>
        /// Позиция игрока
        /// </summary>
        public Vector2 PlayerPosition { get; set; }
        /// <summary>
        /// Повернут ли спрайт игрока
        /// </summary>
        public bool IsPlayerSpriteFlip { get; set; }
        /// <summary>
        /// Передвигался ли игрок
        /// </summary>
        public bool IsPlayerMove { get; set; }
        /// <summary>
        /// Данные о созданной пуле
        /// </summary>
        public BulletNetworkData BulletData { get; set; }
    }
}