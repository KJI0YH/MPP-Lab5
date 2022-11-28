namespace Lab5;
public class Program
{
    public static void Main(string[] args)
    {
        StringFormatter stringFormatter = new StringFormatter();
        if (stringFormatter.Parse("П{Orders[0]"))
            Console.WriteLine("Accept");
        else
            Console.WriteLine("Reject");
    }
}
