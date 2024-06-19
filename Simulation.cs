using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using static Epidemic_Simulation.Person;
using System.Linq;

namespace Epidemic_Simulation
{
    public class Simulation : IScreen
    {
        private Texture2D personTexture;                    // Текстура для отображения объекта Person
        private Texture2D backgroundSimulationTexture;      // Текстура для фона симуляции
        private Texture2D simulationStartButtonTexture;     // Текстура для кнопки "Start" в окне симуляции
        private Texture2D simulationResetButtonTexture;     // Текстура для кнопки "Reset" в окне симуляции
        private Texture2D simulationBackButtonTexture;      // Текстура для кнопки "Reset" в окне симуляции
        private Texture2D rectangleTexture;                 // Текстура для прямоугольника
        private Texture2D trackTexture;                     // Текстура для дорожки ползунка
        private Texture2D thumbTexture;                     // Текстура для ползунка
        private Texture2D circleTexture;                    // Текстура для отображения радиуса заражения
        private Texture2D countPeopleRectangleTexture;      // Текстура для прямоугольника для отображения количества пациентов
        private Texture2D graphBackgroundTexture;           // Текстура для фона графика

        private SpriteFont font;                            // Шрифт для отображения текста

        private Random random;                              // Генератор случайных чисел
        private List<Person> people;                        // Список объектов Person

        private static int defaultNumberOfPeople = 70;         // Количество объектов Person по умолчанию
        private static float defaultInfectionRadius = 30f;     // Радиус заражения по умолчанию
        private static float defaultInfectionChance = 0.3f;    // Шанс заражения по умолчанию
        private static float defaultDeathChance = 0.05f;       // Шанс смерти от инфекции по умолчанию
        private static float defaultSpeed = 70f;               // Скорость по умолчанию
        private static int defaultIncubationPeriod = 5;        // Инкубационный период по умолчанию
        private static int defaultInfectionPeriod = 10;        // Период инфекции по умолчанию

        private int numberOfPeople = defaultNumberOfPeople;         // Количество объектов Person
        private float infectionRadius = defaultInfectionRadius;     // Радиус заражения
        private float infectionChance = defaultInfectionChance;     // Шанс заражения
        private float deathChance = defaultDeathChance;             // Шанс смерти от инфекции

        private Slider infectionChanceSlider;       // Ползунок для шанса заражения
        private Slider deathChanceSlider;           // Ползунок для шанса смерти
        private Slider infectionRadiusSlider;       // Ползунок для радиуса заражения
        private Slider speedSlider;                 // Ползунок для скорости объектов
        private Slider incubationPeriodSlider;      // Ползунок для инкубационного периода
        private Slider infectionPeriodSlider;       // Ползунок для периода инфекции
        private Slider numberOfPeopleSlider;        // Ползунок для количества людей

        private Rectangle countPeopleRectangle;             // Прямоугольник для отображения текущего количества зараженных
        private Rectangle graphRectangle;                   // Прямоугольник для отображения графика
        private Rectangle simulationArea;                   // Прямоугольник для области симуляции
        private Rectangle simulationStartButtonRectangle;   // Прямоугольник для кнопки "Start" в окне симуляции
        private Rectangle simulationResetButtonRectangle;   // Прямоугольник для кнопки "Reset" в окне симуляции
        private Rectangle simulationBackButtonRectangle;    // Прямоугольник для кнопки "Back" в окне симуляции

        private bool simulationStarted = false;             // Флаг для отслеживания начала симуляции

        private InfectionGraph graph;                       // График состояния здоровья людей                  

        // Конструктор класса окна симуляции
        public Simulation(GraphicsDevice graphicsDevice, ContentManager content)
        {
            LoadContent(content);
            Initialize(graphicsDevice);
        }

        // Метод для инициализации окна симуляции
        private void Initialize(GraphicsDevice graphicsDevice)
        {
            people = new List<Person>();    // Инициализация списка объектов Person
            random = new Random();          // Инициализация генератора случайных чисел

            // Инициализация прямоугольника области симуляции
            simulationArea = new Rectangle(10, 12, rectangleTexture.Width - 10, rectangleTexture.Height - 10);

            // Инициализация прямоугольника для отображения количества пациентов
            countPeopleRectangle = new Rectangle(10, 600, countPeopleRectangleTexture.Width, countPeopleRectangleTexture.Height);

            // Инициализация графика состояния здоровья
            graph = new InfectionGraph(graphicsDevice, 100, 718, 90, new Vector2(289, 610));

            // Инициализация прямоугольника для отображения фона графика
            graphRectangle = new Rectangle(285, 600, graphBackgroundTexture.Width, graphBackgroundTexture.Height);

            // Инициализация ползунков для настройки параметров симуляции
            InitializeSliders();

            // Инициализация прямоугольников для кнопок "Start" и "Reset" в окне симуляции
            InitializeButtonRectangles();
        }

