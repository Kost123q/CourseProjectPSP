using EngineLibrary.EngineComponents;
using EngineLibrary.ObjectComponents;
using SharpDX;
using System;
using System.Drawing;
using System.Collections.Generic;
using GameLibrary.Game;
using GameLibrary.Maze.Network;
using NetworkLibrary;
using SharpDX.DirectInput;

namespace GameLibrary.Maze
{
    /// <summary>
    /// Класс лабиринта
    /// </summary>
    public class MazeScene : Scene
    {
        /// <summary>
        /// Рандом, используемый в игре
        /// </summary>
        public static Random GameRandom { get; private set; }

        /// <summary>
        /// Статическая ссылка на класс
        /// </summary>
        public static MazeScene instance = null;
        /// <summary>
        /// Фабрика создания элементов лабиринта
        /// </summary>
        public MazeElementsFactory ElementsFactory { get; private set; }
        /// <summary>
        /// Конструктор синего игрока
        /// </summary>
        public PlayerConstructor FirstPlayerConstructor { get; private set; }
        /// <summary>
        /// Конструктор красного игрока
        /// </summary>
        public PlayerConstructor SecondPlayerConstructor { get; private set; }

        private NetworkManager networkManager;

        private List<Vector2> emptyBlocks = new List<Vector2>();
        private int countOfcoins = 0;

        /// <summary>
        /// Конструктор класса для создания игры на одном экране
        /// </summary>
        public MazeScene()
        {
            GameRandom = new Random();
            FirstPlayerConstructor = new PlayerConstructor("Blue Player",
                new Player(new PlayerControl(AxisOfInput.Horizontal, AxisOfInput.Vertical, Key.Space)));
            SecondPlayerConstructor = new PlayerConstructor("Red Player",
                new Player(new PlayerControl(AxisOfInput.AlternativeHorizontal, AxisOfInput.AlternativeVertical,
                    Key.NumberPadEnter)));
        }
        
        /// <summary>
        /// Конструктор класса для создания игры по сети
        /// </summary>
        /// <param name="handler">Обработчик сетевого взаимодействия</param>
        /// <param name="seed">Сид генерации</param>
        /// <param name="hostPlayer">Тег игрока, играющего на этом компьютере</param>
        /// <param name="networkPlayer">Тег игрока, играющего по интернету</param>
        public MazeScene(INetworkHandler handler, int seed, string hostPlayer, string networkPlayer)
        {
            GameRandom = new Random(seed);
            var playerScript = new Player(new PlayerControl(AxisOfInput.Horizontal, AxisOfInput.Vertical, Key.Space));
            var networkPlayerScript = new NetworkPlayer();
            if (hostPlayer == "Blue Player")
            {
                FirstPlayerConstructor = new PlayerConstructor(hostPlayer, playerScript);
                SecondPlayerConstructor = new PlayerConstructor(networkPlayer, networkPlayerScript);
            }
            else
            {
                SecondPlayerConstructor = new PlayerConstructor(hostPlayer, playerScript);
                FirstPlayerConstructor = new PlayerConstructor(networkPlayer, networkPlayerScript);
            }
            
            networkManager = new NetworkManager(handler);
            networkManager.OnWriteData += playerScript.WriteNetworkData;
            networkManager.OnUpdateData += networkPlayerScript.UpdateNetworkData;
            OnSceneDrawed += networkManager.UpdateData;
        }

        /// <summary>
        /// Обновление пули в сети
        /// </summary>
        /// <param name="data">Сетевые данные о пули</param>
        public void UpdateBullet(BulletNetworkData data)
        {
            networkManager.CurrentPlayerNetworkData.BulletData = data;
        }

        /// <summary>
        /// Создание игровых объектов на сцене
        /// </summary>

