using System; // Необходимо для [Serializable]

[Serializable]
public class WelcomeData
{
    public string Title;
    public string Text;

    // Вы также можете добавить конструктор или другие методы, если нужно
    public WelcomeData(string title, string text)
    {
        Title = title;
        Text = text;
    }

    // Статический метод для получения данных по умолчанию, если Remote Config недоступен
    public static WelcomeData GetDefault()
    {
        return new WelcomeData(
            "Приветствуем!",
            "Произошла ошибка при загрузке сообщения. Наслаждайтесь игрой!"
        );
    }
}