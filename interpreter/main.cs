using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PeachInterpreter
{
    static class Interpreter
    {
        static List<string> cmd_args = new List<string>();

        static readonly Dictionary<String, Tuple<String, Action>> commands = new Dictionary<string, Tuple<string, Action>>() {
            {"q", new Tuple<String, Action>("quits the session", command_quit)},
            {"h", new Tuple<String, Action>("prints help", command_help)},
            {"c", new Tuple<String, Action>("clear screen", command_clear)},
            {"r", new Tuple<String, Action>("run file (usage: :r *.peach)", run_file)},
        };

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                repl();
            }
            else
            {
                run_file();
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
                    String cmd_full = ln.Substring(1);
                    String[] cmd = cmd_full.Split(' ');
                    if (cmd.Length > 1)
                        cmd_args = cmd.ToList().GetRange(1, cmd.Length - 1);
                    if (commands.ContainsKey(cmd[0]))
                        commands[cmd[0]].Item2();
                    else
                        Console.WriteLine($"Command '{cmd_full[0]}' does not exist.");
                }
                else
                    eval(ln);
            }
        }

        static void run_file()
        {
            if (cmd_args.Count == 0)
            {
                Console.WriteLine("Please provide a full file path to *.peach file");
                return;
            }

            if (!File.Exists(cmd_args[0]))
            {

                Console.WriteLine("File does not exist.");
                return;
            }

            if (cmd_args[0].EndsWith(".peach"))
            {
                Console.WriteLine("Please provide a *.peach file.");
                return;
            }

            File.ReadAllText(cmd_args[0]);
        }

        static void command_quit()
        {
            Environment.Exit(0);
        }

        static void command_help()
        {
            foreach (KeyValuePair<String, Tuple<String, Action>> item in commands)
            {
                Console.WriteLine($":{item.Key} | {item.Value.Item1}");
            }
        }

        static void command_clear()
        {
            Console.Write("\u001b[2J\u001b[H");
        }
    }
}
