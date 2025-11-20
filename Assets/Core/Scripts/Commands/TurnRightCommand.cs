// Represents the "Turn Right" command.
public class TurnRightCommand : Command
{
    public override void Execute(RobotController robot)
    {
        robot.TurnRight();
    }
}
