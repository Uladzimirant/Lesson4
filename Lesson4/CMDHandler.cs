using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lesson4
{
    //exception class that should be placed in functions with
    //the purpose to print the message and await new command
    internal class MessageException : Exception
    {
        public MessageException(){}
        public MessageException(string? message) : base(message){}
        public MessageException(string? message, Exception? innerException) : base(message, innerException){}
    }
    /* Class designed to provide a console interface with enterable commands
     * It implements default exit and help behavior and exception interception
     * Also there is special exception for stopping executing function with printing only message.
     */
    internal class CMDHandler
    {
        private bool _continueRunning = true;
        // Dictionary that maps commandName to function to run when command is called, tuple format is
        // <Function itself, list of all aliases including commandName, description>
        private IDictionary<string, Tuple<Action, List<string>, string?>> _commands = new Dictionary<string, Tuple<Action, List<string>, string?>>();
        
        //init in constructor
        private ICollection<Tuple<Action, List<string>, string?>> _customCommands; 
        private ICollection<Tuple<Action, List<string>, string?>> _defaultCommands;

        public string? Prefix = "> ";
        public string? Description = null;

        public CMDHandler()
        {
            _customCommands = new List<Tuple<Action, List<string>, string?>>();
            RegisterCommand("help", printHelp, "This message");
            RegisterCommand(new string[] { "quit", "exit" }, exit, "Ends program");
            _defaultCommands = _customCommands;
            _customCommands = new List<Tuple<Action, List<string>, string?>>();
        }
        /* Main cycle where program will run
         * until spectial command stops it.
         * All exceptions will be intercepted and then
         * the class will await new command.
         */
        public void Run()
        {
            while (_continueRunning)
            {
                if (Prefix != null) Console.Write(Prefix);
                string inputCommand = Console.ReadLine() ?? "quit";
                try
                {
                    if (_commands.TryGetValue(inputCommand, out var action))
                    {
                        action.Item1();
                    }
                    else
                    { handleNoCommand(inputCommand); }
                }
                catch (MessageException e) { handleMessageException(e); }
                catch (Exception e) { handleException(e); }
            }
        }

        //registers command by putting it inside command set and creating one or many references for it in dictionary
        public void RegisterCommand(string command, Action action, string? description = null)
        {
            RegisterCommand(new string[] { command }, action, description);
        }
        public void RegisterCommand(string[] commands, Action action, string? description = null)
        {
            var t = Tuple.Create(action, new List<string>(commands), description);
            foreach (var c in commands) _commands.Add(c, t);
            _customCommands.Add(t);
        }
        //create another alias for command, puts new element in dictionary and adds command to tuple with funcion
        public void RegisterAlias(string newAlias, string existingCommand)
        {
            var t = _commands[existingCommand];
            _commands.Add(newAlias, t);
            t.Item2.Add(newAlias);
        }

        private void exit()
        {
            _continueRunning = false;
        }

        private void printFunc(StringBuilder builder, IEnumerable<Tuple<Action, List<string>, string?>> elems)
        {
            foreach (var elem in elems)
            {
                builder.Append(" ");
                //print all command aliases
                builder.AppendJoin(", ", elem.Item2);
                //if description provided, write it
                if (elem.Item3 != null)
                {
                    builder.Append(" - ");
                    builder.Append(elem.Item3);
                }
                builder.AppendLine();
            }

        }
        private void printHelp()
        {
            StringBuilder builder = new StringBuilder();
            if (Description != null) builder.AppendLine(Description);
            builder.AppendLine("Avaliable commands:");
            printFunc(builder, _customCommands);
            printFunc(builder, _defaultCommands);
            Console.Write(builder.ToString());
        }
        private void handleNoCommand(string s)
        {
            Console.WriteLine($"No such command \"{s}\"");
        }

        private void handleMessageException(MessageException e)
        {
            Console.WriteLine(e.Message);
            if (e.InnerException != null)
            {
                Console.WriteLine("Exception in question:");
                Console.WriteLine(e.InnerException.ToString());
            }
        }
        private void handleException(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
