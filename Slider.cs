using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Epidemic_Simulation
{
    public class Slider
    {
        private Texture2D trackTexture;     // Текстура дорожки ползунка
        private Texture2D thumbTexture;     // Текстура ползунка
        private Rectangle trackRectangle;   // Прямоугольник дорожки
        private Rectangle thumbRectangle;   // Прямоугольник ползунка
        private int minValue;               // Минимальное значение ползунка
        private int maxValue;               // Максимальное значение ползунка
        private int currentValue;           // Текущее значение ползунка
        private bool isDragging;            // Флаг для отслеживания состояния перетаскивания
        private string label;               // Название ползунка
        private SpriteFont font;            // Шрифт для отображения текста
        private string unit;                // Единица измерения ползунка

        // Конструктор класса Slider
        public Slider(Texture2D trackTexture, Texture2D thumbTexture, SpriteFont font, string label, int x, int y, int width, int minValue, int maxValue, int initialValue, string unit)
        {
            this.trackTexture = trackTexture;  // Инициализация текстуры дорожки ползунка
            this.thumbTexture = thumbTexture;  // Инициализация текстуры ползунка
            this.minValue = minValue;          // Установка минимального значения ползунка
            this.maxValue = maxValue;          // Установка максимального значения ползунка
            this.currentValue = initialValue;  // Установка начального значения ползунка
            this.label = label;                // Инициализация названия ползунка
            this.font = font;                  // Инициализация шрифта
            this.unit = unit;                  // Инициализация единицы измерения

            // Создание прямоугольника для дорожки ползунка
            trackRectangle = new Rectangle(x, y, width, trackTexture.Height);

            // Расчет начальной позиции ползунка на дорожке
            int thumbX = x + (int)(((initialValue - minValue) / (float)(maxValue - minValue)) * width) - (thumbTexture.Width / 2);
            // Создание прямоугольника для ползунка с учетом его начальной позиции
            thumbRectangle = new Rectangle(thumbX, y - (thumbTexture.Height / 2) + (trackTexture.Height / 2), thumbTexture.Width, thumbTexture.Height);
        }

        // Метод для обновления состояния ползунка
        public void Update()
        {
            // Получаем текущее состояние мыши
            var mouseState = Mouse.GetState();

            // Проверяем, нажата ли левая кнопка мыши
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                // Проверяем, находится ли курсор мыши на ползунке или уже выполняется перетаскивание
                if (thumbRectangle.Contains(mouseState.Position) || isDragging)
                {
                    isDragging = true;  // Устанавливаем флаг перетаскивания
                                        // Обновляем положение ползунка, ограничивая его движение рамками дорожки
                    thumbRectangle.X = MathHelper.Clamp(
                        mouseState.X - thumbRectangle.Width / 2, // Центрируем ползунок под курсором мыши, смещая позицию на половину ширины ползунка
                        trackRectangle.X - thumbRectangle.Width / 2, // Левая граница дорожки, ограничивающая минимальное значение позиции ползунка
                        trackRectangle.X + trackRectangle.Width - thumbRectangle.Width / 2 // Правая граница дорожки, ограничивающая максимальное значение позиции ползунка
                    );

                    // Обновляем текущее значение ползунка на основе его положения (по центру)
                    currentValue = minValue + (int)(
                        ((thumbRectangle.Center.X - trackRectangle.X) / (float)trackRectangle.Width) // Относительная позиция ползунка на дорожке (от 0 до 1)
                        * (maxValue - minValue) // Умножаем относительную позицию на диапазон значений ползунка
                    );
                }
            }
            else
            {
                isDragging = false; // Сбрасываем флаг перетаскивания, когда левая кнопка мыши отпущена
            }
        }

        // Метод для рисования ползунка
        public void Draw(SpriteBatch spriteBatch)
        {
            // Определяем ширину закрашенной части
            int filledWidth = thumbRectangle.Center.X - trackRectangle.X;

            // Рисуем закрашенную часть дорожки
            spriteBatch.Draw(trackTexture, new Rectangle(trackRectangle.X, trackRectangle.Y, filledWidth, trackRectangle.Height), Color.White);

            // Рисуем незакрашенную часть дорожки
            spriteBatch.Draw(trackTexture, new Rectangle(trackRectangle.X + filledWidth, trackRectangle.Y, trackRectangle.Width - filledWidth, trackRectangle.Height), Color.Gray);

            // Рисуем ползунок
            spriteBatch.Draw(thumbTexture, thumbRectangle, Color.White);

            // Рисуем название ползунка
            spriteBatch.DrawString(font, label, new Vector2(trackRectangle.X, trackRectangle.Y - 30), Color.Black);

            // Рисуем текущее значение ползунка с единицей измерения под ползунком
            spriteBatch.DrawString(font, $"{currentValue} {unit}", new Vector2(trackRectangle.X + trackRectangle.Width / 2 - font.MeasureString($"{currentValue} {unit}").X / 2, trackRectangle.Y + 20), Color.Black);
        }

        // Метод для получения текущего значения ползунка
        public int GetValue()
        {
            return currentValue;
        }

        // Метод для установки текущего значения ползунка
        public void SetValue(int value)
        {
            currentValue = MathHelper.Clamp(value, minValue, maxValue);

            // Обновляем положение ползунка на основе его значения
            thumbRectangle.X = trackRectangle.X + (int)(((currentValue - minValue) / (float)(maxValue - minValue)) * trackRectangle.Width) - (thumbTexture.Width / 2);
        }

        // Метод для проверки, находится ли курсор мыши на ползунке
        public bool IsHovered()
        {
            // Возвращает true, если курсор находится в пределах прямоугольника ползунка
            return thumbRectangle.Contains(Mouse.GetState().Position);
        }

        // Метод для проверки, выполняется ли перетаскивание ползунка
        public bool IsDragging()
        {
            // Возвращает значение флага перетаскивания
            return isDragging;
        }
    }
}