using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Epidemic_Simulation
{
    public class MainMenu
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
        public bool Update(out bool exitRequested)
        {
            exitRequested = false;              // Флаг для запроса выхода из игры
            var mouseState = Mouse.GetState();  // Получаем текущее состояние мыши

            // Проверяем, нажата ли левая кнопка мыши
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // Если нажата кнопка "Start"
                if (startButtonRectangle.Contains(mouseState.Position))
                {
                    return true;    // Запускаем симуляцию
                }
                // Если нажата кнопка "Exit"
                else if (exitButtonRectangle.Contains(mouseState.Position))
                {
                    exitRequested = true;   // Запрашиваем выход из игры
                }
            }
            return false;   // Возвращаем false, если ничего не было нажато
        }

        // Метод для рисования главного меню
        public void Draw(SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {
            // Рисование фона
            _spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            // Рисование кнопок
            _spriteBatch.Draw(startButtonTexture, startButtonRectangle, Color.White);
            _spriteBatch.Draw(exitButtonTexture, exitButtonRectangle, Color.White);
        }
    }
}