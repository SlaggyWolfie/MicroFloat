
namespace Slavi
{
    public abstract class Event { }
    public sealed class WinGameEvent : Event { }
    public sealed class WinLevelEvent : Event { }
    public sealed class LoseLevelEvent : Event { }
    public sealed class ObstacleGrowEvent : Slavi.Event { }
    public sealed class LoseLifeEvent : Event { }
    public sealed class PlayerSuckEvent : Event { }
    public sealed class CoreSuckEvent : Event { }
    //PlayerSuck

    public sealed class LevelInitEvent : Event
    {
        public LevelSettings level;
        public LevelInitEvent(LevelSettings level) { this.level = level; }
    }

    public sealed class PlayerPickupEvent : Event
    {
        public Pickup pickup = null;
        public PlayerPickupEvent(Pickup item) { pickup = item; }
    }

    public sealed class CorePickupEvent : Event
    {
        public Pickup pickup = null;
        public CorePickupEvent(Pickup item) { pickup = item; }
    }
}