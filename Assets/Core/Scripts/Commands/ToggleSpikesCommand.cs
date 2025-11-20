// Represents the "Toggle Spikes" command.
public class ToggleSpikesCommand : Command
{
    // This 'override' keyword tells the compiler that this method is intentionally
    // replacing the 'abstract' method from the base 'Command' class.
    // The signature 'public override void Execute(RobotController robot)' must match exactly.
    public override void Execute(RobotController robot)
    {
        // TODO: Implement interaction with SpikeController
        // This will require a way to find the relevant SpikeController instance.
        // For now, we'll just log that the command was executed.
        UnityEngine.Debug.Log("Executing: Toggle Spikes");
    }
}
