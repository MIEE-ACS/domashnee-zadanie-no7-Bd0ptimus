using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// ДЗ 7. Вариант 2. Буй Тхе Зунг. УТС-22
    /// Include function : - Randomly appear apple at 5%
    ///                    - If the snake eats that apple, the snake will have an additional helmet and can go through the wall 5 times
    public partial class MainWindow : Window
    {
        //Поле на котором живет змея
        Entity field;
        // голова змеи
        Head head;
        // вся змея
        List<PositionedEntity> snake;
        // яблоко
        Apple apple;
        //количество очков
        int score;
        //таймер по которому 
        DispatcherTimer moveTimer;

        int FruitIndex = 1; //The index allows to determine the type of fruit in the game, the FruitIndex = 1 is the normal fruit, the FruitIndex = 2 is the apple
        int ThroughWall = 0; //The index allows to go through the wall, the ThroughWall = 0 is not allowed go through, the FruitIndex = 1 is allowed go through
        int CountWallTimes = 0; //The index allows to determine how many time did the snake go through the walls 

        //конструктор формы, выполняется при запуске программы
        public MainWindow()
        {
            InitializeComponent();
            
            snake = new List<PositionedEntity>();
            //создаем поле 300х300 пикселей
            field = new Entity(600, 600, "pack://application:,,,/Resources/snake.png");

            //создаем таймер срабатывающий раз в 300 мс
            moveTimer = new DispatcherTimer();
            moveTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            moveTimer.Tick += new EventHandler(moveTimer_Tick);
            
        }

        //метод перерисовывающий экран
        private void UpdateField()
        {
            //обновляем положение элементов змеи
            foreach (var p in snake)
            {
                Canvas.SetTop(p.image, p.y);
                Canvas.SetLeft(p.image, p.x);
            }

            //обновляем положение яблока
            Canvas.SetTop(apple.image, apple.y);
            Canvas.SetLeft(apple.image, apple.x);
            
            //обновляем количество очков
            lblScore.Content = String.Format("{0}000", score);
        }

        //обработчик тика таймера. Все движение происходит здесь
        void moveTimer_Tick(object sender, EventArgs e)
        {
            //в обратном порядке двигаем все элементы змеи
            foreach (var p in Enumerable.Reverse(snake))
            {
                p.move();
            }

            //проверяем, что голова змеи не врезалась в тело
            foreach (var p in snake.Where(x => x != head))
            {
                //если координаты головы и какой либо из частей тела совпадают
                if (p.x == head.x && p.y == head.y)
                {
                    //мы проиграли
                    moveTimer.Stop();
                    tbGameOver.Visibility = Visibility.Visible;
                    return;
                }
            }

            // if_else : Determine when snake can go through the wall
            if (ThroughWall == 0)
            {
                // Snake can not go through
                if (head.x < 40 || head.x >= 540 || head.y < 40 || head.y >= 540)
                {
                    moveTimer.Stop();
                    tbGameOver.Visibility = Visibility.Visible;
                    return;
                }
            }
            else
            {
                //Snake can go through
                if (head.x < 40 || head.x >= 540 || head.y < 40 || head.y >= 540)
                {
                    //Determine the position the snake will be appeared after going through the walls
                    if (head.x < 40)
                    {
                        head.x = head.x + 520;
                    }

                    if (head.x >= 540)
                    {
                        head.x = head.x - 520;
                    }

                    if (head.y < 40)
                    {
                        head.y = head.y + 520;
                    }

                    if (head.y >= 540)
                    {
                        head.y = head.y - 520;
                    }
                    CountWallTimes++;
                }
            }
            //проверяем, что голова змеи врезалась в яблоко
            if (head.x == apple.x && head.y == apple.y)
            {
                // If snake meet the fruit,when FruitIndex==2, it means that fruit was a apple and the snake will have the helmet
                // when snake takes the helmet, snake can go through the walls, Index count the times that snake go through walls begin calculate
                if (FruitIndex == 2)
                {
                    CountWallTimes = 0;
                    AddHeadHelmet();
                }

                //Random j from 0-100, each time 5 <= j <= 10 apple will be appeared. It means 5% a appear the apple in fruits 
                Random randFruit = new Random();
                int j = randFruit.Next(100);
                if (j<=10&&j>=5)
                {
                    FruitIndex = 2;
                }
                score++;
                // apple appears
                // Above have "if(FruitIndex == 2))" and in here it's also appeared.
                // Because if don't have "if(FruitIndex == 2))" and "AddHeadHelmet();" inside this the snake will be have the Helmet before it get the apple
                if (FruitIndex == 2)
                {
                    AddExtraApple();
                    apple.move();
                    var part = new BodyPart(snake.Last());
                    canvas1.Children.Add(part.image);
                    snake.Add(part);
                    
                }
                else 
                {
                    AddFruit();
                    apple.move();
                    var part = new BodyPart(snake.Last());
                    canvas1.Children.Add(part.image);
                    snake.Add(part);
                }
            }

            // if CountWallTimes == 5, the snake could not go through the walls
            if (CountWallTimes == 5)
            {
                RemoveHeadHelmet();
                CountWallTimes = 0;
            }
            //перерисовываем экран
            UpdateField();
        }
        //Add helmet and remove helmet
        void AddHeadHelmet()
        {
            head.image.Source = (new ImageSourceConverter()).ConvertFromString("pack://application:,,,/Resources/HeadHelmet.png") as ImageSource;
            ThroughWall = 1;
        }
        void RemoveHeadHelmet()
        {
            head.image.Source = (new ImageSourceConverter()).ConvertFromString("pack://application:,,,/Resources/head.png") as ImageSource;
            ThroughWall = 0;
        }
        //-----
        //Add apple and add fruit
        void AddFruit()
        {
            apple.image.Source = (new ImageSourceConverter()).ConvertFromString("pack://application:,,,/Resources/fruit.png") as ImageSource;
        }
        void AddExtraApple()
        {
            apple.image.Source = (new ImageSourceConverter()).ConvertFromString("pack://application:,,,/Resources/apple.png") as ImageSource;
        }
        //------
        // Обработчик нажатия на кнопку клавиатуры
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    head.direction = Head.Direction.UP;
                    break;
                case Key.Down:
                    head.direction = Head.Direction.DOWN;
                    break;
                case Key.Left:
                    head.direction = Head.Direction.LEFT;
                    break;
                case Key.Right:
                    head.direction = Head.Direction.RIGHT;
                    break;
            }
        }

        // Обработчик нажатия кнопки "Start"
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // обнуляем счет
            score = 0;
            // обнуляем змею
            snake.Clear();
            // очищаем канвас
            canvas1.Children.Clear();
            // скрываем надпись "Game Over"
            tbGameOver.Visibility = Visibility.Hidden;
            
            // добавляем поле на канвас
            canvas1.Children.Add(field.image);
            // создаем новое яблоко и добавлем его
            apple = new Apple(snake);
            canvas1.Children.Add(apple.image);
            // создаем голову
            head = new Head();
            snake.Add(head);
            canvas1.Children.Add(head.image);

            //запускаем таймер
            FruitIndex = 1;
            moveTimer.Start();
            UpdateField();

        }
        
        public class Entity
        {
            protected int m_width;
            protected int m_height;
            
            Image m_image;
            public Entity(int w, int h, string image)
            {
                m_width = w;
                m_height = h;
                m_image = new Image();
                m_image.Source = (new ImageSourceConverter()).ConvertFromString(image) as ImageSource;
                m_image.Width = w;
                m_image.Height = h;

            }

            // Property image - added the set function. "m_image" could be accessed and change the value outside. Help for add Apple and Helmet
            public Image image
            {
                get
                {
                    return m_image;
                }
                set 
                {
                    m_image = value;
                }
            }
        }

        public class PositionedEntity : Entity
        {
            protected int m_x;
            protected int m_y;
            public PositionedEntity(int x, int y, int w, int h, string image)
                : base(w, h, image)
            {
                m_x = x;
                m_y = y;
            }

            public virtual void move() { }

            public int x
            {
                get
                {
                    return m_x;
                }
                set
                {
                    m_x = value;
                }
            }

            public int y
            {
                get
                {
                    return m_y;
                }
                set
                {
                    m_y = value;
                }
            }
        }

        public class Apple : PositionedEntity
        {
            List<PositionedEntity> m_snake;
            public Apple(List<PositionedEntity> s)
                : base(0, 0, 40, 40, "pack://application:,,,/Resources/fruit.png")
            {
                m_snake = s;
                move();
            }

            public override void move()
            {
                Random rand = new Random();
                do
                {
                    x = rand.Next(13) * 40 + 40;
                    y = rand.Next(13) * 40 + 40;
                    bool overlap = false;
                    foreach (var p in m_snake)
                    {
                        if (p.x == x && p.y == y)
                        {
                            overlap = true;
                            break;
                        }
                    }
                    if (!overlap)
                        break;
                } while (true);
            }
        }

        public class Head : PositionedEntity
        {
            public enum Direction
            {
                RIGHT, DOWN, LEFT, UP, NONE
            };

            Direction m_direction;

            public Direction direction {
                set
                {
                    m_direction = value;
                    RotateTransform rotateTransform = new RotateTransform(90 * (int)value);
                    image.RenderTransform = rotateTransform;
                }
            }
            //280, 280, 40, 40, "pack://application:,,,/Resources/HeadHelmet.png"
            public Head()
                : base(280, 280, 40, 40, "pack://application:,,,/Resources/head.png")
            {
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                m_direction = Direction.NONE;
            }

            public override void move()
            {
                switch (m_direction)
                {
                    case Direction.DOWN:
                        y += 40;
                        break;
                    case Direction.UP:
                        y -= 40;
                        break;
                    case Direction.LEFT:
                        x -= 40;
                        break;
                    case Direction.RIGHT:
                        x += 40;
                        break;
                }
            }
        }

        public class BodyPart : PositionedEntity
        {
            PositionedEntity m_next;
            public BodyPart(PositionedEntity next)
                : base(next.x, next.y, 40, 40, "pack://application:,,,/Resources/body.png")
            {
                m_next = next;
            }
            public override void move()
            {
                x = m_next.x;
                y = m_next.y;
            }
        }
    }
}
