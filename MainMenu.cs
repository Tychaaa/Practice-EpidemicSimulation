using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Epidemic_Simulation
{
    public class MainMenu : IScreen
    {
        private Texture2D backgroundTexture;        // Текстура для фона главного меню
        private Texture2D startButtonTexture;       // Текстура для кнопки "Start"
        private Texture2D exitButtonTexture;        // Текстура для кнопки "Exit"
        private Rectangle startButtonRectangle;     // Область на экране для кнопки "Start"
        private Rectangle exitButtonRectangle;      // Область на экране для кнопки "Exit"

        // Конструктор класса MainMenu
        public MainMenu(ContentManager content)
        {
            LoadContent(content);
            Initialize();
        }

        // Метод для инициализации окна главного меню
        private void Initialize()
        {
            // Задание ширины и высоты кнопок
            int buttonWidth = 293;
            int buttonHeight = 77;

            // Устанавливаем область кнопки "Start" на экране
            startButtonRectangle = new Rectangle(778, 312, buttonWidth, buttonHeight);
            // Устанавливаем область кнопки "Exit" на экране
            exitButtonRectangle = new Rectangle(778, 396, buttonWidth, buttonHeight);
        }

        // Метод для загрузки контента главного меню
        public void LoadContent(ContentManager content)
        {
            // Загрузка текстуры кнопки "Start" из каталога контента
            startButtonTexture = content.Load<Texture2D>("startButton");
            // Загрузка текстуры кнопки "Exit" из каталога контента
            exitButtonTexture = content.Load<Texture2D>("exitButton");
            // Загрузка текстуры фона из каталога контента
            backgroundTexture = content.Load<Texture2D>("background");
        }

        // Метод для обновления состояния главного меню
        public void Update(GameTime gameTime, out bool requestStateChange, out bool exitRequested)
        {
            requestStateChange = false;  // Флаг для запроса изменения состояния
            exitRequested = false;       // Флаг для запроса выхода из игры
            var mouseState = Mouse.GetState();  // Получаем текущее состояние мыши

            // Проверяем, нажата ли левая кнопка мыши
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // Если нажата кнопка "Start"
                if (startButtonRectangle.Contains(mouseState.Position))
                {
                    requestStateChange = true;  // Запрашиваем переход к симуляции
                }
                // Если нажата кнопка "Exit"
                else if (exitButtonRectangle.Contains(mouseState.Position))
                {
                    exitRequested = true;  // Запрашиваем выход из игры
                }
            }
        }

        // Метод для рисования главного меню
        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            // Рисование фона
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            // Рисование кнопок
            spriteBatch.Draw(startButtonTexture, startButtonRectangle, Color.White);
            spriteBatch.Draw(exitButtonTexture, exitButtonRectangle, Color.White);
        }
    }
}