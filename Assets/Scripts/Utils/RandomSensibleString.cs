namespace Utils {
    public static class RandomSensibleString {
        private static readonly string[] List1 = { "Fast", "Wide", "Hairy", "Extremely", "Long" };
        private static readonly string[] List2 = { "White", "YeastFree", "Windy", "Loose", "Bendy" };
        private static readonly string[] List3 = { "Cat", "Cabbage", "Rotunda", "Brewery", "TrainCar" };

        private static readonly string[] NameList =
            { "Wendy", "Lewis", "Sheena", "Nic", "Alex", "Evelyn", "Boris", "Max", "Petria", "Emir" };
        public static string GenerateString(){
            var str1 = List1[UnityEngine.Random.Range(0, List1.Length)];
            var str2 =  List2[UnityEngine.Random.Range(0, List2.Length)];
            var str3 =  List3[UnityEngine.Random.Range(0, List3.Length)];
            return $"{str1}-{str2}-{str3}";
        }

        public static string GenerateNameString() {
            return NameList[UnityEngine.Random.Range(0, NameList.Length)];
        }
    }
}