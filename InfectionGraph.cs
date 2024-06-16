using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Epidemic_Simulation
{
    public class InfectionGraph
    {
        private Texture2D pixel;            // Текстура для рисования пикселей
        private List<int> healthyData;      // Данные по здоровым людям
        private List<int> infectedData;     // Данные по зараженным людям
        private List<int> recoveredData;    // Данные по выздоровевшим людям
        private List<int> deadData;         // Данные по умершим людям
        private int maxDataPoints;          // Максимальное количество точек данных для графика
        private int graphWidth;             // Ширина графика
        private int graphHeight;            // Высота графика
        private Vector2 graphPosition;      // Позиция графика на экране

        // Конструктор класса InfectionGraph
        public InfectionGraph(GraphicsDevice graphicsDevice, int maxDataPoints, int graphWidth, int graphHeight, Vector2 graphPosition)
        {
            this.maxDataPoints = maxDataPoints;  // Установка максимального количества точек данных
            this.graphWidth = graphWidth;        // Установка ширины графика
            this.graphHeight = graphHeight;      // Установка высоты графика
            this.graphPosition = graphPosition;  // Установка позиции графика на экране

            // Инициализация списков данных для различных состояний (здоровые, зараженные, выздоровевшие, умершие)
            healthyData = new List<int>();
            infectedData = new List<int>();
            recoveredData = new List<int>();
            deadData = new List<int>();

            // Создание текстуры для пикселя (1x1) и установка данных цвета (белый)
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        // Метод для добавления новой точки данных
        public void AddDataPoint(int healthy, int infected, int recovered, int dead)
        {
            // Если количество точек данных превышает максимум, удаляем старые данные
            if (healthyData.Count >= maxDataPoints)
            {
                // Удаление первых (старейших) элементов из каждого списка данных
                healthyData.RemoveAt(0);
                infectedData.RemoveAt(0);
                recoveredData.RemoveAt(0);
                deadData.RemoveAt(0);
            }

            // Добавление новых данных в каждый список
            healthyData.Add(healthy);
            infectedData.Add(infected);
            recoveredData.Add(recovered);
            deadData.Add(dead);
        }

        // Метод для рисования графика
        public void Draw(SpriteBatch spriteBatch)
        {
            // Определяем максимальное значение среди всех данных, чтобы масштабировать график по вертикали
            int maxCount = Math.Max(1, Math.Max(Math.Max(Math.Max(
                            healthyData.Count > 0 ? healthyData.Max() : 0,
                            infectedData.Count > 0 ? infectedData.Max() : 0),
                            recoveredData.Count > 0 ? recoveredData.Max() : 0),
                            deadData.Count > 0 ? deadData.Max() : 0));

            // Определяем начальную точку (origin) для рисования графика
            Vector2 origin = graphPosition + new Vector2(0, graphHeight);

            // Определяем масштаб по оси X
            // Масштаб по оси X равен ширине графика, деленной на максимальное количество точек данных
            // Это определяет расстояние между точками данных по оси X
            float xScale = (float)graphWidth / maxDataPoints;

            // Определяем масштаб по оси Y
            // Масштаб по оси Y равен высоте графика, деленной на максимальное значение данных
            // Это определяет, насколько высота графика будет растянута или сжата в зависимости от значения данных
            float yScale = (float)graphHeight / maxCount;

            // Рисование линий графика
            DrawGraphLines(spriteBatch, origin, xScale, yScale);

            // Рисование данных графика (заполненные области)
            DrawGraphData(spriteBatch, origin, xScale, yScale);
        }

        // Метод для рисования линий графика
        private void DrawGraphLines(SpriteBatch spriteBatch, Vector2 origin, float xScale, float yScale)
        {
            // Проходим по всем точкам данных, кроме последней
            for (int i = 0; i < healthyData.Count - 1; i++)
            {
                // Рисуем линии для каждого типа данных, соединяя точки данных между собой

                // Линия для здоровых
                // Начальная точка: (i * xScale, -healthyData[i] * yScale) — координаты текущей точки
                // Конечная точка: ((i + 1) * xScale, -healthyData[i + 1] * yScale) — координаты следующей точки
                DrawLine(spriteBatch,
                         origin + new Vector2(i * xScale, -healthyData[i] * yScale),
                         origin + new Vector2((i + 1) * xScale, -healthyData[i + 1] * yScale),
                         Color.Blue);

                // Линия для инфицированных
                DrawLine(spriteBatch,
                         origin + new Vector2(i * xScale, -infectedData[i] * yScale),
                         origin + new Vector2((i + 1) * xScale, -infectedData[i + 1] * yScale),
                         Color.Orange);

                // Линия для выздоровевших
                DrawLine(spriteBatch,
                         origin + new Vector2(i * xScale, -recoveredData[i] * yScale),
                         origin + new Vector2((i + 1) * xScale, -recoveredData[i + 1] * yScale),
                         Color.Purple);

                // Линия для умерших
                DrawLine(spriteBatch,
                         origin + new Vector2(i * xScale, -deadData[i] * yScale),
                         origin + new Vector2((i + 1) * xScale, -deadData[i + 1] * yScale),
                         Color.Black);
            }
        }

        // Метод для рисования данных графика
        private void DrawGraphData(SpriteBatch spriteBatch, Vector2 origin, float xScale, float yScale)
        {
            float opacity = 0.3f;     // Прозрачность для заполненных областей

            // Рисование заполненных областей для каждого типа данных

            // Заполнение области для здоровых
            // Используем цвет LightBlue с прозрачностью 30%
            DrawFilledArea(spriteBatch, healthyData, origin, xScale, yScale, Color.Blue * opacity);

            // Заполнение области для инфицированных
            // Используем цвет Orange с прозрачностью 30%
            DrawFilledArea(spriteBatch, infectedData, origin, xScale, yScale, Color.Orange * opacity);

            // Заполнение области для умерших
            // Используем черный цвет с прозрачностью 30%
            DrawFilledArea(spriteBatch, deadData, origin, xScale, yScale, Color.Black * opacity);

            // Заполнение области для выздоровевших
            // Используем цвет Purple с прозрачностью 30%
            DrawFilledArea(spriteBatch, recoveredData, origin, xScale, yScale, Color.Purple * opacity);
        }

        // Метод для рисования заполненной области графика
        private void DrawFilledArea(SpriteBatch spriteBatch, List<int> data, Vector2 origin, float xScale, float yScale, Color color)
        {
            // Проходим по всем точкам данных, кроме последней
            for (int i = 0; i < data.Count - 1; i++)
            {
                // Вычисление координат вершин текущего квадрата заполненной области

                // Верхняя левая вершина
                Vector2 p1 = origin + new Vector2(i * xScale, -data[i] * yScale);

                // Верхняя правая вершина
                Vector2 p2 = origin + new Vector2((i + 1) * xScale, -data[i + 1] * yScale);

                // Нижняя правая вершина
                Vector2 p3 = origin + new Vector2((i + 1) * xScale, 0);

                // Нижняя левая вершина
                Vector2 p4 = origin + new Vector2(i * xScale, 0);

                // Рисование квадрата, представляющего заполненную область, между точками данных
                DrawQuad(spriteBatch, p1, p2, p3, p4, color);
            }
        }

        // Метод для рисования линии
        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            // Вычисление расстояния между начальной и конечной точками линии
            float distance = Vector2.Distance(start, end);

            // Вычисление угла наклона линии
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            // Рисование линии с использованием текстуры пикселя
            spriteBatch.Draw(
                pixel,                      // Текстура для рисования
                start,                      // Начальная точка линии
                null,                       // Область текстуры для рисования (null - вся текстура)
                color,                      // Цвет линии
                angle,                      // Угол наклона линии
                Vector2.Zero,               // Точка происхождения (верхний левый угол текстуры)
                new Vector2(distance, 1),   // Масштабирование текстуры (длина линии и толщина)
                SpriteEffects.None,         // Эффекты для спрайта (без эффектов)
                0                           // Слой отрисовки (0 - самый нижний слой)
            );
        }

        // Метод для рисования четырехугольника
        private void DrawQuad(SpriteBatch spriteBatch, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
        {
            // Рисование сторон четырехугольника
            DrawLine(spriteBatch, p1, p2, color); // Линия от p1 до p2
            DrawLine(spriteBatch, p2, p3, color); // Линия от p2 до p3
            DrawLine(spriteBatch, p3, p4, color); // Линия от p3 до p4
            DrawLine(spriteBatch, p4, p1, color); // Линия от p4 до p1

            // Вычисление минимального значения Y среди вершин четырехугольника
            float minY = Math.Min(Math.Min(p1.Y, p2.Y), Math.Min(p3.Y, p4.Y));

            // Корректировка Y-координат вершин для выравнивания
            Vector2 adjustedP1 = new Vector2(p1.X, p1.Y - minY);
            Vector2 adjustedP2 = new Vector2(p2.X, p2.Y - minY);
            Vector2 adjustedP3 = new Vector2(p3.X, p3.Y - minY);
            Vector2 adjustedP4 = new Vector2(p4.X, p4.Y - minY);

            // Вычисление верхней левой и нижней правой точек четырехугольника
            Vector2 topLeft = new Vector2(Math.Min(adjustedP1.X, adjustedP4.X), Math.Min(adjustedP1.Y, adjustedP2.Y));
            Vector2 bottomRight = new Vector2(Math.Max(adjustedP2.X, adjustedP3.X), Math.Max(adjustedP3.Y, adjustedP4.Y));

            // Вычисление размера четырехугольника
            Vector2 size = bottomRight - topLeft;

            // Рисование заполненного четырехугольника с использованием текстуры пикселя
            spriteBatch.Draw(
                pixel,                          // Текстура для рисования (пиксель)
                topLeft + new Vector2(0, minY), // Позиция верхней левой точки четырехугольника
                null,                           // Область текстуры для рисования (null - вся текстура)
                color,                          // Цвет заполнения
                0f,                             // Угол поворота (0 - без поворота)
                Vector2.Zero,                   // Точка происхождения (верхний левый угол текстуры)
                size,                           // Масштабирование текстуры (размер четырехугольника)
                SpriteEffects.None,             // Эффекты для спрайта (без эффектов)
                0                               // Слой отрисовки (0 - самый нижний слой)
            );
        }
    }
}