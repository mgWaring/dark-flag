namespace Multiplayer {
    public static class CrossScene
    {
        public static MapScriptable map { get; set; }
        public static int laps { get; set; }
        public static int players { get; set; }
        public static int bots { get; set; }
        public static bool cameFromMainMenu { get; set; }
        public static RacerInfo[] racerInfo { get; set; }
    }
}
