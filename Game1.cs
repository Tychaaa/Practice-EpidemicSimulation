using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace Epidemic_Simulation
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;    // Менеджер графических устройств
        private SpriteBatch _spriteBatch;           // Пакет для рисования спрайтов

        Texture2D personTexture;                    // Текстура для отображения объекта Person
        private Texture2D backgroundTexture;        // Текстура для фона главного меню
        private Texture2D backgroundSimulationTexture; // Текстура для фона симуляции
        private Texture2D startButtonTexture;       // Текстура кнопки "Start"
        private Texture2D exitButtonTexture;        // Текстура кнопки "Exit"
        private Texture2D rectangleTexture;         // Текстура для прямоугольника
        private Texture2D trackTexture;             // Текстура для дорожки ползунка
        private Texture2D thumbTexture;             // Текстура для ползунка
        private SpriteFont font;                    // Шрифт для отображения текста

        private Rectangle simulationArea;           // Прямоугольник для области симуляции

        Random random;                              // Генератор случайных чисел

        List<Person> people;                        // Список объектов Person
        int numberOfPeople = 70;                    // Количество объектов Person
        float infectionRadius = 30f;                // Радиус заражения
        float infectionChance = 0.3f;               // Шанс заражения
        float deathChance = 0.05f;                  // Шанс смерти от инфекции

        private MainMenu mainMenu;                  // Экземпляр класса MainMenu

        private Slider infectionChanceSlider;       // Ползунок для шанса заражения
        private Slider deathChanceSlider;           // Ползунок для шанса смерти
        private Slider infectionRadiusSlider;       // Ползунок для радиуса заражения
        private Slider speedSlider;                 // Ползунок для скорости объектов
        private Slider incubationPeriodSlider;      // Ползунок для инкубационного периода
        private Slider infectionPeriodSlider;       // Ползунок для периода инфекции

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
            random = new Random();          // Инициализация генератора случайных чисел
            people = new List<Person>();    // Инициализация списка объектов Person

            // Вызов базового метода инициализации
            base.Initialize();
        }

        // Метод для загрузки контента игры
        protected override void LoadContent()
        {
            // Создание экземпляра SpriteBatch для рисования спрайтов
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // Загрузка текстуры для объекта Person из каталога контента
            personTexture = Content.Load<Texture2D>("person");
            // Загрузка текстуры кнопки "Start" из каталога контента
            startButtonTexture = Content.Load<Texture2D>("startButton");
            // Загрузка текстуры кнопки "Exit" из каталога контента
            exitButtonTexture = Content.Load<Texture2D>("exitButton");
            // Загрузка текстуры фона из каталога контента
            backgroundTexture = Content.Load<Texture2D>("background");
            // Загрузка текстуры фона симуляции из каталога контента
            backgroundSimulationTexture = Content.Load<Texture2D>("backgroundSimulation");
            // Загрузка текстуры прямоугольника
            rectangleTexture = Content.Load<Texture2D>("simulationRectangle");
            // Загрузка текстур ползунка
            trackTexture = Content.Load<Texture2D>("track");
            thumbTexture = Content.Load<Texture2D>("thumb");
            // Загрузка шрифта
            font = Content.Load<SpriteFont>("font");

            // Инициализация прямоугольника области симуляции
            simulationArea = new Rectangle(10, 12, rectangleTexture.Width - 10, rectangleTexture.Height - 10);

            // Создание экземпляра главного меню
            mainMenu = new MainMenu(backgroundTexture, startButtonTexture, exitButtonTexture, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            // Создание ползунков
            infectionChanceSlider = new Slider(trackTexture, thumbTexture, font, "Infection Chance", 1040, 50, 200, 0, 100, (int)(infectionChance * 100));
            deathChanceSlider = new Slider(trackTexture, thumbTexture, font, "Death Chance", 1040, 150, 200, 0, 100, (int)(deathChance * 100));
            infectionRadiusSlider = new Slider(trackTexture, thumbTexture, font, "Infection Radius", 1040, 250, 200, 30, 100, (int)(infectionRadius));
            speedSlider = new Slider(trackTexture, thumbTexture, font, "Speed", 1040, 350, 200, 10, 200, (int)(Person.defaultSpeed));
            incubationPeriodSlider = new Slider(trackTexture, thumbTexture, font, "Incubation Period", 1040, 450, 200, 0, 10, Person.defaultIncubationPeriod);
            infectionPeriodSlider = new Slider(trackTexture, thumbTexture, font, "Infection Period", 1040, 550, 200, 1, 15, Person.defaultInfectionPeriod);

            // Создание объектов Person и добавление их в список
            for (int i = 0; i < numberOfPeople; i++)
            {
                // Генерация случайной позиции внутри границ экрана
                var position = GenerateRandomPosition(personTexture.Width, personTexture.Height);
                // Создание нового объекта Person
                var person = new Person(position, personTexture.Width);
                // Добавление объекта в список
                people.Add(person);
            }

            // Заражение объектов в списке
            for (int num = 0; num < 3; num++)
            {
                people[num].Infect();
            }
        }

        // Метод для генерации случайной позиции внутри границ экрана с учетом размеров текстуры
        private Vector2 GenerateRandomPosition(int textureWidth, int textureHeight)
        {
            // Генерация случайной координаты X
            // Начинаем от половины ширины текстуры, чтобы текстура не выходила за левую границу
            // и заканчиваем на расстоянии половины ширины текстуры от правой границы
            int x = random.Next(simulationArea.Left + textureWidth, simulationArea.Right - textureWidth / 2);

            // Генерация случайной координаты Y
            // Начинаем от половины высоты текстуры, чтобы текстура не выходила за верхнюю границу
            // и заканчиваем на расстоянии половины высоты текстуры от нижней границы
            int y = random.Next(simulationArea.Top + textureHeight, simulationArea.Bottom - textureHeight / 2);

            // Возвращаем сгенерированную случайную позицию как вектор
            return new Vector2(x, y);
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
                    UpdateSimulation(gameTime);
                    infectionChanceSlider.Update();
                    infectionChance = infectionChanceSlider.GetValue() / 100f;

                    deathChanceSlider.Update();
                    deathChance = deathChanceSlider.GetValue() / 100f;

                    infectionRadiusSlider.Update();
                    infectionRadius = infectionRadiusSlider.GetValue();

                    speedSlider.Update();
                    float newSpeed = speedSlider.GetValue();
                    foreach (var person in people)
                    {
                        person.SetSpeed(newSpeed);
                    }

                    incubationPeriodSlider.Update();
                    Person.defaultIncubationPeriod = incubationPeriodSlider.GetValue();

                    infectionPeriodSlider.Update();
                    Person.defaultInfectionPeriod = infectionPeriodSlider.GetValue();
                    break;
            }

            // Вызов базового метода обновления
            base.Update(gameTime);
        }

        // Метод для обновления состояния симуляции
        private void UpdateSimulation(GameTime gameTime)
        {
            // Обновление состояния каждого объекта Person
            for (int i = 0; i < people.Count; i++)
            {
                var person = people[i];
                // Обновление состояния и позиции объекта Person
                person.Update(gameTime, simulationArea, person.Position, deathChance);

                // Проверка столкновений и инфекций с другими объектами Person
                for (int j = i + 1; j < people.Count; j++)
                {
                    var otherPerson = people[j];
                    // Вычисление расстояния между двумя объектами Person
                    float distance = Vector2.Distance(person.Position, otherPerson.Position);
                    person.CheckCollision(otherPerson);

                    // Проверка условия заражения для первого объекта
                    if ((person.IsInfected || person.IsCarrier) && !otherPerson.IsInfected && !otherPerson.IsCarrier && !otherPerson.IsDead && distance < infectionRadius && random.NextDouble() < infectionChance)
                    {
                        // Заражение второго объекта
                        otherPerson.Infect();
                    }

                    // Проверка условия заражения для второго объекта
                    if ((otherPerson.IsInfected || otherPerson.IsCarrier) && !person.IsInfected && !person.IsCarrier && !person.IsDead && distance < infectionRadius && random.NextDouble() < infectionChance)
                    {
                        // Заражение первого объекта
                        person.Infect();
                    }
                }
            }

            // Проверка и устранение застревания объектов в границах
            CheckAndResolveBoundarySticking();
        }

        // Метод для проверки и устранения застревания объектов Person в границах прямоугольника
        private void CheckAndResolveBoundarySticking()
        {
            // Проход по каждому объекту Person в списке people
            foreach (var person in people)
            {
                if (person.IsDead)
                    continue; // Пропускаем мертвых

                bool stuck = false;     // Флаг, указывающий, застрял ли объект

                // Проверка застревания в левой или правой границе
                if (person.Position.X - person.Radius <= simulationArea.Left)
                {
                    // Если объект застрял в левой границе, переместим его внутрь
                    person.Position = new Vector2(simulationArea.Left + person.Radius, person.Position.Y);
                    // Установим направление движения в сторону от границы
                    person.SetDirection(new Vector2(Math.Abs(person.GetDirection().X), person.GetDirection().Y)); // Направление в сторону от границы
                    stuck = true;
                }
                else if (person.Position.X + person.Radius >= simulationArea.Right)
                {
                    // Если объект застрял в правой границе, переместим его внутрь
                    person.Position = new Vector2(simulationArea.Right - person.Radius, person.Position.Y);
                    // Установим направление движения в сторону от границы
                    person.SetDirection(new Vector2(-Math.Abs(person.GetDirection().X), person.GetDirection().Y)); // Направление в сторону от границы
                    stuck = true;
                }

                // Проверка застревания в верхней или нижней границе
                if (person.Position.Y - person.Radius <= simulationArea.Top)
                {
                    // Если объект застрял в верхней границе, переместим его внутрь
                    person.Position = new Vector2(person.Position.X, simulationArea.Top + person.Radius);
                    // Установим направление движения в сторону от границы
                    person.SetDirection(new Vector2(person.GetDirection().X, Math.Abs(person.GetDirection().Y))); // Направление в сторону от границы
                    stuck = true;
                }
                else if (person.Position.Y + person.Radius >= simulationArea.Bottom)
                {
                    // Если объект застрял в нижней границе, переместим его внутрь
                    person.Position = new Vector2(person.Position.X, simulationArea.Bottom - person.Radius);
                    // Установим направление движения в сторону от границы
                    person.SetDirection(new Vector2(person.GetDirection().X, -Math.Abs(person.GetDirection().Y))); // Направление в сторону от границы
                    stuck = true;
                }

                // Если объект застрял, скорректируем его направление случайным образом
                if (stuck)
                {
                    Vector2 newDirection = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
                    person.SetDirection(newDirection);
                }
            }
        }

        // Метод для рисования на экране
        protected override void Draw(GameTime gameTime)
        {
            // Очистка экрана заданным цветом (CornflowerBlue)
            GraphicsDevice.Clear(Color.DarkSeaGreen);

            // Начало рисования спрайтов
            _spriteBatch.Begin();

            // Рисование в зависимости от текущего состояния игры
            switch (currentState)
            {
                // Рисование главного меню
                case GameState.MainMenu:
                    mainMenu.Draw(_spriteBatch, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                    break;
                // Рисование симуляции
                case GameState.Simulation:
                    DrawSimulation();
                    infectionChanceSlider.Draw(_spriteBatch);
                    deathChanceSlider.Draw(_spriteBatch);
                    infectionRadiusSlider.Draw(_spriteBatch);
                    speedSlider.Draw(_spriteBatch);
                    incubationPeriodSlider.Draw(_spriteBatch);
                    infectionPeriodSlider.Draw(_spriteBatch);
                    break;
            }

            // Завершение рисования спрайтов
            _spriteBatch.End();

            // Вызов базового метода рисования
            base.Draw(gameTime);
        }

        // Метод для рисования симуляции на экране
        private void DrawSimulation()
        {
            // Рисование фона симуляции
            _spriteBatch.Draw(backgroundSimulationTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            // Рисование прямоугольника для области симуляции
            _spriteBatch.Draw(rectangleTexture, simulationArea, Color.White);

            // Сначала рисуем мертвые объекты Person
            foreach (var person in people)
            {
                if (person.IsDead)
                {
                    // Определение цвета для рисования мертвого объекта (черный)
                    Color color = Color.Black;

                    // Рисование мертвого объекта Person с заданной текстурой и цветом
                    _spriteBatch.Draw(
                        personTexture,             // Текстура для рисования
                        person.Position,           // Позиция объекта
                        null,                      // Область текстуры для рисования (null - вся текстура)
                        color,                     // Цвет для рисования
                        0f,                        // Угол поворота (0 - без поворота)
                        new Vector2(personTexture.Width / 2, personTexture.Height / 2), // Точка происхождения (центр текстуры)
                        Vector2.One,               // Масштаб (Vector2.One - без изменения размера)
                        SpriteEffects.None,        // Эффекты для спрайта (без эффектов)
                        0f                         // Слой отрисовки (0 - самый нижний слой)
                    );
                }
            }

            // Затем рисуем все остальные объекты Person
            foreach (var person in people)
            {
                if (!person.IsDead)
                {
                    // Определение цвета для рисования на основе состояния объекта Person
                    Color color = person.IsInfected ? Color.Red :                   // Красный, если объект заражен
                                  person.IsCarrier ? Color.LightPink :              // Светло-розовый, если объект носитель
                                  (person.IsRecovered ? Color.Gray : Color.White);  // Серый, если объект выздоровел, иначе белый

                    // Рисование живого объекта Person с заданной текстурой и цветом
                    _spriteBatch.Draw(
                        personTexture,             // Текстура для рисования
                        person.Position,           // Позиция объекта
                        null,                      // Область текстуры для рисования (null - вся текстура)
                        color,                     // Цвет для рисования
                        0f,                        // Угол поворота (0 - без поворота)
                        new Vector2(personTexture.Width / 2, personTexture.Height / 2), // Точка происхождения (центр текстуры)
                        Vector2.One,               // Масштаб (Vector2.One - без изменения размера)
                        SpriteEffects.None,        // Эффекты для спрайта (без эффектов)
                        0f                         // Слой отрисовки (0 - самый нижний слой)
                    );
                }
            }
        }
    }
}