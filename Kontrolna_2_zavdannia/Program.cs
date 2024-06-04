using System;
using System.Collections.Generic;
using System.Linq;

public class User
{
    public string Login { get; set; }
    public string Password { get; set; }
    public DateTime BirthDate { get; set; }
}

public class Question
{
    public string Text { get; set; }
    public List<string> Options { get; set; }
    public List<int> CorrectAnswers { get; set; }
}

public class Quiz
{
    public string Category { get; set; }
    public List<Question> Questions { get; set; }
}

public class UserManager
{
    private List<User> users = new List<User>();

    public bool Register(string login, string password, DateTime birthDate)
    {
        if (users.Any(u => u.Login == login))
        {
            return false;
        }

        users.Add(new User { Login = login, Password = password, BirthDate = birthDate });
        return true;
    }

    public User Login(string login, string password)
    {
        return users.FirstOrDefault(u => u.Login == login && u.Password == password);
    }

    public bool ChangePassword(User user, string newPassword)
    {
        user.Password = newPassword;
        return true;
    }

    public bool ChangeBirthDate(User user, DateTime newBirthDate)
    {
        user.BirthDate = newBirthDate;
        return true;
    }
}

public class QuizManager
{
    private List<Quiz> quizzes = new List<Quiz>();

    public QuizManager()
    {
        quizzes.Add(new Quiz
        {
            Category = "Iсторiя",
            Questions = new List<Question>
            {
                new Question { Text = "Хто відкрив Америку?", Options = new List<string> { "Колумб", "Магеллан", "Кук", "Кортес" }, CorrectAnswers = new List<int> { 0 } },
                new Question { Text = "Коли почалася Друга свiтова вiйна?", Options = new List<string> { "1914", "1939", "1945", "1923" }, CorrectAnswers = new List<int> { 1 } },
                // Додати інші питання
            }
        });

        quizzes.Add(new Quiz
        {
            Category = "Географiя",
            Questions = new List<Question>
            {
                new Question { Text = "Яка найбiльша рiчка у свiтi?", Options = new List<string> { "Амазонка", "Нiл", "Янцзи", "Мiссiсiпi" }, CorrectAnswers = new List<int> { 1 } },
                new Question { Text = "Яка найвища гора у свiтi?", Options = new List<string> { "Еверест", "Кiлiманджаро", "К2", "Аконкагуа" }, CorrectAnswers = new List<int> { 0 } },
                // Додати інші питання
            }
        });
    }

    public Quiz GetQuizByCategory(string category)
    {
        return quizzes.FirstOrDefault(q => q.Category == category);
    }

    public Quiz GetRandomQuiz()
    {
        var random = new Random();
        var allQuestions = quizzes.SelectMany(q => q.Questions).OrderBy(q => random.Next()).Take(20).ToList();
        return new Quiz { Category = "Змiшана", Questions = allQuestions };
    }
}

public class QuizSession
{
    private Quiz quiz;
    private int currentQuestionIndex = 0;
    private int score = 0;

    public QuizSession(Quiz quiz)
    {
        this.quiz = quiz;
    }

    public Question GetNextQuestion()
    {
        if (currentQuestionIndex < quiz.Questions.Count)
        {
            return quiz.Questions[currentQuestionIndex++];
        }
        return null;
    }

    public void SubmitAnswer(List<int> selectedAnswers)
    {
        var currentQuestion = quiz.Questions[currentQuestionIndex - 1];
        if (selectedAnswers.SequenceEqual(currentQuestion.CorrectAnswers))
        {
            score++;
        }
    }

    public int GetScore()
    {
        return score;
    }
}

public class Program
{
    private static UserManager userManager = new UserManager();
    private static QuizManager quizManager = new QuizManager();
    private static User currentUser;

    public static void Main()
    {
        while (true)
        {
            Console.WriteLine("1. Увiйти");
            Console.WriteLine("2. Зареєструватися");
            var choice = Console.ReadLine();

            if (choice == "1")
            {
                Login();
                if (currentUser != null)
                {
                    MainMenu();
                }
            }
            else if (choice == "2")
            {
                Register();
            }
        }
    }

    private static void Login()
    {
        Console.Write("Логiн: ");
        var login = Console.ReadLine();
        Console.Write("Пароль: ");
        var password = Console.ReadLine();
        currentUser = userManager.Login(login, password);
        if (currentUser != null)
        {
            Console.WriteLine("Вхiд успiшний!");
        }
        else
        {
            Console.WriteLine("Невiрний логiн або пароль.");
        }
    }

    private static void Register()
    {
        Console.Write("Логiн: ");
        var login = Console.ReadLine();
        Console.Write("Пароль: ");
        var password = Console.ReadLine();
        Console.Write("Дата народження (yyyy-mm-dd): ");
        var birthDate = DateTime.Parse(Console.ReadLine());

        if (userManager.Register(login, password, birthDate))
        {
            Console.WriteLine("Реєстрацiя успiшна!");
        }
        else
        {
            Console.WriteLine("Логiн вже iснує.");
        }
    }

    private static void MainMenu()
    {
        while (true)
        {
            Console.WriteLine("1. Стартувати нову вiкторину");
            Console.WriteLine("2. Переглянути результати минулих вiкторин");
            Console.WriteLine("3. Переглянути Топ-20");
            Console.WriteLine("4. Змiнити налаштування");
            Console.WriteLine("5. Вийти");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                StartQuiz();
            }
            else if (choice == "2")
            {
                // Переглянути результати минулих вікторин
            }
            else if (choice == "3")
            {
                // Переглянути Топ-20
            }
            else if (choice == "4")
            {
                ChangeSettings();
            }
            else if (choice == "5")
            {
                currentUser = null;
                break;
            }
        }
    }

    private static void StartQuiz()
    {
        Console.WriteLine("Оберiть категорiю вiкторини:");
        Console.WriteLine("1. Iсторiя");
        Console.WriteLine("2. Географiя");
        Console.WriteLine("3. Змiшана");

        var choice = Console.ReadLine();
        Quiz quiz = null;

        if (choice == "1")
        {
            quiz = quizManager.GetQuizByCategory("Історiя");
        }
        else if (choice == "2")
        {
            quiz = quizManager.GetQuizByCategory("Географiя");
        }
        else if (choice == "3")
        {
            quiz = quizManager.GetRandomQuiz();
        }

        if (quiz != null)
        {
            var session = new QuizSession(quiz);
            Question question;
            while ((question = session.GetNextQuestion()) != null)
            {
                Console.WriteLine(question.Text);
                for (int i = 0; i < question.Options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {question.Options[i]}");
                }

                var answers = Console.ReadLine().Split(',').Select(int.Parse).ToList();
                session.SubmitAnswer(answers.Select(a => a - 1).ToList());
            }

            Console.WriteLine($"Ваша кiлькiсть правильних вiдповiдей: {session.GetScore()}");
        }
    }

    private static void ChangeSettings()
    {
        Console.WriteLine("1. Змiнити пароль");
        Console.WriteLine("2. Змiнити дату народження");

        var choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.Write("Новий пароль: ");
            var newPassword = Console.ReadLine();
            userManager.ChangePassword(currentUser, newPassword);
            Console.WriteLine("Пароль змiнено!");
        }
        else if (choice == "2")
        {
            Console.Write("Нова дата народження (yyyy-mm-dd): ");
            var newBirthDate = DateTime.Parse(Console.ReadLine());
            userManager.ChangeBirthDate(currentUser, newBirthDate);
            Console.WriteLine("Дата народження змiнена!");
        }
    }
}
