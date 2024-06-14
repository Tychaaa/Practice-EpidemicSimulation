using Microsoft.Xna.Framework;
using System;

namespace Epidemic_Simulation
{
    public class Person
    {
        // Перечисление, описывающее состояние здоровья объекта
        public enum HealthState
        {
            Healthy,    // Здоровый объект
            Carrier,    // Объект носитель инфекции
            Infected,   // Зараженный объект
            Recovered,  // Выздоровевший объект
            Dead        // Умерший объект
        }

        public Vector2 Position { get; set; }           // Позиция объекта
        public HealthState State { get; private set; }  // Состояние здоровья объекта
        public float Radius { get; private set; }       // Радиус объекта

        private TimeSpan infectionTimeRemaining;        // Оставшееся время до конца инфекции
        private TimeSpan incubationTimeRemaining;       // Оставшееся время до окончания инкубационного периода
        private Vector2 direction;                      // Направление движения объекта
        private float speed;                            // Скорость движения объекта

        private TimeSpan deathCheckInterval = TimeSpan.FromSeconds(5); // Интервал проверки на смерть
        private TimeSpan timeSinceLastDeathCheck = TimeSpan.Zero;      // Время с последней проверки на смерть

        private static Random random = new Random();    // Генератор случайных чисел
        public static float defaultSpeed = 70f;
        public static int defaultIncubationPeriod = 5;
        public static int defaultInfectionPeriod = 10;

        // Конструктор, инициализирующий объект в заданной позиции
        public Person(Vector2 position, float textureWidth)
        {
            Position = position;    // Установка начальной позиции

            // Генерация случайного направления движения с координатами от -1 до 1
            direction = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
            direction.Normalize();          // Нормализация вектора направления для единичной длины

            speed = defaultSpeed;           // Установка скорости движения
            Radius = textureWidth / 2;      // Расчет радиуса на основе ширины текстуры
            State = HealthState.Healthy;    // Инициализация состояния как здоровый
        }

        // Метод для заражения объекта
        public void Infect()
        {
            // Проверяем, что объект не заражен, не выздоровел и не является носителем или мертвым
            if (State == HealthState.Healthy)
            {
                // Устанавливаем состояние объекта как носитель инфекции
                State = HealthState.Carrier;

                // Устанавливаем инкубационный период
                incubationTimeRemaining = TimeSpan.FromSeconds(defaultIncubationPeriod);

                // Устанавливаем период инфекции
                infectionTimeRemaining = TimeSpan.FromSeconds(defaultInfectionPeriod);
            }
        }

        // Метод для обновления состояния объекта
        public void Update(GameTime gameTime, Rectangle simulationArea, Vector2 position, float deathChance)
        {
            // Если объект мертв, он не двигается
            if (State == HealthState.Dead) { return; }

            // Если объект является носителем инфекции
            if (State == HealthState.Carrier)
            {
                // Уменьшаем оставшееся время инкубационного периода
                incubationTimeRemaining -= gameTime.ElapsedGameTime;

                // Если инкубационный период закончился
                if (incubationTimeRemaining <= TimeSpan.Zero)
                {
                    // Переводим состояние из носителя в зараженный
                    State = HealthState.Infected;
                }
            }
            // Если объект заражен
            else if (State == HealthState.Infected)
            {
                // Уменьшаем оставшееся время инфекции
                infectionTimeRemaining -= gameTime.ElapsedGameTime;
                timeSinceLastDeathCheck += gameTime.ElapsedGameTime;

                // Проверка на смерть с интервалом
                if (timeSinceLastDeathCheck >= deathCheckInterval)
                {
                    timeSinceLastDeathCheck = TimeSpan.Zero; // Сбрасываем время с последней проверки
                    if (random.NextDouble() < deathChance)
                    {
                        // Объект умирает
                        State = HealthState.Dead;
                        return;
                    }
                }

                // Если период инфекции закончился
                if (infectionTimeRemaining <= TimeSpan.Zero)
                {
                    // Переводим состояние из зараженного в выздоровевший
                    State = HealthState.Recovered;
                }
            }

            // Обновляем позицию объекта на основе направления и скорости
            // Умножаем нормализованный вектор направления (direction) на скорость (speed),
            // чтобы получить вектор скорости. Затем умножаем этот вектор на время,
            // прошедшее с последнего кадра (в секундах), чтобы получить вектор смещения.
            // Добавляем вектор смещения к текущей позиции (Position).
            Position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Проверка выхода за границы прямоугольника
            CheckBoundsCollision(simulationArea, position);
        }

