using Microsoft.Xna.Framework;
using System;

namespace Epidemic_Simulation
{
    public class Person
    {
        public Vector2 Position { get; set; }           // Позиция объекта
        public bool IsInfected { get; private set; }    // Флаг, указывающий, заражен ли объект
        public bool IsRecovered { get; private set; }   // Флаг, указывающий, выздоровел ли объект
        public bool IsCarrier { get; private set; }     // Флаг, указывающий, является ли объект носителем инфекции
        public bool IsDead { get; private set; }        // Флаг, указывающий, мертв ли объект
        public float Radius { get; private set; }       // Радиус объекта

        private TimeSpan infectionTimeRemaining;        // Оставшееся время до конца инфекции
        private TimeSpan incubationTimeRemaining;       // Оставшееся время до окончания инкубационного периода
        private Vector2 direction;                      // Направление движения объекта
        private float speed;                            // Скорость движения объекта

        private TimeSpan deathCheckInterval = TimeSpan.FromSeconds(5); // Интервал проверки на смерть
        private TimeSpan timeSinceLastDeathCheck = TimeSpan.Zero;      // Время с последней проверки на смерть

        private static Random random = new Random();    // Генератор случайных чисел

        // Конструктор, инициализирующий объект в заданной позиции
        public Person(Vector2 position, float textureWidth)
        {
            Position = position;    // Установка начальной позиции

            // Генерация случайного направления движения с координатами от -1 до 1
            direction = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
            direction.Normalize();  // Нормализация вектора направления для единичной длины

            speed = 70f;                // Установка скорости движения
            Radius = textureWidth / 2;  // Расчет радиуса на основе ширины текстуры
            IsInfected = false;         // Инициализация состояния как незараженного
            IsRecovered = false;        // Инициализация состояния как не выздоровевшего
            IsCarrier = false;          // Инициализация состояния как неносителя
            IsDead = false;             // Инициализация состояния как живого
        }

        // Метод для заражения объекта
        public void Infect()
        {
            // Проверяем, что объект не заражен, не выздоровел и не является носителем или мертвым
            if (!IsInfected && !IsRecovered && !IsCarrier && !IsDead)
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

        // Метод для обновления состояния объекта
        public void Update(GameTime gameTime, Rectangle simulationArea, Vector2 position, float deathChance)
        {
            // Если объект мертв, он не двигается
            if (IsDead) { return; }

            // Если объект является носителем инфекции
            if (IsCarrier)
            {
                // Уменьшаем оставшееся время инкубационного периода
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
                infectionTimeRemaining -= gameTime.ElapsedGameTime;
                timeSinceLastDeathCheck += gameTime.ElapsedGameTime;

                // Проверка на смерть с интервалом
                if (timeSinceLastDeathCheck >= deathCheckInterval)
                {
                    timeSinceLastDeathCheck = TimeSpan.Zero; // Сбрасываем время с последней проверки
                    if (random.NextDouble() < deathChance)
                    {
                        // Объект умирает
                        IsInfected = false;
                        IsDead = true;
                        return;
                    }
                }

                // Если период инфекции закончился
                if (infectionTimeRemaining <= TimeSpan.Zero)
                {
                    // Переводим состояние из зараженного в выздоровевший
                    IsInfected = false;
                    IsRecovered = true;
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
            if (IsDead || otherPerson.IsDead)
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