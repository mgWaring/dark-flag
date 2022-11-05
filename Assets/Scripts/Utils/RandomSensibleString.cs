
using System;
public class RandomSensibleString {
    private static string[] list1 = new string[5]{ "Fast", "Wide", "Hairy", "Extremely", "Long" };
    private static string[] list2 = new string[5]{ "White", "YeastFree", "Windy", "Loose", "Bendy" };
    private static string[] list3 = new string[5]{ "Cat", "Cabbage", "Rotunda", "Brewery", "TrainCar" };
    public static string GenerateString(){
        string str1 = list1[UnityEngine.Random.Range(0, list1.Length)];
        string str2 =  list2[UnityEngine.Random.Range(0, list2.Length)];;
        string str3 =  list3[UnityEngine.Random.Range(0, list3.Length)];;
        return String.Format("{0}-{1}-{2}", str1, str2, str3);
    }
}