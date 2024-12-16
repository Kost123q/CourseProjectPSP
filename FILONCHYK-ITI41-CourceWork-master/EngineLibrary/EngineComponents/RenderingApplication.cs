﻿using System;
using System.Drawing;
using SharpDX.Windows;
using SharpDX.Direct2D1;
using System.Windows.Forms;

namespace EngineLibrary.EngineComponents
{
    /// <summary>
    /// Класс отрисовки и обновление всех процессов
    /// </summary>
    public class RenderingApplication : IDisposable
    {
        private const float unitsPerHeight = 18.9f;

        private const int widthOfApplication = 1280;
        private const int heightOfApplication = 720;

        private WindowRenderTarget renderTarget;

        private RenderingSystem rendering;
        private InputHandler input;
        private Scene scene;

        public bool IsFocused { get; set; } = true;

        /// <summary>
        /// Форма отрисовки
        /// </summary>
        public RenderForm RenderForm { get; private set; }

        public InputHandler InputHandler
        {
            get => default;
            set
            {
            }
        }

        public RenderingSystem RenderingSystem
        {
            get => default;
            set
            {
            }
        }

        public Scene Scene
        {
            get => default;
            set
            {
            }
        }

        /// <summary>
        /// Конструктор класса, инциализирующий его компоненты
        /// </summary>
        public RenderingApplication()
        {
            RenderForm = new RenderForm("Game");
            RenderForm.ClientSize = new Size(widthOfApplication, heightOfApplication);
            RenderForm.TopLevel = false;
            RenderForm.AllowUserResizing = false;
            RenderForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            rendering = new RenderingSystem(RenderForm);
            renderTarget = rendering.RenderTarget;
            input = new InputHandler(RenderForm);
            Input.SetupInputHandler(input);
        }

        /// <summary>
        /// Установка игровой сцены
        /// </summary>
        /// <param name="targetScene"></param>
        public void SetScene(Scene targetScene)
        {
            scene = targetScene;
            scene.InitializeScene(renderTarget, RenderForm.ClientSize.Height / unitsPerHeight);
        }

        /// <summary>
        /// Запуск приложения
        /// </summary>
        public void Run()
        {
            RenderLoop.Run(RenderForm, RenderCallback);
        }

        /// <summary>
        /// Формирование каждого кадра
        /// </summary>
        private void RenderCallback()
        {
            if (IsFocused)
            {
                input.UpdateKeyboardState();
            }
            
            Time.UpdateTime();

            renderTarget.BeginDraw();
            renderTarget.Clear(SharpDX.Color.Black);

            scene.DrawScene();

            renderTarget.EndDraw();

            if (!scene.IsDrawScene)
            {
                Dispose();
            }
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            RenderForm.Dispose();
            rendering.Dispose();
            input.Dispose();
        }
    }
}