        protected override void CreateGameObjectsOnScene()
        {
            if (instance == null)
                instance = this;

            ElementsFactory = new MazeElementsFactory();

            GameObject gameObject = new GameObject();
            gameObject.InitializeObjectComponent(new TransformComponent(new Vector2(0f, 0f), new Size2F(27, 15)));
            gameObject.InitializeObjectComponent(new SpriteComponent(RenderingSystem.LoadBitmap("Resources/Фон.png")));
            gameObject.GameObjectTag = "Background";

            gameObjects.Add(gameObject);

            gameObject = new GameObject();
            gameObject.InitializeObjectComponent(new TransformComponent(new Vector2(0f, 0f), new Size2F(1, 1)));
            gameObject.GameObjectTag = "GameManager";
            gameObject.InitializeObjectScript(new SpawnManager());

            gameObjects.Add(gameObject);

            CreateMaze();

            GameObject player = FirstPlayerConstructor.CreatePlayer();

            gameObjects.Add(player);
            gameObjects.Add(FirstPlayerConstructor.CreateArms());

            player = SecondPlayerConstructor.CreatePlayer();

            gameObjects.Add(player);
            gameObjects.Add(SecondPlayerConstructor.CreateArms());
        }

        /// <summary>
        /// Метод создания лабиринта
        /// </summary>
        public void CreateMaze()
        {
            string text = "Resources/Mazes/Maze " + GameRandom.Next(1, 6) + ".bmp";

            Bitmap bitmap = new Bitmap(text);

            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    System.Drawing.Color color = bitmap.GetPixel(j, i);

                    GameObject gameObject = null;

                    if (color.R == 0 && color.G == 0 && color.B == 0)
                        gameObject = ElementsFactory.CreateMazeElement(new Vector2(j, i), "Wall");
                    else if (color.R == 255 && color.G == 0 && color.B == 0)
                        gameObject = ElementsFactory.CreateMazeElement(new Vector2(j, i), "BreakWall");
                    else if (color.R == 0 && color.G == 255 && color.B == 0)
                        gameObject = ElementsFactory.CreateMazeElement(new Vector2(j, i), "Platform");
                    else if (color.R == 0 && color.G == 0 && color.B == 255)
                        gameObject = ElementsFactory.CreateMazeElement(new Vector2(j, i), "Stair");
                    else if (color.R == 255 && color.G == 255 && color.B == 0)
                    {
                        gameObject = ElementsFactory.CreateCoin(new Vector2(j, i));
                        countOfcoins++;
                    }
                    else if (color.R == 125 && color.G == 0 && color.B == 0)
                        SecondPlayerConstructor.StartPosition = new Vector2(j, i);
                    else if (color.R == 0 && color.G == 0 && color.B == 125)
                        FirstPlayerConstructor.StartPosition = new Vector2(j, i);
                    else
                        emptyBlocks.Add(new Vector2(j, i));

                    if (gameObject != null)
                        gameObjects.Add(gameObject);
                }
            }
        }

        /// <summary>
        /// Добавление объекта в лист отрисовки
        /// </summary>
        /// <param name="gameObject">Игровой объект</param>
        public void AddObjectOnScene(GameObject gameObject)
        {
            gameObjectsToAdd.Add(gameObject);
        }

        /// <summary>
        /// Удаления объекта из листа отрисовки
        /// </summary>
        /// <param name="gameObject">Игровой объект</param>
        public void RemoveObjectFromScene(GameObject gameObject)
        {
            if (gameObject.GameObjectTag == "Spawn")
                emptyBlocks.Add(gameObject.Transform.Position);

            gameObjectsToRemove.Add(gameObject);

            if (gameObject.GameObjectTag == "Coin")
            {
                countOfcoins--;
                if(countOfcoins == 0)
                    EndScene();
            }
        }

        /// <summary>
        /// Рандомное место в лабиринте
        /// </summary>
        /// <returns>Позицию</returns>
        public Vector2 GetRandomPosition()
        {
            int index = GameRandom.Next(0, emptyBlocks.Count);

            Vector2 position = emptyBlocks[index];

            emptyBlocks.Remove(position);

            return position;
        }

        /// <summary>
        /// Поведение при завершении сцены
        /// </summary>
        protected override void EndScene()
        {
            base.EndScene();

            string winPlayer = "";
            int firstPlayerCountCoins = ((BasePlayer) FirstPlayerConstructor.PlayerGameObject.Script).Coins;
            int secondPlayerCountCoins = ((BasePlayer) SecondPlayerConstructor.PlayerGameObject.Script).Coins;

            if(firstPlayerCountCoins < secondPlayerCountCoins)
                winPlayer = SecondPlayerConstructor.PlayerTag;
            else
                winPlayer = FirstPlayerConstructor.PlayerTag;

            GameEvents.EndGame?.Invoke(winPlayer);
        }
    }
}