        // Метод для инициализации ползунков
        private void InitializeSliders()
        {
            int initialY = 40;      // Начальная координата Y для первого ползунка
            int offsetY = 85;       // Расстояние между ползунками

            // Создание ползунка для регулировки шанса заражения
            infectionChanceSlider = new Slider(trackTexture, thumbTexture, font, "Infection Chance", 1040, initialY, 200, 0, 100, (int)(infectionChance * 100), "%");
            // Создание ползунка для регулировки шанса смерти
            deathChanceSlider = new Slider(trackTexture, thumbTexture, font, "Death Chance", 1040, initialY + offsetY, 200, 0, 100, (int)(deathChance * 100), "%");
            // Создание ползунка для регулировки радиуса заражения
            infectionRadiusSlider = new Slider(trackTexture, thumbTexture, font, "Infection Radius", 1040, initialY + 2 * offsetY, 200, 30, 100, (int)(infectionRadius), " units");
            // Создание ползунка для регулировки скорости объектов
            speedSlider = new Slider(trackTexture, thumbTexture, font, "Speed", 1040, initialY + 3 * offsetY, 200, 10, 200, (int)(Person.defaultSpeed), " units/sec");
            // Создание ползунка для регулировки инкубационного периода
            incubationPeriodSlider = new Slider(trackTexture, thumbTexture, font, "Incubation Period", 1040, initialY + 4 * offsetY, 200, 0, 10, Person.defaultIncubationPeriod, " sec");
            // Создание ползунка для регулировки периода инфекции
            infectionPeriodSlider = new Slider(trackTexture, thumbTexture, font, "Infection Period", 1040, initialY + 5 * offsetY, 200, 1, 15, Person.defaultInfectionPeriod, " sec");
            // Создание ползунка для регулировки количества людей
            numberOfPeopleSlider = new Slider(trackTexture, thumbTexture, font, "Number of People", 1040, initialY + 6 * offsetY, 200, 50, 150, numberOfPeople, " people");
        }

        // Метод для инициализации прямоугольников для кнопок "Start" и "Reset" в окне симуляции
        private void InitializeButtonRectangles()
        {
            // Прямоугольник для кнопки "Start" в окне симуляции
            simulationStartButtonRectangle = new Rectangle(1025, 593, simulationStartButtonTexture.Width, simulationStartButtonTexture.Height);
            // Прямоугольник для кнопки "Reset" в окне симуляции
            simulationResetButtonRectangle = new Rectangle(1139, 653, simulationResetButtonTexture.Width, simulationResetButtonTexture.Height);
            // Прямоугольник для кнопки "Back" в окне симуляции
            simulationBackButtonRectangle = new Rectangle(1025, 653, simulationBackButtonTexture.Width, simulationBackButtonTexture.Height);
        }

        // Метод для загрузки контента симуляции
        public void LoadContent(ContentManager content)
        {
            // Загрузка текстуры для объекта Person из каталога контента
            personTexture = content.Load<Texture2D>("person");
            // Загрузка текстуры для кнопки "Start" в окне симуляции
            simulationStartButtonTexture = content.Load<Texture2D>("sim_startButton");
            // Загрузка текстуры для кнопки "Reset" в окне симуляции
            simulationResetButtonTexture = content.Load<Texture2D>("sim_resetButton");
            // Загрузка текстуры для кнопки "Back" в окне симуляции
            simulationBackButtonTexture = content.Load<Texture2D>("sim_backButton");
            // Загрузка текстуры фона симуляции из каталога контента
            backgroundSimulationTexture = content.Load<Texture2D>("backgroundSimulation");
            // Загрузка текстур ползунка
            trackTexture = content.Load<Texture2D>("track");
            thumbTexture = content.Load<Texture2D>("thumb");
            // Загрузка текстуры прямоугольника
            rectangleTexture = content.Load<Texture2D>("simulationRectangle");
            // Загрузка текстуры для радиуса заражения
            circleTexture = content.Load<Texture2D>("circle");
            // Загрузка текстуры для прямоугольника для отображения количества пациентов
            countPeopleRectangleTexture = content.Load<Texture2D>("countPeople_rectangle");
            // Загрузка шрифта
            font = content.Load<SpriteFont>("font");
            // Загрузка текстуры для фона графика
            graphBackgroundTexture = content.Load<Texture2D>("graphBackgroundTexture");
        }

