public abstract class Command
{
    // The core method for any command. When called, the command performs its action.
    // We will pass a reference to the 'RobotController' or whatever object
    // the commands are supposed to manipulate.
    public abstract void Execute(RobotController robot);
}