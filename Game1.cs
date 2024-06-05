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

            // Создание объектов Person и добавление их в список
            for (int i = 0; i < numberOfPeople; i++)
            {
                // Генерация случайной позиции внутри границ экрана
                var position = new Vector2(random.Next(_graphics.PreferredBackBufferWidth), random.Next(_graphics.PreferredBackBufferHeight));
                // Создание нового объекта Person
                var person = new Person(position);
                // Добавление объекта в список
                people.Add(person);
            }

            // Заражение первого объекта в списке
            people[0].Infect();

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
                person.Update(gameTime, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, person.GetPosition());

                // Проверка столкновений и инфекций с другими объектами Person
                for (int j = i + 1; j < people.Count; j++)
                {
                    var otherPerson = people[j];
                    // Вычисление расстояния между двумя объектами Person
                    float distance = Vector2.Distance(person.Position, otherPerson.Position);

                    // Если объекты сталкиваются (расстояние меньше суммы их радиусов)
                    if (distance < person.Radius + otherPerson.Radius)
                    {
                        // Вычисление направления столкновения
                        Vector2 collisionDirection = person.Position - otherPerson.Position;
                        collisionDirection.Normalize();

                        // Разделение объектов, чтобы предотвратить их наложение
                        person.Position += collisionDirection * (person.Radius + otherPerson.Radius - distance) / 2;
                        otherPerson.Position -= collisionDirection * (person.Radius + otherPerson.Radius - distance) / 2;

                        // Изменение направления движения объектов после столкновения
                        person.ChangeDirection(collisionDirection);
                        otherPerson.ChangeDirection(-collisionDirection);
                    }

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

            // Вызов базового метода обновления
            base.Update(gameTime);
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