using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Epidemic_Simulation
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;    // Менеджер графических устройств
        private SpriteBatch _spriteBatch;           // Пакет для рисования спрайтов

        private MainMenu mainMenu;                  // Экземпляр класса MainMenu
        private Simulation simulation;              // Экземпляр класса Simulation

        // Перечисление состояний игры
        private enum GameState
        {
            MainMenu,    // Главное меню
            Simulation   // Симуляция
        }

        // Текущее состояние игры, начальное состояние - главное меню
        private GameState currentState = GameState.MainMenu;

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
        }

        // Метод для обновления состояния игры
        protected override void Update(GameTime gameTime)
        {
            // Проверка, была ли нажата клавиша "Escape" на клавиатуре для выхода
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Обработка логики в зависимости от текущего состояния игры
            switch (currentState)
            {
                // Обновление состояния главного меню
                case GameState.MainMenu:
                    bool exitRequested;
                    // Если выбрана опция "Start", переходим в состояние симуляции
                    if (mainMenu.Update(out exitRequested))
                    {
                        currentState = GameState.Simulation;
                    }
                    // Если выбрана опция "Exit", выходим из игры
                    else if (exitRequested)
                    {
                        Exit();
                    }
                    break;
                // Обновление состояния симуляции
                case GameState.Simulation:
                    simulation.Update(gameTime);
                    break;
            }

            // Вызов базового метода обновления
            base.Update(gameTime);
        }

        // Метод для рисования на экране
        protected override void Draw(GameTime gameTime)
        {
            // Очистка экрана заданным цветом (CornflowerBlue)
            GraphicsDevice.Clear(Color.White);

            // Начало рисования спрайтов
            _spriteBatch.Begin();

            // Рисование в зависимости от текущего состояния игры
            switch (currentState)
            {
                // Рисование главного меню
                case GameState.MainMenu:
                    // Рисование главного меню с указанием размеров экрана
                    mainMenu.Draw(_spriteBatch, _graphics);
                    break;
                // Рисование симуляции
                case GameState.Simulation:
                    // Вызов метода рисования симуляции
                    simulation.Draw(_spriteBatch, _graphics);
                    break;
            }

            // Завершение рисования спрайтов
            _spriteBatch.End();

            // Вызов базового метода рисования
            base.Draw(gameTime);
        }        
    }
}