using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        public MainMenu(Texture2D backgroundTexture, Texture2D startButtonTexture, Texture2D exitButtonTexture, int screenWidth, int screenHeight)
        {
            // Инициализация текстуры фона
            this.backgroundTexture = backgroundTexture;
            // Инициализация текстуры кнопки "Start"
            this.startButtonTexture = startButtonTexture;
            // Инициализация текстуры кнопки "Exit"
            this.exitButtonTexture = exitButtonTexture;

            // Задание ширины и высоты кнопок
            int buttonWidth = 293;
            int buttonHeight = 77;

            // Задание координат X и Y для кнопки "Start"
            int startButtonX = 778;
            int startButtonY = 312;

            // Задание координат X и Y для кнопки "Exit"
            int exitButtonX = startButtonX;
            int exitButtonY = 396;

            // Устанавливаем область кнопки "Start" на экране
            startButtonRectangle = new Rectangle(startButtonX, startButtonY, buttonWidth, buttonHeight);
            // Устанавливаем область кнопки "Exit" на экране
            exitButtonRectangle = new Rectangle(exitButtonX, exitButtonY, buttonWidth, buttonHeight);
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
        public void Draw(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
        {
            // Рисование фона
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);

            // Рисование кнопок
            spriteBatch.Draw(startButtonTexture, startButtonRectangle, Color.White);
            spriteBatch.Draw(exitButtonTexture, exitButtonRectangle, Color.White);
        }
    }
}