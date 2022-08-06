using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PeachInterpreter
{
    static class Interpreter
    {
        static List<string> cmd_args = new List<string>();

        static readonly Dictionary<string, Tuple<string, Action>> commands = new Dictionary<string, Tuple<string, Action>>() {
            {"q", new Tuple<string, Action>("quits the session", command_quit)},
            {"h", new Tuple<string, Action>("prints help", command_help)},
            {"c", new Tuple<string, Action>("clear screen", command_clear)},
            {"r", new Tuple<string, Action>("run file (usage: :r *.peach)", run_file)},
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

        static void eval(string line)
        {
            // mach halt
        }

        static void repl()
        {
            Console.WriteLine("Exit with :q, help qith :h");
            for (; ; )
            {
                Console.Write(">>  ");
                string ln = Console.ReadLine();

                if (ln.StartsWith(":"))
                {
                    string cmd_full = ln.Substring(1);
                    string[] cmd = cmd_full.Split(' ');
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

            if (!cmd_args[0].Trim().EndsWith(".peach"))
            {
                Console.WriteLine("Please provide a *.peach file.");
                return;
            }

            eval(File.ReadAllLines(cmd_args[0]));
        }

        static void eval(string[] code)
        {
            Lexer ben = Lexer.tokenize(code.ToList());

            if (ben.had_error)
            {
                foreach (var err in ben.error)
                {
                    Console.WriteLine(err);
                }
            }

            Console.WriteLine("+++ DUMP +++");
            foreach (var token in ben.Tokens)
            {
                Console.WriteLine(token);
            }
        }

        static void command_quit()
        {
            Environment.Exit(0);
        }

        static void command_help()
        {
            foreach (KeyValuePair<string, Tuple<string, Action>> item in commands)
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