        // Метод для обновления состояния симуляции
        public void Update(GameTime gameTime, out bool requestStateChange, out bool exitRequested)
        {
            requestStateChange = false;  // Флаг для запроса выхода в главное меню
            exitRequested = false;       // Симуляция не требует выхода из игры

            // Обновление настроек симуляции с использованием ползунков
            UpdateSimulationSettings();
            // Если симуляция не запущена
            if (!simulationStarted)
            {
                // Проверка, нажата ли кнопка "Start" для запуска симуляции
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && simulationStartButtonRectangle.Contains(Mouse.GetState().Position))
                {
                    simulationStarted = true;
                    InitializePeople();         // Инициализация объектов Person
                }
            }
            else
            {
                // Обновление состояния симуляции
                UpdateSimulation(gameTime);
            }

            // Проверка, нажата ли кнопка "Reset" для сброса симуляции
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && simulationResetButtonRectangle.Contains(Mouse.GetState().Position))
            {
                simulationStarted = false;
                ResetSimulation();              // Сброс симуляции
            }

            // Проверка, нажата ли кнопка "Back" для выхода в главное меню
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && simulationBackButtonRectangle.Contains(Mouse.GetState().Position))
            {
                requestStateChange = true;      // Запрашиваем выход в главное меню
                simulationStarted = false;
                ResetSimulation();              // Сброс симуляции
            }

            // Подсчет количества здоровых людей
            int healthyCount = people.Count(p => p.State == HealthState.Healthy);
            // Подсчет количества инфицированных и носителей
            int infectedCount = people.Count(p => p.State == HealthState.Infected || p.State == HealthState.Carrier);
            // Подсчет количества выздоровевших
            int recoveredCount = people.Count(p => p.State == HealthState.Recovered);
            // Подсчет количества умерших
            int deadCount = people.Count(p => p.State == HealthState.Dead);

            // Добавление текущих данных в график
            graph.AddDataPoint(healthyCount, infectedCount, recoveredCount, deadCount);
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
                    if ((person.State == HealthState.Infected || person.State == HealthState.Carrier) &&
                        otherPerson.State == HealthState.Healthy && distance < infectionRadius && random.NextDouble() < infectionChance)
                    {
                        // Заражение второго объекта
                        otherPerson.Infect();
                    }

                    // Проверка условия заражения для второго объекта
                    if ((otherPerson.State == HealthState.Infected || otherPerson.State == HealthState.Carrier) &&
                        person.State == HealthState.Healthy && distance < infectionRadius && random.NextDouble() < infectionChance)
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
                if (person.State == HealthState.Dead)
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

        // Метод для обновления настроек симуляции с использованием ползунков
        private void UpdateSimulationSettings()
        {
            // Обновление значения шанса заражения
            infectionChanceSlider.Update();
            infectionChance = infectionChanceSlider.GetValue() / 100f;

            // Обновление значения шанса смерти
            deathChanceSlider.Update();
            deathChance = deathChanceSlider.GetValue() / 100f;

            // Обновление значения радиуса заражения
            infectionRadiusSlider.Update();
            infectionRadius = infectionRadiusSlider.GetValue();

            // Обновление значения скорости объектов
            speedSlider.Update();
            float newSpeed = speedSlider.GetValue();
            foreach (var person in people)
            {
                person.SetSpeed(newSpeed);
            }

            // Обновление значения инкубационного периода
            incubationPeriodSlider.Update();
            Person.defaultIncubationPeriod = incubationPeriodSlider.GetValue();

            // Обновление значения периода инфекции
            infectionPeriodSlider.Update();
            Person.defaultInfectionPeriod = infectionPeriodSlider.GetValue();

            // Обновление значения количества людей
            if (!simulationStarted)
            {
                numberOfPeopleSlider.Update();
                numberOfPeople = numberOfPeopleSlider.GetValue();
            }
        }

        // Метод для сброса симуляции к начальному состоянию
        private void ResetSimulation()
        {
            // Сброс значений ползунков к значениям по умолчанию
            infectionChanceSlider.SetValue((int)(defaultInfectionChance * 100));
            deathChanceSlider.SetValue((int)(defaultDeathChance * 100));
            infectionRadiusSlider.SetValue((int)defaultInfectionRadius);
            speedSlider.SetValue((int)defaultSpeed);
            incubationPeriodSlider.SetValue(defaultIncubationPeriod);
            infectionPeriodSlider.SetValue(defaultInfectionPeriod);
            numberOfPeopleSlider.SetValue(defaultNumberOfPeople);

            // Очистка списка объектов Person
            people.Clear();
        }

        // Метод для инициализации объектов Person
        private void InitializePeople()
        {
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

        // Метод для рисования симуляции на экране
        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            // Рисование фона симуляции
            spriteBatch.Draw(backgroundSimulationTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            // Рисование прямоугольника для области симуляции
            spriteBatch.Draw(rectangleTexture, simulationArea, Color.White);

            // Рисование прямоугольника
            spriteBatch.Draw(countPeopleRectangleTexture, countPeopleRectangle, Color.White);

            // Рисование прямоугольника позади графика
            spriteBatch.Draw(graphBackgroundTexture, graphRectangle, Color.White);

            // Определение нужно ли рисовать радиус
            bool drawRadius = infectionRadiusSlider.IsHovered() || infectionRadiusSlider.IsDragging();

            // Сначала рисуем мертвые объекты Person
            foreach (var person in people)
            {
                if (person.State == HealthState.Dead)
                {
                    // Определение цвета для рисования мертвого объекта (черный)
                    Color color = Color.Black;

                    // Рисование мертвого объекта Person с заданной текстурой и цветом
                    spriteBatch.Draw(
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
                if (person.State != HealthState.Dead)
                {
                    // Рисование радиуса заражения, если нужно
                    if (drawRadius)
                    {
                        float radius = infectionRadiusSlider.GetValue();
                        spriteBatch.Draw(
                            circleTexture,
                            new Vector2(person.Position.X - radius, person.Position.Y - radius),
                            null,
                            Color.DarkSeaGreen * 0.3f, // Полупрозрачный зеленый цвет
                            0f,
                            Vector2.Zero,
                            new Vector2(radius * 2 / circleTexture.Width, radius * 2 / circleTexture.Height),
                            SpriteEffects.None,
                            0f
                        );
                    }

                    // Определение цвета для рисования на основе состояния объекта Person
                    Color color = person.State switch
                    {
                        HealthState.Infected => Color.Red,          // Красный, если объект заражен
                        HealthState.Carrier => Color.LightPink,     // Светло-розовый, если объект носитель
                        HealthState.Recovered => Color.Gray,        // Серый, если объект выздоровел, иначе белый
                        _ => Color.White,
                    };

                    // Рисование живого объекта Person с заданной текстурой и цветом
                    spriteBatch.Draw(
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

            // Рисование количества пациентов
            DrawPatientCount(spriteBatch, "Recovered: ", people.Count(p => p.State == HealthState.Recovered), new Vector2(30, 615), Color.Purple);
            DrawPatientCount(spriteBatch, "Healthy: ", people.Count(p => p.State == HealthState.Healthy), new Vector2(30, 635), Color.Blue);
            DrawPatientCount(spriteBatch, "Infected: ", people.Count(p => p.State == HealthState.Infected || p.State == HealthState.Carrier), new Vector2(30, 655), Color.Orange);
            DrawPatientCount(spriteBatch, "Died: ", people.Count(p => p.State == HealthState.Dead), new Vector2(30, 675), Color.Black);

            // Рисование ползунков для настройки параметров симуляции
            infectionChanceSlider.Draw(spriteBatch);
            deathChanceSlider.Draw(spriteBatch);
            infectionRadiusSlider.Draw(spriteBatch);
            speedSlider.Draw(spriteBatch);
            incubationPeriodSlider.Draw(spriteBatch);
            infectionPeriodSlider.Draw(spriteBatch);
            numberOfPeopleSlider.Draw(spriteBatch);

            // Рисование кнопок "Start" и "Reset" в окне симуляции
            spriteBatch.Draw(simulationStartButtonTexture, simulationStartButtonRectangle, Color.White);
            spriteBatch.Draw(simulationResetButtonTexture, simulationResetButtonRectangle, Color.White);
            spriteBatch.Draw(simulationBackButtonTexture, simulationBackButtonRectangle, Color.White);

            // Отрисовка графика
            graph.Draw(spriteBatch);
        }

        // Метод для рисования количества пациентов
        private void DrawPatientCount(SpriteBatch spriteBatch, string label, int count, Vector2 position, Color color)
        {
            // Рисуем метку (label) черным цветом
            spriteBatch.DrawString(font, label, position, Color.Black);

            // Рисуем количество пациентов рядом с меткой
            spriteBatch.DrawString(font, count.ToString(),
                new Vector2(position.X + font.MeasureString(label).X, position.Y), // Позиция для числа пациентов рядом с меткой
                color); // Цвет числа пациентов
        }
    }
}