        // Метод для проверки и обработки столкновений с границами прямоугольника
        private void CheckBoundsCollision(Rectangle simulationArea, Vector2 position)
        {
            // Проверка выхода за границы прямоугольника и изменение направления при столкновении с границей по оси X
            if (Position.X - Radius <= simulationArea.Left || Position.X + Radius >= simulationArea.Right)
            {
                direction.X = -direction.X; // Инвертируем направление по оси X
                position.X = Math.Clamp(Position.X, simulationArea.Left + Radius, simulationArea.Right - Radius); // Ограничиваем позицию в пределах прямоугольника
            }

            // Проверка выхода за границы прямоугольника и изменение направления при столкновении с границей по оси Y
            if (Position.Y - Radius <= simulationArea.Top || Position.Y + Radius >= simulationArea.Bottom)
            {
                direction.Y = -direction.Y; // Инвертируем направление по оси Y
                position.Y = Math.Clamp(Position.Y, simulationArea.Top + Radius, simulationArea.Bottom - Radius); // Ограничиваем позицию в пределах прямоугольника
            }
        }

        // Метод для проверки и обработки столкновений с другим объектом Person
        public void CheckCollision(Person otherPerson)
        {
            if (State == HealthState.Dead || otherPerson.State == HealthState.Dead)
                return; // Не проверяем коллизии с мертвыми объектами

            // Вычисляем расстояние между центрами двух объектов
            float distance = Vector2.Distance(this.Position, otherPerson.Position);

            // Проверяем, пересекаются ли круги, описанные вокруг двух объектов
            if (distance <= this.Radius + otherPerson.Radius)
            {
                // Если пересекаются, обрабатываем столкновение
                HandleCollision(otherPerson, distance);
            }
        }

        // Метод для обработки столкновения
        private void HandleCollision(Person otherPerson, float distance)
        {
            // Вычисляем нормаль столкновения
            Vector2 collisionNormal = this.Position - otherPerson.Position;
            collisionNormal.Normalize();

            // Вычисляем относительную скорость
            Vector2 relativeVelocity = this.direction * this.speed - otherPerson.direction * otherPerson.speed;

            // Вычисляем скорость вдоль нормали столкновения
            float velocityAlongNormal = Vector2.Dot(relativeVelocity, collisionNormal);

            // Если скорости расходятся, нет необходимости обрабатывать столкновение
            if (velocityAlongNormal > 0)
                return;

            // Вычисляем коэффициент упругости (elasticity)
            float elasticity = 1f; // Для абсолютно упругого столкновения

            // Вычисляем импульс столкновения
            float j = -(1 + elasticity) * velocityAlongNormal;
            j /= (1 / this.Radius + 1 / otherPerson.Radius);

            // Применяем импульс к каждому объекту
            Vector2 impulse = j * collisionNormal;
            this.direction += impulse / this.Radius;
            otherPerson.direction -= impulse / otherPerson.Radius;

            // Нормализуем направления
            this.direction.Normalize();
            otherPerson.direction.Normalize();

            // Обновляем позиции, чтобы избежать застревания
            float overlap = 0.5f * (this.Radius + otherPerson.Radius - distance);
            this.Position += collisionNormal * overlap;
            otherPerson.Position -= collisionNormal * overlap;
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

        // Метод для получения направления
        public Vector2 GetDirection()
        {
            return direction;
        }

        // Метод для установки направления
        public void SetDirection(Vector2 newDirection)
        {
            direction = newDirection;
            direction.Normalize();
        }
    }
}