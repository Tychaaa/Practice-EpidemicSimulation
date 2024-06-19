using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Epidemic_Simulation
{
    // Интерфейс для экранов (экран меню, экран симуляции и т.д.)
    public interface IScreen
    {
        // Метод для загрузки контента экрана
        void LoadContent(ContentManager content);

        // Метод для обновления состояния экрана
        // Параметры:
        // gameTime - игровое время
        // requestStateChange - флаг для запроса смены состояния
        // exitRequested - флаг для запроса выхода из игры
        void Update(GameTime gameTime, out bool requestStateChange, out bool exitRequested);

        // Метод для рисования экрана
        // Параметры:
        // spriteBatch - пакет для рисования спрайтов
        // graphics - менеджер графических устройств
        void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics);
    }

}