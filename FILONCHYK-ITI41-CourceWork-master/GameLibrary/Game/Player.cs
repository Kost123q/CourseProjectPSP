using EngineLibrary.EngineComponents;
using SharpDX;

namespace GameLibrary.Game
{
    /// <summary>
    /// Класс, описывающий сценарий поведения игрока
    /// </summary>
    public class Player : BasePlayer
    {
        /// <summary>
        /// Управление игрока
        /// </summary>
        public PlayerControl Control { get; private set; }
        /// <summary>
        /// Скорость игрока
        /// </summary>
        public float Speed { get; set; } = 5;
        /// <summary>
        /// Возможность двигаться у игрока
        /// </summary>
        public bool IsCanMove { get; set; } = true;
        
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="control">Управления игрока</param>
        public Player(PlayerControl control)
        {
            Control = control;
        }

        /// <summary>
        /// Обновление игрового объекта
        /// </summary>
        public override void Update()
        {
            if (gameObject.IsActive && IsCanMove)
                Move();     
            base.Update();
        }

        /// <summary>
        /// Метод движения игрока
        /// </summary>
        private void Move()
        {
            int directionX = 0, directionY = 0;

            directionX = Input.GetAxis(Control.HorizontalAxis);

            if (gameObject.Collider.CheckIntersection("Stair"))
            {
                directionY = Input.GetAxis(Control.VerticalAxis);
                gameObject.Transform.IsUseGravitation = gameObject.Collider.CheckIntersection("Wall");
            }
            else
            {
                gameObject.Transform.IsUseGravitation = true;
            }

            Vector2 direction;

            if (directionX == 0)
            {
                direction = new Vector2(0, directionY);
            }
            else
            {
                gameObject.Sprite.IsFlipX = directionX < 0;
                direction = new Vector2(directionX, 0);
            }
            isMove = directionX != 0;

            Vector2 movement = direction * Speed * Time.DeltaTime;
            gameObject.Transform.SetMovement(movement);

            DetectCollision();
        }

        /// <summary>
        /// Распознавание столкновений и реакция на них
        /// </summary>
        private void DetectCollision()
        {
            if (gameObject.Collider.CheckIntersection("Wall","BreakWall"))
            {
                gameObject.Transform.ResetMovement();
            }

            if (gameObject.GameObjectTag == "Blue Player" || gameObject.GameObjectTag == "Red Player")
                gameObject.Transform.AddGravitation();

            string tag = (Input.GetAxis(Control.VerticalAxis) == -1) ? "" : "Platform";

            if (gameObject.Collider.CheckIntersection("Wall", tag))
            {
                gameObject.Transform.ResetGravitation();
            }
        }
    }
}
