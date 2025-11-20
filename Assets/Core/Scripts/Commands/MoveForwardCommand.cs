// Represents the "Move Forward" command.
public class MoveForwardCommand : Command
{
    public override void Execute(RobotController robot)
    {
        robot.MoveForward();
    }
}
