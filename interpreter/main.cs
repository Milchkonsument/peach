using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace PeachInterpreter
{
    static class Interpreter
    {
        const string CMD_HIST_PATH = "./.history";

        private static List<string> cmd_args = new List<string>();
        private static List<string> cmd_hist = new List<string>();
        private static int cmd_hist_index = 0;

        private static Thread input_thread = new Thread(new ThreadStart(input_thread_task));

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
                cmd_args = new List<string>() { args[0] };
                run_file();
            }
        }


        static void repl()
        {
            setup_command_history();

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
                    {
                        commands[cmd[0]].Item2();
                        cmd_hist.Add(ln);
                    }
                    else
                        Console.WriteLine($"Command '{cmd_full[0]}' does not exist.");
                }
                else
                    eval(ln);
            }
        }

        static void setup_command_history()
        {
            // read past commands from disk
            if (File.Exists(CMD_HIST_PATH))
            {
                cmd_hist = File.ReadAllLines(CMD_HIST_PATH).ToList();
            }

            // start thread to read uparrow & downarrow
            input_thread.IsBackground = true;
            input_thread.Start();
        }

        static void input_thread_task()
        {
            for (; ; )
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    Console.Write("\r{0}", cmd_args.Count == 0 ? "" : cmd_args[cmd_hist_index++]);
                    cmd_hist_index %= cmd_args.Count;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    Console.Write("\r{0}", cmd_args.Count == 0 ? "" : cmd_args[Math.Min(--cmd_hist_index, 0)]);
                    cmd_hist_index %= cmd_args.Count;
                }
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

        static void eval(string line) => eval(new string[] { line });

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

            foreach (var token in ben.Tokens)
            {
                Console.WriteLine(token);
            }
        }

        static void command_quit()
        {
            File.WriteAllLines(CMD_HIST_PATH, cmd_hist);
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
