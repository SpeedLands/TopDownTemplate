// Represents the "Turn Left" command.
public class TurnLeftCommand : Command
{
    public override void Execute(RobotController robot)
    {
        robot.TurnLeft();
    }
}
