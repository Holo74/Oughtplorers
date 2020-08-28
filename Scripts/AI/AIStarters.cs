using TestAI;
namespace AIStartLinks
{
    public static class AIStarters
    {
        public static AILink Start(AIStartNames name, MasterController controller)
        {
            switch (name)
            {
                case AIStartNames.TinyCreature:
                    return new TinyCreatureStart(controller);
                default:
                    return new InitialStartup(controller);
            }

        }
    }

    public enum AIStartNames
    {
        TinyCreature
    }

}
