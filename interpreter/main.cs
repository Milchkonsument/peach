namespace PeachInterpreter
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    class Interpreter
    {
        static Dictionary<String, Tuple<String, Action>> commands = new Dictionary<string, Tuple<string, Action>>() {
            {"q", new Tuple<String, Action>("quits the session", command_quit)},
            {"h", new Tuple<String, Action>("prints help", command_help)},
        };

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                repl();
            }
            else
            {
                if (File.Exists(args[1]))
                {
                    // blah
                }
            }
        }

        static void eval(String line)
        {
            // mach halt
        }

        static void repl()
        {
            Console.WriteLine("Exit with :q, help qith :h");
            for (; ; )
            {
                Console.Write(">>  ");
                String ln = Console.ReadLine();

                if (ln.StartsWith(':'))
                {
                    String cmd = ln.Substring(1);
                    if (commands.ContainsKey(cmd))
                        commands[cmd].Item2();
                    else
                        Console.WriteLine($"Command '{cmd}' does not exist.");
                }
                else
                    eval(ln);
            }
        }

        static void command_quit()
        {
            Environment.Exit(0);
        }

        static void command_help()
        {
            Console.WriteLine("Nobody is here to help. :<");
        }
    }
}
