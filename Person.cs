using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epidemic_Simulation
{
    public class Person
    {
        public Vector2 Position { get; set; }           // Позиция объекта
        public bool IsInfected { get; private set; }    // Флаг, указывающий, заражен ли объект
        public bool IsRecovered { get; private set; }   // Флаг, указывающий, выздоровел ли объект
        public bool IsCarrier { get; private set; }     // Флаг, указывающий, является ли объект носителем инфекции
        public float Radius { get; private set; }       // Радиус объекта

        private TimeSpan infectionTimeRemaining;        // Оставшееся время до конца инфекции
        private TimeSpan incubationTimeRemaining;       // Оставшееся время до окончания инкубационного периода
        private Vector2 direction;                      // Направление движения объекта
        private float speed;                            // Скорость движения объекта

        private static Random random = new Random();    // Генератор случайных чисел

        // Конструктор, инициализирующий объект в заданной позиции
        public Person(Vector2 position)
        {
            Position = position;    // Установка начальной позиции

            // Генерация случайного направления движения с координатами от -1 до 1
            direction = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
            direction.Normalize();  // Нормализация вектора направления для единичной длины

            speed = 70f;            // Установка скорости движения
            Radius = 10f;           // Радиус каждого объекта равен 10 единицам
            IsInfected = false;     // Инициализация состояния как незараженного
            IsRecovered = false;    // Инициализация состояния как не выздоровевшего
            IsCarrier = false;      // Инициализация состояния как неносителя
        }

        // Метод для заражения объекта
        public void Infect()
        {
            // Проверяем, что объект не заражен, не выздоровел и не является носителем
            if (!IsInfected && !IsRecovered && !IsCarrier)
            {
                // Устанавливаем состояние объекта как носитель инфекции
                IsCarrier = true;

                // Генерация случайной продолжительности инкубационного периода от 5 до 10 секунд
                double incubationSeconds = 5 + random.NextDouble() * 5;
                // Преобразование сгенерированного времени в объект TimeSpan
                incubationTimeRemaining = TimeSpan.FromSeconds(incubationSeconds);

                // Генерация случайной продолжительности инфекции от 10 до 15 секунд
                double infectionSeconds = 10 + random.NextDouble() * 5;
                // Преобразование сгенерированного времени в объект TimeSpan
                infectionTimeRemaining = TimeSpan.FromSeconds(infectionSeconds);
            }
        }

        // Метод для получения текущей позиции объекта
        public Vector2 GetPosition()
        {
            return Position;
        }

        // Метод для обновления состояния объекта
        public void Update(GameTime gameTime, int screenWidth, int screenHeight, Vector2 position)
        {
            // Если объект является носителем инфекции
            if (IsCarrier)
            {
                // Уменьшаем оставшееся время инкубационного периода
                // на время, прошедшее с последнего обновления
                incubationTimeRemaining -= gameTime.ElapsedGameTime;

                // Если инкубационный период закончился
                if (incubationTimeRemaining <= TimeSpan.Zero)
                {
                    // Переводим состояние из носителя в зараженный
                    IsCarrier = false;
                    IsInfected = true;
                }
            }
            // Если объект заражен
            else if (IsInfected)
            {
                // Уменьшаем оставшееся время инфекции
                // на время, прошедшее с последнего обновления
                infectionTimeRemaining -= gameTime.ElapsedGameTime;

                // Если период инфекции закончился
                if (infectionTimeRemaining <= TimeSpan.Zero)
                {
                    // Переводим состояние из зараженного в выздоровевший
                    IsInfected = false;
                    IsRecovered = true;
                }
            }

            // Обновляем позицию объекта на основе направления и скорости
            Position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Проверка выхода за границы экрана и изменение направления при столкновении с границей по оси X
            if (Position.X < Radius || Position.X > screenWidth - Radius)
            {
                // Инвертируем направление по оси X
                direction.X *= -1;
                // Ограничиваем позицию в пределах экрана
                position.X = Math.Clamp(Position.X, Radius, screenWidth - Radius);
            }

            // Проверка выхода за границы экрана и изменение направления при столкновении с границей по оси Y
            if (Position.Y < Radius || Position.Y > screenHeight - Radius)
            {
                // Инвертируем направление по оси Y
                direction.Y *= -1;
                // Ограничиваем позицию в пределах экрана
                position.Y = Math.Clamp(Position.Y, Radius, screenHeight - Radius);
            }
        }

        // Метод для изменения направления движения
        public void ChangeDirection(Vector2 newDirection)
        {
            direction = newDirection;   // Устанавливаем новое направление движения
            direction.Normalize();      // Нормализуем вектор направления
        }

        // Метод для установки новой скорости
        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }
    }
}