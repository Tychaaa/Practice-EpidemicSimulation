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

        Texture2D personTexture;        // Текстура для отображения объекта Person
        List<Person> people;            // Список объектов Person

        Random random;                  // Генератор случайных чисел
        int numberOfPeople = 100;       // Количество объектов Person
        float infectionRadius = 20f;    // Радиус заражения
        float infectionChance = 0.3f;   // Шанс заражения

        // Конструктор класса Game1
        public Game1()
        {
            // Инициализация менеджера графических устройств
            _graphics = new GraphicsDeviceManager(this);
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

            // Заражение первого объекта в списке
            people[0].Infect();
        }

        // Метод для генерации случайной позиции внутри границ экрана с учетом размеров текстуры
        private Vector2 GenerateRandomPosition(int textureWidth, int textureHeight)
        {
            // Генерация случайной координаты X
            // Начинаем от половины ширины текстуры, чтобы текстура не выходила за левую границу
            // и заканчиваем на расстоянии половины ширины текстуры от правой границы
            int x = random.Next(textureWidth / 2, _graphics.PreferredBackBufferWidth - textureWidth / 2);

            // Генерация случайной координаты Y
            // Начинаем от половины высоты текстуры, чтобы текстура не выходила за верхнюю границу
            // и заканчиваем на расстоянии половины высоты текстуры от нижней границы
            int y = random.Next(textureHeight / 2, _graphics.PreferredBackBufferHeight - textureHeight / 2);

            // Возвращаем сгенерированную случайную позицию как вектор
            return new Vector2(x, y);
        }

        // Метод для обновления состояния игры
        protected override void Update(GameTime gameTime)
        {
            // Проверка, была ли нажата клавиша "Escape" на клавиатуре для выхода
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Обновление состояния каждого объекта Person
            for (int i = 0; i < people.Count; i++)
            {
                var person = people[i];
                // Обновление состояния и позиции объекта Person
                person.Update(gameTime, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, person.Position);

                // Проверка столкновений и инфекций с другими объектами Person
                for (int j = i + 1; j < people.Count; j++)
                {
                    var otherPerson = people[j];
                    // Вычисление расстояния между двумя объектами Person
                    float distance = Vector2.Distance(person.Position, otherPerson.Position);
                    person.CheckCollision(otherPerson);

                    // Проверка условия заражения для первого объекта
                    if ((person.IsInfected || person.IsCarrier) && !otherPerson.IsInfected && !otherPerson.IsCarrier && distance < infectionRadius && random.NextDouble() < infectionChance)
                    {
                        // Заражение второго объекта
                        otherPerson.Infect();
                    }

                    // Проверка условия заражения для второго объекта
                    if ((otherPerson.IsInfected || otherPerson.IsCarrier) && !person.IsInfected && !person.IsCarrier && distance < infectionRadius && random.NextDouble() < infectionChance)
                    {
                        // Заражение первого объекта
                        person.Infect();
                    }
                }
            }

            // Проверка и устранение застревания объектов в границах
            CheckAndResolveBoundarySticking();

            // Вызов базового метода обновления
            base.Update(gameTime);
        }

        // Метод для проверки и устранения застревания объектов Person в границах экрана
        private void CheckAndResolveBoundarySticking()
        {
            // Проход по каждому объекту Person в списке people
            foreach (var person in people)
            {
                bool stuck = false;     // Флаг, указывающий, застрял ли объект

                // Проверка застревания в левой или правой границе
                if (person.Position.X - person.Radius <= 0)
                {
                    // Если объект застрял в левой границе, переместим его внутрь
                    person.Position = new Vector2(person.Radius, person.Position.Y);
                    // Установим направление движения в сторону от границы
                    person.SetDirection(new Vector2(Math.Abs(person.GetDirection().X), person.GetDirection().Y)); // Направление в сторону от границы
                    stuck = true;
                }
                else if (person.Position.X + person.Radius >= _graphics.PreferredBackBufferWidth)
                {
                    // Если объект застрял в правой границе, переместим его внутрь
                    person.Position = new Vector2(_graphics.PreferredBackBufferWidth - person.Radius, person.Position.Y);
                    // Установим направление движения в сторону от границы
                    person.SetDirection(new Vector2(-Math.Abs(person.GetDirection().X), person.GetDirection().Y)); // Направление в сторону от границы
                    stuck = true;
                }

                // Проверка застревания в верхней или нижней границе
                if (person.Position.Y - person.Radius <= 0)
                {
                    // Если объект застрял в верхней границе, переместим его внутрь
                    person.Position = new Vector2(person.Position.X, person.Radius);
                    // Установим направление движения в сторону от границы
                    person.SetDirection(new Vector2(person.GetDirection().X, Math.Abs(person.GetDirection().Y))); // Направление в сторону от границы
                    stuck = true;
                }
                else if (person.Position.Y + person.Radius >= _graphics.PreferredBackBufferHeight)
                {
                    // Если объект застрял в нижней границе, переместим его внутрь
                    person.Position = new Vector2(person.Position.X, _graphics.PreferredBackBufferHeight - person.Radius);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Начало рисования спрайтов
            _spriteBatch.Begin();

            // Проход по каждому объекту Person в списке people
            foreach (var person in people)
            {
                // Определение цвета для рисования на основе состояния объекта Person
                Color color = person.IsInfected ? Color.Red :                   // Красный, если объект заражен
                              person.IsCarrier ? Color.LightPink :              // Светло-розовый, если объект носитель
                              (person.IsRecovered ? Color.Gray : Color.White);  // Серый, если объект выздоровел, иначе белый
                                                                                // Рисование объекта Person с заданной текстурой и цветом
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

            // Завершение рисования спрайтов
            _spriteBatch.End();

            // Вызов базового метода рисования
            base.Draw(gameTime);
        }
    }
}