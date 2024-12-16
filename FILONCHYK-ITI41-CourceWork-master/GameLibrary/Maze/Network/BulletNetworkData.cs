using SharpDX;

namespace GameLibrary.Maze.Network
{
    /// <summary>
    /// Игровые данные, отправляемые по сети
    /// </summary>
    public class BulletNetworkData
    {
        /// <summary>
        /// Индефикатор пули
        /// </summary>
        public int TypeId { get; set; }
        /// <summary>
        /// Позиция появления
        /// </summary>
        public Vector2 SpawnPosition { get; set; }
        /// <summary>
        /// Направление пули
        /// </summary>
        public Vector2 Direction { get; set; }
        /// <summary>
        /// Тег игрока, создавшего пулю
        /// </summary>
        public string Tag { get; set; }
    }
}