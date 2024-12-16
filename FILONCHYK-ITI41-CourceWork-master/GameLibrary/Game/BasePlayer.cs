using EngineLibrary.EngineComponents;
using EngineLibrary.ObjectComponents;
using GameLibrary.Maze;
using GameLibrary.Maze.Network;

namespace GameLibrary.Game
{
    /// <summary>
    /// Базовый класс игрока
    /// </summary>
    public class BasePlayer : ObjectScript
    {
        /// <summary>
        /// Собарнные монеты
        /// </summary>
        public int Coins { get; private set; } = 0;
        
        protected bool isMove;
        private GameObject childGameObject;

        /// <summary>
        /// Изменение значени монет
        /// </summary>
        /// <param name="value">Значение, которое прибавляется к текущему значению монет</param>
        public void ChangeCoinsValue(int value)
        {
            Coins += value;
            GameEvents.ChangeCoins?.Invoke(gameObject.GameObjectTag, Coins);
        }
        
        /// <summary>
        /// Установка дочернего объекта 
        /// </summary>
        /// <param name="gameObject">Дочерний объект</param>
        public void SetChildGameObject(GameObject gameObject)
        {
            if (childGameObject != null)
                MazeScene.instance.RemoveObjectFromScene(childGameObject);

            childGameObject = gameObject;
        }

        /// <summary>
        /// Запись сетевых данных об игроке
        /// </summary>
        /// <param name="manager">Менеджер сетевого взаимодействия</param>
        public void WriteNetworkData(NetworkManager manager)
        {
            manager.CurrentPlayerNetworkData.PlayerPosition = gameObject.Transform.Position;
            manager.CurrentPlayerNetworkData.IsPlayerSpriteFlip = gameObject.Sprite.IsFlipX;
            manager.CurrentPlayerNetworkData.IsPlayerMove = isMove;
        }
        
        /// <summary>
        /// Обновление данные сетевого игрока
        /// </summary>
        /// <param name="manager">Менеджер сетевого взаимодействия</param>
        public void UpdateNetworkData(NetworkManager manager)
        {
            gameObject.Transform.Position = manager.NetworkPlayerNetworkData.PlayerPosition;
            gameObject.Sprite.IsFlipX = manager.NetworkPlayerNetworkData.IsPlayerSpriteFlip;
            isMove = manager.NetworkPlayerNetworkData.IsPlayerMove;
            UpdatePlayerFlip();
        }

        /// <summary>
        /// Поведение на момент создание игрового объекта
        /// </summary>
        public override void Start()
        {
            Animation animation;

            if (gameObject.GameObjectTag == "Blue Player")
            {
                animation = new Animation(RenderingSystem.LoadAnimation("Resources/Blue Player/left idle ", 2), 0.2f, true);
                gameObject.Sprite.AddAnimation("idleLeft", animation);
                animation = new Animation(RenderingSystem.LoadAnimation("Resources/Blue Player/left run ", 4), 0.2f, true);
                gameObject.Sprite.AddAnimation("runLeft", animation);
                animation = new Animation(RenderingSystem.LoadAnimation("Resources/Blue Player/right idle ", 2), 0.2f, true);
                gameObject.Sprite.AddAnimation("idleRight", animation);
                animation = new Animation(RenderingSystem.LoadAnimation("Resources/Blue Player/right run ", 4), 0.2f, true);
                gameObject.Sprite.AddAnimation("runRight", animation);
            }
            else
            {
                animation = new Animation(RenderingSystem.LoadAnimation("Resources/Red Player/left idle ", 2), 0.2f, true);
                gameObject.Sprite.AddAnimation("idleLeft", animation);
                animation = new Animation(RenderingSystem.LoadAnimation("Resources/Red Player/left run ", 4), 0.2f, true);
                gameObject.Sprite.AddAnimation("runLeft", animation);
                animation = new Animation(RenderingSystem.LoadAnimation("Resources/Red Player/right idle ", 2), 0.2f, true);
                gameObject.Sprite.AddAnimation("idleRight", animation);
                animation = new Animation(RenderingSystem.LoadAnimation("Resources/Red Player/right run ", 4), 0.2f, true);
                gameObject.Sprite.AddAnimation("runRight", animation);
            }

            gameObject.Sprite.SetAnimation("idleLeft");
        }

        /// <summary>
        /// Обновление игрового объекта
        /// </summary>
        public override void Update()
        {
            UpdatePlayerFlip();
        }

        /// <summary>
        /// Обновление вращения игрока
        /// </summary>
        private void UpdatePlayerFlip()
        {
            if (isMove)
            {
                if (childGameObject != null)
                    childGameObject.Sprite.IsFlipX = gameObject.Sprite.IsFlipX;

                if (childGameObject != null && childGameObject.Sprite.IsFlipX)
                    childGameObject.Sprite.SetAnimation("runLeft");
                else if (childGameObject != null)
                    childGameObject.Sprite.SetAnimation("runRight");

                if (gameObject.Sprite.IsFlipX)
                    gameObject.Sprite.SetAnimation("runLeft");
                else
                    gameObject.Sprite.SetAnimation("runRight");
            }
            else
            {
                if(childGameObject != null)
                    childGameObject.Sprite.IsFlipX = gameObject.Sprite.IsFlipX;

                if (childGameObject != null && childGameObject.Sprite.IsFlipX)
                    childGameObject.Sprite.SetAnimation("idleLeft");
                else if (childGameObject != null)
                    childGameObject.Sprite.SetAnimation("idleRight");

                if (gameObject.Sprite.IsFlipX)
                    gameObject.Sprite.SetAnimation("idleLeft");
                else
                    gameObject.Sprite.SetAnimation("idleRight");
            }
        }
    }
}