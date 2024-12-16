using EngineLibrary.ObjectComponents;
using GameLibrary.Effects.EffectFactories;
using SharpDX;

namespace GameLibrary.Bullets.BulletFactories
{
    /// <summary>
    /// Абстрактный класс фабрики создания пуль 
    /// </summary>
    public abstract class BulletFactory
    {
        /// <summary>
        /// Создание игрового объекта пули
        /// </summary>
        /// <param name="position">Позиция появления пули</param>
        /// <param name="direction">Направление пули</param>
        /// <param name="tag">Тег игрового объекта, создающий пулю</param>
        /// <returns>Игровой объект</returns>
        public abstract GameObject CreateBullet(Vector2 position, Vector2 direction, string tag);

        /// <summary>
        /// Создание игрового объекта пули
        /// </summary>
        /// <param name="type">Тип пули</param>
        /// <param name="position">Позиция появления пули</param>
        /// <param name="direction">Направление пули</param>
        /// <param name="tag">Тег игрового объекта, создающий пулю</param>
        /// <returns>Игровой объект</returns>
        public static GameObject CreateBullet(BulletType type, Vector2 position, Vector2 direction, string tag)
        {
            switch (type)
            {
                case BulletType.Damage:
                    return new DamageBulletFactory().CreateBullet(position, direction, tag);
                case BulletType.Slowdown:
                    return new SlowdownBulletFactory().CreateBullet(position, direction, tag);
                case BulletType.Frezze:
                    return new FrezzeBulletFactory().CreateBullet(position, direction, tag);
                default:
                    return null;
            }
        }
    }
}
