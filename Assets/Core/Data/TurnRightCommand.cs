public class TurnRightCommand : Command
{
    public override void Execute(RobotController robot)
    {
        robot.TurnRight();
    }
}