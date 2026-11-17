namespace SportApp;

// Клас "Спортсмен"
public class Athlete
{
    // ---------- загальнодоступні поля (характеристики) ----------
    public string name;               // ім'я спортсмена
    public DateTime birthDate;        // дата народження
    public SportType sport;           // вид спорту
    public double weight;             // вага, кг
    public int medals;                // кількість медалей
    public bool isProfessional;       // професіонал чи аматор

    // ---------- закриті поля (внутрішній стан) ----------
    private int energy = 100;         // рівень енергії, 0..100
    private int trainingHours = 0;    // загальна кількість годин тренувань

    // ---------- загальнодоступні методи (поведінка) ----------

    // Тренування: кожна година витрачає 10 одиниць енергії
    public bool Train(int hours)
    {
        if (hours <= 0) return false;
        int cost = hours * 10;
        if (energy < cost) return false;
        energy -= cost;
        trainingHours += hours;
        return true;
    }

    // Відпочинок: кожна година відновлює 15 одиниць енергії (максимум 100)
    public void Rest(int hours)
    {
        if (hours <= 0) return;
        energy = Math.Min(100, energy + hours * 15);
    }

    // Чи може спортсмен взяти участь у змаганні (потрібно мінімум 30 енергії)
    public bool CanCompete()
    {
        return energy >= 30;
    }

    // Участь у змаганні проти суперника з заданим рейтингом.
    // Перемога => +1 медаль. Змагання витрачає 30 енергії.
    public bool Compete(int opponentRating)
    {
        if (!CanCompete()) return false;
        int rating = GetRating();
        energy -= 30;
        if (rating > opponentRating)
        {
            medals++;
            return true;
        }
        return false;
    }

    // Рейтинг спортсмена
    public int GetRating()
    {
        return medals * 2 + trainingHours + (isProfessional ? 20 : 0) + energy / 10;
    }

    // Вік спортсмена (повних років)
    public int GetAge()
    {
        DateTime today = DateTime.Today;
        int age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }

    public int GetEnergy()
    {
        return energy;
    }

    public int GetTrainingHours()
    {
        return trainingHours;
    }
}
