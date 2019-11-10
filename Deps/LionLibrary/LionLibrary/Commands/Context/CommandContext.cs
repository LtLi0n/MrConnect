namespace LionLibrary.Commands
{
    public class CommandContext
    {
        public Command Command { get; internal set; }
        public CommandModule Module { get; internal set; }

        public string Header => Command.Header;

        public CommandContext() { }
    }
}
