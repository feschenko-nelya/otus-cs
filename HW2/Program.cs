namespace HW2
{
    internal class Program
    {
        enum ProgramState
        {
            None,
            WasWelcome,
            Started,
            Finished
        }

        private readonly static Version version = new(1, 0);
        private readonly static DateTime creationDate = new(2025, 3, 19);

        static void Main()
        {
            ProgramState programState = ProgramState.None;
            string? name = "";
            bool wasGreeting = false;

            while (programState != ProgramState.Finished)
            {
                Console.Clear();

                if (!wasGreeting)
                {
                    Console.WriteLine("Здравствуйте.");
                    wasGreeting = true;
                }

                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Введите одну из команд:");
                }
                else
                {
                    Console.WriteLine(name + ", введите одну из команд:");
                }
                                
                Console.WriteLine("/start");
                if (programState == ProgramState.Started)
                {
                    Console.WriteLine("/echo");
                }
                Console.WriteLine("/help");
                Console.WriteLine("/info");
                Console.WriteLine("/exit");
                Console.WriteLine();

                string? userRequest = Console.ReadLine();
                Console.Clear();

                switch (userRequest)
                {
                    case "/start":
                    {
                        Console.WriteLine("Введите имя, по которому к Вам можно обратиться:");

                        while (string.IsNullOrEmpty(name))
                        {
                            name = Console.ReadLine();
                        }

                        Console.Clear();
                        Console.WriteLine("Здравствуйте, " + name);

                        programState = ProgramState.Started;
                        break;
                    }
                    case "/help":
                        {
                            Console.WriteLine("""
                            Краткая информация о программе:
                            /start - начало работы программы: ввод имени, по которому обращаться к пользователю;
                            /help - краткая информация о программе;
                            /info - информация о версии программы и дате её создания;
                            /echo - повторный вывод введенного пользователем текста после названия команды через пробел (доступно после команды /start);
                            /exit - завершение работы программы.
                            """);
                            break;
                        }
                    case "/info":
                        {
                            Console.WriteLine("Версия программы: " + version.ToString());
                            Console.WriteLine("Дата создания: " + creationDate.ToString("dd.MM.yyyy"));
                            break;
                        }
                    case "/exit":
                        {
                            Console.Write("До свидания");
                            if (string.IsNullOrEmpty(name))
                            {
                                Console.WriteLine(".");
                            }
                            else
                            {
                                Console.WriteLine(", " + name + ".");
                            }
                            programState = ProgramState.Finished;
                            break;
                        }
                    default:
                        {
                            if (!string.IsNullOrEmpty(userRequest)
                                && (programState == ProgramState.Started)
                                && userRequest.StartsWith("/echo "))
                            {
                                Console.WriteLine(userRequest.Remove(0, "/echo ".Length));
                            }
                        }
                        break;
                }

                Console.ReadLine();
            }
        }
    }
}
