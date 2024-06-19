using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Epidemic_Simulation
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;    // Менеджер графических устройств
        private SpriteBatch _spriteBatch;           // Пакет для рисования спрайтов

        private IScreen currentScreen;              // Текущий экран (главное меню или симуляция)
        private MainMenu mainMenu;                  // Экземпляр класса MainMenu
        private Simulation simulation;              // Экземпляр класса Simulation
        private Song menuTheme;                     // Тема для меню

        // Конструктор класса Game1
        public Game1()
        {
            // Инициализация менеджера графических устройств
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,    // Установка ширины окна
                PreferredBackBufferHeight = 720     // Установка высоты окна
            };

            // Установка корневого каталога для контента
            Content.RootDirectory = "Content";
            // Включение видимости курсора мыши
            IsMouseVisible = true;

            //_graphics.IsFullScreen = true;
        }

        // Метод для инициализации игры
        protected override void Initialize()
        {
            // Вызов базового метода инициализации
            base.Initialize();
        }

        // Метод для загрузки контента игры
        protected override void LoadContent()
        {
            // Создание экземпляра SpriteBatch для рисования спрайтов
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Создание экземпляра главного меню
            mainMenu = new MainMenu(Content);

            // Создание и загрузка контента для симуляции
            simulation = new Simulation(GraphicsDevice, Content);

            // Установка начального экрана
            currentScreen = mainMenu;

            // Загрузка и воспроизведение музыки
            menuTheme = Content.Load<Song>("MenuTheme");
            MediaPlayer.IsRepeating = true;  // Установка повтора музыки
            MediaPlayer.Volume = 0.05f;      // Установка громкости
            MediaPlayer.Play(menuTheme);     // Воспроизведение фоновой музыки
        }

        // Метод для обновления состояния игры
        protected override void Update(GameTime gameTime)
        {
            // Проверка, была ли нажата клавиша "Escape" на клавиатуре для выхода
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Обновление текущего экрана
            currentScreen.Update(gameTime, out bool requestStateChange, out bool exitRequested);
            if (exitRequested)
            {
                Exit();  // Выход из игры, если запрос на выход
            }
            else if (requestStateChange)
            {
                // Переход между главным меню и симуляцией
                currentScreen = currentScreen == mainMenu ? (IScreen)simulation : (IScreen)mainMenu;
            }

            // Вызов базового метода обновления
            base.Update(gameTime);
        }

        // Метод для рисования на экране
        protected override void Draw(GameTime gameTime)
        {
            // Очистка экрана заданным цветом (White)
            GraphicsDevice.Clear(Color.White);

            // Начало рисования спрайтов
            _spriteBatch.Begin();

            // Рисование текущего экрана
            currentScreen.Draw(_spriteBatch, _graphics);

            // Завершение рисования спрайтов
            _spriteBatch.End();

            // Вызов базового метода рисования
            base.Draw(gameTime);
        }
    }
}
